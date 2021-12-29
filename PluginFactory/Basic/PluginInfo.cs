using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Basic
{
	public class PluginInfo
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Alias { get; set; }

		public bool IsEnable { get; set; } = true;


		public Type PluginType { get; set; }

		public bool CanConfig { get; set; }

		public bool CanInit { get; set; }

		public Type ConfigType { get; set; }

		public int Order { get; set; }
	}
}
