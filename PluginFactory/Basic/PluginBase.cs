using PluginFactory.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Basic
{
	public abstract class PluginBase : IPlugin
	{
		public virtual Task StartAsync(IPluginContext context)
		{
			return Task.CompletedTask;
		}

		public virtual Task StopAsync(IPluginContext context)
		{
			return Task.CompletedTask;
		}
	}
}
