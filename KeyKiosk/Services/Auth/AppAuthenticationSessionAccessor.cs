
using KeyKiosk.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace KeyKiosk.Services.Auth
{
	public class AppAuthenticationSessionAccessor
	{
		private readonly AppAuthenticationSessionStorage sessionStorage;
		private readonly ProtectedLocalStorage browserLocalStorage;

		static readonly string SessionStorageKey = "SessionId";

		public AppAuthenticationSessionAccessor(AppAuthenticationSessionStorage sessionStorage, ProtectedLocalStorage browserLocalStorage)
		{
			this.sessionStorage = sessionStorage;
			this.browserLocalStorage = browserLocalStorage;
		}

		public async Task<Session?> FetchSessionFromBrowserAsync()
		{
			try // When Blazor Server is rendering at server side, there is no local storage. Therefore, put an empty try catch to avoid error
			{
				var sessionId = await browserLocalStorage.GetAsync<string>(SessionStorageKey);

				if (!sessionId.Success || !Guid.TryParse(sessionId.Value, out var sessionGud)) return null;

				return sessionStorage.Sessions.FirstOrDefault(s => s.Id == sessionGud);
			}
			catch { }

			return null;

		}

		public ClaimsPrincipal ToClaimsPrincipal(Session session)
		{
			var claims = new List<Claim>(new Claim[]
			{
				new(ClaimTypes.Hash, session.Id.ToString()),
				new(ClaimTypes.Role, session.User.UserType.ToString()),
				new("loginType", session.LoginType.ToString()),
			});


			return new ClaimsPrincipal(
				new ClaimsIdentity(claims, "SessionClaim")
			);
		}

		public async Task<Session?> FromClaimsPrincipal(ClaimsPrincipal principal)
		{
			var sessionIdClaim = principal.FindFirstValue(ClaimTypes.Hash);
			if (sessionIdClaim == null) return null;
			if (!Guid.TryParse(sessionIdClaim, out var sessionId)) return null;
			return sessionStorage.Sessions.FirstOrDefault(s => s.Id == sessionId);
		}

		internal async Task persistSession(Session session)
		{
			if(!sessionStorage.Sessions.Exists(s => s.Id == session.Id))
			{
				sessionStorage.Sessions.Add(session);
			}

			if (session.LoginType != SessionLoginType.Kiosk)
			{
				await browserLocalStorage.SetAsync(SessionStorageKey, session.Id.ToString());
			}
		}

		internal async Task clearBrowserSession()
		{
			await browserLocalStorage.DeleteAsync(SessionStorageKey);
		}
	}

	public class AppAuthenticationSessionStorage
	{
		public List<Session> Sessions { get; set; } = new List<Session>();
	}

	//todo move me
	public class Session
	{
		[SetsRequiredMembers]
		public Session(User user, SessionLoginType loginType, bool vaild)
		{
			this.Id = Guid.NewGuid();
			this.User = user;
			this.LoginType = loginType;
			this.Valid = vaild;
		}

		public Guid Id;
		public required User User;
		public required SessionLoginType LoginType;
		public required bool Valid;
	}

	public enum SessionLoginType { 
		Desktop,
		Kiosk
	}
}
