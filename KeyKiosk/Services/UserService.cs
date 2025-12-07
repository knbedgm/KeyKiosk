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

		public async Task SetUserDesktopLoginAsync(int userId, string username, string password)
		{

            var user = await GetUserById(userId);
            if (user is null)
            {
                throw new ArgumentException("No user with that ID", "userId");
            }

			var hash = passwordHasher.HashPassword(null, password);

            user.DesktopLogin = new UserDesktopLogin { Username = username, HashedPassword = hash };

            await db.SaveChangesAsync();
		}

		public async Task<User?> GetUserByKioskLoginAsync(string pin)
		{
			return await (from u in db.Users where u.Pin == pin select u).FirstOrDefaultAsync();

		}

		public async Task<User?> GetUserByRFIDAsync(string uid)
		{
			return await (from u in db.Users where u.RFIDUid == uid select u).FirstOrDefaultAsync();

		}

        /// <summary>
        /// Get all  users
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllAsync()
        {
            return await db.Users
                .OrderBy(u => u.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get a single user by id
        /// </summary>
        /// <param name="id">Id of user to get from database</param>
        /// <returns></returns>
        public async Task<User?> GetUserById(int id)
        {
            return await db.Users.FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Add a new user to the database
        /// </summary>
        /// <param name="newUser">NewUser to add</param>
        public async Task<User> AddAsync(User newUser)
        {
            db.Users.Add(newUser);
            await db.SaveChangesAsync();
            return newUser;
        }

        /// <summary>
        /// Update existing user using id
        /// </summary>
        /// <param name="id">Id of user to update</param>
        /// <param name="updated">Updated user values</param>
    public async Task UpdateAsync(int id, User updated)
        {
            var userToUpdate = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userToUpdate is null) return;
            userToUpdate.Name = updated.Name ?? userToUpdate.Name;
            userToUpdate.Pin = updated.Pin ?? userToUpdate.Pin;
            userToUpdate.UserType = updated.UserType;
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete existing user using id
        /// </summary>
        /// <param name="id">Id of user to delete</param>
        public async Task DeleteAsync(int id)
        {
            var userToDelete = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userToDelete is null) return;
            db.Users.Remove(userToDelete);
            await db.SaveChangesAsync();
        }
    }
}
