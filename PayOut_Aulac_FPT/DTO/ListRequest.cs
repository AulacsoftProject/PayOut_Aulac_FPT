using PayOut_Aulac_FPT.Core.Utils.Models;
using System.ComponentModel.DataAnnotations;

namespace PayOut_Aulac_FPT.DTO
{
    public class ListRequest<TSearch, TSort>
        where TSearch : BaseSearch
        where TSort : BaseSort
    {
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
