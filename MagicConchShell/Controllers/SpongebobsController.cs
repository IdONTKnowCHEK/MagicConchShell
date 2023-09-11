using MagicConchShell.Dtos.Webhook;
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

        // GET api/<SpongebobsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SpongebobsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SpongebobsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SpongebobsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("Webhook")]
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
