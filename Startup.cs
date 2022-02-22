using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RoteamentoURLs
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(opts => {
                opts.ConstraintMap.Add("parametroLocal", typeof(ConstraintParametroLocal));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            //app.UseMiddleware<MiddlewareConsultaPop>();
            //app.UseMiddleware<MiddlewareConsultaCep>();
            app.UseRouting();
            app.UseEndpoints(endpoints =>{
                endpoints.MapGet("num/{valor:int}", async context =>{
                    await context.Response.WriteAsync("Endpoint para inteiro");
                }).Add(b => ((RouteEndpointBuilder)b).Order =1);
                endpoints.MapGet("num/{valor:double}", async context =>{
                    await context.Response.WriteAsync("Endpoint para double");
                }).Add(b => ((RouteEndpointBuilder)b).Order =2);
                endpoints.MapGet("{p1}/{p2}/{p3}", async context =>{
                //endpoints.MapGet("arq/{arquivo}.{ext}", async context =>{
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("Requisição foi roteada\n");
                    foreach(var item in context.Request.RouteValues)
                    {
                        await context.Response.WriteAsync($"{item.Key}: {item.Value}\n");
                    } 
                });
                endpoints.MapGet("rota", async context =>{
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("Requisição foi roteada");
                });
                endpoints.MapGet("hab/{*local:parametroLocal}", EndpointConsultaPop.Endpoint)
                    .WithMetadata(new RouteNameMetadata("consultapop"));
                endpoints.MapGet("cep/{cep:regex(^\\d{{8}}$)?}", EndpointConsultaCep.Endpoint);
            });
            app.Use(async (context, next) =>{
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("Middleware terminal alcancado");
            });
        }
    }
}
