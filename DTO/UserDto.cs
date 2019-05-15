using System;

namespace slingshotx.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public string Initials { get; set; }
        public string RoleName { get; set; }
		public Guid ContactId { get; set; }
    }
}
