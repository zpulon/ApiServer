using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using PluginFactory.Basic;
using PluginFactory.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginFactory.Plugin
{
    public static class PluginFactoryServiceCollectionExtensions
	{
		public static readonly string DEFAULT_PLUGIN_PATH = "Plugins";

		public static IServiceCollection AddPluginFactory(this IServiceCollection services)
		{
			services.AddOptions();
			PluginFactoryOptions options = createDefaultOptions();
			services.AddPluginFactory(options, null);
			return services;
		}

		public static IServiceCollection AddPluginFactory(this IServiceCollection services, IConfiguration configuration)
		{
			return services.AddPluginFactory(configuration, (Assembly)null);
		}

		public static IServiceCollection AddPluginFactory(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
		{
			return services.AddPluginFactory(configuration, new Assembly[1] { assembly });
		}

		public static IServiceCollection AddPluginFactory(this IServiceCollection services, IConfiguration configuration, IEnumerable<Assembly> assemblies)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			PluginFactoryConfigration pluginFactoryConfigration = new PluginFactoryConfigration(configuration);
			services.TryAddSingleton(pluginFactoryConfigration);
			PluginFactoryOptions pluginFactoryOptions = createDefaultOptions();
			if (assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
				{
					if (!(assembly == null))
					{
						pluginFactoryOptions.AddAssembly(assembly);
					}
				}
			}
			pluginFactoryOptions.ConfigFromConfigration(pluginFactoryConfigration);
			services.AddPluginFactory(pluginFactoryOptions, configuration);
			return services;
		}

		public static IServiceCollection AddPluginFactory(this IServiceCollection services, Action<PluginFactoryOptions> configureOptions)
		{
			return services.AddPluginFactory(null, configureOptions);
		}

		public static IServiceCollection AddPluginFactory(this IServiceCollection services, IConfiguration configuration, Action<PluginFactoryOptions> configureOptions)
		{
			PluginFactoryOptions pluginFactoryOptions = createDefaultOptions();
			configureOptions?.Invoke(pluginFactoryOptions);
			services.AddPluginFactory(pluginFactoryOptions, configuration);
			return services;
		}

		public static IServiceCollection AddPluginFactory(this IServiceCollection services, PluginFactoryOptions options, IConfiguration configuration)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			services.AddLogging();
			services.AddOptions();
			services.TryAddSingleton(options);
			if (configuration == null)
			{
				configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
			}
			services.TryAddSingleton(configuration);
			services.TryAddSingleton(new PluginFactoryConfigration(configuration));
			services.TryAddSingleton(typeof(IPluginConfigrationProvider<>), typeof(PluginConfigrationProvider<>));
			services.TryAddSingleton(typeof(IOptionsChangeTokenSource<>), typeof(ConfigurationChangeTokenSource<>));
			services.TryAddSingleton<IPluginFactory, DefaultPluginFactory>();
			services.AddHostedService((IServiceProvider sp) => sp.GetRequiredService<IPluginFactory>());
			IPluginLoader instance = createPluginLoader(services, options);
			services.TryAddSingleton(instance);
			return services;
		}

		private static PluginFactoryOptions createDefaultOptions()
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DEFAULT_PLUGIN_PATH);
			PluginFactoryOptions pluginFactoryOptions = new PluginFactoryOptions
			{
				PluginPath = text
			};
			if (Directory.Exists(text))
			{
				pluginFactoryOptions.FileProvider = new PhysicalFileProvider(text);
			}
			return pluginFactoryOptions;
		}

		private static IPluginLoader createPluginLoader(IServiceCollection services, PluginFactoryOptions options)
		{
			IPluginLoader pluginLoader = new DefaultPluginLoader(options, services);
			pluginLoader.Load();
			foreach (PluginInfo plugin in pluginLoader.PluginList)
			{
				if (plugin.IsEnable)
				{
					services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IPlugin), plugin.PluginType));
				}
			}
			foreach (PluginInfo item in pluginLoader.PluginList.Where((PluginInfo x) => x.CanConfig && x.IsEnable).ToList())
			{
				Type service = typeof(IConfigureOptions<>).MakeGenericType(item.ConfigType);
				Type implementationType = typeof(PluginConfigrationOptions<,>).MakeGenericType(item.PluginType, item.ConfigType);
				services.TryAddEnumerable(ServiceDescriptor.Singleton(service, implementationType));
			}
			pluginLoader.Init();
			return pluginLoader;
		}
	}

}
