using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

public static class EndpointConsultaCep
{
    

    public static async Task Endpoint(HttpContext context)
    {
       string cep = context.Request.RouteValues["cep"] as string ?? "01001000";
       var objetoCep = await ConsultaCep(cep);
       if(objetoCep == null)
       {
           context.Response.StatusCode = StatusCodes.Status404NotFound;
       }else
       {
        context.Response.ContentType = "text/html; charset=utf-8";
        StringBuilder html = new StringBuilder();
        html.Append($"<h3>Dados do Cep {objetoCep.CEP}</h3>");
        html.Append($"<p>Logradouro: {objetoCep.Logradouro}</p>");
        html.Append($"<p>Bairro: {objetoCep.Bairro}</p>");
        html.Append($"<p>Cidade/Uf: {objetoCep.Localidade}/{objetoCep.Estado}</p>");
        string localidade = HttpUtility.UrlDecode($"{objetoCep.Localidade}-{objetoCep.Estado}");
        LinkGenerator geradorLink = context.RequestServices.GetService<LinkGenerator>();
        string url = geradorLink.GetPathByRouteValues(
            context,"consultapop" ,new{local = localidade} 
        );
        html.Append($"<p><a href={url}> Consultar Populacao</p>");
        await context.Response.WriteAsync(html.ToString());
       }
    }

    private static async Task<JsonCep> ConsultaCep(string cep)
    {
        var url = $"https://viacep.com.br/ws/{cep}/json";
        var cliente = new HttpClient();
        cliente.DefaultRequestHeaders.Add("User-Agent", "Middleware Consulta Cep");
        var response = await cliente.GetAsync(url);
        
        var dadosCep = await response.Content.ReadAsStringAsync();
        dadosCep = dadosCep.Replace("?(","").Replace(");", "").Trim();
        return dadosCep.Contains("\"erro\":") ? null : JsonConvert.DeserializeObject<JsonCep>(dadosCep);
    }
}