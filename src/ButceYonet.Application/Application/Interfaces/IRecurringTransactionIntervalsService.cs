using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Interfaces;

public interface IRecurringTransactionIntervalsService
{
    DateTime CalculateInterval(DateTime currentDate, RecurringTransactionIntervals intervalType,
        int? intervalCount = null);
}