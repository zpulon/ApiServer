using CommonAspNetCore.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CommonAspNetCore.Middleware
{
	internal class HttpRequestLogScopeMiddleware : IMiddleware
	{
		private ILoggerFactory _loggerFactory;

		private WebApiConfig _config;

		public HttpRequestLogScopeMiddleware(ILoggerFactory loggerFactory, WebApiConfig config)
		{
			_loggerFactory = loggerFactory;
			_config = config;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			ILogger<HttpRequestLogScopeMiddleware> logger = _loggerFactory.CreateLogger<HttpRequestLogScopeMiddleware>();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (context != null && context.Request != null && context.Request.Headers != null)
			{
				foreach (string httpHeader in _config.HttpHeaders)
				{
					if (context.Request.Headers.TryGetValue(httpHeader, out var value))
					{
						dictionary.Add(httpHeader, value.ToString());
					}
				}
			}
			IDisposable scope = new EmptyDisposable();
			if (dictionary.Count > 0)
			{
				scope = logger.BeginScope(dictionary);
			}
			string url = context.Request.Path + "?" + context.Request.QueryString.Value;
			LogLevel? logLevel = LogLevelConverter.Converter(_config.RequestLogLevel);
			if (_config.RequestLogLevel.HasValue && logger.IsEnabled(logLevel.Value))
			{
				try
				{
					context.Request.EnableBuffering();
					string text = await new StreamReader(context.Request.Body).ReadToEndAsync();
					logger.Log(logLevel.Value, "请求：{method} {url} \n {@headers} \n {body} ", context.Request.Method, url, context.Request.Headers, text);
					context.Request.Body.Position = 0L;
				}
				catch (Exception exception)
				{
					context.Request.Body.Position = 0L;
					logger.LogError(exception, "记录请求日志异常");
				}
			}
			bool logResposne = _config.RequestLogLevel.HasValue && logger.IsEnabled(logLevel.Value);
			Stream bodyStream = context.Response.Body;
			MemoryStream tempResponseBodyStream = null;
			if (logResposne)
			{
				tempResponseBodyStream = new MemoryStream();
				context.Response.Body = tempResponseBodyStream;
			}
			await next(context);
			if (logResposne && tempResponseBodyStream != null)
			{
				try
				{
					_ = 2;
					try
					{
						string text2 = "";
						tempResponseBodyStream.Seek(0L, SeekOrigin.Begin);
						if (tempResponseBodyStream.Length > 0)
						{
							text2 = await new StreamReader(context.Response.Body).ReadToEndAsync();
						}
						logger.Log(logLevel.Value, "请求应答：{statuscode} {method} {url} \n {@headers} \n {body} ", context.Response?.StatusCode, context.Request.Method, url, context.Response.Headers, text2);
						tempResponseBodyStream.Seek(0L, SeekOrigin.Begin);
					}
					catch (Exception exception2)
					{
						context.Request.Body.Position = 0L;
						logger.LogError(exception2, "记录请求应答日志异常");
					}
				}
				finally
				{
					await tempResponseBodyStream.CopyToAsync(bodyStream);
				}
			}
			sw.Stop();
			logger.LogDebug($"请求耗时：{sw.ElapsedMilliseconds}ms {context.Response?.StatusCode} {context.Request.Method} {url}");
			scope.Dispose();
		}
	}
}
