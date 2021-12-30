using Microsoft.Extensions.DependencyInjection;
using PluginFactory.Basic;
using PluginFactory.Interface;
using System;
using System.Linq;

namespace PluginFactory.Plugin
{
    public class PluginInitContext : IPluginInitContext
	{
		public IPluginLoader PluginLoader { get; }

		public IServiceCollection ServiceCollection { get; }

		public IServiceProvider InitServiceProvider { get; }

		public string PluginPath { get; }

		public PluginInitContext(string pluginPath, IPluginLoader pluginLoader, IServiceCollection services)
		{
			PluginLoader = pluginLoader;
			ServiceCollection = services;
			PluginPath = pluginPath;
			IServiceCollection serviceCollection = new ServiceCollection();
			foreach (ServiceDescriptor service in services)
			{
				serviceCollection.Add(service);
			}
			foreach (PluginInfo item in pluginLoader.PluginList.Where((PluginInfo x) => x.IsEnable && x.CanInit))
			{
				serviceCollection.AddSingleton(typeof(ISupportInitPlugin), item.PluginType);
			}
			InitServiceProvider = serviceCollection.BuildServiceProvider();
		}
	}
}
