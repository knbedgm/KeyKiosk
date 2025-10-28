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

        /// <summary>
        /// Get all  users
        /// </summary>
        /// <returns></returns>
        public List<User> GetAllUsers()
        {
            return db.Users
                     .OrderBy(u => u.Id)
                     .ToList();
        }

        /// <summary>
        /// Get a single user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUserById(int id)
        {
            return db.Users
                     .First(t => t.Id == id);
        }

        /// <summary>
        /// Add a new user to the database
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(User user)
        {
            var newUser = new User { Name = user.Name, Pin = user.Pin, UserType = user.UserType };
            db.Users.Add(newUser);
            db.SaveChanges();
        }

        /// <summary>
        /// Update existing user using id
        /// </summary>
        /// <param name="idToUpdate"></param>
        /// <param name="user"></param>
        public void UpdateUser(User updatedUser)
        {
            var userToUpdate = db.Users.FirstOrDefault(t => t.Id == updatedUser.Id);
            if (userToUpdate != null)
            {
                userToUpdate.Name = updatedUser.Name;
                userToUpdate.Pin = updatedUser.Pin;
                userToUpdate.UserType = updatedUser.UserType;
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Delete existing user using id
        /// </summary>
        /// <param name="idToDelete"></param>
        public void DeleteUser(int idToDelete)
        {
            var userToDelete = db.Users.FirstOrDefault(t => t.Id == idToDelete);
            if (userToDelete != null)
            {
                db.Users.Remove(userToDelete);
            }
            db.SaveChanges();
        }
    }
}
