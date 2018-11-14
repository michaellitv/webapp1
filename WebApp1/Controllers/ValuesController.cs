using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Microsoft.Azure.ServiceBus;


using System;
using System.Text;
//using System.Web.Script.Serialization;

namespace WebApp1.Controllers
{
	public class LogMessage
	{
		public string Tag { get; set; }

		public string RequestId { get; set; }

		public string Component { get; set; }

		public string Method { get; set; }

		public string Message { get; set; }

		public IEnumerable<int> SimpleList { get; set; }

		public IEnumerable<SubSub> ComplexList { get; set; }

		public Dictionary<string, object> ComplexDictionary { get; set; }
	}

	public class SubSub
	{
		public int Id { get; set; }

		public string Name { get; set; }
	}

	//[Produces("application/json")]
	//[Route("api/[controller]")]
	[System.Web.Http.RoutePrefix("api/values")]
	public class ValuesController : ApiController
	{
		private Logger log;
		private IWork work;

		public ValuesController()
		{
			//this.work = work;

			//this.work.doWork();
			//System.Web.Http.
			// this LoggerSinkConfiguration loggerSinkConfiguration, string host, int port, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Debug
			log = new LoggerConfiguration()
				//.WriteTo.Fluentd("10.101.0.4", 24224 )
				.WriteTo.Fluentd(new FluentdHandlerSettings
				{
					Tag = "My.App",
					Host = "10.101.0.4",
					Port = 24224
				})
				.CreateLogger();
			Log.Logger = new LoggerConfiguration().CreateLogger();
		}

		[System.Web.Http.Route("~/my/json")]
		public System.Web.Http.Results.JsonResult<string> GetJn()
		{
			string busConn = "";
			string queueName = "qname";
			try
			{
				IQueueClient queueClient = new QueueClient(busConn, queueName);
				queueClient.SendAsync(new Message(Encoding.ASCII.GetBytes("some text")));
				// to be pulled like string str = Encoding.ASCII.GetString(bytes);
			}
			catch (Exception ex)
			{
				return Json("Failed with exception");
			}

			return Json("OK");
		}

		[System.Web.Http.Route("~/customers/{id}/orders")]
		//[System.Web.Http.Route("customers/{customerId}/orders")]
		//[System.Web.Http.HttpGet]
		public string GetAny(int id)
		{
			return "ok" + id;
		}

		[System.Web.Http.Route("the/sub")]
		//[System.Web.Http.Route("customers/{customerId}/orders")]
		//[System.Web.Http.HttpGet]
		public string GetSub()
		{
			return "ok-sub";
		}

		// GET api/values
		public IEnumerable<string> Get()
		{

			var info = new LogMessage
			{
				RequestId = "239423049FL",
				Component = "Startup",
				Method = "Configure",
				Message = "I did stuff",
				SimpleList = new[] { 9, 8, 7 },
				ComplexList = new[] { new SubSub { Id = 1, Name = "Vicent" }, new SubSub { Id = 2, Name = "Jules" } },
				ComplexDictionary = new Dictionary<string, object> { { "Id", 1 }, { "Name", "Fred" }, { "Sub", new SubSub { Id = 6, Name = "Joe" } } }
			};

			var info1 = new {
				First = "Mister",
				Second = "Twister",
				Age = 50,
				tp = 1
			};
			log.Warning("{@info1}", info1);
			log.Error("{@info}", new { Name = "myname", Surname = "mysurname" } );

			log.Information("{@info}", info);

			var count = 456;
			log.Information("Retrieved {Count} records", count);

			log.Warning("{@info.RequestId}", info.RequestId, count );


			log.Error( "Manual - error report" );

			Log.Error("No one listens to me!");


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
