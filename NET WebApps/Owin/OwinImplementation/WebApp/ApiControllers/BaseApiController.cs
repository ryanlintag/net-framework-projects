using Microsoft.Ajax.Utilities;
using System.Web.Http;

namespace WebApp.ApiControllers
{

    public abstract class BaseApiController : ApiController
    {
        public const string baseApiRoute = "api";
        public const string adminGroupRoute = baseApiRoute + "/admin";
    }
}