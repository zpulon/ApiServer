using Microsoft.Extensions.Options;
using PluginFactory.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Basic
{
	public abstract class SupportConfigPluginBase<TOptions> : PluginBase, ISupportConfigPlugin<TOptions> where TOptions : class, new()
	{
		protected TOptions Options { get; private set; }

		public SupportConfigPluginBase(IOptionsMonitor<TOptions> options)
		{
			if (options != null)
			{
				Options = options.CurrentValue;
				options.OnChange(OnOptionsChanged);
			}
		}

		protected virtual void OnOptionsChanged(TOptions options)
		{
			Options = options;
		}
	}

}
