using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Basic
{
	public class PluginFactoryConfigration
	{
		public static readonly string DEFAULT_CONFIG_KEY = "Plugins";

		public IConfiguration Configuration { get; }

		public PluginFactoryConfigration(IConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			Configuration = configuration.GetSection(DEFAULT_CONFIG_KEY);
		}
	}

}
