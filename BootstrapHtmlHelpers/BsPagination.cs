using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BootstrapHtmlHelpers;

public static class BsPagination
{
    public static int TotalPages(int totalItems, int itemsPerPage)
    {
        return (int)Math.Ceiling((decimal)totalItems / itemsPerPage);
    }
    
    public static IHtmlContent Paginate(int currentPage, int totalPages, Func<int, string> buildHref, string ulClasses = "", string nonLinkClass = "page-text")
    {
        currentPage = int.Max(1, int.Min(totalPages, currentPage));
        
        var ul = new TagBuilder("ul");
        ul.AddCssClass($"pagination justify-content-center {ulClasses}");

        // build "Previous"
        if (currentPage > 1)
        {
            ul.InnerHtml.AppendHtml(BuildPageLink(buildHref(currentPage - 1), "Previous"));
        }

        // build first and ellipsis
        if (currentPage > 2)
        {
            ul.InnerHtml.AppendHtml(BuildPageLink(buildHref(1), "1"));
            ul.InnerHtml.AppendHtml(BuildPageItem("&hellip;", nonLinkClass));
        }

        // build previous
        if (currentPage > 1)
        {
            ul.InnerHtml.AppendHtml(BuildPageLink(buildHref(currentPage - 1), (currentPage - 1).ToString()));
        }

        // build active
        ul.InnerHtml.AppendHtml(BuildPageLink(buildHref(currentPage), currentPage.ToString(), "active"));

        // build next
        if (currentPage < totalPages)
        {
            ul.InnerHtml.AppendHtml(BuildPageLink(buildHref(currentPage + 1), (currentPage + 1).ToString()));
        }

        // build ellipsis and last
        if (currentPage < totalPages - 1)
        {
            ul.InnerHtml.AppendHtml(BuildPageItem("&hellip;", nonLinkClass));
            ul.InnerHtml.AppendHtml(BuildPageLink(buildHref(totalPages), totalPages.ToString()));
        }

        // build "Next"
        if (currentPage < totalPages)
        {
            ul.InnerHtml.AppendHtml(BuildPageLink(buildHref(currentPage + 1), "Next"));
        }

        var nav = new TagBuilder("nav");
        nav.Attributes["aria-label"] = "Page navigation";
        nav.InnerHtml.AppendHtml(ul);
        return nav;
    }

    private static TagBuilder BuildPageLink(string href, string aInnerHtml, string liClass = "")
    {
        var li = new TagBuilder("li");
        li.AddCssClass($"page-item {liClass}");
        var a = new TagBuilder("a");
        a.AddCssClass("page-link");
        a.Attributes["href"] = href;
        a.InnerHtml.Append(aInnerHtml);
        li.InnerHtml.AppendHtml(a);
        return li;
    }

    private static TagBuilder BuildPageItem(string text, string spanClass = "page-text")
    {
        var li = new TagBuilder("li");
        li.AddCssClass("page-item");
        var span = new TagBuilder("span");
        span.AddCssClass(spanClass);
        span.InnerHtml.AppendHtml(text);
        li.InnerHtml.AppendHtml(span);
        return li;
    }
}