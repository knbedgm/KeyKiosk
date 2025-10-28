
using KeyKiosk.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
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
		private readonly IJSRuntime jsRuntime;
		private Lazy<IJSObjectReference> cookieAccessorJsRef = new();


		static readonly string SessionStorageKey = "SessionId";

		public AppAuthenticationSessionAccessor(AppAuthenticationSessionStorage sessionStorage, ProtectedLocalStorage browserLocalStorage, IJSRuntime jsRuntime)
		{
			this.sessionStorage = sessionStorage;
			this.browserLocalStorage = browserLocalStorage;
			this.jsRuntime = jsRuntime;
		}

		public async Task<Session?> FetchSessionFromBrowserAsync()
		{
			try // When Blazor Server is rendering at server side, there is no local storage. Therefore, put an empty try catch to avoid error
			{
				var sessionId = await browserLocalStorage.GetAsync<string>(SessionStorageKey);

				if (!sessionId.Success ) return null;

				return sessionStorage.GetSessionById(sessionId.Value);
			}
			catch { }

			return null;

		}

		internal async Task PersistSession(Session session)
		{
			if(!sessionStorage.Sessions.Exists(s => s.Id == session.Id))
			{
				sessionStorage.Sessions.Add(session);
			}
			if (session.LoginType != SessionLoginType.Kiosk)
			{
				try // When Blazor Server is rendering at server side, there is no local storage. Therefore, put an empty try catch to avoid error
				{
					await Task.WhenAll([
						browserLocalStorage.SetAsync(SessionStorageKey, session.Id.ToString()).AsTask(),
					SaveSessionCookie(session.Id),
				]);
				}
				catch { }
			}
		}

		private async Task WaitForJSCookieReference()
		{
			if (cookieAccessorJsRef.IsValueCreated is false)
			{
				cookieAccessorJsRef = new(await jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/CookieStorageAccessor.js"));
			}
		}

		protected async Task SaveSessionCookie(Guid sessionId)
		{
			try // When Blazor Server is rendering at server side, there is no local storage. Therefore, put an empty try catch to avoid error
			{
				await WaitForJSCookieReference();
				await cookieAccessorJsRef.Value.InvokeVoidAsync("set", SessionTokenAuthenticationSchemeHandler.SessionCookieName, sessionId.ToString());
			}
			catch { }
		}

		protected async Task RemoveSessionCookie()
		{
			try // When Blazor Server is rendering at server side, there is no local storage. Therefore, put an empty try catch to avoid error
			{
				await WaitForJSCookieReference();
				await cookieAccessorJsRef.Value.InvokeVoidAsync("remove", SessionTokenAuthenticationSchemeHandler.SessionCookieName);
			} catch { };
		}

		internal void invalidateSession(Session session)
		{
			session.Valid = false;
			sessionStorage.Sessions.Remove(session);
		}

		internal async Task clearBrowserSession()
		{
			try // When Blazor Server is rendering at server side, there is no local storage. Therefore, put an empty try catch to avoid error
			{
				await Task.WhenAll([
					browserLocalStorage.DeleteAsync(SessionStorageKey).AsTask(),
					RemoveSessionCookie(),
				]);
			} catch { }
		}

		internal ClaimsPrincipal ToClaimsPrincipal(Session session)
		{
			return Session.ToClaimsPrincipal(session);
		}

		internal async Task<Session?> FromClaimsPrincipal(ClaimsPrincipal claim)
		{
			return await Session.FromClaimsPrincipal(sessionStorage, claim);
		}
	}

	public class AppAuthenticationSessionStorage
	{
		public List<Session> Sessions { get; set; } = new List<Session>();

		public Session? GetSessionById(string id)
		{
			if (!Guid.TryParse(id, out var sessionGuid)) return null;

			return GetSessionById(sessionGuid);
		}
		public Session? GetSessionById(Guid id)
		{
			return Sessions.FirstOrDefault(s => s.Id == id);
		}
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


		public static ClaimsPrincipal ToClaimsPrincipal(Session session)
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

		public static async Task<Session?> FromClaimsPrincipal(AppAuthenticationSessionStorage sessionStorage, ClaimsPrincipal principal)
		{
			var sessionIdClaim = principal.FindFirstValue(ClaimTypes.Hash);
			if (sessionIdClaim == null) return null;
			return sessionStorage.GetSessionById(sessionIdClaim);
		}
	}

	public enum SessionLoginType { 
		Desktop,
		Kiosk
	}
}
