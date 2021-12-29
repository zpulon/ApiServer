using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Basic
{
	public class PluginFactoryOptions
	{
		public const string DEFAULT_PLUGIN_PATH_KEY = "Path";

		public const string DEFAULT_ISENABLED_KEY = "IsEnabled";

		private List<Assembly> _additionalAssemblies = new List<Assembly>();

		private List<string> _disabledPluginList = new List<string>();

		public string PluginPath { get; set; }

		public IFileProvider FileProvider { get; set; }

		public IReadOnlyList<Assembly> AdditionalAssemblies => _additionalAssemblies;

		public Func<string, bool> Predicate { get; set; } = (string _) => true;


		public IReadOnlyList<string> DisabledPluginList => _disabledPluginList;

		public void DisablePlugin(string pluginTypeName)
		{
			if (!_disabledPluginList.Any((string x) => x.Equals(pluginTypeName, StringComparison.OrdinalIgnoreCase)))
			{
				_disabledPluginList.Add(pluginTypeName);
			}
		}

		public void AddAssembly(Assembly assembly)
		{
			_additionalAssemblies.Add(assembly);
		}

		public void ConfigFromConfigration(PluginFactoryConfigration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			IConfiguration configuration2 = configuration.Configuration;
			string text = configuration2["Path"];
			if (!string.IsNullOrEmpty(text) && !string.Equals(text, PluginPath, StringComparison.OrdinalIgnoreCase))
			{
				if (!Path.IsPathRooted(text))
				{
					text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, text);
				}
				text = (PluginPath = Path.GetFullPath(text));
				if (FileProvider == null || FileProvider is PhysicalFileProvider)
				{
					if (Directory.Exists(text))
					{
						FileProvider = new PhysicalFileProvider(text);
					}
					else
					{
						FileProvider = null;
					}
				}
			}
			configDisablePlugin(configuration2);
		}

		private void configDisablePlugin(IConfiguration pluginConfig)
		{
			foreach (IConfigurationSection child in pluginConfig.GetChildren())
			{
				IConfigurationSection section = child.GetSection("IsEnabled");
				if (section.Exists() && (section.Value == "0" || section.Value.ToLower() == "false"))
				{
					DisablePlugin(child.Key);
				}
			}
		}
	}
}
