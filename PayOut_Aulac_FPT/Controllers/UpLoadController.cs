using Microsoft.AspNetCore.Mvc;

namespace PayOut_Aulac_FPT.Controllers
{
    [Tags("Quản lý file")]
    [ApiController]
    [Route("upload")]
    public class UpLoadController : ControllerBase
    {
        private readonly IConfiguration _config;
        public UpLoadController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [HttpGet("{parent}/{name}")]
        public IActionResult Get(string parent, string name)
        {
            string url = $"{_config["Minio:Protocol"]}://{_config["Minio:Domain"]}:{_config["Minio:Port"]}/{_config["Minio:BucketName"]}/{parent}/{name}";
            return Redirect(url);
        }
    }
}
