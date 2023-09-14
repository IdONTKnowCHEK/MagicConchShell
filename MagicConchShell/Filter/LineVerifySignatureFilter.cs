using MagicConchShell.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace MagicConchShell.Filter
{
    public class LineVerifySignatureFilter : IAuthorizationFilter
    {
        private readonly string accessToken;
        private readonly string channelSecret;
        public LineVerifySignatureFilter(IOptions<LineBotSettings> lineBotSettings)
        {
            accessToken = lineBotSettings.Value.AccessToken;
            channelSecret = lineBotSettings.Value.ChannelSecret;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.Request.EnableBuffering();

            string requestBody = new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync().Result;
            context.HttpContext.Request.Body.Position = 0;

            var xLineSignature = context.HttpContext.Request.Headers["X-Line-Signature"].ToString();
            var channelSecretKey = Encoding.UTF8.GetBytes(channelSecret);
            var body = Encoding.UTF8.GetBytes(requestBody);

            using (HMACSHA256 hmac = new HMACSHA256(channelSecretKey))
            {
                var hash = hmac.ComputeHash(body, 0, body.Length);
                var xLineBytes = Convert.FromBase64String(xLineSignature);
                if (SlowEquals(xLineBytes, hash) == false)
                {
                    context.Result = new ForbidResult();
                }
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }
    }
}
