using System.ComponentModel.DataAnnotations;

namespace PayOut_Aulac_FPT.Attributes
{
    public class StringLengthRangeAttribute : StringLengthAttribute
    {

        public StringLengthRangeAttribute(int minimumLength, int maximumLength) : base(maximumLength)
        {
            MinimumLength = minimumLength;
            ErrorMessage = "Độ dài {0} trong khoảng [{1}, {2}]";
        }
    }
}
