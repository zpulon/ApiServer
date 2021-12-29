using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Basic
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class PluginAttribute : Attribute
	{
		public string Id { get; set; }

		public string Alias { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}

}
