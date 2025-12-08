using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KeyKiosk.Data
{
	[Index(nameof(Pin), IsUnique = true)]
	public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Pin { get; set; } = null;
        public string? RFIDUid { get; set; } = null;
        public UserDesktopLogin? DesktopLogin { get; set; }
        public UserType UserType { get; set; } = UserType.User;
    }

    public enum UserType
    {
        User,
        Manager,
        Admin,
        Terminated
    }

	[Owned]
	[Index(nameof(Username), IsUnique=true)]
    public class UserDesktopLogin
    {
		public required string Username { get; set; }
		public required string HashedPassword { get; set; }
	}
}
