using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PluginFactory.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Plugin
{
	public static class HostBuilderExtensions
	{
		public static IHostBuilder UsePluginFactory(this IHostBuilder hostBuilder)
		{
			return hostBuilder.UsePluginFactory((IConfiguration)null);
		}

		public static IHostBuilder UsePluginFactory(this IHostBuilder hostBuilder, IConfiguration configuration)
		{
			return hostBuilder.UsePluginFactory(configuration, (Assembly)null);
		}

		public static IHostBuilder UsePluginFactory(this IHostBuilder hostBuilder, IConfiguration configuration, Assembly assembly)
		{
			return hostBuilder.UsePluginFactory(configuration, new Assembly[1] { assembly });
		}

		public static IHostBuilder UsePluginFactory(this IHostBuilder hostBuilder, IConfiguration configuration, IEnumerable<Assembly> assemblies)
		{
			hostBuilder.ConfigureServices(delegate (HostBuilderContext context, IServiceCollection sc)
			{
				configuration = configuration ?? context.Configuration;
				sc.AddPluginFactory(configuration, assemblies);
			});
			return hostBuilder;
		}

		public static IHostBuilder UsePluginFactory(this IHostBuilder hostBuilder, Assembly assembly)
		{
			return hostBuilder.UsePluginFactory(new Assembly[1] { assembly });
		}

		public static IHostBuilder UsePluginFactory(this IHostBuilder hostBuilder, IEnumerable<Assembly> assemblies)
		{
			return hostBuilder.UsePluginFactory(null, assemblies);
		}

		public static IHostBuilder UsePluginFactory(this IHostBuilder hostBuilder, Action<PluginFactoryOptions> options)
		{
			return hostBuilder.UsePluginFactory(null, options);
		}

		public static IHostBuilder UsePluginFactory(this IHostBuilder hostBuilder, IConfiguration configuration, Action<PluginFactoryOptions> configureOptions)
		{
			hostBuilder.ConfigureServices(delegate (HostBuilderContext context, IServiceCollection sc)
			{
				configuration = configuration ?? context.Configuration;
				sc.AddPluginFactory(configuration, configureOptions);
			});
			return hostBuilder;
		}
	}
}
