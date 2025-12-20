using UniTrack.Domain.Entities;
using UniTrack.Domain.Enums;

namespace UniTrack.Application.Common.EventTime
{
    public static class EventTimeHelper
    {
        public static Time Calculate(Event e, DateTime now)
        {
            // 1️⃣ İptal her şeyden önce gelir
            if (!e.IsActived || e.Time == Time.Cancelled)
                return Time.Cancelled;

            // 2️⃣ Gelecek
            if (now < e.StartDate)
                return Time.Future;

            // 3️⃣ Devam ediyor
            if (now >= e.StartDate && now <= e.EndDate)
                return Time.Ongoing;

            // 4️⃣ Geçmiş
            return Time.Past;
        }
    }
}
