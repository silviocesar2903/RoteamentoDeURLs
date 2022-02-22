using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public class MiddlewareConsultaCep
{
    private readonly RequestDelegate next;
    
    public MiddlewareConsultaCep(){}

    public MiddlewareConsultaCep(RequestDelegate nextMiddleware)
    {
        next = nextMiddleware;
    }

    public async Task Invoke(HttpContext context)
    {
       string cep = context.Request.RouteValues["cep"] as string;
       var objetoCep = await ConsultaCep(cep);
       context.Response.ContentType = "text/html; charset=utf-8";
       StringBuilder html = new StringBuilder();
       html.Append($"<h3>Dados do Cep {objetoCep.CEP}</h3>");
       html.Append($"<p>Logradouro: {objetoCep.Logradouro}</p>");
       html.Append($"<p>Bairro: {objetoCep.Bairro}</p>");
       html.Append($"<p>Cidade/Uf: {objetoCep.Localidade}/{objetoCep.Estado}</p>");
       string localidade = HttpUtility.UrlDecode($"{objetoCep.Localidade}-{objetoCep.Estado}");
       html.Append($"<p><a href=/pop/{localidade}'> Consultar Populacao</p>");
       await context.Response.WriteAsync(html.ToString());
       if(next != null)
        {
            await next(context);
        }
    }

    private async Task<JsonCep> ConsultaCep(string cep)
    {
        var url = $"https://viacep.com.br/ws/{cep}/json";
        var cliente = new HttpClient();
        cliente.DefaultRequestHeaders.Add("User-Agent", "Middleware Consulta Cep");
        var response = await cliente.GetAsync(url);
        
        var dadosCep = await response.Content.ReadAsStringAsync();
        dadosCep = dadosCep.Replace("?(","").Replace(");", "").Trim();
        return JsonConvert.DeserializeObject<JsonCep>(dadosCep);
    }
}