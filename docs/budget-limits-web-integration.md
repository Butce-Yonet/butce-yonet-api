# Bütçe Limitleri: Web Entegrasyon Dokümanı

| | |
|---|---|
| **Hedef kitle** | Web (frontend) ekibi |
| **API sürümü** | `feature/budgets` branch'i |
| **Base route** | `{API_BASE_URL}/api/budgets` |
| **Kimlik doğrulama** | Diğer endpoint'lerle aynı: `Authorization: Bearer <access_token>` |
| **İlgili doküman** | Backend tasarımı ve gerekçeler için [`budget-limits.md`](./budget-limits.md) |

> Yerel geliştirmede API varsayılan olarak `http://localhost:5088` (veya `https://localhost:7215`) adresinde çalışır.

---

## İçindekiler

1. [Hızlı başlangıç](#1-hızlı-başlangıç)
2. [Genel kurallar ve dikkat edilecekler](#2-genel-kurallar-ve-dikkat-edilecekler)
3. [TypeScript tipleri](#3-typescript-tipleri)
4. [Endpoint referansı](#4-endpoint-referansı)
5. [Hata yönetimi](#5-hata-yönetimi)
6. [Örnek API istemcisi](#6-örnek-api-istemcisi)
7. [Veri tazeliği ve cache stratejisi](#7-veri-tazeliği-ve-cache-stratejisi)
8. [Ekran ve UX kılavuzu](#8-ekran-ve-ux-kılavuzu)
9. [Push bildirimleri](#9-push-bildirimleri)
10. [Bilinen sınırlamalar](#10-bilinen-sınırlamalar)
11. [Kabul / test senaryoları](#11-kabul--test-senaryoları)

---

## 1. Hızlı başlangıç

**Kavramlar:**

- **Bütçe**, bir etiket ya da tüm giderler için tanımlanan **aylık harcama limitidir**.
- **Etiket bütçesi**, `userLabelId` dolu olan bütçedir. Örnek: "Market için 5.000 ₺".
- **Genel bütçe**, `userLabelId` boş olan bütçedir ve tüm giderleri kapsar.
- **Dönem**, varsayılan olarak ayın 1'inden ay sonuna kadardır. Kullanıcı başlangıç gününü 1–28 arası seçebilir. Örneğin 15 seçilirse dönem 15 Mart – 14 Nisan olur.
- Harcanan tutar, kalan tutar, yüzde ve durum **sunucuda hesaplanır**. Web tarafında hesaplama yapmanız gerekmez.

**En kısa akış:**

```http
# 1) Form için seçenekleri al
GET /api/UserLabels
GET /api/Currencies

# 2) Bütçe oluştur
POST /api/budgets
{ "userLabelId": 12, "amount": 5000, "currencyId": 1, "periodStartDay": 1 }

# 3) Listeyi göster
GET /api/budgets
```

---

## 2. Genel kurallar ve dikkat edilecekler

### 2.1 Yanıt zarfı

Tüm yanıtlar aynı zarfla döner. HTTP status kodu zarfın içinde değil, HTTP yanıtının kendisindedir.

```json
{
  "data": { },
  "messages": [],
  "isSuccess": true
}
```

### 2.2 ⚠️ Hata yanıtlarında alan adları PascalCase

Bu, entegrasyonda en kolay atlanan nokta:

| Yanıt türü | Alan adları | Örnek |
|---|---|---|
| Başarılı yanıtlar (2xx) | **camelCase** | `{"data":{...},"messages":[],"isSuccess":true}` |
| Hata yanıtları (400, 401, 403, 404, 500) | **PascalCase** | `{"Data":{},"Messages":["..."],"IsSuccess":false}` |

Sebebi şu: başarılı yanıtları ASP.NET MVC serileştiriyor (varsayılan camelCase), hata yanıtlarını ise exception middleware'i ve JWT event'leri ayarsız `JsonSerializer` ile yazıyor. **İstemcide iki biçimi de okuyan bir normalize fonksiyonu kullanın** ([bölüm 6](#6-örnek-api-istemcisi)'daki `normalizeEnvelope`).

### 2.3 Enum'lar sayı olarak gelir

`status` alanı `"Warning"` değil `2` olarak gelir. Eşleme [bölüm 3](#3-typescript-tipleri)'te.

### 2.4 Tarihler saat dilimi bilgisi içermez

```json
"periodStart": "2026-11-15T00:00:00"
```

- Sonunda `Z` ya da offset **yoktur**. Değer bir **takvim günüdür**, belirli bir an değildir.
- `new Date("2026-11-15T00:00:00")` tarayıcıda yerel saat olarak yorumlanır. Bu tarih için sorun çıkarmaz, ama **değeri UTC'ye çevirip geri formatlamayın**; gün kayabilir.
- Gösterim için yalnızca tarih kısmını kullanın (`"2026-11-15"` → `15 Kas`).
- `periodEnd` **dahildir**: dönemin son günüdür.

### 2.5 Tutarlar

- Tüm tutarlar JSON `number` olarak gelir (`5000`, `4012.5`). En fazla 2 ondalık basamak içerir.
- Para birimi sembolü ve konumu bütçenin `currency` nesnesinde gelir: `symbol` ve `isSymbolRight`. Biçimlendirmeyi buna göre yapın ([bölüm 8.6](#86-tutar-biçimlendirme)).

### 2.6 Kimlik doğrulama

- Tüm endpoint'ler `Authorization: Bearer <token>` ister.
- Token yoksa veya süresi dolmuşsa yanıt `401` olur ve gövdesi PascalCase zarftır, `Messages` boştur.
- Tüm işlemler yalnızca token sahibinin kendi bütçeleri üzerinde yapılır. Başka bir kullanıcının bütçe id'si ile yapılan istek `404` döner.

---

## 3. TypeScript tipleri

```ts
// ---- Zarf ----
export interface ApiEnvelope<T> {
  data: T;
  messages: string[];
  isSuccess: boolean;
}

// ---- Enum ----
export enum BudgetStatus {
  OnTrack = 1,   // harcama < %80
  Warning = 2,   // %80 <= harcama < %100
  Exceeded = 3,  // harcama >= %100
}

// ---- Ortak ----
export interface CurrencyDto {
  id: number;
  code: string;          // "TRY"
  name: string;
  symbol: string;        // "₺"
  isSymbolRight: boolean;
  rank: number;          // sıralama için
}

export interface UserLabelDto {
  id: number;
  name: string;
  colorCode: string;     // "#4CAF50"
}

// ---- Bütçe ----
export interface BudgetPeriodDto {
  periodStart: string;   // "YYYY-MM-DDT00:00:00"
  periodEnd: string;     // dahil
  spentAmount: number;
}

export interface BudgetDto {
  id: number;
  amount: number;                 // aylık limit
  currency: CurrencyDto;
  label: UserLabelDto | null;     // genel bütçede null
  isGeneral: boolean;
  periodStartDay: number;         // 1-28

  // Aktif dönem (sunucuda hesaplanır)
  periodStart: string;
  periodEnd: string;
  spentAmount: number;
  remainingAmount: number;        // asla negatif değil
  overAmount: number;             // limit aşımı, yoksa 0
  percentage: number;             // 100'ü geçebilir (örn. 130.5)
  status: BudgetStatus;
  daysLeft: number;               // bugün dahil, son gün = 1
  dailyAllowance: number;         // remainingAmount / daysLeft

  history: BudgetPeriodDto[];     // sadece detayda dolu, listede []
}

export interface BudgetSummaryDto {
  currency: CurrencyDto;
  totalAmount: number;
  totalSpent: number;
  remainingAmount: number;
  budgetCount: number;
  onTrackCount: number;
  warningCount: number;
  exceededCount: number;
}

// ---- İstekler ----
export interface CreateBudgetRequest {
  userLabelId?: number | null;    // boş = genel bütçe
  amount: number;                 // > 0
  currencyId: number;             // > 0
  periodStartDay?: number;        // 1-28, varsayılan 1
}

export interface UpdateBudgetRequest {
  amount: number;                 // > 0
  periodStartDay: number;         // 1-28
}
```

---

## 4. Endpoint referansı

| Method | Route | Açıklama |
|---|---|---|
| `GET` | [`/api/budgets`](#41-bütçeleri-listele) | Bütçeler ve aktif dönem durumları |
| `GET` | [`/api/budgets/summary`](#42-özet) | Para birimi bazında özet |
| `GET` | [`/api/budgets/{budgetId}`](#43-bütçe-detayı) | Detay ve dönem geçmişi |
| `POST` | [`/api/budgets`](#44-bütçe-oluştur) | Oluştur |
| `PUT` | [`/api/budgets/{budgetId}`](#45-bütçe-güncelle) | Limit ve başlangıç gününü güncelle |
| `DELETE` | [`/api/budgets/{budgetId}`](#46-bütçe-sil) | Sil |

Form için kullanılan yardımcı endpoint'ler (mevcut):

| Method | Route | Kullanım |
|---|---|---|
| `GET` | `/api/UserLabels` | Etiket seçimi (kullanıcı etiketleri ve sistem etiketleri) |
| `GET` | `/api/Currencies` | Para birimi seçimi |

---

### 4.1 Bütçeleri listele

```http
GET /api/budgets?currencyId=1
```

| Query | Tip | Zorunlu | Açıklama |
|---|---|---|---|
| `currencyId` | number | Hayır | Verilirse yalnızca o para birimindeki bütçeler döner |

**200 OK**

```json
{
  "data": [
    {
      "id": 3,
      "amount": 20000,
      "currency": { "id": 1, "code": "TRY", "name": "Türk Lirası", "symbol": "₺", "isSymbolRight": true, "rank": 1 },
      "label": null,
      "isGeneral": true,
      "periodStartDay": 1,
      "periodStart": "2026-11-01T00:00:00",
      "periodEnd": "2026-11-30T00:00:00",
      "spentAmount": 13450,
      "remainingAmount": 6550,
      "overAmount": 0,
      "percentage": 67.25,
      "status": 1,
      "daysLeft": 15,
      "dailyAllowance": 436.67,
      "history": []
    },
    {
      "id": 7,
      "amount": 5000,
      "currency": { "id": 1, "code": "TRY", "name": "Türk Lirası", "symbol": "₺", "isSymbolRight": true, "rank": 1 },
      "label": { "id": 12, "name": "Market", "colorCode": "#4CAF50" },
      "isGeneral": false,
      "periodStartDay": 1,
      "periodStart": "2026-11-01T00:00:00",
      "periodEnd": "2026-11-30T00:00:00",
      "spentAmount": 5240,
      "remainingAmount": 0,
      "overAmount": 240,
      "percentage": 104.8,
      "status": 3,
      "daysLeft": 15,
      "dailyAllowance": 0,
      "history": []
    }
  ],
  "messages": [],
  "isSuccess": true
}
```

**Notlar:**
- Sayfalama **yoktur**, tüm bütçeler tek seferde döner.
- Sıralama sunucuda yapılır: önce genel bütçe, sonra yüzdesi en yüksek olan. Bu sırayı koruyun.
- Hiç bütçe yoksa `data: []` döner (boş durum ekranı, [bölüm 8.1](#81-bütçe-listesi)).
- Farklı bütçelerin dönemleri farklı olabilir (farklı `periodStartDay`). Her kartta kendi `periodStart` / `periodEnd` değerini gösterin.

---

### 4.2 Özet

Dashboard kartı için.

```http
GET /api/budgets/summary
```

**200 OK**

```json
{
  "data": [
    {
      "currency": { "id": 1, "code": "TRY", "name": "Türk Lirası", "symbol": "₺", "isSymbolRight": true, "rank": 1 },
      "totalAmount": 20000,
      "totalSpent": 13450,
      "remainingAmount": 6550,
      "budgetCount": 5,
      "onTrackCount": 3,
      "warningCount": 1,
      "exceededCount": 1
    }
  ],
  "messages": [],
  "isSuccess": true
}
```

**Notlar:**
- Her para birimi için bir eleman döner, `currency.rank`'e göre sıralıdır. Bütçe yoksa `[]` döner.
- `totalAmount` / `totalSpent`:
  - O para biriminde **genel bütçe varsa** onun rakamlarıdır.
  - **Genel bütçe yoksa** etiket bütçelerinin toplamıdır. Bu durumda birden fazla etiketli bir işlem iki bütçeye de sayılabilir, yani toplam harcama gerçekten biraz yüksek görünebilir.

---

### 4.3 Bütçe detayı

```http
GET /api/budgets/7?historyCount=6
```

| Parametre | Yer | Tip | Zorunlu | Açıklama |
|---|---|---|---|---|
| `budgetId` | path | number | Evet | |
| `historyCount` | query | number | Hayır | Aktif dönem dahil kaç dönem dönülsün. 1–12, varsayılan 6 |

**200 OK**: `BudgetDto`. `history` doludur, **eskiden yeniye** sıralıdır ve **son eleman aktif dönemdir**.

```json
{
  "data": {
    "id": 7,
    "amount": 5000,
    "label": { "id": 12, "name": "Market", "colorCode": "#4CAF50" },
    "isGeneral": false,
    "...": "liste elemanıyla aynı alanlar",
    "history": [
      { "periodStart": "2026-06-01T00:00:00", "periodEnd": "2026-06-30T00:00:00", "spentAmount": 4630 },
      { "periodStart": "2026-07-01T00:00:00", "periodEnd": "2026-07-31T00:00:00", "spentAmount": 5120 },
      { "periodStart": "2026-08-01T00:00:00", "periodEnd": "2026-08-31T00:00:00", "spentAmount": 3980 },
      { "periodStart": "2026-09-01T00:00:00", "periodEnd": "2026-09-30T00:00:00", "spentAmount": 4410 },
      { "periodStart": "2026-10-01T00:00:00", "periodEnd": "2026-10-31T00:00:00", "spentAmount": 4890 },
      { "periodStart": "2026-11-01T00:00:00", "periodEnd": "2026-11-30T00:00:00", "spentAmount": 5240 }
    ]
  },
  "messages": [],
  "isSuccess": true
}
```

**Notlar:**
- `history` her dönem için **bugünkü limiti** (`amount`) baz alır. Limit geçmişi tutulmaz. Grafikte limit çizgisini tek bir yatay çizgi olarak gösterin.
- Harcama olmayan dönemler `spentAmount: 0` ile gelir, eksik eleman olmaz.
- Hatalar: `404` (bütçe yok veya başkasına ait), `400` (`historyCount` 1–12 dışında).

---

### 4.4 Bütçe oluştur

```http
POST /api/budgets
Content-Type: application/json

{
  "userLabelId": 12,
  "amount": 5000,
  "currencyId": 1,
  "periodStartDay": 1
}
```

| Alan | Tip | Zorunlu | Kural |
|---|---|---|---|
| `userLabelId` | number \| null | Hayır | Boş veya `null` ise **genel bütçe**. Doluysa `> 0` olmalı ve kullanıcının etiketi ya da sistem etiketi olmalı |
| `amount` | number | Evet | `> 0` |
| `currencyId` | number | Evet | `> 0`, mevcut bir para birimi |
| `periodStartDay` | number | Hayır | 1–28, varsayılan `1` |

**200 OK**

```json
{ "data": { "id": 7 }, "messages": [], "isSuccess": true }
```

**Hatalar:**

| Durum | HTTP | `Messages` |
|---|---|---|
| Validasyon hatası | 400 | FluentValidation mesajları (birden fazla olabilir) |
| Para birimi bulunamadı | 404 | `["Record Not Found"]` |
| Etiket bulunamadı veya kullanıcıya ait değil | 404 | `["Record Not Found"]` |
| **Aynı etiket (veya genel) ve para birimi için bütçe zaten var** | **404** ⚠️ | `["Record Already Exists"]` |

> ⚠️ "Zaten var" hatası mevcut altyapı gereği `409` değil **`404`** ile döner. Ayırt etmek için mesaja bakın (`"Record Already Exists"`). Bu backend'de ileride `409`'a çevrilebilir; istemci kodunu her iki durumu da karşılayacak yazın ([bölüm 5](#5-hata-yönetimi)).

**Öneri:** Formu açarken mevcut bütçe listesine bakıp, **zaten bütçesi olan etiket ve para birimi kombinasyonlarını seçilemez yapın**. Böylece bu hatayı kullanıcı neredeyse hiç görmez.

---

### 4.5 Bütçe güncelle

```http
PUT /api/budgets/7
Content-Type: application/json

{ "amount": 6000, "periodStartDay": 15 }
```

| Alan | Tip | Zorunlu | Kural |
|---|---|---|---|
| `amount` | number | Evet | `> 0` |
| `periodStartDay` | number | Evet | 1–28 |

**200 OK**: `{ "data": {}, "messages": [], "isSuccess": true }`

**Önemli:**
- **Etiket ve para birimi değiştirilemez.** Body'de gönderilseler bile yok sayılırlar. Düzenleme formunda bu alanları **salt okunur** gösterin. Kullanıcı etiketi değiştirmek isterse "Silip yeniden oluştur" yönlendirmesi yapın.
- `periodStartDay` değişince aktif dönem hemen yeniden hesaplanır. Güncellemeden sonra listeyi veya detayı yeniden çekin.
- Hatalar: `400` (validasyon), `404` (bulunamadı).

---

### 4.6 Bütçe sil

```http
DELETE /api/budgets/7
```

**200 OK**: `{ "data": {}, "messages": [], "isSuccess": true }`

- Silme geri alınamaz (UI açısından). Bir onay diyaloğu gösterin.
- Silinen bütçeye ait geçmiş bildirimler etkilenmez.
- Hatalar: `404`.

**Dolaylı silme:** Kullanıcı bir **etiketi sildiğinde** (`DELETE /api/UserLabels/{id}`), o etikete bağlı bütçeler de otomatik silinir. Etiket silme onay diyaloğunda bunu belirtin: *"Bu etikete bağlı bütçe de silinecek."*

---

## 5. Hata yönetimi

### 5.1 HTTP kodu ve mesaj eşlemesi

| HTTP | Ne zaman | Gövde | Önerilen UI |
|---|---|---|---|
| 400 | Validasyon hatası | `Messages`: bir veya daha fazla mesaj | Form alanının altında veya toast olarak göster |
| 400 | Bozuk JSON / yanlış tip (örn. `amount: "abc"`) | ASP.NET **ProblemDetails** (`{ title, status, errors }`), zarf değil | Genel hata mesajı. Normalde form doğrulaması bunu engellemeli |
| 401 | Token yok veya süresi dolmuş | PascalCase zarf, `Messages` boş | Token yenile veya oturum aç ekranına yönlendir |
| 403 | Yetki yok | PascalCase zarf | Genel yetki mesajı |
| 404 | `"Record Not Found"` | PascalCase zarf | "Bütçe bulunamadı" (listeyi yenile) |
| 404 | `"Record Already Exists"` | PascalCase zarf | "Bu etiket için zaten bir bütçen var" |
| 500 | Beklenmeyen hata | PascalCase zarf, `Messages`'ta teknik mesaj | Genel hata mesajı. **Teknik mesajı kullanıcıya göstermeyin** |

### 5.2 Validasyon mesajları

Validasyon mesajları sunucudaki FluentValidation varsayılan metinleridir ve kullanıcıya gösterilecek kalitede olmayabilir. Örnek: `'Amount' must be greater than '0'.` Önerilen yaklaşım:

1. **İstemci tarafı doğrulamayı** aynı kurallarla yapın ([bölüm 8.3](#83-oluşturma--düzenleme-formu)). Sunucu validasyonu son savunma hattı olsun.
2. Sunucudan 400 gelirse kendi genel mesajınızı gösterin; `Messages`'ı geliştirici konsoluna loglayın.

### 5.3 Hata sınıflandırma örneği

```ts
export type BudgetApiErrorKind =
  | 'validation' | 'unauthorized' | 'forbidden'
  | 'notFound' | 'alreadyExists' | 'server' | 'network';

export function classifyError(status: number, messages: string[]): BudgetApiErrorKind {
  if (status === 400) return 'validation';
  if (status === 401) return 'unauthorized';
  if (status === 403) return 'forbidden';
  if (status === 409 || messages.includes('Record Already Exists')) return 'alreadyExists'; // 409 ileriye dönük
  if (status === 404) return 'notFound';
  return 'server';
}
```

---

## 6. Örnek API istemcisi

Framework'ten bağımsız, `fetch` tabanlı bir istemci. Token alma ve yenileme mantığını mevcut auth katmanınıza bağlayın.

```ts
// budgetsApi.ts
import type {
  ApiEnvelope, BudgetDto, BudgetSummaryDto,
  CreateBudgetRequest, UpdateBudgetRequest,
} from './types';

const BASE = `${import.meta.env.VITE_API_BASE_URL}/api/budgets`;

export class ApiError extends Error {
  constructor(
    public status: number,
    public messages: string[],
    public kind: ReturnType<typeof classifyError>,
  ) {
    super(messages[0] ?? `HTTP ${status}`);
  }
}

/** Başarılı yanıtlar camelCase, hata yanıtları PascalCase döner; ikisini de okur. */
function normalizeEnvelope<T>(raw: any): ApiEnvelope<T> {
  return {
    data: raw?.data ?? raw?.Data,
    messages: raw?.messages ?? raw?.Messages ?? [],
    isSuccess: raw?.isSuccess ?? raw?.IsSuccess ?? false,
  };
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  let res: Response;
  try {
    res = await fetch(`${BASE}${path}`, {
      ...init,
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${await getAccessToken()}`, // mevcut auth katmanınız
        ...init.headers,
      },
    });
  } catch {
    throw new ApiError(0, [], 'network');
  }

  const raw = await res.json().catch(() => null);

  if (!res.ok) {
    // ProblemDetails (bozuk JSON vb.) zarf değildir
    const messages: string[] = raw?.errors
      ? Object.values(raw.errors).flat() as string[]
      : normalizeEnvelope(raw).messages;
    throw new ApiError(res.status, messages, classifyError(res.status, messages));
  }

  return normalizeEnvelope<T>(raw).data;
}

export const budgetsApi = {
  list: (currencyId?: number) =>
    request<BudgetDto[]>(currencyId ? `?currencyId=${currencyId}` : ''),

  summary: () =>
    request<BudgetSummaryDto[]>('/summary'),

  get: (id: number, historyCount = 6) =>
    request<BudgetDto>(`/${id}?historyCount=${historyCount}`),

  create: (body: CreateBudgetRequest) =>
    request<{ id: number }>('', { method: 'POST', body: JSON.stringify(body) }),

  update: (id: number, body: UpdateBudgetRequest) =>
    request<Record<string, never>>(`/${id}`, { method: 'PUT', body: JSON.stringify(body) }),

  remove: (id: number) =>
    request<Record<string, never>>(`/${id}`, { method: 'DELETE' }),
};
```

### React Query kullanıyorsanız (opsiyonel)

```ts
export const budgetKeys = {
  all: ['budgets'] as const,
  list: (currencyId?: number) => [...budgetKeys.all, 'list', currencyId ?? 'all'] as const,
  summary: () => [...budgetKeys.all, 'summary'] as const,
  detail: (id: number, h: number) => [...budgetKeys.all, 'detail', id, h] as const,
};

export const useBudgets = (currencyId?: number) =>
  useQuery({ queryKey: budgetKeys.list(currencyId), queryFn: () => budgetsApi.list(currencyId) });

export const useCreateBudget = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: budgetsApi.create,
    onSuccess: () => qc.invalidateQueries({ queryKey: budgetKeys.all }),
  });
};
```

---

## 7. Veri tazeliği ve cache stratejisi

### 7.1 ⚠️ İşlem sonrası gecikme (eventual consistency)

Bütçe harcaması, işlem tablosundan değil **rapor tablolarından** okunur. Bu tablolar işlem oluşturulduktan veya güncellendikten sonra **arka planda, mesaj kuyruğu üzerinden** güncellenir.

Sonuç: `POST /api/transactions` başarılı döndükten **hemen sonra** `GET /api/budgets` çağrılırsa yeni harcama **henüz yansımamış olabilir**. Gecikme normalde birkaç saniyeden kısadır.

**Önerilen yaklaşım:**

| Tetikleyici | Yapılacak |
|---|---|
| Bütçe oluştur / güncelle / sil | Bütçe sorgularını **hemen** invalidate et (bunlar senkrondur) |
| İşlem oluştur / güncelle / sil | Bütçe sorgularını **gecikmeli** invalidate et (örn. 1,5 sn sonra) ve bir kez daha (örn. 4 sn sonra) |
| Etiket sil | Bütçe sorgularını hemen invalidate et |
| Sayfaya dönüş / pencere odağı | Normal refetch |

```ts
// İşlem mutation'larının onSuccess'ine ekleyin
function refreshBudgetsAfterTransaction(qc: QueryClient) {
  [1500, 4000].forEach(ms =>
    setTimeout(() => qc.invalidateQueries({ queryKey: budgetKeys.all }), ms));
}
```

> İsterseniz işlem eklendiğinde kartı **iyimser (optimistic) olarak** güncelleyebilirsiniz: aynı para birimi, aynı etiket ve aktif dönem içinde bir gider ise `spentAmount += amount`. Ama yüzde, durum ve günlük limit hesabı sunucuyla birebir aynı olmalı ([bölüm 8.2](#82-bütçe-kartı)). Basitlik için gecikmeli refetch önerilir.

### 7.2 Sunucu tarafı cache

Bütçe endpoint'leri sunucuda **cache'lenmez**. İstemci tarafında kısa bir `staleTime` (örn. 30 sn) yeterlidir.

---

## 8. Ekran ve UX kılavuzu

### 8.1 Bütçe listesi

- Sunucunun döndüğü sırayı koruyun: genel bütçe üstte, sonra en çok dolan.
- Üstte bir özet kartı (`/summary`) gösterilebilir.
- Birden fazla para birimi varsa `currencyId` filtresiyle sekme veya segment yapın; ya da kartlarda para birimini belirgin gösterin.
- **Boş durum:** *"Henüz bütçen yok. Aylık harcamalarını kontrol altında tutmak için ilk bütçeni oluştur."* ve "Bütçe oluştur" butonu.

### 8.2 Bütçe kartı

| Öğe | Kaynak | Not |
|---|---|---|
| Başlık | `isGeneral ? "Genel bütçe" : label.name` | Etiket rengi: `label.colorCode` |
| İlerleme çubuğu | `percentage` | Çubuğu **%100'de kırpın** (`Math.min(percentage, 100)`), metinde gerçek değeri gösterin (`%104,8`) |
| Çubuk rengi | `status` | Aşağıdaki tablo |
| "Harcanan / Limit" | `spentAmount` / `amount` | `4.012,50 ₺ / 5.000 ₺` |
| Alt satır | `status` ve diğer alanlar | Aşağıdaki tablo |
| Dönem | `periodStart` – `periodEnd` | `1 Kas – 30 Kas` · `15 gün kaldı` |

| `status` | Renk önerisi | Alt satır metni |
|---|---|---|
| `1` OnTrack | Yeşil / nötr | `987,50 ₺ kaldı · Günde 65,83 ₺ harcayabilirsin` |
| `2` Warning | Turuncu | `987,50 ₺ kaldı · Günde 65,83 ₺ harcayabilirsin` |
| `3` Exceeded | Kırmızı | `Limiti 240 ₺ aştın` (`overAmount`) |

**Kenar durumlar:**
- `percentage` tam `100` ise `status = 3` olur ve `overAmount = 0`. Metin: *"Limitine ulaştın"*.
- `daysLeft = 1` (dönemin son günü): *"Bugün son gün"*.
- `dailyAllowance` yalnızca `status !== 3` iken anlamlıdır.
- Renk eşikleri sunucudaki eşiklerle aynıdır. **Rengi `percentage`'tan kendiniz hesaplamayın, `status`'u kullanın.** Böylece bildirim davranışıyla tutarlı kalır.

### 8.3 Oluşturma / düzenleme formu

| Alan | Bileşen | Kaynak | İstemci doğrulaması |
|---|---|---|---|
| Kapsam | Seçim: "Genel bütçe" veya etiket listesi | `GET /api/UserLabels` | Zaten bütçesi olan etiket ve para birimi kombinasyonlarını devre dışı bırak |
| Para birimi | Seçim | `GET /api/Currencies` (`rank`'e göre sırala) | Zorunlu |
| Aylık limit | Sayı (para) | | `> 0`, en fazla 2 ondalık |
| Dönem başlangıç günü | 1–28 arası seçim | | Varsayılan 1 |

**Dönem başlangıç günü için yardım metni:**
> *Bütçen her ay bu günde yenilenir. Örneğin maaşın ayın 15'inde yatıyorsa 15'i seç; dönemin 15'inden bir sonraki ayın 14'üne kadar sürer.*

Seçim değiştikçe örnek dönemi canlı göstermek kullanıcıya yardımcı olur. Hesaplama kuralı:

```ts
/** Sunucudaki BudgetPeriodCalculator.GetPeriod ile aynı kural (yalnızca önizleme için). */
export function previewPeriod(startDay: number, today = new Date()) {
  const y = today.getFullYear(), m = today.getMonth(), d = today.getDate();
  const start = d >= startDay ? new Date(y, m, startDay) : new Date(y, m - 1, startDay);
  const end = new Date(start.getFullYear(), start.getMonth() + 1, startDay - 1);
  return { start, end }; // end dahil
}
```

**Düzenleme modunda:**
- "Kapsam" ve "Para birimi" **salt okunur** olsun. Yanında *"Değiştirmek için bütçeyi silip yeniden oluştur"* notu bulunsun.
- Yalnızca `amount` ve `periodStartDay` gönderilir.

### 8.4 Bütçe detayı

- Üstte kartın büyük hali (bölüm 8.2).
- **Geçmiş grafiği:** `history` için bar chart. X ekseni dönem (`Haz`, `Tem`, …), Y ekseni `spentAmount`. Bugünkü limit (`amount`) yatay bir referans çizgisi olarak çizilir. Limiti aşan barlar kırmızı olsun. Son bar (aktif dönem) "devam ediyor" olarak farklı tonda gösterilebilir.
- "İşlemleri gör" bağlantısı: `GET /api/transactions` mevcut filtreleriyle (etiket ve tarih aralığı = `periodStart`..`periodEnd`).
- Düzenle ve sil aksiyonları.

### 8.5 Dashboard özeti

`/summary` her para birimi için:
- `totalSpent / totalAmount` için halka veya çubuk grafik
- `3 yolunda · 1 uyarıda · 1 aşıldı` (`onTrackCount`, `warningCount`, `exceededCount`)
- Tıklanınca bütçe listesine gitsin.

### 8.6 Tutar biçimlendirme

```ts
export function formatMoney(amount: number, c: CurrencyDto, locale = 'tr-TR') {
  const n = new Intl.NumberFormat(locale, { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(amount);
  return c.isSymbolRight ? `${n} ${c.symbol}` : `${c.symbol}${n}`;
}
// formatMoney(4012.5, TRY) → "4.012,50 ₺"
```

---

## 9. Push bildirimleri

Bütçe bildirimleri **FCM üzerinden sunucu tarafından** gönderilir. Web uygulamasının bildirimi tetiklemek için bir şey yapmasına gerek yoktur. Web push (FCM) kullanıyorsanız gelen bildirimin `data` alanı şöyledir:

```json
{
  "type": "BudgetThresholdReached",   // veya "BudgetExceeded"
  "signalId": "1234",
  "budgetId": "7",
  "transactionId": "5678"
}
```

| `type` | Ne zaman |
|---|---|
| `BudgetThresholdReached` | Harcama ilk kez %80'i geçtiğinde |
| `BudgetExceeded` | Harcama ilk kez %100'e ulaştığında |

- Tüm değerler **string**'dir (FCM kısıtı).
- Bildirime tıklanınca `/budgets/{budgetId}` detay sayfasına yönlendirin.
- Bildirim alındığında açık olan bütçe sorgularını invalidate edin.
- Aynı dönemde aynı eşik için **tek bildirim** gelir. Tek işlemle iki eşik birden geçilirse yalnızca `BudgetExceeded` gelir.
- Başlık ve metin sunucuda kullanıcının dilinde (TR/EN) hazırlanır.

---

## 10. Bilinen sınırlamalar

Kullanıcıya yansıyabilecek davranışlar. Gerekirse UI'da açıklama metniyle desteklenebilir.

| # | Davranış | UI önerisi |
|---|---|---|
| 1 | **Etiketsiz giderler bütçeye sayılmaz.** Genel bütçe de dahil, yalnızca en az bir etiketi olan giderler hesaba girer | Genel bütçe kartında veya formda: *"Yalnızca etiketli giderler hesaba katılır."* |
| 2 | Etiketsiz girilip **sonradan etiketlenen** işlemler bütçeye yansımaz (backend'de bilinen hata, düzeltilecek) | Şimdilik bir şey gerekmez; QA'de bilinen durum olarak işaretleyin |
| 3 | Para birimleri dönüştürülmez: TRY bütçesi yalnızca TRY giderlerini sayar | Formda para birimi seçiminin yanında kısa not |
| 4 | Limit geçmişi tutulmaz: detay grafiği tüm dönemleri bugünkü limitle karşılaştırır | Grafikte limit çizgisi etiketi: *"Güncel limit"* |
| 5 | İşlem sonrası birkaç saniyelik gecikme | [Bölüm 7.1](#71-️-işlem-sonrası-gecikme-eventual-consistency) |

---

## 11. Kabul / test senaryoları

### Oluşturma
- [ ] Etiket seçilerek bütçe oluşturulur, listede görünür.
- [ ] "Genel bütçe" seçilerek (`userLabelId` gönderilmeden) oluşturulur; kartta "Genel bütçe" yazar ve en üstte görünür.
- [ ] Aynı etiket ve para birimiyle ikinci bütçe denenince "zaten var" mesajı görünür (`404` + `"Record Already Exists"`). Form bu kombinasyonu zaten devre dışı bırakıyorsa bu senaryo API düzeyinde test edilir.
- [ ] Aynı etiketle **farklı para biriminde** bütçe oluşturulabilir.
- [ ] `amount = 0` veya negatif değer istemci tarafında engellenir.
- [ ] `periodStartDay` yalnızca 1–28 arası seçilebilir; önizleme doğru dönemi gösterir.

### Görüntüleme
- [ ] `status` 1/2/3 için doğru renkler ve alt satır metinleri görünür.
- [ ] `percentage > 100` iken çubuk taşmaz, metin gerçek yüzdeyi gösterir, "Limiti X aştın" yazar.
- [ ] `percentage = 100` iken "Limitine ulaştın" yazar.
- [ ] Tarihler gün kayması olmadan gösterilir (`periodEnd` dahil).
- [ ] `isSymbolRight` true/false olan para birimleri doğru biçimlenir.
- [ ] Detayda `history` eskiden yeniye sıralı, son bar aktif dönem.

### Güncelleme / silme
- [ ] Düzenlemede etiket ve para birimi salt okunur.
- [ ] `periodStartDay` değiştirilince kartta yeni dönem tarihleri görünür.
- [ ] Silme onay ister, sonra listeden kalkar.
- [ ] Bütçesi olan bir etiket silinince bütçe de listeden kalkar.

### Veri tazeliği
- [ ] Bütçeli bir etikete gider eklendikten birkaç saniye içinde kartın harcaması güncellenir (sayfa yenilemeden).
- [ ] Gider silinince harcama düşer.

### Hata durumları
- [ ] Süresi dolmuş token → oturum yenileme veya giriş akışı.
- [ ] Başka kullanıcının bütçe id'si ile detay → "Bulunamadı" ekranı.
- [ ] Hata yanıtındaki PascalCase alanlar doğru okunur (mesaj boş veya `undefined` görünmez).
- [ ] Ağ hatasında genel hata mesajı ve "Tekrar dene".
