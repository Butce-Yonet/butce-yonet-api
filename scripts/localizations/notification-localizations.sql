-- Mail ve push notification metinleri (DotBoil.Localization -> Localizations tablosu).
--
-- Gruplar:
--   ButceYonet.Mail          -> Razor mail şablonları (@await Model.Text("...")) ve mail konuları
--   ButceYonet.Notification  -> FCM push başlık/gövdeleri
--
-- Dil kodları "TR" / "EN" (User.ActiveLanguage bu değerlere normalize edilir, bilinmeyen dil TR'ye düşer).
-- {0}, {1}, ... string.Format argümanlarıdır. Mail değerleri HTML içerebilir (<strong> vb.); argümanlar ise
-- kod tarafında HTML-encode edilir.
--
-- Script idempotent'tir: iki grubu silip baştan yazar.
-- ÖNEMLİ: DotBoil.Localization değerleri Redis'te cache'ler (DotBoil:Localization:{Lang}:{Group}:{Key}).
-- Var olan bir metni değiştirdiyseniz ilgili anahtarları Redis'ten silin ya da servisleri yeniden başlatın.

SET NAMES utf8mb4;

START TRANSACTION;

DELETE FROM `Localizations` WHERE `Group` IN ('ButceYonet.Mail', 'ButceYonet.Notification');

INSERT INTO `Localizations` (`Language`, `Group`, `Key`, `Value`) VALUES
-- ===================== MAIL: Ortak =====================
('TR', 'ButceYonet.Mail', 'Common.Greeting', 'Merhaba,'),
('EN', 'ButceYonet.Mail', 'Common.Greeting', 'Hello,'),
('TR', 'ButceYonet.Mail', 'Common.GreetingWithName', 'Merhaba <strong>{0}</strong>,'),
('EN', 'ButceYonet.Mail', 'Common.GreetingWithName', 'Hello <strong>{0}</strong>,'),
('TR', 'ButceYonet.Mail', 'Common.TeamSignature', 'Bütçe Yönet Ekibi'),
('EN', 'ButceYonet.Mail', 'Common.TeamSignature', 'The Bütçe Yönet Team'),
('TR', 'ButceYonet.Mail', 'Common.AutoSentNotice', 'Bu e-posta otomatik olarak gönderilmiştir.'),
('EN', 'ButceYonet.Mail', 'Common.AutoSentNotice', 'This email was sent automatically.'),
('TR', 'ButceYonet.Mail', 'Common.SeeDetailsInApp', 'Detayları uygulama üzerinden görüntüleyebilirsiniz.'),
('EN', 'ButceYonet.Mail', 'Common.SeeDetailsInApp', 'You can see the details in the app.'),
('TR', 'ButceYonet.Mail', 'Common.Income', 'Gelir'),
('EN', 'ButceYonet.Mail', 'Common.Income', 'Income'),
('TR', 'ButceYonet.Mail', 'Common.Expense', 'Gider'),
('EN', 'ButceYonet.Mail', 'Common.Expense', 'Expense'),

-- ===================== MAIL: Hoş geldin =====================
('TR', 'ButceYonet.Mail', 'UserCreated.Subject', 'Bütçe Yönet''e hoş geldiniz'),
('EN', 'ButceYonet.Mail', 'UserCreated.Subject', 'Welcome to Bütçe Yönet'),
('TR', 'ButceYonet.Mail', 'UserCreated.Intro', '<strong>Bütçe Yönet</strong> uygulamasına başarıyla kayıt oldunuz 🎉 Artık gelir ve giderlerinizi tek bir yerden takip edebilir, harcamalarınızı daha bilinçli şekilde yönetebilirsiniz.'),
('EN', 'ButceYonet.Mail', 'UserCreated.Intro', 'You have successfully signed up for <strong>Bütçe Yönet</strong> 🎉 You can now track your income and expenses in one place and manage your spending more consciously.'),
('TR', 'ButceYonet.Mail', 'UserCreated.FeaturesTitle', 'Bütçe Yönet ile:'),
('EN', 'ButceYonet.Mail', 'UserCreated.FeaturesTitle', 'With Bütçe Yönet you can:'),
('TR', 'ButceYonet.Mail', 'UserCreated.Feature1', 'Gelir ve giderlerinizi kategorilere ayırabilirsiniz'),
('EN', 'ButceYonet.Mail', 'UserCreated.Feature1', 'Organize your income and expenses into categories'),
('TR', 'ButceYonet.Mail', 'UserCreated.Feature2', 'Aylık harcama özetlerinizi görüntüleyebilirsiniz'),
('EN', 'ButceYonet.Mail', 'UserCreated.Feature2', 'View your monthly spending summaries'),
('TR', 'ButceYonet.Mail', 'UserCreated.Feature3', 'Bütçenizi kontrol altında tutabilirsiniz'),
('EN', 'ButceYonet.Mail', 'UserCreated.Feature3', 'Keep your budget under control'),
('TR', 'ButceYonet.Mail', 'UserCreated.Contact', 'Eğer herhangi bir sorunuz olursa, bizimle dilediğiniz zaman iletişime geçebilirsiniz.'),
('EN', 'ButceYonet.Mail', 'UserCreated.Contact', 'If you have any questions, feel free to contact us at any time.'),
('TR', 'ButceYonet.Mail', 'UserCreated.Closing', 'Keyifli kullanımlar dileriz,'),
('EN', 'ButceYonet.Mail', 'UserCreated.Closing', 'Enjoy,'),

-- ===================== MAIL: Şifre sıfırlama =====================
('TR', 'ButceYonet.Mail', 'ForgotPassword.Subject', 'Bütçe Yönet - Şifre yenileme kodunuz'),
('EN', 'ButceYonet.Mail', 'ForgotPassword.Subject', 'Bütçe Yönet - Your password reset code'),
('TR', 'ButceYonet.Mail', 'ForgotPassword.RequestReceived', '<strong>Bütçe Yönet</strong> hesabınız için bir şifre yenileme talebi aldık.'),
('EN', 'ButceYonet.Mail', 'ForgotPassword.RequestReceived', 'We received a password reset request for your <strong>Bütçe Yönet</strong> account.'),
('TR', 'ButceYonet.Mail', 'ForgotPassword.UseCode', 'Şifrenizi yenilemek için aşağıdaki <strong>tek kullanımlık doğrulama kodunu (OTP)</strong> kullanabilirsiniz:'),
('EN', 'ButceYonet.Mail', 'ForgotPassword.UseCode', 'Use the <strong>one-time verification code (OTP)</strong> below to reset your password:'),
('TR', 'ButceYonet.Mail', 'ForgotPassword.CodeValidity', 'Bu kod <strong>kısa bir süre</strong> için geçerlidir ve güvenliğiniz için yalnızca bir kez kullanılabilir.'),
('EN', 'ButceYonet.Mail', 'ForgotPassword.CodeValidity', 'This code is valid for <strong>a short time</strong> and, for your security, can only be used once.'),
('TR', 'ButceYonet.Mail', 'ForgotPassword.NotYou', 'Eğer bu şifre yenileme talebini siz oluşturmadıysanız, bu e-postayı dikkate almayabilir veya bizimle iletişime geçebilirsiniz.'),
('EN', 'ButceYonet.Mail', 'ForgotPassword.NotYou', 'If you did not request a password reset, you can ignore this email or contact us.'),
('TR', 'ButceYonet.Mail', 'ForgotPassword.SecurityMatters', 'Güvenliğiniz bizim için önemlidir.'),
('EN', 'ButceYonet.Mail', 'ForgotPassword.SecurityMatters', 'Your security matters to us.'),

-- ===================== MAIL: Tekrarlayan işlemler işlendi =====================
('TR', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.Subject', 'Bütçe Yönet - Tekrarlayan gelir-gider işlendi'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.Subject', 'Bütçe Yönet - Recurring transactions processed'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.Intro', '<strong>{0}</strong> tarihinde planlanan tekrarlayan gelir-gider kayıtlarınız işlendi.'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.Intro', 'Your recurring income and expenses scheduled for <strong>{0}</strong> have been processed.'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ItemsTitle', 'Aşağıdaki tekrarlayan gelir-gider kalemleri defterinize eklenmiştir:'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ItemsTitle', 'The following recurring items have been added to your notebook:'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ColumnItem', 'Kalem'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ColumnItem', 'Item'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ColumnType', 'Tür'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ColumnType', 'Type'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ColumnDescription', 'Açıklama'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ColumnDescription', 'Description'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ColumnAmount', 'Tutar'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionsProcessed.ColumnAmount', 'Amount'),

-- ===================== MAIL: Tekrarlayan kayıt sona erdi =====================
('TR', 'ButceYonet.Mail', 'RecurringTransactionEnded.Subject', 'Bütçe Yönet - Tekrarlayan bir kaydınız sona erdi'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionEnded.Subject', 'Bütçe Yönet - A recurring entry has ended'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionEnded.Intro', '<strong>{0}</strong> tarihi itibarıyla aşağıdaki tekrarlayan kayıtlarınız sona erdi.'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionEnded.Intro', 'As of <strong>{0}</strong>, the following recurring entries have ended.'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionEnded.IncomeTitle', 'Bir gelir kaynağınız sona erdi'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionEnded.IncomeTitle', 'One of your income sources has ended'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionEnded.IncomeText', 'Üzülmeyin, bu bir kayıp değil — sadece planladığınız sürenin sonuna geldiniz. Dilerseniz uygulama üzerinden yeni bir tekrarlayan gelir tanımlayarak devam edebilirsiniz.'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionEnded.IncomeText', 'Don''t worry, this isn''t a loss — you have simply reached the end of the period you planned. You can add a new recurring income in the app whenever you like.'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionEnded.ExpenseTitle', 'Güzel haber! Bir masrafınız sona erdi 🎉'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionEnded.ExpenseTitle', 'Good news! One of your expenses has ended 🎉'),
('TR', 'ButceYonet.Mail', 'RecurringTransactionEnded.ExpenseText', 'Aşağıdaki tekrarlayan gideriniz artık defterinize eklenmeyecek. Bütçenizde biraz daha nefes alanı oluştu, tebrikler!'),
('EN', 'ButceYonet.Mail', 'RecurringTransactionEnded.ExpenseText', 'The recurring expense below will no longer be added to your notebook. Your budget just got a little more breathing room, congratulations!'),

-- ===================== MAIL: Abonelik hatırlatma =====================
('TR', 'ButceYonet.Mail', 'SubscriptionReminder.Subject', 'Bütçe Yönet - Abonelik hatırlatması'),
('EN', 'ButceYonet.Mail', 'SubscriptionReminder.Subject', 'Bütçe Yönet - Subscription reminder'),
('TR', 'ButceYonet.Mail', 'SubscriptionReminder.Intro', 'Abonelik/fatura takibinizde dikkatinizi bekleyen kayıtlar var.'),
('EN', 'ButceYonet.Mail', 'SubscriptionReminder.Intro', 'Some of your subscriptions and bills need your attention.'),
('TR', 'ButceYonet.Mail', 'SubscriptionReminder.OverdueTitle', 'Vadesi geçmiş abonelikleriniz'),
('EN', 'ButceYonet.Mail', 'SubscriptionReminder.OverdueTitle', 'Overdue subscriptions'),
('TR', 'ButceYonet.Mail', 'SubscriptionReminder.OverdueDays', '{0} gün gecikti'),
('EN', 'ButceYonet.Mail', 'SubscriptionReminder.OverdueDays', '{0} day(s) overdue'),
('TR', 'ButceYonet.Mail', 'SubscriptionReminder.UpcomingTitle', 'Vadesi yaklaşan abonelikleriniz'),
('EN', 'ButceYonet.Mail', 'SubscriptionReminder.UpcomingTitle', 'Upcoming subscriptions'),
('TR', 'ButceYonet.Mail', 'SubscriptionReminder.DueToday', 'bugün vadesi doluyor'),
('EN', 'ButceYonet.Mail', 'SubscriptionReminder.DueToday', 'due today'),
('TR', 'ButceYonet.Mail', 'SubscriptionReminder.DueInDays', '{0} gün sonra vadesi doluyor'),
('EN', 'ButceYonet.Mail', 'SubscriptionReminder.DueInDays', 'due in {0} day(s)'),
('TR', 'ButceYonet.Mail', 'SubscriptionReminder.AmountUnknown', 'Tutar belirsiz'),
('EN', 'ButceYonet.Mail', 'SubscriptionReminder.AmountUnknown', 'Amount not set'),
('TR', 'ButceYonet.Mail', 'SubscriptionReminder.Outro', 'Detayları ve ödeme işaretlemeyi uygulama üzerinden yapabilirsiniz.'),
('EN', 'ButceYonet.Mail', 'SubscriptionReminder.Outro', 'You can see the details and mark payments in the app.'),

-- ===================== PUSH: Abonelik / tekrarlayan =====================
-- {0} = vadesi geçmiş sayısı, {1} = yaklaşan sayısı
('TR', 'ButceYonet.Notification', 'SubscriptionReminder.Title', 'Abonelik hatırlatması'),
('EN', 'ButceYonet.Notification', 'SubscriptionReminder.Title', 'Subscription reminder'),
('TR', 'ButceYonet.Notification', 'SubscriptionReminder.Body', '{0} vadesi geçmiş, {1} vadesi yaklaşan aboneliğiniz var.'),
('EN', 'ButceYonet.Notification', 'SubscriptionReminder.Body', 'You have {0} overdue and {1} upcoming subscription(s).'),
-- {0} = işlenen kayıt sayısı
('TR', 'ButceYonet.Notification', 'RecurringTransactionsProcessed.Title', 'Tekrarlayan kayıtlar işlendi'),
('EN', 'ButceYonet.Notification', 'RecurringTransactionsProcessed.Title', 'Recurring entries processed'),
('TR', 'ButceYonet.Notification', 'RecurringTransactionsProcessed.Body', '{0} tekrarlayan gelir-gider kaydınız defterinize eklendi.'),
('EN', 'ButceYonet.Notification', 'RecurringTransactionsProcessed.Body', 'Recurring entries added to your notebook: {0}.'),
-- {0} = sona eren kayıt sayısı
('TR', 'ButceYonet.Notification', 'RecurringTransactionEnded.Title', 'Tekrarlayan kayıt sona erdi'),
('EN', 'ButceYonet.Notification', 'RecurringTransactionEnded.Title', 'Recurring entry ended'),
('TR', 'ButceYonet.Notification', 'RecurringTransactionEnded.Body', '{0} tekrarlayan kaydınız sona erdi.'),
('EN', 'ButceYonet.Notification', 'RecurringTransactionEnded.Body', 'Recurring entries that have ended: {0}.'),

-- ===================== PUSH: EngagementSignal =====================
-- GoalProgressMilestone: {0} = hedef adı, {1} = yüzde
('TR', 'ButceYonet.Notification', 'EngagementSignal.GoalProgressMilestone.Title', 'Hedefine yaklaşıyorsun 🎯'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.GoalProgressMilestone.Title', 'You''re getting closer 🎯'),
('TR', 'ButceYonet.Notification', 'EngagementSignal.GoalProgressMilestone.Body', '"{0}" hedefinde %{1} seviyesine ulaştın, böyle devam!'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.GoalProgressMilestone.Body', 'You''ve reached {1}% of your "{0}" goal, keep going!'),
-- GoalCompleted: {0} = hedef adı
('TR', 'ButceYonet.Notification', 'EngagementSignal.GoalCompleted.Title', 'Hedef tamamlandı 🎉'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.GoalCompleted.Title', 'Goal completed 🎉'),
('TR', 'ButceYonet.Notification', 'EngagementSignal.GoalCompleted.Body', 'Tebrikler! "{0}" hedefini tamamladın.'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.GoalCompleted.Body', 'Congratulations! You completed your "{0}" goal.'),
-- GoalDeadlineApproaching: {0} = hedef adı, {1} = kalan gün, {2} = kalan tutar
('TR', 'ButceYonet.Notification', 'EngagementSignal.GoalDeadlineApproaching.Title', 'Hedef tarihin yaklaşıyor ⏳'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.GoalDeadlineApproaching.Title', 'Your goal deadline is near ⏳'),
('TR', 'ButceYonet.Notification', 'EngagementSignal.GoalDeadlineApproaching.Body', '"{0}" hedefi için {1} gün kaldı. Tamamlamak için {2} daha gerekiyor.'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.GoalDeadlineApproaching.Body', '{1} day(s) left for your "{0}" goal. You need {2} more to complete it.'),
-- StreakMilestone: {0} = gün sayısı
('TR', 'ButceYonet.Notification', 'EngagementSignal.StreakMilestone.Title', '{0} günlük seri 🔥'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.StreakMilestone.Title', '{0}-day streak 🔥'),
('TR', 'ButceYonet.Notification', 'EngagementSignal.StreakMilestone.Body', '{0} gündür her gün kayıt giriyorsun. Harika gidiyorsun!'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.StreakMilestone.Body', 'You''ve logged entries every day for {0} days. Great job!'),
-- InactivityWarning: {0} = son işlemden beri geçen gün
('TR', 'ButceYonet.Notification', 'EngagementSignal.InactivityWarning.Title', 'Seni özledik 👋'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.InactivityWarning.Title', 'We miss you 👋'),
('TR', 'ButceYonet.Notification', 'EngagementSignal.InactivityWarning.Body', '{0} gündür kayıt girmedin. Harcamalarını güncel tutmak için birkaç saniye yeter.'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.InactivityWarning.Body', 'You haven''t logged anything for {0} days. It only takes a few seconds to stay on top of your spending.'),
-- AnomalousSpending: {0} = işlem adı, {1} = tutar, {2} = ortalamaya oranı (örn. 2,3)
('TR', 'ButceYonet.Notification', 'EngagementSignal.AnomalousSpending.Title', 'Olağan dışı harcama'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.AnomalousSpending.Title', 'Unusual spending'),
('TR', 'ButceYonet.Notification', 'EngagementSignal.AnomalousSpending.Body', '"{0}" ({1}) bu kategorideki ortalama harcamanın {2} katı.'),
('EN', 'ButceYonet.Notification', 'EngagementSignal.AnomalousSpending.Body', '"{0}" ({1}) is {2}x your average spending in this category.');

COMMIT;
