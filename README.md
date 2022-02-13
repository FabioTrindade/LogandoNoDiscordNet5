# LogandoNoDiscordNet5
Utilizando o Discord como repositório de log em NET 5

# Criando um webhook no Discord
Os webhooks é uma forma de recebimento de informações, que são passadas quando um evento acontece ou seja nada mais são do que uma URL criada para um canal do Discord. Essa URL, ao receber um request, envia uma mensagem para o seu respectivo canal.

Para mais informações de como criar um webhook para um canal do Discord, consulte https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhooks

Para esse projeto eu criei um canal qualquer e para ele realizei a criação de um webhook:

<img width="918" alt="imagem_1" src="https://user-images.githubusercontent.com/30089341/153731545-1455d339-967e-42b0-9c90-2f18ea2b8a63.png">


# Iniciando o projeto
Nesse exemplo utilizamos um projeto ASP.NET Core Web API utilizando .NET 5. Para criar o projeto foi utilizado a IDE do Visual Studio Community 2019 for Mac.

# Adicionando o pacote
Iremos utilizar o pacote:

- logger-discord-provider

Este foi adicionado via nuget

# Configurando o projeto
Vamos alterar o arquivo Startup.cs, adicionando as modificações necessárias para a utilização do pacote
``` C#
        public void SetConfigureDiscord(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // URL do webhook do Discord por onde as mensagens serão enviadas
            string webHookDiscord = Configuration.GetValue<string>("Discord:WebhookUrl");

            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            // Adicionamos a injeção de dependência
            var httpContext = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

            // Obtemos uma instância de LoggerFactory para adicionar o provider do Discord.
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            // Adiciona o logger provider para o Discord.
            loggerFactory.AddDiscord(new DiscordLoggerOptions(webHookDiscord)
            {
                ApplicationName = "LogandoNoDiscordNet5", // Nome da nossa aplicação
                EnvironmentName = env.ApplicationName, // Ambiente em qual a aplicação está sendo executada
                UserName = "bot" // Nome do usuário responsável pelo envio da mensagem no canal do Discord (pode ser qualquer nome).
            }, httpContext);
        }
```

Realizamos a chamada do método passando os parâmetros esperados
``` C#
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
```

No arquivo de configuração appSettings.json devemos colocar a URL do webhook que será utilizada:

``` C#
 "Discord": {
    "WebhookUrl": "https://discord.com/api/webhooks/..."
  }
```
> Aquela url criada lá no Discord na opcão `Novo webhook`


# Enviando mensagens para o Discord
No projeto temos o controller WeatherForecastController. Iremos alterar a rota HttpGet (método Get), incluindo a chamada ao método LogInformation de ILogger:
``` C#
        [HttpGet]
        public IEnumerable<WeatherForecastModel> Get()
        {
            // Envia a mensagem para o canal do Discord (via webhook)
            _logger.LogInformation("Olá! Você recebeu uma mensagem pelo Discord.");

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastModel
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
```

Vamos rodar o projeto, se tudo deu bom a página abaixo deverá ser visualizada:
<img width="1440" alt="imagem_1_1" src="https://user-images.githubusercontent.com/30089341/153732864-845d2096-74b8-4a31-b7bd-93d38324db74.png">

Expandindo a rota GET, clique no botão `Try it out` em seguida em `Execute`.
<img width="1440" alt="imagem_2_1" src="https://user-images.githubusercontent.com/30089341/153732879-fc60fff4-1536-48cb-98de-3a4ae7653d29.png">

Ao fazer isso, você deverá receber a seguinte mensagem no canal do Discord:
<img width="523" alt="imagem_2" src="https://user-images.githubusercontent.com/30089341/153732900-235a0eef-991b-45ba-8c9e-5d7b3482b804.png">

# Logando exceptions no Discord
Outra aplicação do componente é utilizá-lo para enviar detalhes de uma exception disparada para um canal do Discord.

No projeto, vamos agora alterar a rota HttpPost (método POST), forçando que uma exception seja disparada (DivideByZeroException):
``` C#
        [HttpPost]
        public IEnumerable<WeatherForecastModel> Post()
        {
            var i = 0;

            // Dispara a exception em runtime
            var x = 5 / i;

            return new List<WeatherForecastModel>();
        }
```

Alterado o método POST, vamos rodar o projeto novamente. Exibida a página da API, vamos agora clicar no botão `Try it out` e em `Execute` da rota POST:
<img width="1439" alt="imagem_3_1" src="https://user-images.githubusercontent.com/30089341/153732940-59d348da-2466-45bc-b0a2-685e08fd50cd.png">


Ao fazer isso, você deverá receber a seguinte mensagem no canal do Discord:
<img width="710" alt="imagem_3" src="https://user-images.githubusercontent.com/30089341/153732948-8fc31bc5-5fa5-4374-bd80-d471c5faa254.png">

No registro de exceptions, a mensagem enviada para o Discord contém um arquivo txt (exception-details.txt) com informações relacionadas a exception disparada (tipo da exception, base exception, stack trace, request headers).

Com isso finalizamos nosso projeto!


# Considerações finais
A implementação em NET 5 foi inspirada na publicação do blog https://balta.io/blog/utilizando-discord-repositorio-log-dotnet

O Discord é uma plataforma que conta com uma API muito rica e possibilita a utilização de vários outros recursos. Para maiores informações acesse https://discord.com/developers/docs/intro .

O repositório do componente Discord .NET Logger Provider está disponível em https://github.com/jlnpinheiro/discord-webhook-client.
