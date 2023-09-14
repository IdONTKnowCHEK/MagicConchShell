using MagicConchShell.Dtos.Webhook;
using MagicConchShell.Filter;
using MagicConchShell.Models;
using MagicConchShell.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MagicConchShell.Controllers
{
    [Route("/")]
    [ApiController]
    public class SpongebobsController : ControllerBase
    {

        private readonly SpongebobsContext _spongebobsContextContext;
        private readonly LineBotService _lineBotService;


        public SpongebobsController(SpongebobsContext spongebobsContextContext, LineBotService lineBotService)
        {
            _lineBotService = lineBotService;
            _spongebobsContextContext = spongebobsContextContext;
        }


        // GET: api/<SpongebobsController>
        

        // GET: api/<SpongebobsController>
        [HttpGet]
        public IEnumerable<SpongebobDatum> Get()
        {
            return _spongebobsContextContext.SpongebobData.ToList();
        }

        [HttpPost("Webhook")]
        [LineVerifySignature]
        public IActionResult Webhook(WebhookRequestBodyDto body)
        {
            _lineBotService.ReceiveWebhook(body, _spongebobsContextContext.SpongebobData.ToList());
            return Ok();
        }

        [HttpPost("SendMessage/Broadcast")]
        public IActionResult Broadcast([Required] string messageType, object body)
        {
            _lineBotService.BroadcastMessageHandler(messageType, body);
            return Ok();
        }
    }
}
