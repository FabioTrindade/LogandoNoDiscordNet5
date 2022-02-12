using JNogueira.Logger.Discord;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace LogandoNoDiscordNet5
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
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LogandoNoDiscordNet5", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LogandoNoDiscordNet5 v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Set Config Discord
            SetConfigureDiscord(app, env);
        }

        public void SetConfigureDiscord(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string webHookDiscord = Configuration.GetValue<string>("Discord:WebhookUrl");

            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            // Adicionamos a injeção de dependência
            var httpContext = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

            // Obtemos uma instância de LoggerFactory para adicionar o provider do Discord.
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            // Adiciona o logger provider para o Discord.
            loggerFactory.AddDiscord(new DiscordLoggerOptions(webHookDiscord) // URL do webhook do Discord por onde as mensagens serão enviadas
            {
                ApplicationName = "LogandoNoDiscordNet5", // Nome da nossa aplicação
                EnvironmentName = env.ApplicationName, // Ambiente em qual a aplicação está sendo executada
                UserName = "bot" // Nome do usuário responsável pelo envio da mensagem no canal do Discord (pode ser qualquer nome).
            }, httpContext);
        }
    }
}
