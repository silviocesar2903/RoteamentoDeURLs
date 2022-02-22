using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public class ConstraintParametroLocal : IRouteConstraint
{
    private static string[] estados ={"es", "mg", "rj", "sp"};
    public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        string valorParametro = values[routeKey] as string ?? "";
        string estado = valorParametro.Substring(valorParametro.Length -2);
        return Array.IndexOf(estados, estado.ToLower()) > -1;
    }
}