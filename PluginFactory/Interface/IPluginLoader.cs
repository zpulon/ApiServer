using PluginFactory.Basic;
using System.Collections.Generic;

namespace PluginFactory.Interface
{
   public interface IPluginLoader
    {
        IReadOnlyList<PluginInfo> PluginList { get; }

        void Load();

        void Init();
    }
}
