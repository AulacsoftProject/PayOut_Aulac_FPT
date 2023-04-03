using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PayOut_Aulac_FPT.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase
    {

        protected Guid? GetIdNguoiDung()
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return value != null ? Guid.Parse(value) : null;
        }

        protected Guid? GetIdCho()
        {
            var value = User.FindFirst(ClaimTypes.PrimarySid)?.Value;
            return value != null ? Guid.Parse(value) : null;
        }
    }
}
