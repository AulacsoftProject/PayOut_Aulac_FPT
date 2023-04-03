using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.DTO.CauHinh;
using PayOut_Aulac_FPT.DTO;
using System.ComponentModel.DataAnnotations;

namespace PayOut_Aulac_FPT.Controllers
{
    [Tags("Cấu hình")]
    [Route("cau-hinh")]
    public class CauHinhController : BaseController
    {
        private readonly ICauHinhService _service;
        private readonly ILogger<CauHinhController> _logger;
        private readonly IMapper _mapper;
        public CauHinhController(ILogger<CauHinhController> logger, ICauHinhService service, IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Danh sách phân trang cấu hình.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("items")]
        public ActionResult<SuccessResponse<ListPagedResponse<CauHinhDTO>>> Items([FromQuery] ListPagedRequest<CauHinhSearch, CauHinhSort> request)
        {
            var result = _service.GetPage(request.PageNumber, request.PageSize, request.Sort, request.Search, out int totalRow);
            var data = result.Select(_mapper.Map<CauHinhDTO>);

            return Ok(new SuccessResponse<ListPagedResponse<CauHinhDTO>>(
                new ListPagedResponse<CauHinhDTO>()
                {
                    Data = data,
                    TotalRow = totalRow,
                }
            ));
        }

        /// <summary>
        /// Thông tin cấu hình.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<SuccessResponse<CauHinhDTO>> Info(Guid id)
        {
            var cauHinh = _service.Get(new CauHinh { Id = id });
            return Ok(new SuccessResponse<CauHinhDTO>(
                _mapper.Map<CauHinhDTO>(cauHinh)
            ));
        }

        /// <summary>
        /// Thông tin cấu hình email
        /// </summary>
        /// <returns></returns>
        [HttpGet("email")]
        public ActionResult<SuccessResponse<CauHinhDTO>> Info()
        {
            return Ok();
        }


        /// <summary>
        /// Tạo mới cấu hình.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("")]
        public ActionResult<SuccessResponse<CauHinhCreateResponse>> Create([FromForm] CauHinhCreateRequest request)
        {
            try
            {
                var result = _service.GetExist(new CauHinh { Ma = request.Ma, Ten = request.Ten });

                if (result.Count() == 0)
                {
                    var idNguoiTao = GetIdNguoiDung();
                    var idCauHinh = _service.Create(new CauHinh
                    {
                        CapNhatBoi = idNguoiTao,
                        TaoBoi = idNguoiTao,
                        Ma = request.Ma,
                        Ten = request.Ten,
                        GiaTri = request.GiaTri,
                        MoTa = request.MoTa
                    });
                    return Ok(new SuccessResponse<CauHinhCreateResponse>(
                        new CauHinhCreateResponse
                        {
                            Id = idCauHinh
                        }
                    ));
                }
                else
                {
                    return Ok(new ErrorResponse("Cấu hình đã tồn tại không thể thêm mới!"));
                }
            }
            catch
            {
                return Ok(new ErrorResponse("Tạo mới cấu hình không thành công!"));
            }
        }

        /// <summary>
        /// Chỉnh sửa toàn bộ thông tin cấu hình.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("")]
        public ActionResult<SuccessResponse<CauHinhDTO>> UpdatePut([FromForm] CauHinhUpdateRequest request)
        {
            try
            {
                var check = _service.CheckExist(new CauHinh { Id = request.Id }, new CauHinh { Ma = request.Ma, Ten = request.Ten });
                if (check.Count() == 0)
                {
                    var idNguoiCapNhat = GetIdNguoiDung();
                    var success = _service.Update(new CauHinh
                    {
                        Id = request.Id,
                        CapNhatBoi = idNguoiCapNhat,
                        TaoBoi = idNguoiCapNhat,
                        Ma = request.Ma,
                        Ten = request.Ten,
                        GiaTri = request.GiaTri,
                        MoTa = request.MoTa
                    });
                    if (success)
                    {
                        var result = _service.Get(new CauHinh { Id = request.Id });
                        return Ok(new SuccessResponse<CauHinhDTO>(
                            _mapper.Map<CauHinhDTO>(result)
                        ));
                    }
                    else
                    {
                        return Ok(new ErrorResponse("Cập nhật cấu hình không thành công!"));
                    }
                }
                else
                {
                    return Ok(new ErrorResponse("Cấu hình đã tồn tại không thể cập nhật!"));
                }
            }
            catch
            {
                return Ok(new ErrorResponse("Cập nhật cấu hình không thành công!"));
            }
        }

        /// <summary>
        /// Chỉnh sửa cấu hình email
        /// </summary>
        /// <returns></returns>
        [HttpPut("email")]
        public ActionResult<SuccessResponse<CauHinhDTO>> UpdateEmail()
        {
            return Ok();
        }

        /// <summary>
        /// Xoá cấu hình.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult<SuccessResponse<string>> Delete([Required] Guid id)
        {
            var success = _service.Delete(new CauHinh { Id = id });
            if (success)
            {
                return Ok(new SuccessResponse<string>(null));
            }
            else
            {
                return Ok(new ErrorResponse("Không thể xoá!"));
            }
        }
    }
}
