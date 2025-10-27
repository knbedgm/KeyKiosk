using KeyKiosk.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeyKiosk.Services
{
	public class UserService
	{
		protected readonly ApplicationDbContext db;

		private readonly PasswordHasher<object> passwordHasher = new();

		public UserService(ApplicationDbContext db)
		{
			this.db = db;
		}


		public async Task<User?> GetUserByDesktopLoginAsync(string username, string password)
		{

			var user = await (from u in db.Users
						where u.DesktopLogin != null && u.DesktopLogin.Username == username
						select u).FirstOrDefaultAsync();

			if (user == null) return null;

			switch (passwordHasher.VerifyHashedPassword(null, user.DesktopLogin!.HashedPassword, password))
			{
				case PasswordVerificationResult.Success:
					return user;
				case PasswordVerificationResult.SuccessRehashNeeded:
					//TODO rehash password
					return user;
				case PasswordVerificationResult.Failed:
				default:
					return null;
			}
		}
		public async Task<User?> GetUserByKioskLoginAsync(string pin)
		{
			return await (from u in db.Users where u.Pin == pin select u).FirstOrDefaultAsync();

		}
	}
}
