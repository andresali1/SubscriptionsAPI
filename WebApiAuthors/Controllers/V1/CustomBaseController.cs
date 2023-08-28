using Microsoft.AspNetCore.Mvc;

namespace WebApiAuthors.Controllers.V1
{
    public class CustomBaseController : ControllerBase
    {
        /// <summary>
        /// Method to get the user Id
        /// </summary>
        /// <returns></returns>
        protected string GetUserId()
        {
            var userClaim = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();

            var userId = userClaim.Value;

            return userId;
        }
    }
}
