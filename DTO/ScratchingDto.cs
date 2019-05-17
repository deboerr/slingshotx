using System;

namespace slingshotx.DTO
{
    public class ScratchingDTO
    {
        // meeting
        public Guid MeetingId { get; set; }
        public string Location { get; set; }
        public int? NumberOfRaces { get; set; }
        public string MeetingName { get; set; }
        public string VenueName { get; set; }
        public string ScheduledType { get; set; }
        public string MeetingType { get; set; }
        public string VenueCode { get; set; }
        public string BravoMnemonic { get; set; }
        public string TrackRatingName { get; set; }
        public int? TrackRatingNumeric { get; set; }
        public string Weather { get; set; }
        public DateTimeOffset? MeetingStartTimeUTC { get; set; }
        public string MeetingDate { get; set; }
        public DateTime? NotificationValueAdded { get; set; }
        // race
        public Guid RaceId { get; set; }
        public int RaceNumber { get; set; }
        public string RaceName { get; set; }
        public DateTimeOffset? StartTimeUTC { get; set; }
        public int NumberOfAcceptors { get; set; }
        public int NumberOfStarters { get; set; }
        public int NumberOfScratchings { get; set; }
        // runner
        public Guid RunnerId { get; set; }
        public int RunnerNumber { get; set; }
        public string RunnerName { get; set; }
        public int Barrier { get; set; }
        public string RunnerCode { get; set; }
        public int ProviderScratchStatus { get; set; }
        public string ProviderScratchReason { get; set; }
        public DateTimeOffset? ProviderScratchUpdateTime { get; set; }
        public string BravoScratchingStatus { get; set; }
        public string RiderDriver { get; set; }
        public bool RiderNameChange { get; set; }
    }
}
