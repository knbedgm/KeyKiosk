using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace KeyKiosk.Services.Auth
{
	public class AppAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly UserService userService;
		private readonly AppAuthenticationSessionAccessor sessionAccessor;

		public Session? CurrentSession { get; set; } = null;

		public AppAuthenticationStateProvider(UserService userService, AppAuthenticationSessionAccessor sessionAccessor)
		{
			AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
			this.userService = userService;
			this.sessionAccessor = sessionAccessor;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var principal = new ClaimsPrincipal();

			if (CurrentSession is null)
			{
				CurrentSession = await sessionAccessor.FetchSessionFromBrowserAsync();
			}

			if (CurrentSession is not null)
			{
				if (CurrentSession.Valid)
				{
					principal = sessionAccessor.ToClaimsPrincipal(CurrentSession);
					await StartSession(CurrentSession, false);
				}
				else 
				{
					await StartSession(null, false);
				}
			}

			return new(principal);
		}


		private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
		{
			var authenticationState = await task;

			if (authenticationState is not null)
			{
				CurrentSession = (await sessionAccessor.FromClaimsPrincipal(authenticationState.User));
			}
		}

		public async Task<bool> LoginDesktopAsync(string username, string password)
		{
			var user = await userService.GetUserByDesktopLoginAsync(username, password);

			if (user is not null)
			{
				var session = new Session(user, SessionLoginType.Desktop, true);
				await StartSession(session);
				return false;

			} else
			{
				await StartSession(null);
				return true;
			}
		}

		public async Task<bool> LoginKioskAsync(string pin)
		{
			var user = await userService.GetUserByKioskLoginAsync(pin);

			if (user is not null)
			{
				var session = new Session(user, SessionLoginType.Kiosk, true);
				await StartSession(session);
				return true;

			} else
			{
				await StartSession(null);
				return false;
			}
		}

		public async Task<bool> LoginRFIDAsync(string uid)
		{
			var user = await userService.GetUserByRFIDAsync(uid);

			if (user is not null)
			{
				var session = new Session(user, SessionLoginType.Kiosk, true);
				await StartSession(session);
				return true;

			} else
			{
				await StartSession(null);
				return false;
			}
		}

		public async Task Logout()
		{
			if (CurrentSession is not null)
			{
				CurrentSession.Valid = false;
				
			}
			await StartSession(null);
		}

		internal async Task StartSession(Session? session, bool persist = true)
		{
			CurrentSession = session;
			if (CurrentSession is not null)
			{
				var principal = sessionAccessor.ToClaimsPrincipal(CurrentSession);
				if(persist)
				{
					await sessionAccessor.PersistSession(CurrentSession);
				}
				NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
			} else
			{
				NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
				await sessionAccessor.clearBrowserSession();
			}
		}

	}
}
