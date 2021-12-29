using Microsoft.Extensions.Configuration;

namespace PluginFactory.Interface
{
    public interface IPluginConfigrationProvider<TPlugin>
    {
        IConfiguration Configuration { get; }
    }
}
