
using PayOut_Aulac_FPT.Core.Utils.Enums;
using PayOut_Aulac_FPT.Core.Utils.Models;
using System.ComponentModel.DataAnnotations;

namespace PayOut_Aulac_FPT.DTO
{
    public class ListPagedRequest<TSearch, TSort>
        where TSearch : BaseSearch
        where TSort : BaseSort
    {
        /// <summary>
        /// Trang
        /// </summary>
        [Required]
        [Display(Name = "Trang")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Số trường của 1 trang
        /// </summary>
        [Required]
        [Display(Name = "Số trường trong 1 trang")]
        public int PageSize { get; set; }

        /// <summary>
        /// Sắp xếp theo
        /// </summary>
        [Display(Name = "Sắp xếp")]
        public TSort? Sort { get; set; }

        /// <summary>
        /// Tìm kiếm theo
        /// </summary>
        [Display(Name = "Tìm kiếm")]
        public TSearch? Search { get; set; }
    }
}
