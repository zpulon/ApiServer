using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using PluginFactory.Basic;
using PluginFactory.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Plugin
{
	public class DefaultPluginLoader : IPluginLoader
	{
		private readonly PluginFactoryOptions _options;

		private readonly IServiceCollection _services;

		private List<PluginInfo> _pluginList = new List<PluginInfo>();

		public IReadOnlyList<PluginInfo> PluginList => _pluginList;

		public DefaultPluginLoader(PluginFactoryOptions options, IServiceCollection services)
		{
			_options = options;
			_services = services;
		}

		public virtual void Load()
		{
			lock (_pluginList)
			{
				foreach (Assembly additionalAssembly in _options.AdditionalAssemblies)
				{
					LoadPluginFromAssembly(additionalAssembly);
				}
				if (_options.FileProvider == null)
				{
					return;
				}
				IDirectoryContents directoryContents = _options.FileProvider.GetDirectoryContents(string.Empty);
				if (!directoryContents.Exists)
				{
					return;
				}
				foreach (IFileInfo item in directoryContents)
				{
					if (item.IsDirectory)
					{
						foreach (IFileInfo directoryContent in _options.FileProvider.GetDirectoryContents(item.Name))
						{
							if (!directoryContent.IsDirectory && Path.GetFileNameWithoutExtension(directoryContent.PhysicalPath)!.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
							{
								LoadPluginFromAssembly(directoryContent.PhysicalPath);
							}
						}
					}
					else if (item.PhysicalPath != null && Path.GetExtension(item.PhysicalPath) == ".dll")
					{
						LoadPluginFromAssembly(item.PhysicalPath);
					}
				}
			}
		}

		protected virtual void LoadPluginFromAssembly(string assemblyPath)
		{
			if (_options.Predicate == null || _options.Predicate(assemblyPath))
			{
				Assembly assembly = new SolationAssemblyLoadContext(assemblyPath).Load();
				if (assembly != null)
				{
					LoadPluginFromAssembly(assembly);
				}
			}
		}

		protected virtual void LoadPluginFromAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			Type[] exportedTypes = assembly.GetExportedTypes();
			new List<PluginInfo>();
			Type[] array = exportedTypes;
			foreach (Type type in array)
			{
				PluginInfo pi = LoadPluginFromType(type);
				if (pi != null && !_pluginList.Any((PluginInfo p) => p.PluginType == pi.PluginType))
				{
					_pluginList.Add(pi);
				}
			}
		}

		protected virtual PluginInfo LoadPluginFromType(Type type)
		{
			if (type.IsAbstract)
			{
				return null;
			}
			Type[] interfaces = type.GetInterfaces();
			if (interfaces == null || interfaces.Length == 0)
			{
				return null;
			}
			PluginInfo pluginInfo = null;
			if (typeof(IPlugin).GetTypeInfo().IsAssignableFrom(type))
			{
				pluginInfo = new PluginInfo
				{
					PluginType = type,
					CanInit = false,
					CanConfig = false
				};
			}
			if (pluginInfo == null)
			{
				return null;
			}
			PluginAttribute pluginAttribute = type.GetCustomAttributes(typeof(PluginAttribute), inherit: false).OfType<PluginAttribute>().FirstOrDefault();
			if (pluginAttribute != null)
			{
				pluginInfo.Id = pluginAttribute.Id;
				pluginInfo.Name = pluginAttribute.Name;
				pluginInfo.Alias = pluginAttribute.Alias;
				pluginInfo.Description = pluginAttribute.Description;
			}
			pluginInfo.Id = (string.IsNullOrEmpty(pluginInfo.Id) ? type.FullName : pluginInfo.Id);
			pluginInfo.Name = ((!string.IsNullOrEmpty(pluginInfo.Name)) ? pluginInfo.Name : (string.IsNullOrEmpty(pluginInfo.Alias) ? type.FullName : pluginInfo.Alias));
			if (typeof(ISupportInitPlugin).IsAssignableFrom(type))
			{
				pluginInfo.CanInit = true;
			}
			Type type2 = interfaces.FirstOrDefault((Type x) => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISupportConfigPlugin<>));
			if (type2 != null)
			{
				pluginInfo.ConfigType = type2.GenericTypeArguments[0];
				pluginInfo.CanConfig = true;
			}
			if ((!string.IsNullOrEmpty(pluginInfo.Alias) && _options.DisabledPluginList.Contains(pluginInfo.Alias)) || _options.DisabledPluginList.Contains(pluginInfo.Name) || _options.DisabledPluginList.Contains<string>(pluginInfo.PluginType.FullName) || _options.DisabledPluginList.Contains<string>(pluginInfo.PluginType.FullName!.Replace("+", ".")))
			{
				pluginInfo.IsEnable = false;
			}
			return pluginInfo;
		}

		public virtual void Init()
		{
			if (_pluginList.Where((PluginInfo x) => x.CanInit && x.IsEnable).ToList().Count == 0)
			{
				return;
			}
			IPluginInitContext pluginInitContext = new PluginInitContext(_options.PluginPath, this, _services);
			foreach (ISupportInitPlugin item in pluginInitContext.InitServiceProvider.GetRequiredService<IEnumerable<ISupportInitPlugin>>())
			{
				item.Init(pluginInitContext);
			}
		}
	}
}
