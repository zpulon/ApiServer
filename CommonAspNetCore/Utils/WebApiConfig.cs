using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAspNetCore.Utils
{
   public class WebApiConfig
    {
		public int Port { get; set; }

		public LogEventLevel SystemLogLevel { get; set; }

		public LogEventLevel AppLogLevel { get; set; }

		public LogEventLevel EFCoreCommandLevel { get; set; }

		public LogEventLevel? RequestLogLevel { get; set; }

		public bool ConsoleLog { get; set; }

		public List<string> HttpHeaders { get; set; }

		public bool WaitIstioProxyReady { get; set; }

		public string ProxyReadyCheckUrl { get; set; }

		public bool ProxyIsReady { get; internal set; }

		public string ReadyResponse { get; set; }

		public int MaxLogLength { get; set; }

		public WebApiConfig()
		{
			Port = 80;
			SystemLogLevel = (LogEventLevel)3;
			AppLogLevel = (LogEventLevel)0;
			EFCoreCommandLevel = (LogEventLevel)1;
			RequestLogLevel = (LogEventLevel)0;
			ConsoleLog = true;
			HttpHeaders = new List<string> {  };
			WaitIstioProxyReady = false;
			ProxyReadyCheckUrl = "http://127.0.0.1:15000/ready";
			ProxyIsReady = false;
			ReadyResponse = "LIVE";
			MaxLogLength = 8192;
		}
	}
}
