using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace KeyKiosk.Services.Auth
{
	/// <summary>
	/// Custom authentication state provider that manages user sessions
	/// and translates them to ClaimsPrincipal instances for Blazor.
	/// </summary>
	public class AppAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly UserService userService;
		private readonly AppAuthenticationSessionAccessor sessionAccessor;

		/// <summary>
		/// The currently active session (may be null if no user is logged in).
		/// </summary>
		public Session? CurrentSession { get; set; } = null;

		public AppAuthenticationStateProvider(UserService userService, AppAuthenticationSessionAccessor sessionAccessor)
		{
			// Subscribe to authentication state changes so we can keep CurrentSession updated.
			AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
			this.userService = userService;
			this.sessionAccessor = sessionAccessor;
		}

		/// <summary>
		/// Returns the current authentication state for Blazor.
		/// Attempts to restore a persisted session from the browser if needed.
		/// </summary>
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var principal = new ClaimsPrincipal();

			// Load session from browser storage if not yet loaded.
			if (CurrentSession is null)
			{
				CurrentSession = await sessionAccessor.FetchSessionFromBrowserAsync();
			}

			if (CurrentSession is not null)
			{
				if (CurrentSession.Valid)
				{
					// Convert stored session to ClaimsPrincipal and re-establish session silently.
					principal = sessionAccessor.ToClaimsPrincipal(CurrentSession);
					await StartSession(CurrentSession, false); // 'false' = don't re-persist session.
				}
				else
				{
					// Session exists but is invalid.
					await StartSession(null, false);
				}
			}

			return new(principal);
		}

		/// <summary>
		/// Triggered when the AuthenticationState changes.
		/// Keeps <see cref="CurrentSession"/> synchronized with ClaimsPrincipals.
		/// </summary>
		private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
		{
			var authenticationState = await task;

			if (authenticationState is not null)
			{
				// Extract session info from claims.
				CurrentSession = (await sessionAccessor.FromClaimsPrincipal(authenticationState.User));
			}
		}

		/// <summary>
		/// Attempts a desktop login with username and password.
		/// Returns true on success and false on failure.
		/// </summary>
		public async Task<bool> LoginDesktopAsync(string username, string password)
		{
			var user = await userService.GetUserByDesktopLoginAsync(username, password);

			if (user is not null)
			{
				var session = new Session(user, SessionLoginType.Desktop, true);
				await StartSession(session);
				return true; // Login successful
			}
			else
			{
				await StartSession(null);
				return false; // Login failed
			}
		}

		/// <summary>
		/// Attempts a kiosk login using a PIN code.
		/// Returns true on success and false on failure.
		/// </summary>
		public async Task<bool> LoginKioskAsync(string pin)
		{
			var user = await userService.GetUserByKioskLoginAsync(pin);

			if (user is not null)
			{
				var session = new Session(user, SessionLoginType.Kiosk, true);
				await StartSession(session);
				return true;
			}
			else
			{
				await StartSession(null);
				return false;
			}
		}

		/// <summary>
		/// Attempts login via an RFID UID.
		/// Returns true on success and false on failure.
		/// </summary>
		public async Task<bool> LoginRFIDAsync(string uid)
		{
			var user = await userService.GetUserByRFIDAsync(uid);

			if (user is not null)
			{
				var session = new Session(user, SessionLoginType.Kiosk, true);
				await StartSession(session);
				return true;
			}
			else
			{
				await StartSession(null);
				return false;
			}
		}

		/// <summary>
		/// Logs out the current user and clears persisted session data.
		/// </summary>
		public async Task Logout()
		{
			if (CurrentSession is not null)
			{
				// Mark the session as invalid, but the StartSession(null) call
				// will effectively clear it.
				CurrentSession.Valid = false;
			}

			await StartSession(null);
		}

		/// <summary>
		/// Starts or ends a session, persists it if requested, and updates the Blazor auth state.
		/// </summary>
		/// <param name="session">The new session, or null to log out.</param>
		/// <param name="persist">If true, save the session to browser storage.</param>
		internal async Task StartSession(Session? session, bool persist = true)
		{
			CurrentSession = session;

			if (CurrentSession is not null)
			{
				// Convert session to claims and push auth state.
				var principal = sessionAccessor.ToClaimsPrincipal(CurrentSession);

				if (persist)
				{
					await sessionAccessor.PersistSession(CurrentSession);
				}

				NotifyAuthenticationStateChanged(
					Task.FromResult(new AuthenticationState(principal)));
			}
			else
			{
				// Clear claims and browser session storage.
				NotifyAuthenticationStateChanged(
					Task.FromResult(new AuthenticationState(new())));

				await sessionAccessor.clearBrowserSession();
			}
		}
	}
}
