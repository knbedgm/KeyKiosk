using KeyKiosk.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace KeyKiosk.Services
{
    public class NavAuthService
    {
        [Inject]
        public required UserSessionService UserSessionService { get; set; }

        public NavAuthService(UserSessionService userSessionService)
        {
            this.UserSessionService = userSessionService;
        }

        /// <summary>
        /// Checks if the current user is allowed to access a given path
        /// </summary>
        /// <param name="path">the path to authorizre</param>
        /// <returns>null if authorized, else the fallback path to redirect to</returns>
        public string? UserCanAccessPath(string path)
        {

            Console.WriteLine($"attemt auth {path} for {UserSessionService.User?.Name}");
            List<string> publicPaths = ["splash"];
            List<string> userPaths = [""];
            userPaths.AddRange(publicPaths);
            List<string> managerPaths = [""];
            managerPaths.AddRange(userPaths);

            var user = UserSessionService.User;

            // list of authorized roles
            List<UserType> requiredRole = [UserType.Admin];

            // allow if public
            if (publicPaths.Contains(path))
            {
                return null;
            }

            // early kick for non-logged in
            if (user is null)
            {
                return publicPaths.First();
            }
            
            // determine what roles are allowed
            if (userPaths.Contains(path))
            {
                requiredRole.Add(UserType.User);
            }
            if (managerPaths.Contains(path))
            {
                requiredRole.Add(UserType.Manager);
            }


            if (requiredRole.Contains(user.UserType))
            {
                return null;
            }

            if (!(UserSessionService.User is null))
            {
                switch (user.UserType)
                {
                    case UserType.User:
                        return userPaths.First();

                    case UserType.Manager:
                        return managerPaths.First();
                }
            }

            return "splash";
        }
    }
}
