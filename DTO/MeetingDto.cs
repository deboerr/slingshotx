using System;

namespace slingshotx.DTO
{
    public class MeetingDTO
    {
        public Guid MeetingId { get; set; }

        public string Location { get; set; }

        public int? NumberOfRaces { get; set; }

        public string MeetingName { get; set; }

        public string VenueName { get; set; }

        public string ScheduledType { get; set; }

        public string MeetingType { get; set; }

        public string VenueCode { get; set; }

        public string BravoMnemonic { get; set; }

        public DateTimeOffset? MeetingStartTimeUTC { get; set; }

        public DateTime? NotificationValueAdded { get; set; }
    }
}
