using System.ComponentModel.DataAnnotations;

namespace PayOut_Aulac_FPT.Attributes
{
    public class RequiredCustomAttribute: RequiredAttribute
    {
        public RequiredCustomAttribute() {
            ErrorMessage = "{0} là bắt buộc";
        }
    }
}
