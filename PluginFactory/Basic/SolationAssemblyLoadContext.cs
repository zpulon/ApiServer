using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Basic
{
    public class SolationAssemblyLoadContext : AssemblyLoadContext
	{
		private AssemblyDependencyResolver _resolver;

		private readonly string _assemblyName;

		public SolationAssemblyLoadContext(string assemblyPath)
		{
			if (string.IsNullOrEmpty(assemblyPath))
			{
				throw new ArgumentNullException("assemblyPath");
			}
			_assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
			_resolver = new AssemblyDependencyResolver(assemblyPath);
		}

		public Assembly Load()
		{
			return Load(new AssemblyName(_assemblyName));
		}

		protected override Assembly Load(AssemblyName assemblyName)
		{
			string text = _resolver.ResolveAssemblyToPath(assemblyName);
			if (text != null)
			{
				return LoadFromAssemblyPath(text);
			}
			return null;
		}

		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
		{
			string text = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
			if (text != null)
			{
				return LoadUnmanagedDllFromPath(text);
			}
			return IntPtr.Zero;
		}
	}
}
