using Microsoft.AspNetCore.Mvc;

namespace MagicConchShell.Filter
{
    public class LineVerifySignatureAttribute : TypeFilterAttribute
    {
        public LineVerifySignatureAttribute() : base(typeof(LineVerifySignatureFilter))
        {
        }
    }
}
