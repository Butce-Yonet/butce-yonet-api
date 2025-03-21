using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Exceptions;

namespace ButceYonet.Application.Infrastructure.Services;

public class RecurringTransactionIntervalsService : IRecurringTransactionIntervalsService
{
    public DateTime CalculateInterval(DateTime currentDate, RecurringTransactionIntervals intervalType, int? interval = null)
    {
        if (intervalType == RecurringTransactionIntervals.Daily)
        {
            return currentDate.AddDays(interval ?? 0);
        }
        else if (intervalType == RecurringTransactionIntervals.Weekly)
        {
            var dayCount = (interval ?? 0) * 7;
            return currentDate.AddDays(dayCount);
        }
        else if (intervalType == RecurringTransactionIntervals.Monthly)
        {
            return currentDate.AddMonths(interval ?? 0);
        }
        else if (intervalType == RecurringTransactionIntervals.Yearly)
        {
            return currentDate.AddYears(interval ?? 0);
        }
        else if (intervalType == RecurringTransactionIntervals.LastDayOfTheMonth)
        {
            var buffDay = currentDate.AddMonths(1);
            int year = buffDay.Year;
            int month = buffDay.Month;
            int lastDay = DateTime.DaysInMonth(year, month);
            
            return new DateTime(year, month, lastDay);
        }
        else if (intervalType == RecurringTransactionIntervals.FirstBusinessDayOfTheMonth)
        {
            var buffDay = currentDate.AddMonths(1);
            buffDay = new DateTime(buffDay.Year, buffDay.Month, 1);
            
            while (buffDay.DayOfWeek == DayOfWeek.Saturday || buffDay.DayOfWeek == DayOfWeek.Sunday)
            {
                buffDay = buffDay.AddDays(1);
            }

            return buffDay;
        }
        else if (intervalType == RecurringTransactionIntervals.LastBusinessDayOfTheMonth)
        {
            var buffDay = currentDate.AddMonths(1);
            int year = buffDay.Year;
            int month = buffDay.Month;
            int lastDay = DateTime.DaysInMonth(year, month);
            buffDay = new DateTime(year, month, lastDay);
            
            while (buffDay.DayOfWeek == DayOfWeek.Saturday || buffDay.DayOfWeek == DayOfWeek.Sunday)
            {
                buffDay = buffDay.AddDays(-1);
            }

            return buffDay;
        }
        else if (intervalType == RecurringTransactionIntervals.XThOfTheMonth)
        {
            var buffDay = currentDate.AddMonths(1);
            int lastDay = DateTime.DaysInMonth(buffDay.Year, buffDay.Month);

            int day = interval ?? 1;
            
            if (day > lastDay)
                day = lastDay;

            return new DateTime(buffDay.Year, buffDay.Month, day);
        }

        throw new NotFoundException(typeof(RecurringTransactionIntervals));
    }
}