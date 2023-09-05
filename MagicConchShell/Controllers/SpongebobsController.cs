using MagicConchShell.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MagicConchShell.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpongebobsController : ControllerBase
    {

        private readonly SpongebobsContext _context;

        public SpongebobsController(SpongebobsContext context)
        {
            _context = context;
        }


        // GET: api/<SpongebobsController>
        [HttpGet]
        public IEnumerable<SpongebobDatum> Get()
        {
            return _context.SpongebobData.ToList();
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
    }
}
