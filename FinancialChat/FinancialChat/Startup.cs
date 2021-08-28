using System;
using FinancialChat.Commands;
using FinancialChat.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plain.RabbitMQ;
using RabbitMQ.Client;

namespace FinancialChat
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
            services.AddRazorPages();

            services.AddSignalR();

            services.AddSingleton<IConnectionProvider>(
                        new ConnectionProvider("amqp://guest@localhost:5672"));

            services.AddScoped<IPublisher>(x => new Publisher(
                x.GetService<IConnectionProvider>(),
                "stockbot-request-exchange",
                ExchangeType.Direct));

            services.AddSingleton<ISubscriber>(x => new Subscriber(
                x.GetService<IConnectionProvider>(),
                "stockbot-report-exchange",
                "stockbot-report-queue",
                "stockbot-report",
                ExchangeType.Direct));

            services.AddScoped<IStockBotCommandSender, StockBotCommandSender>();
            services.AddHostedService<StockBotCommandReceiver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<FinancialChatHub>("/chatHub");
            });
            

            //env.ApplicationStarted.Register(() => RegisterSignalRWithRabbitMQ(app.ApplicationServices));
        }

        public void RegisterSignalRWithRabbitMQ(IServiceProvider serviceProvider)
        {
            // Connect to RabbitMQ
            //var rabbitMQService = (IRabbitMQService)serviceProvider.GetService(typeof(IRabbitMQService));
            //rabbitMQService.Connect();
        }
    }
}
