using KeyKiosk.Data;
using KeyKiosk.Services.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace KeyKiosk.Services
{
    public class KioskNavAuthService
    {
        public required AppAuthenticationStateProvider UserSessionService { get; set; }

        public KioskNavAuthService(AppAuthenticationStateProvider userSessionService)
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
            if (!path.StartsWith("kiosk/")) return null;
            path = path.Substring("kiosk/".Length);

            Console.WriteLine($"attemt auth {path} for {UserSessionService.CurrentSession?.User.Name}");
            List<string> publicPaths = [""];
            List<string> userPaths = ["home"];
            List<string> managerPaths = ["home"];
            userPaths.AddRange(publicPaths);
            managerPaths.AddRange(userPaths);

            var user = UserSessionService.CurrentSession?.User;

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
                return "/kiosk/" + publicPaths.First();
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

            if (!(UserSessionService.CurrentSession?.User is null))
            {
                switch (user.UserType)
                {
                    case UserType.User:
                        return "/kiosk/" + userPaths.First();

                    case UserType.Manager:
                        return "/kiosk/" + managerPaths.First();
                }
            }

            return "/kiosk";
        }
    }
}
