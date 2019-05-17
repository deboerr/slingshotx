using System;

namespace slingshotx.DTO
{
    public class ReadyToPayDTO
    {
		public Guid RaceId { get; set; }
        public Guid MeetingId { get; set; }
        public string MeetingName { get; set; }
		public int? RaceNumber { get; set; }
		public string SellCode { get; set; }
		public string BravoResults { get; set; }
		public DateTimeOffset? BravoResultsTimeStamp { get; set; }
		public string Scratchings { get; set; }
		public bool HasProtest { get; set; }
		public string Protest { get; set; }
		public string OutCome { get; set; }
		public int? SLATarget { get; set; }
        public long SlaSortKey { get; set; }
    }
}
