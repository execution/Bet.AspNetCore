using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OptionsValidationException = Bet.AspNetCore.Options.OptionsValidationException;
using Microsoft.Extensions.Hosting;

namespace Bet.AspNetCore.UnitTest
{
    public class OptionsValidationTests
    {
        [Fact]
        public void Bind_Object_Fail()
        {
            var dic = new Dictionary<string, string>
            {
                {"FakeOptions:Id", "-2" },
                {"FakeOptions:Name", "" }
            };

            var config = new ConfigurationBuilder().AddInMemoryCollection(dic).Build().GetSection(nameof(FakeOptions));

            var options = new FakeOptions();

            void act() => config.Bind<FakeOptions>(options, opt =>
            {
                if (opt.Id > 0 && !string.IsNullOrWhiteSpace(opt.Name))
                {
                    return true;
                }
                return false;
            }, "Validation Failed");

            Assert.Throws<OptionsValidationException>(act);
        }

        [Fact]
        public void Bind_Object_With_DataAnnotation_Fail()
        {
            var dic = new Dictionary<string, string>
            {
                {"FakeOptions:Id", "-2" },
                {"FakeOptions:Name", "" }
            };

            var config = new ConfigurationBuilder().AddInMemoryCollection(dic).Build().GetSection(nameof(FakeOptions));

            var options = new FakeOptionsWithDataAnnotations();

            void act() => config.Bind<FakeOptionsWithDataAnnotations>(options);

            Assert.Throws<OptionsValidationException>(act);
        }

        [Fact]
        public void Configure_With_DataAnnotation_Validation_Fail()
        {
            var dic = new Dictionary<string, string>
            {
                {"FakeOptions:Id", "-2" },
                {"FakeOptions:Name", "" }
            };

            IConfiguration Configuration = null;

            var host = new WebHostBuilder()
                .ConfigureAppConfiguration((hostingContext, configBuiler) =>
                {
                    Configuration = configBuiler.AddInMemoryCollection(dic).Build();
                })
                .ConfigureServices((services) =>
                {
                    services.AddMvcCore().AddApplicationPart(typeof(TestStartup).Assembly);
                    services.AddOptions();

                    services.ConfigureWithDataAnnotationsValidation<FakeOptionsWithDataAnnotations>(Configuration, sectionName: "FakeOptions");
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            Assert.Throws<OptionsValidationException>(() => new TestServer(host));
        }

        [Fact]
        public void Configure_With_DataAnnotation_Validation_Succeded()
        {
            var dic = new Dictionary<string, string>
            {
                {"FakeOptions:Id", "2" },
                {"FakeOptions:Name", "tet" }
            };

            IConfiguration Configuration = null;

            var host = new WebHostBuilder()
                .ConfigureAppConfiguration((hostingContext, configBuiler) =>
                {
                    Configuration = configBuiler.AddInMemoryCollection(dic).Build();
                })
                .ConfigureServices((services) =>
                {
                    services.AddMvcCore().AddApplicationPart(typeof(TestStartup).Assembly);
                    services.AddOptions();

                    services.ConfigureWithDataAnnotationsValidation<FakeOptionsWithDataAnnotations>(Configuration, sectionName: "FakeOptions");
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(host);

            var result = server.Host.Services.GetService<FakeOptionsWithDataAnnotations>();

            Assert.Equal(2, result.Id);
        }

        [Fact]
        public void Configure_With_Validation_Fail()
        {
            var dic = new Dictionary<string, string>
            {
                {"FakeOptions:Id", "-2" },
                {"FakeOptions:Name", "" }
            };

            IConfiguration Configuration = null;

            var host = new WebHostBuilder()
                .ConfigureAppConfiguration((hostingContext, configBuiler) =>
                {
                    Configuration = configBuiler.AddInMemoryCollection(dic).Build();
                })
                .ConfigureServices((services) =>
                {
                    services.AddMvcCore().AddApplicationPart(typeof(TestStartup).Assembly);
                    services.AddOptions();

                    services.ConfigureWithValidation<FakeOptions>(Configuration, opt =>
                    {
                        if (opt.Id > 0 && !string.IsNullOrWhiteSpace(opt.Name))
                        {
                            return true;
                        }
                        return false;
                    }, "This didn't validated.");
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            Assert.Throws<OptionsValidationException>(() => new TestServer(host));
        }

        [Fact]
        public void Configure_HostBuilder_With_DataAnnotation_Fail()
        {
            var dic = new Dictionary<string, string>
            {
                { "FakeOptions:Id", "-2" },
                { "FakeOptions:Name", string.Empty }
            };

            IConfiguration configuration = null;

            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, configBuiler) =>
                {
                    configuration = configBuiler.AddInMemoryCollection(dic).Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<HostStartupService>();
                    services.ConfigureWithDataAnnotationsValidation<FakeOptionsWithDataAnnotations>(configuration, sectionName: "FakeOptions");
                }).Build();

            void Act() => host.Run();

            Assert.Throws<OptionsValidationException>(() => Act());
        }

        [Fact]
        public void Configure_HostBuilder_With_DataAnnotation_Succeeded()
        {
            var dic = new Dictionary<string, string>
            {
                { "FakeOptions:Id", "2" },
                { "FakeOptions:Name", "ha" }
            };

            IConfiguration configuration = null;

            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, configBuiler) =>
                {
                    configuration = configBuiler.AddInMemoryCollection(dic).Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<HostStartupService>();
                    services.ConfigureWithDataAnnotationsValidation<FakeOptionsWithDataAnnotations>(configuration, sectionName: "FakeOptions");
                }).Build();

            var sp = hostBuilder.Services;

            var result = sp.GetService<FakeOptionsWithDataAnnotations>();

            Assert.Equal(2, result.Id);
        }
    }
}