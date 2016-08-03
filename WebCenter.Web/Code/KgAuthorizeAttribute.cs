using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebCenter.DAL;
using WebCenter.Entities;
using WebCenter.IServices;

namespace WebCenter.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class KgAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public const string HttpAuthorizationHeader = "Authorization";
        
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException();
            }

            //base.OnAuthorization(filterContext);
            if (!Authenticate(filterContext.HttpContext))
            {
                SetUnauthorizedResult(filterContext);
            }
            else
            {
                if (AuthorizeCore(filterContext.HttpContext))
                {
                    HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                    cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                    cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
                }
                else
                {
                    filterContext.Result = new HttpBasicUnauthorizedResult();
                }
            }
        }

        private bool Authenticate(HttpContextBase context)
        {
            string authHeader = null;

            if (!context.Request.Headers.AllKeys.Contains(HttpAuthorizationHeader))
            {
                if (string.IsNullOrEmpty(authHeader))
                {
                    var cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];

                    if (null == cookie)
                    {
                        return false;
                    }
                    authHeader = "Basic " + cookie.Value;
                    //var decrypted = FormsAuthentication.Decrypt(cookie.Value);
                    //authHeader = decrypted.UserData;
                }
            }
            else
            {
                authHeader = context.Request.Headers[HttpAuthorizationHeader];
            }

            IPrincipal principal;
            if (TryGetPrincipal(authHeader, context, out principal))
            {
                HttpContext.Current.User = principal;
                System.Threading.Thread.CurrentPrincipal = principal;
                return true;
            }
            return false;
        }

        private void SetUnauthorizedResult(AuthorizationContext filterContext)
        {
            string xRequestedWithHeaderKey = "X-Requested-With";
            if (filterContext.RequestContext.HttpContext.Request.Headers.AllKeys.Contains(xRequestedWithHeaderKey) && filterContext.RequestContext.HttpContext.Request.Headers[xRequestedWithHeaderKey].Equals("XMLHttpRequest"))
            {
                //called with the header: "X-Requested-With: XMLHttpRequest"
                // This will allow EMS to see that the request is unatorized and deal with it, and at the same time avoid having the browser to intersect this request with auth dialog
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                // Not EMS calling?
                filterContext.Result = new HttpBasicUnauthorizedResult();
            }
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        private bool TryGetPrincipal(string authHeader, HttpContextBase context, out IPrincipal principal)
        {
            var creds = ParseAuthHeader(authHeader);
            if (creds != null)
            {
                if (TryGetPrincipal(creds[0], creds[1], context, out principal)) return true;
            }

            principal = null;
            return false;
        }

        private string[] ParseAuthHeader(string authHeader)
        {
            // Check this is a Basic Auth header 
            if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("Basic"))
            {
                return null;
            }

            // Pull out the Credentials with are seperated by ':' and Base64 encoded 
            string base64Credentials = authHeader.Substring(6);
            string[] credentials = HttpUtility.UrlDecode(Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials))).Split(new char[] { ':' }, 2);

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
            {
                return null;
            }

            // Okay this is the credentials 
            return credentials;
        }
        
        private bool TryGetPrincipal(string userId, string tokens, HttpContextBase context, out IPrincipal principal)
        {
            if(string.IsNullOrEmpty(userId))
            {
                principal = null;
                return false;
            }

            //userTokens is actually mobile:name
            var userTokens = tokens.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (userTokens.Length != 2)
            {
                principal = null;
                return false;
            }
            if(string.IsNullOrEmpty(userTokens[0]))
            {
                principal = null;
                return false;
            }

            var _id = 0;
            int.TryParse(userId, out _id);
            if(_id == 0)
            {
                principal = null;
                return false;
            }

            var _username = userTokens[0];
            BaseRepository<member> repository = new BaseRepository<member>();
            var _user = repository.GetAll(a => a.id == _id && a.username == _username).Select(u => new LoginResponse
            {
                id = u.id,
                name = u.name,
                username = u.username
            }).FirstOrDefault();

            if (_user == null)
            {
                principal = null;
                return false;
            }

            var identity = new UserIdentity(_user);           
            principal = new GenericPrincipal(identity, null);
            return true;
        }
    }

    public class HttpBasicUnauthorizedResult : HttpUnauthorizedResult
    {
        public HttpBasicUnauthorizedResult() : base() { }
        public HttpBasicUnauthorizedResult(string statusDescription) : base(statusDescription) { }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            context.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic Realm=\"EvaticMobileService\"");
            base.ExecuteResult(context);
        }
    }
}