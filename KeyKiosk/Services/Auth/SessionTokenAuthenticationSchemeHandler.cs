using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace KeyKiosk.Services.Auth
{
	public class SessionTokenAuthenticationSchemeHandler : AuthenticationHandler<SessionTokenAuthenticationSchemeOptions>
	{

		public static readonly string SessionCookieName = "sessionId";

		protected AppAuthenticationSessionStorage sessionStorage;
		protected AppAuthenticationStateProvider stateProvider;
		//protected IDataProtector dataProtector;
		public SessionTokenAuthenticationSchemeHandler(
			AppAuthenticationSessionStorage sessionStorage,
			AppAuthenticationStateProvider stateProvider,
			//IDataProtectionProvider dataProtector,
			IOptionsMonitor<SessionTokenAuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder) : base(options, logger, encoder)
		{
			this.sessionStorage = sessionStorage;
			this.stateProvider = stateProvider;
			//this.dataProtector = dataProtector.CreateProtector($"{GetType().FullName}:{SessionCookieName}");

		}

		protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			// Read the token from request headers/cookies
			// Check that it's a valid session, depending on your implementation

			if (!Request.Cookies.TryGetValue(SessionCookieName, out var sessionCookie)) return AuthenticateResult.NoResult();
			//if (!Request.Cookies.TryGetValue(SessionCookieName, out var sessionCookie)) return BuildNoClaims();
			if (sessionCookie is null) return AuthenticateResult.NoResult();
			//if (sessionCookie is null) return BuildNoClaims();

			var session = sessionStorage.GetSessionById(sessionCookie);

			if (session is null || !session.Valid) return AuthenticateResult.NoResult();
			//if (session is null || !session.Valid) return BuildNoClaims();

			// If the session is valid, return success:
			//var claims = new[] { new Claim(ClaimTypes.Name, "Test") };
			//var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
			var principal = Session.ToClaimsPrincipal(session);
			var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
			await stateProvider.StartSession(session, false);
			return AuthenticateResult.Success(ticket);

			// If the token is missing or the session is invalid, return failure:
			// return AuthenticateResult.Fail("Authentication failed");
		}

		protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
		{
			if (Request.Path.StartsWithSegments("/kiosk"))
			{
				Response.Redirect("/kiosk");
			} else
			{
				Response.Redirect("/");
			}
			
		}

		protected AuthenticateResult BuildNoClaims() => AuthenticateResult.Success(new AuthenticationTicket(new(), null));
	}

	public class SessionTokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
	{

	}

}
