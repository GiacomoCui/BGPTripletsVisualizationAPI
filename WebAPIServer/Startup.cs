using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Shared;
using Shared.Converters;
using Shared.Database;
using Shared.Database.Services;
using System.IO.Compression;

namespace WebAPIServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<TripletService>();
			services.AddScoped<PairServices>();
			services.AddScoped<RIBEntryServices>();

			services.AddSingleton<MongoConnector>();

			services.AddResponseCaching(options =>
			{
				options.MaximumBodySize = 64 * 1024 * 1024;
				options.SizeLimit = 1024 * 1024 * 1024;
				options.UseCaseSensitivePaths = true;
			});

			services.Configure<GzipCompressionProviderOptions>(options =>
			{
				options.Level = CompressionLevel.Optimal;
			});

			services.Configure<BrotliCompressionProviderOptions>(options =>
			{
				options.Level = CompressionLevel.Optimal;
			});

			services.AddResponseCompression(options =>
			{
				//IEnumerable<string> MimeTypes = new[]
				//{
				//	 // General
				//	 "text/plain",
				//	 "text/html",
				//	 "text/css",
				//	 "font/woff2",
				//	 "application/javascript",
				//	 "image/x-icon",
				//	 "image/png"
				//};

				//options.MimeTypes = MimeTypes;
				options.EnableForHttps = true;
				options.Providers.Add<BrotliCompressionProvider>();
				options.Providers.Add<GzipCompressionProvider>();
			});

			services.AddControllers().AddNewtonsoftJson(settings =>
			{
				settings.SerializerSettings.Converters.Add(new IPAddressJsonConverter());
				settings.SerializerSettings.Converters.Add(new IPPrefixJsonConverter());
				settings.SerializerSettings.Converters.Add(new ObservedPeersArrayConverter());
				settings.SerializerSettings.Converters.Add(new StringEnumConverter());
				settings.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
				settings.SerializerSettings.Formatting = Formatting.None;
				settings.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				settings.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
				settings.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
				settings.SerializerSettings.DateFormatString = "u";
			});

			services.AddCors(options =>
			{
				options.AddDefaultPolicy(
					builder =>
					{
						builder.WithOrigins("*");
					});
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				Log.Error("Running in developing mode");
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseSerilogRequestLogging(options =>
				{
					// Customize the message template
					options.MessageTemplate += " from {RequestHost}";

					// Attach additional properties to the request completion event
					options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
					{
						if (httpContext.Request.Headers["X-Real-IP"].Count > 0)
						{
							diagnosticContext.Set("RequestHost", httpContext.Request.Headers["X-Real-IP"][0]);
						}
						else
						{
							diagnosticContext.Set("RequestHost", httpContext.Connection.RemoteIpAddress?.ToString());
						}

					};

				});
				app.UseExceptionHandler("/");
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors();

			app.UseResponseCaching();

			app.UseResponseCompression();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
