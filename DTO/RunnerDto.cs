using System;

namespace slingshotx.DTO
{
    public class RunnerDTO
    {
        public Guid RaceId { get; set; }
        public Guid UniqueContestantId { get; set; }
        public int? ContestantNumber { get; set; }
        public string ContestantName { get; set; }
        public int? Barrier { get; set; }
        public bool? Emergency { get; set; }
        public string Rider { get; set; }
        public string Trainer { get; set; }
        public bool? Blinkers { get; set; }
        public bool? HasColours { get; set; }
        public string RunnerCode { get; set; }
        public int? ProviderScratchStatus { get; set; }
        public string ProviderScratchReason { get; set; }
        public DateTimeOffset? ProviderScratchUpdateTime { get; set; }
        public string BravoScratchingStatus { get; set; }
    }
}
