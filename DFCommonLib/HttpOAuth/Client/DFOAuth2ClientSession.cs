using Microsoft.AspNetCore.Http;

namespace DFCommonLib.HttpApi.OAuth2
{
    public interface IAccountSessionProvider
    {
        void RemoveSession();
        string GetToken();
        void SetToken(string token);
        string GetEmail();
        void SetEmail(string email);
    }

    public class DFOAuth2ClientSession : DFSessionProvider, IAccountSessionProvider
    {
        public static readonly string SessionEmail = "Email";
        public static readonly string SessionTokenKey = "Token";

        public DFOAuth2ClientSession( IHttpContextAccessor httpContext ) : base("DFOAuth2ClientSession",httpContext)
        {
        }

        public DFOAuth2ClientSession(string sessionKey, IHttpContextAccessor httpContext) : base(sessionKey, httpContext)
        {
        }

        public void RemoveSession()
        {
            RemoveConfig(SessionTokenKey);
            RemoveConfig(SessionEmail);
        }

        public string GetEmail()
        {
            return GetConfigString(SessionEmail);
        }

        public void SetEmail(string email)
        {
            SetConfigString(SessionEmail, email);
        }

        public string GetToken()
        {
            return GetConfigString(SessionTokenKey);
        }

        public void SetToken(string token)
        {
            SetConfigString(SessionTokenKey, token);
        }
    }
}
