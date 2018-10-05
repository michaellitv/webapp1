using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Fluentd;

namespace WebApp1.Controllers
{
	public class ValuesController : ApiController
	{
		public ValuesController()
		{
			Log.Logger = new LoggerConfiguration().CreateLogger();
		}

		// GET api/values
		public IEnumerable<string> Get()
		{
			//var log = new LoggerConfiguration()
			//    .WriteTo.Fluentd()
			//    .CreateLogger();


			/*var log = new LoggerConfiguration()
                .WriteTo.Fluentd(new FluentdHandlerSettings
                {
                    Tag = "My.SampleApp"
                })
                .CreateLogger();*/

			Log.Information("No one listens to me!");


			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}
	}
}
