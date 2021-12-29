using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Interface
{
	public interface IPlugin
	{
		Task StartAsync(IPluginContext context);

		Task StopAsync(IPluginContext context);
	}
}
