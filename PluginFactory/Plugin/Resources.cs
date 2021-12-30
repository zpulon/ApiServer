using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Plugin
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("PluginFactory.Resources", typeof(Resources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string InvalidInitPlugin => ResourceManager.GetString("InvalidInitPlugin", resourceCulture);

		internal static string PluginBeginStart => ResourceManager.GetString("PluginBeginStart", resourceCulture);

		internal static string PluginBeginStop => ResourceManager.GetString("PluginBeginStop", resourceCulture);

		internal static string PluginCompleteStart => ResourceManager.GetString("PluginCompleteStart", resourceCulture);

		internal static string PluginCompleteStop => ResourceManager.GetString("PluginCompleteStop", resourceCulture);

		internal static string PluginInfo => ResourceManager.GetString("PluginInfo", resourceCulture);

		internal static string PluginStartException => ResourceManager.GetString("PluginStartException", resourceCulture);

		internal static string PluginStopException => ResourceManager.GetString("PluginStopException", resourceCulture);

		internal Resources()
		{
		}
	}

}
