using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BootstrapHtmlHelpers;

public static class TagBuilderExtensions
{
    public static void AppendElement(this TagBuilder tagBuilder, string element, IHtmlContent? innerHtmlContent, string? cssClass = null)
    {
        if (innerHtmlContent == null) return;

        var inner = new TagBuilder(element);
        if (!string.IsNullOrEmpty(cssClass)) inner.AddCssClass(cssClass);
        inner.InnerHtml.AppendHtml(innerHtmlContent);
        tagBuilder.InnerHtml.AppendHtml(inner);
    }

    public static void AppendElement(this TagBuilder tagBuilder, string element, string? innerContent, string? cssClass = null)
    {
        if (string.IsNullOrEmpty(innerContent)) return;

        var inner = new TagBuilder(element);
        if (!string.IsNullOrEmpty(cssClass)) inner.AddCssClass(cssClass);
        inner.InnerHtml.Append(innerContent);
        tagBuilder.InnerHtml.AppendHtml(inner);
    }

    public static void AddAttributes(this TagBuilder tagBuilder, IDictionary<string, object> attrs)
    {
        foreach (var attrsKey in attrs.Keys)
        {
            tagBuilder.Attributes[attrsKey] = attrs[attrsKey].ToString();
        }
    }
}
