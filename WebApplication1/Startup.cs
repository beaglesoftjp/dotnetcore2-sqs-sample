using Amazon.SQS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Models;

namespace WebApplication1
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
            services.AddMvc();

            var awsConfig = Configuration.GetSection("AWS");
            var sqsConfigParam = new SqsConfigParam
            {
                ServiceUrl = awsConfig["serviceURL"],
                ReceiveMessageWaitTimeSeconds = int.Parse(awsConfig["receiveMessageWaitTimeSeconds"]),
                VisibilityTimeout = int.Parse(awsConfig["visibilityTimeout"])
            };
            
            services.AddSingleton(config => sqsConfigParam);

            services.AddSingleton(client =>
                {
                    var c = new AmazonSQSConfig {ServiceURL = sqsConfigParam.ServiceUrl};
                    return new AmazonSQSClient(c);
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}