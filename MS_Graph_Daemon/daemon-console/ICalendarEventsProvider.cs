using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace daemon_console
{
    interface ICalendarEventsProvider
    {
        Task<IEnumerable<CalendarEvent>> GetEventsInMonthAsync(int year, int month);
        System.Threading.Tasks.Task AddEventAsync(CalendarEvent calendarEvent);
    }
}
