using ServiceStack;
using ServiceStack.Auth;

namespace Durwella.UrlShortening.Web
{
    class AlwaysAuthorizedAuthProvider : AuthProvider
    {
        public override object Authenticate(IServiceBase authService, IAuthSession session, Authenticate request)
        {
            return new AuthenticateResponse
            {
                UserName = "Anonymous",
                UserId = "Anonymous",
                DisplayName = "Anonymous User",
                SessionId = session.Id,
            };
        }

        public override bool IsAuthorized(IAuthSession session, IAuthTokens tokens, Authenticate request = null)
        {
            return true;
        }
    }
}