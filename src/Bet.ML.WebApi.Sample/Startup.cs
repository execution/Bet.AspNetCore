using System.Collections.Generic;

using Bet.AspNetCore.Logging.Azure;
using Bet.AspNetCore.Middleware.Diagnostics;
using Bet.Extensions.ML.ModelStorageProviders;
using Bet.Extensions.ML.Sentiment;
using Bet.Extensions.ML.Sentiment.Models;
using Bet.Extensions.ML.Spam;
using Bet.Extensions.ML.Spam.Models;
using Bet.ML.WebApi.Sample.Jobs;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Bet.ML.WebApi.Sample
{
    public class Startup
    {
        private const string AppName = "Bet.ML.WebApi.Sample";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var instrumentId = Configuration.Bind<ApplicationInsightsOptions>("ApplicationInsights", true);

            services.AddApplicationInsightsTelemetry(options =>
            {
                options.InstrumentationKey = instrumentId.InstrumentationKey;
            });

            services.AddDeveloperListRegisteredServices(o =>
            {
                o.PathOutputOptions = PathOutputOptions.Json;
            });

            services.AddControllers();

            services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = $"{AppName} API", Version = "v1" }));

            // add ML.NET Models
            var spamName = "SpamModel";

            var spamInMemoryModelStorageProvider = new InMemoryModelStorageProvider();
            services.AddSpamModelEngine(modelName: spamName, modelStorageProvider: spamInMemoryModelStorageProvider);

            services.AddModelPredictionEngine<SpamInput, SpamPrediction>(spamName)
                .WithStorageProvider(nameof(spamName), spamInMemoryModelStorageProvider);

            var sentimentName = "SentimentModel";

            var sentimentFileModeStorageProvider = new FileModelStorageProvider();
            services.AddSentimentModelEngine(modelName: sentimentName, modelStorageProvider: sentimentFileModeStorageProvider);

            services.AddModelPredictionEngine<SentimentIssue, SentimentPrediction>(sentimentName)
                .WithStorageProvider($"{nameof(sentimentName)}.zip", sentimentFileModeStorageProvider);

            services.AddScheduler(builder =>
            {
                builder.AddJob<RebuildMLModelScheduledJob, RebuildMLModelsOptions>();
                builder.UnobservedTaskExceptionHandler = null;
            });

            // add healthchecks
            services.AddHealthChecks()
                .AddMemoryHealthCheck()
                .AddMachineLearningModelCheck<SpamInput, SpamPrediction>($"{spamName}_check")
                .AddMachineLearningModelCheck<SentimentIssue, SentimentPrediction>($"{sentimentName}_check")
                .AddSigtermCheck("sigterm_check")
                .AddLoggerPublisher(new List<string> { "sigterm_check" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var enableHttpsRedirection = configuration.GetValue<bool>("EnableHttpsRedirection");

            if (enableHttpsRedirection)
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            // app.UseAuthentication();

            // app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppName} API v1"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // returns 200 okay
                endpoints.MapLivenessHealthCheck();
                endpoints.MapHealthyHealthCheck();
            });
        }
    }
}
