using Microsoft.Extensions.Options;
using PluginFactory.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Basic
{
	public class PluginConfigrationOptions<TPlugin, TPluginOptions> : ConfigureFromConfigurationOptions<TPluginOptions> where TPluginOptions : class, new()
	{
		public PluginConfigrationOptions(IPluginConfigrationProvider<TPlugin> provider)
			: base(provider.Configuration)
		{
		}
	}
}
