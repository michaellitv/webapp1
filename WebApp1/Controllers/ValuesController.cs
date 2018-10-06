﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Fluentd;
using Serilog.Sinks.SystemConsole;

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

	public class ValuesController : ApiController
	{
		private Logger log;

		public ValuesController()
		{
			// this LoggerSinkConfiguration loggerSinkConfiguration, string host, int port, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Debug
			log = new LoggerConfiguration()//.WriteTo.Console()
				.WriteTo.Fluentd("10.101.0.4", 24224 )
				//.WriteTo.File("logogo.txt")
				.CreateLogger();
			Log.Logger = new LoggerConfiguration().CreateLogger();
		}

		// GET api/values
		public IEnumerable<string> Get()
		{
			//var log = new LoggerConfiguration()
			//    .WriteTo.Fluentd()
			//    .CreateLogger();


			//new Serilog.Sinks.Fluentd..FluentdHandlerSettings { };
			/*var log = new LoggerConfiguration()
                .WriteTo.Fluentd(new FluentdHandlerSettings
                {
                    Tag = "My.SampleApp"
                })
                .CreateLogger();*/

			var info = new LogMessage
			{
				Tag = "My.App",
				RequestId = "239423049FL",
				Component = "Startup",
				Method = "Configure",
				Message = "I did stuff",
				SimpleList = new[] { 9, 8, 7 },
				ComplexList = new[] { new SubSub { Id = 1, Name = "Vicent" }, new SubSub { Id = 2, Name = "Jules" } },
				ComplexDictionary = new Dictionary<string, object> { { "Id", 1 }, { "Name", "Fred" }, { "Sub", new SubSub { Id = 6, Name = "Joe" } } }
			};

			log.Information("{@info}", info);

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
