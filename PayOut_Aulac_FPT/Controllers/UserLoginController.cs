using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.DTO;
using PayOut_Aulac_FPT.DTO.UserLogin;

namespace PayOut_Aulac_FPT.Controllers
{
    [Tags("User Login")]
    [Route("api/login")]
    public class UserLoginController : ControllerBase
    {
        private readonly IUserLoginService _userLoginService;
        private readonly ILogger<UserLoginController> _logger;
        private readonly IJWTManagerRepository _jWTManager;
        private readonly IMapper _mapper;

        public UserLoginController(ILogger<UserLoginController> logger, IUserLoginService userLoginService, IJWTManagerRepository jWTManager, IMapper mapper)
        {
            _logger = logger;
            _userLoginService = userLoginService;
            _jWTManager = jWTManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Đăng nhập.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("/api/login")]
        public ActionResult<SuccessResponse<UserLoginResponse>> Login([FromForm] UserLoginRequest body)
        {
            var userLogin = _userLoginService.Login(body.UserPaymentID, body.UserPaymentPassWord);
            if (userLogin != null)
            {
                var data = _mapper.Map<UserLoginDTO>(userLogin);//nguoiDung.Select(_mapper.Map<UserLoginDTO>);

                var token = _jWTManager.GenerateToken(data.UserPaymentID.ToString());
                if (token == null)
                {
                    return Unauthorized(new ErrorResponse(
                        "Invalid Attempt!"
                    ));
                }

                return Ok(new SuccessResponse<UserLoginResponse>(
                    new UserLoginResponse
                    {
                        AccessToken = token.AccessToken,
                        Expiration = token.Expiration,
                        Data = data,
                        //Permission = permision.ToArray()
                    }
                ));
            }
            else
            {
                return Unauthorized(new ErrorResponse(
                    "Tài khoản hoặc mật khẩu không đúng!"
                ));
            }
        }
    }
}
