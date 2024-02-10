using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BootstrapHtmlHelpers;

public static class HtmlHelperExtensions
{
    public static BootstrapTagBuilder<TModel> Bootstrap<TModel>(this IHtmlHelper<TModel> html)
    {
        return new BootstrapTagBuilder<TModel>(html);
    }
    
    public static IHtmlContent RenderAlerts(this IHtmlHelper html)
    {
        var content = new HtmlContentBuilder();
        if (html.ViewData["alerts"] is not List<Alert> alerts) return content;

        foreach (var alert in alerts)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass($"alert alert-{alert.AlertClass}");
            tag.Attributes["role"] = "alert";
            tag.InnerHtml.Append(alert.Message);
            content.AppendHtml(tag);
        }

        return content;
    }
}
