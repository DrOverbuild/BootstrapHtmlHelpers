using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BootstrapHtmlHelpers;

public static class HtmlHelperExtensions
{
    public static BootstrapTagBuilder<TModel> Bootstrap<TModel>(this IHtmlHelper<TModel> html)
    {
        return new BootstrapTagBuilder<TModel>(html);
    }

    public static IHtmlContent RenderAlerts(this IHtmlHelper html, bool pullTempData = true)
    {
        var content = new HtmlContentBuilder();
        if (html.ViewData["alerts"] is List<Alert> alerts)
        {
            content.RenderAndAppend(alerts);
        }


        if (pullTempData && html.TempData["alerts"] is IEnumerable<string> tempDataAlerts)
        {
            foreach (var tempDataAlert in tempDataAlerts)
            {
                content.AppendHtml(tempDataAlert);
            }
            
            html.TempData.Remove("alerts");
        }

        return content;
    }

    public static void RenderAndAppend(this HtmlContentBuilder htmlContent, IEnumerable<Alert> alerts)
    {
        foreach (var alert in alerts)
        {
            htmlContent.RenderAndAppend(alert);
        }
    }

    public static void RenderAndAppend(this HtmlContentBuilder htmlContent, Alert alert)
    {
        var tag = new TagBuilder("div");
        tag.AddCssClass($"alert alert-{alert.AlertClass.ToString().ToLowerInvariant()}");
        tag.Attributes["role"] = "alert";
        tag.InnerHtml.AppendHtml(alert.Message);
        htmlContent.AppendHtml(tag);        
    }
}