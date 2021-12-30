using Microsoft.Extensions.Logging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAspNetCore.Middleware
{
	public static class LogLevelConverter
	{
		public static LogLevel? Converter(LogEventLevel? level)
		{
			if (!level.HasValue)
			{
				return null;
			}
			LogLevel value = LogLevel.Trace;
			switch (level)
			{
				case LogEventLevel.Debug:
					value = LogLevel.Debug;
					break;
				case LogEventLevel.Error:
					value = LogLevel.Error;
					break;
				case LogEventLevel.Fatal:
					value = LogLevel.Critical;
					break;
				case LogEventLevel.Information:
					value = LogLevel.Information;
					break;
				case LogEventLevel.Verbose:
					value = LogLevel.Trace;
					break;
				case LogEventLevel.Warning:
					value = LogLevel.Warning;
					break;
			}
			return value;
		}
	}

}
