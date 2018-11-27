using Autofac;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApp1
{
	//
	// Summary:
	//     Defines the methods that simplify service location and dependency resolution.
	public interface IWork
	{
		void doWork();
	}

	public class TheWork : IWork
	{
		public void doWork()
		{
			throw new NotImplementedException();
		}
	}

	public class SampleDependencyResolver : IDependencyResolver
	{
		private IDependencyResolver baseResolver;

		public SampleDependencyResolver(IDependencyResolver baseResolver)
		{
			this.baseResolver = baseResolver;
		}
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IWork))
			{
				return new TheWork();
			}

			if (baseResolver != null)
			{
				return baseResolver.GetService(serviceType);
			}

			return null;
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return baseResolver.GetServices(serviceType);
			//return Enumerable.Empty<Object>();
		}
	}


	/// <summary>
	/// Structure for Application Control message sent via SB Topic
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class AppControlMessage
	{
		[JsonProperty(PropertyName = "app")]
		public string AppName { get; set; }

		[JsonProperty(PropertyName = "loglevel")]
		public string LogLevel { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}

	public static class WebApiConfig
	{
		static ISubscriptionClient subscriptionClient;

		static async Task ProcessAppControlMessagesAsync(Message message, CancellationToken token)
		{
			string strBody = Encoding.UTF8.GetString(message.Body);

			try
			{
				AppControlMessage controlObject = JsonConvert.DeserializeObject<AppControlMessage>(strBody);
				if (0 == controlObject.AppName.CompareTo("hoho"))
				{
				}
			}
			catch (Exception ex)
			{
				string se = ex.Message;
			}

			// Complete the message so that it is not received again.
			// This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
			if (!token.IsCancellationRequested)
			{
				await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
			}

			// Note: Use the cancellationToken passed as necessary to determine if the subscriptionClient has already been closed.
			// If subscriptionClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
			// to avoid unnecessary exceptions.
		}

		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			//new AutofacWebApiDependencyResolver();
			//ContainerBuilder cb = new ContainerBuilder();

			//var builder = new ContainerBuilder();
			//var container = builder.Build();
			//var gConfig = GlobalConfiguration.Configuration;
			//gConfig.DependencyResolver = new Autofac.Integration.WebApi.AutofacWebApiDependencyResolver(container);



			//DependencyResolver
			DependencyResolver.SetResolver(new SampleDependencyResolver(DependencyResolver.Current));

			AppControlMessage appMsg = new AppControlMessage()
			{
				AppName = "webapi",
				LogLevel = "Fatal"
			};
			string sObj = JsonConvert.SerializeObject(appMsg);

			//tc.ScheduleMessageAsync(new Message(Encoding.UTF8.GetBytes(sObj)), DateTimeOffset.Now);

			// EntityPath=testqueue;
			string testQueuePath = "Endpoint=sb://shieldox-servicebus-dev.servicebus.windows.net/;SharedAccessKeyName=testpolicy;SharedAccessKey=jNrqpkk5kqMNsKtzZIfIr+dZ7uq2gvV1I8j9aswXMUs=;";
			string testEntityPath = "testqueue";
			QueueClient queueClient = new QueueClient(testQueuePath, testEntityPath/*, ReceiveMode receiveMode = ReceiveMode.PeekLock*/);
			queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(sObj))).Wait();


			string sbConnectionString = "Endpoint=sb://shieldox-servicebus-dev.servicebus.windows.net/;SharedAccessKeyName=app.control.topic.access.policy;SharedAccessKey=qU6zO7htfvVoc0Jd6vofesnJkV+kP/NBQz2gnI5teM0=";
			string sbTopic = "app.control.topic";
			string sbSubscription = "subscrtest";

			subscriptionClient = new SubscriptionClient(sbConnectionString, sbTopic, sbSubscription);

			subscriptionClient.RegisterMessageHandler(
				ProcessAppControlMessagesAsync,
				new MessageHandlerOptions(args => {
						//var context = args.ExceptionReceivedContext;
						// context.Endpoint, context.EntityPath, context.Action
						return Task.CompletedTask;
				}
				)
				{
					MaxConcurrentCalls = 1,
					AutoComplete = false
				}
				);


			try
			{
				TopicClient tc = new TopicClient(sbConnectionString, sbTopic);
				//Message message, DateTimeOffset scheduleEnqueueTimeUtc

				//tc.ScheduleMessageAsync(new Message(Encoding.UTF8.GetBytes(sObj)), DateTimeOffset.Now);

				//string strBody = Encoding.UTF8.GetString(message.Body);

				//IQueueClient queueClient = new QueueClient(busConn, queueName);
				//queueClient.SendAsync(new Message(Encoding.ASCII.GetBytes("some text")));
				// to be pulled like string str = Encoding.ASCII.GetString(bytes);
			}
			catch (Exception ex)
			{
				string str = ex.Message;
			}

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
