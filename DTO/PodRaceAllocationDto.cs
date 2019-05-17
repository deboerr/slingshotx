using System;

namespace slingshotx.DTO
{
    public class PodRaceAllocationDTO
    {
        public Guid RaceId { get; set; }
        public Guid MeetingId { get; set; }
        public Guid UserSessionId { get; set; }
        public int? EventNumber { get; set; }
        public string EventName { get; set; }
        public DateTimeOffset? StartTimeUTC { get; set; }
        public string Stage { get; set; }
        public string MeetingName { get; set; }
        public string VenueLocation { get; set; }
        public string MeetingType { get; set; }
        public string SellCode { get; set; }
        public int? PodNumber { get; set; }
        public int? SeatNumber { get; set; }
        public string BravoStatus { get; set; }
        public string BravoResults { get; set; }
        public string Landline { get; set; }
        public string Initials { get; set; }
        public Guid? UserId { get; set; }
        public int? Distance { get; set; }
        public string Track { get; set; }
        public string Weather { get; set; }
        public string Role { get; set; }
        public DateTimeOffset? FirstRaceStartTimeUTC { get; set; }
    }
}
