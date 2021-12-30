using PluginFactory.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluginFactory.Plugin
{
	internal class PluginContext : IPluginContext
	{
		public IPluginFactory PluginFactory { get; }

		public IServiceProvider ServiceProvider { get; }

		public CancellationToken CancellationToken { get; }

		public PluginContext(IPluginFactory pluginFactory, IServiceProvider serviceProvider, CancellationToken cancellationToken)
		{
			PluginFactory = pluginFactory;
			ServiceProvider = serviceProvider;
			CancellationToken = cancellationToken;
		}
	}

}
