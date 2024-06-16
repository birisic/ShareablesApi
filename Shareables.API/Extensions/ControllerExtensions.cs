using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Shareables.API.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult InternalServerError(this ControllerBase controller, object o)
        {
            return controller.StatusCode(500, o);
        }
    }
}
