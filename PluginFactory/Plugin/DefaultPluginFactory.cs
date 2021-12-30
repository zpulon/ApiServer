using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PluginFactory.Basic;
using PluginFactory.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluginFactory.Plugin
{
	public class DefaultPluginFactory : IPluginFactory, IHostedService
	{
		private static class Log
		{
			private static EventId PluginStartingEventId = new EventId(100, "PluginStarting");

			private static EventId PluginStartFinishEventId = new EventId(101, "PluginStartFinish");

			private static EventId PluginStoppingEventId = new EventId(102, "PluginStopping");

			private static EventId PluginStopFinishEventId = new EventId(103, "PluginStopFinish");

			private static EventId PluginStartErrorEventId = new EventId(110, "PluginStartError");

			private static EventId PluginStopErrorEventId = new EventId(111, "PluginStopError");

			public static Action<ILogger, Exception> _pluginBeginStart = LoggerMessage.Define(LogLevel.Information, PluginStartingEventId, Resources.PluginBeginStart);

			public static Action<ILogger, long, Exception> _pluginCompleteStart = LoggerMessage.Define<long>(LogLevel.Information, PluginStartFinishEventId, Resources.PluginCompleteStart);

			public static Action<ILogger, string, Exception> _pluginStartError = LoggerMessage.Define<string>(LogLevel.Error, PluginStartErrorEventId, Resources.PluginStartException);

			public static Action<ILogger, Exception> _pluginBeginStop = LoggerMessage.Define(LogLevel.Information, PluginStoppingEventId, Resources.PluginBeginStop);

			public static Action<ILogger, long, Exception> _pluginCompleteStop = LoggerMessage.Define<long>(LogLevel.Information, PluginStartFinishEventId, Resources.PluginCompleteStop);

			public static Action<ILogger, string, Exception> _pluginStopError = LoggerMessage.Define<string>(LogLevel.Error, PluginStopErrorEventId, Resources.PluginStopException);
		}

		private IPluginLoader _loader;

		private readonly ILogger _logger;

		private readonly IServiceProvider _serviceProvider;

		private List<IPlugin> _pluginList = new List<IPlugin>();

		public DefaultPluginFactory(IPluginLoader loader, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IEnumerable<IPlugin> plugins)
		{
			if (loader == null)
			{
				throw new ArgumentNullException("loader");
			}
			if (loggerFactory == null)
			{
				throw new ArgumentNullException("loggerFactory");
			}
			if (plugins != null)
			{
				loader.PluginList.Where((PluginInfo x) => !x.IsEnable).ToList();
				_pluginList.AddRange(plugins);
			}
			_loader = loader;
			_logger = loggerFactory.CreateLogger("PluginFactory");
			_serviceProvider = serviceProvider;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await ForEachPlugin(async delegate (IPlugin plugin, IPluginContext ctx)
			{
				Log._pluginBeginStart(_logger, null);
				Stopwatch sw = new Stopwatch();
				try
				{
					await plugin.StartAsync(ctx);
				}
				catch (Exception ex)
				{
					Log._pluginStartError(_logger, ex.ToString(), ex);
					throw;
				}
				sw.Stop();
				Log._pluginCompleteStart(_logger, sw.ElapsedMilliseconds, null);
			}, cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await ForEachPlugin(async delegate (IPlugin plugin, IPluginContext ctx)
			{
				Log._pluginBeginStop(_logger, null);
				Stopwatch sw = new Stopwatch();
				try
				{
					await plugin.StartAsync(ctx);
				}
				catch (Exception ex)
				{
					Log._pluginStopError(_logger, ex.ToString(), ex);
					throw;
				}
				sw.Stop();
				Log._pluginCompleteStop(_logger, sw.ElapsedMilliseconds, null);
			}, cancellationToken);
		}

		protected async Task ForEachPlugin(Func<IPlugin, IPluginContext, Task> proc, CancellationToken cancellationToken)
		{
			foreach (IPlugin plugin in _pluginList)
			{
				PluginInfoLogValue state = new PluginInfoLogValue(getPluginInfo(plugin.GetType()));
				using (_logger.BeginScope(state))
				{
					IPluginContext arg = new PluginContext(this, _serviceProvider, cancellationToken);
					await proc(plugin, arg);
				}
			}
		}

		private PluginInfo getPluginInfo(Type pluginType)
		{
			return _loader.PluginList.First((PluginInfo x) => x.PluginType == pluginType);
		}

		public IPlugin GetPluginById(string id)
		{
			PluginInfo pi = _loader.PluginList.FirstOrDefault((PluginInfo x) => x.Id == id);
			if (pi == null)
			{
				return null;
			}
			return _pluginList.FirstOrDefault((IPlugin x) => x.GetType() == pi.PluginType);
		}

		public IPlugin GetPluginByName(string name)
		{
			PluginInfo pi = _loader.PluginList.FirstOrDefault((PluginInfo x) => x.Name == name || x.Alias == name);
			if (pi == null)
			{
				return null;
			}
			return _pluginList.FirstOrDefault((IPlugin x) => x.GetType() == pi.PluginType);
		}

		public TPlugin GetPlugin<TPlugin>() where TPlugin : class, IPlugin
		{
			return _pluginList.FirstOrDefault((IPlugin x) => x.GetType() == typeof(TPlugin)) as TPlugin;
		}
	}
}
