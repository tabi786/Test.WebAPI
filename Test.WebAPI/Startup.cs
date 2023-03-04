using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.Data.SqlClient;
using Azure.Identity;
using Microsoft.AspNetCore.Rewrite;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Channel;

namespace Test.WebAPI
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
			services.AddCors(options =>
			{
				options.AddPolicy(
											"CorsPolicy",
											builder => builder.AllowAnyOrigin()
															.AllowAnyMethod()
															.AllowAnyHeader());
			});

			//using var channel = new InMemoryChannel();
			//services.Configure<TelemetryConfiguration>(config => config.TelemetryChannel = channel);
#if DEBUG
			var applicationInsightsSettings = Configuration.GetSection("ApplicationInsights").Get<ApplicationInsightsSettings>();
#else
			ApplicationInsightsSettings applicationInsightsSettings = new();
			applicationInsightsSettings.ConnectionString = Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
#endif

			IServiceProvider serviceProvider = services.BuildServiceProvider();
			ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();
			//logger.LogInformation("Logger is working...");

			var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

			services.Configure<B2CUserSettings>
							(this.Configuration.GetSection("B2C"));

			services.AddControllers().AddOData(options => options
					.Select()
					.Filter()
					.Expand()
					.SetMaxTop(100)
					.Count()
					.OrderBy());

			var storageQueueSettings = Configuration.GetSection("StorageQueue").Get<StorageQueueSettings>();

#if DEBUG
			services.AddDbContext<TestDataContext>(item => item.UseSqlServer(Configuration.GetConnectionString("DNS")));
#else
			services.AddDbContext<FreedomDataContext>(item => item.UseSqlServer(Configuration.GetConnectionString("DNS")));
			storageQueueSettings.ClientId = Configuration["DBDecryptClientId"];
      storageQueueSettings.ClientSecret = Configuration["DBDecryptClientSecret"];
      storageQueueSettings.QueueName = Configuration["BlobQueueName"];
      storageQueueSettings.QueueConnectionString = Configuration["BlobConnectionString"];
