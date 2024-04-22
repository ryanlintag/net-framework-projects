using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApp.ApiControllers
{
    [RoutePrefix(BaseApiController.adminGroupRoute + "/test")]
    public class TestApiController : BaseApiController
    {
        // GET api/<controller>
        [Route("")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [Route("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [Route("")]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        [Route("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        [Route("{id}")]
        public void Delete(int id)
        {
        }
    }
}