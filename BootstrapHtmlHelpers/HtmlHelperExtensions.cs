using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BootstrapHtmlHelpers;

public static class HtmlHelperExtensions
{
    public static BootstrapTagBuilder<TModel> Bootstrap<TModel>(this IHtmlHelper<TModel> html)
    {
        return new BootstrapTagBuilder<TModel>(html);
    }
}