#endif

			//_logger.LogInformation("AKV provider Start registration {DT}", DateTime.UtcNow.ToLongTimeString());
			SqlColumnEncryptionAzureKeyVaultProvider sqlColumnEncryptionAzureKeyVaultProvider =
							new SqlColumnEncryptionAzureKeyVaultProvider(new ClientSecretCredential(
									storageQueueSettings.TenantId, storageQueueSettings.ClientId, storageQueueSettings.ClientSecret));

			// Register AKV provider
			SqlConnection.RegisterColumnEncryptionKeyStoreProviders(customProviders: new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>(capacity: 1, comparer: StringComparer.OrdinalIgnoreCase)
			{
				{ SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, sqlColumnEncryptionAzureKeyVaultProvider}
			});
			//_logger.LogInformation("AKV provider Registered {DT}", DateTime.UtcNow.ToLongTimeString());

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test.WebAPI", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Freedom.Web.API v1"));
			}
			else
			{
				//app.UseExceptionHandler("/api/v1/exception");
				app.UseDeveloperExceptionPage();
			}
			app.Use(async (context, next) =>
			{
				validateHttpRequest(context);
				await next();
			});

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();

			});


		}

		/// <summary>
		/// Validates request to prevent unauthorized data access
		/// </summary>
		/// <param name="context"></param>
		void validateHttpRequest(HttpContext context)
		{
			var request = context.Request;
			if (request.Path.Value.StartsWith("/api/v1/data/users", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/userpersonalidentifications", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/userinvitations", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/useraddresses", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/userbankinformations", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/usernotifications", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/userledgeraccounts", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/ledgertransactions", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/usercreditcards", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/uservouchers", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/uservouchershares", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/userbrokers", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/userdocuments", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/userincomeinformations", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/usercompanyinformations", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/userproducts", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/usersubscriptions", StringComparison.CurrentCultureIgnoreCase)
			 || request.Path.Value.StartsWith("/api/v1/data/usermarkets", StringComparison.CurrentCultureIgnoreCase)
			 )

			{
				var currentPrincipalId = "2B510E07-3699-4B16-8374-6E3A1933D664";
				if (request.Path.Value.Equals("/api/v1/data/users", StringComparison.CurrentCultureIgnoreCase))
				{
					if (!request.Query.ContainsKey("filter") && !request.Query.ContainsKey("$filter"))
						request.QueryString = request.QueryString.Add("$filter", $"Id eq {currentPrincipalId}");
					StringBuilder queryString = new StringBuilder("?");
					foreach (var item in request.Query)
					{
						if (item.Key.Equals("filter", StringComparison.OrdinalIgnoreCase) || item.Key.Equals("$filter", StringComparison.OrdinalIgnoreCase))
							queryString.Append(item.Key + "=(" + item.Value.ToString().Replace("@UserId", currentPrincipalId, StringComparison.CurrentCultureIgnoreCase) + ") and Id eq " + currentPrincipalId + "&");
						else
							queryString.Append(item.Key + "=" + item.Value.ToString() + "&");
					}
					context.Request.QueryString = new Microsoft.AspNetCore.Http.QueryString(queryString.ToString());
				}
				else if (request.Path.Value.Equals("/api/v1/data/userinvitations", StringComparison.CurrentCultureIgnoreCase))
				{
					if (!context.Request.Query.ContainsKey("filter") && !context.Request.Query.ContainsKey("$filter"))
						context.Request.Path = new Microsoft.AspNetCore.Http.PathString("/api/v1/data/unknown");
					else
					{
						StringBuilder queryString = new StringBuilder("?");
						foreach (var item in request.Query)
						{
							if (item.Key.Equals("filter", StringComparison.OrdinalIgnoreCase) || item.Key.Equals("$filter", StringComparison.OrdinalIgnoreCase))
							{
								if (item.Value.ToString().StartsWith("InvitedByUserID", StringComparison.OrdinalIgnoreCase))
									queryString.Append(item.Key + "=(" + item.Value.ToString().Replace("@UserId", currentPrincipalId, StringComparison.CurrentCultureIgnoreCase) + ") and InvitedByUserId eq " + currentPrincipalId + "&");
								else if (item.Value.ToString().StartsWith("InvitedUserID", StringComparison.OrdinalIgnoreCase))
									queryString.Append(item.Key + "=(" + item.Value.ToString().Replace("@UserId", currentPrincipalId, StringComparison.CurrentCultureIgnoreCase) + ") and InvitedUserId eq " + currentPrincipalId + "&");
								else if (item.Value.ToString().StartsWith("InvitationCode", StringComparison.OrdinalIgnoreCase))
									queryString.Append(item.Key + "=(" + item.Value.ToString() + ") and (Status eq 1 or Status eq 2)&");
								else if (item.Value.ToString().StartsWith("Email", StringComparison.OrdinalIgnoreCase))
									queryString.Append(item.Key + "=(" + item.Value.ToString() + ") and (Status eq 1 or Status eq 2)&");
								else
								{
									queryString.Clear();
									break;
								}
							}
							else
								queryString.Append(item.Key + "=" + item.Value.ToString() + "&");
						}

						if (queryString.Length > 0)
							context.Request.QueryString = new Microsoft.AspNetCore.Http.QueryString(queryString.ToString());
						else
							context.Request.Path = new Microsoft.AspNetCore.Http.PathString("/api/v1/data/unknown");
					}
				}
				else if (request.Path.Value.Equals("/api/v1/data/uservouchers", StringComparison.CurrentCultureIgnoreCase))
				{
					if (!context.Request.Query.ContainsKey("filter") && !context.Request.Query.ContainsKey("$filter"))
						context.Request.Path = new Microsoft.AspNetCore.Http.PathString("/api/v1/data/unknown");
					else
					{
						StringBuilder queryString = new StringBuilder("?");
						foreach (var item in request.Query)
						{
							if (item.Key.Equals("filter", StringComparison.OrdinalIgnoreCase) || item.Key.Equals("$filter", StringComparison.OrdinalIgnoreCase))
							{
								if (item.Value.ToString().StartsWith("IssuerUserId", StringComparison.OrdinalIgnoreCase))
									queryString.Append(item.Key + "=(" + item.Value.ToString().Replace("@UserId", currentPrincipalId, StringComparison.CurrentCultureIgnoreCase) + ") and IssuerUserId eq " + currentPrincipalId + "&");
								else if (item.Value.ToString().StartsWith("RedeemUserId", StringComparison.OrdinalIgnoreCase))
									queryString.Append(item.Key + "=(" + item.Value.ToString().Replace("@UserId", currentPrincipalId, StringComparison.CurrentCultureIgnoreCase) + ") and RedeemUserId eq " + currentPrincipalId + "&");
								else
								{
									queryString.Clear();
									break;
								}
							}
							else
								queryString.Append(item.Key + "=" + item.Value.ToString() + "&");
						}

						if (queryString.Length > 0)
							context.Request.QueryString = new Microsoft.AspNetCore.Http.QueryString(queryString.ToString());
						else
							context.Request.Path = new Microsoft.AspNetCore.Http.PathString("/api/v1/data/unknown");
					}
				}
				else if (request.Path.Value.Equals("/api/v1/data/uservouchershares", StringComparison.CurrentCultureIgnoreCase))
				{
					if (!context.Request.Query.ContainsKey("filter") && !context.Request.Query.ContainsKey("$filter"))
						context.Request.Path = new Microsoft.AspNetCore.Http.PathString("/api/v1/data/unknown");
					else
					{
						StringBuilder queryString = new StringBuilder("?");
						foreach (var item in request.Query)
						{
							if (item.Key.Equals("filter", StringComparison.OrdinalIgnoreCase) || item.Key.Equals("$filter", StringComparison.OrdinalIgnoreCase))
							{
								if (item.Value.ToString().StartsWith("SharedUserId", StringComparison.OrdinalIgnoreCase))
									queryString.Append(item.Key + "=(" + item.Value.ToString().Replace("@UserId", currentPrincipalId, StringComparison.CurrentCultureIgnoreCase) + ") and SharedUserId eq " + currentPrincipalId + "&");
								else
								{
									queryString.Clear();
									break;
								}
							}
							else
								queryString.Append(item.Key + "=" + item.Value.ToString() + "&");
						}

						if (queryString.Length > 0)
							context.Request.QueryString = new Microsoft.AspNetCore.Http.QueryString(queryString.ToString());
						else
							context.Request.Path = new Microsoft.AspNetCore.Http.PathString("/api/v1/data/unknown");
					}
				}
				else if (request.Path.Value.Equals("/api/v1/data/userpersonalidentifications", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/useraddresses", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/userbankinformations", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/usernotifications", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/userledgeraccounts", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/usercreditcards", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/userbrokers", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/userdocuments", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/userincomeinformations", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/usercompanyinformations", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/userproducts", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/usersubscriptions", StringComparison.CurrentCultureIgnoreCase)
				|| request.Path.Value.Equals("/api/v1/data/usermarkets", StringComparison.CurrentCultureIgnoreCase)
				)
				{
					if (!request.Query.ContainsKey("filter") && !request.Query.ContainsKey("$filter"))
						request.QueryString = request.QueryString.Add("$filter", $"UserId eq {currentPrincipalId}");

					StringBuilder queryString = new StringBuilder("?");
					foreach (var item in request.Query)
					{
						if (item.Key.Equals("filter", StringComparison.OrdinalIgnoreCase) || item.Key.Equals("$filter", StringComparison.OrdinalIgnoreCase))
							queryString.Append(item.Key + "=(" + item.Value.ToString().Replace("@UserId", currentPrincipalId, StringComparison.CurrentCultureIgnoreCase) + ") and UserId eq " + currentPrincipalId + "&");
						else
							queryString.Append(item.Key + "=" + item.Value.ToString() + "&");
					}

					context.Request.QueryString = new Microsoft.AspNetCore.Http.QueryString(queryString.ToString());
				}
			}
		}
	}
}
