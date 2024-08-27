using KeyKiosk.Data;
using Microsoft.AspNetCore.Components;

namespace KeyKiosk.Services
{
    public class UserSessionService
    {
        public User? User { get => _user; }
        User? _user = null;

        private readonly ApplicationDbContext dbContext;
        private readonly NavigationManager navigationManager;

        public UserSessionService(ApplicationDbContext dbContext, NavigationManager navigationManager) {
            this.dbContext = dbContext;
            this.navigationManager = navigationManager;
        }

        public bool Login(string pin)
        {
            var usr = (from user in dbContext.Users where user.Pin == pin select user).FirstOrDefault();

            Console.WriteLine((from user in dbContext.Users where user.UserType == UserType.Admin select user).Count());

            if (usr is null)
                return false;

            _user = usr;
            return true;
        }

        public void Logout()
        {
            _user = null;
            navigationManager.NavigateTo("splash");
        }

    }
}
