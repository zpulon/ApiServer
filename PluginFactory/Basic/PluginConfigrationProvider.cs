using Microsoft.Extensions.Configuration;
using PluginFactory.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Basic
{
	public class PluginConfigrationProvider<TPlugin> : IPluginConfigrationProvider<TPlugin> where TPlugin : IPlugin
	{
		public const string DEFAULT_SHARE_KEY = "_Share";

		public IConfiguration Configuration { get; }

		public PluginConfigrationProvider(PluginFactoryConfigration configration)
		{
			if (configration == null)
			{
				throw new ArgumentNullException("configration");
			}
			Type typeFromHandle = typeof(TPlugin);
			string fullName = typeof(TPlugin).FullName;
			IConfigurationSection section = configration.Configuration.GetSection(fullName);
			if (!section.Exists())
			{
				fullName = fullName.Replace("+", ".");
				section = configration.Configuration.GetSection(fullName);
			}
			PluginAttribute pluginAttribute = typeFromHandle.GetCustomAttributes(typeof(PluginAttribute), inherit: false).OfType<PluginAttribute>().FirstOrDefault();
			IConfigurationSection configurationSection = null;
			if (pluginAttribute != null && !string.IsNullOrEmpty(pluginAttribute.Alias))
			{
				fullName = pluginAttribute.Alias;
				IConfigurationSection section2 = configration.Configuration.GetSection(fullName);
				if (section2.Exists())
				{
					configurationSection = section2;
				}
			}
			IConfigurationSection section3 = configration.Configuration.GetSection("_Share");
			IConfigurationSection[] source = new IConfigurationSection[3] { section3, section, configurationSection };
			if (source.Count((IConfigurationSection x) => x?.Exists() ?? false) > 1)
			{
				ConfigurationBuilder cb = new ConfigurationBuilder();
				source.All(delegate (IConfigurationSection s)
				{
					if (s != null && s.Exists())
					{
						cb.AddConfiguration(s);
					}
					return true;
				});
				Configuration = cb.Build();
			}
			else
			{
				Configuration = source.FirstOrDefault((IConfigurationSection x) => x?.Exists() ?? false);
				if (Configuration == null)
				{
					Configuration = section;
				}
			}
		}
	}
}
