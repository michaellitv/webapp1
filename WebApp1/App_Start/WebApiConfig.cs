using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
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

	public static class WebApiConfig
	{
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
