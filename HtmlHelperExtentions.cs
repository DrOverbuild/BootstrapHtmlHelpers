using BootstrapExtensions.HtmlHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;

namespace BootstrapExtensions.Extensions;

public static class HtmlHelperExtentions
{
    public static BootstrapHtmlHelper<TModel> Bootstrap<TModel>(this IHtmlHelper<TModel> htmlHelper)
    {
        return new BootstrapHtmlHelper<TModel>(htmlHelper);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="htmlHelper"></param>
    /// <param name="formControlSize">Sets the form controls to either form-control-lg, form-control-sm, or the default size</param>
    /// <returns></returns>
    public static BootstrapHtmlHelper<TModel> Bootstrap<TModel>(this IHtmlHelper<TModel> htmlHelper, FormControlSize formControlSize)
    {
        return new BootstrapHtmlHelper<TModel>(htmlHelper, formControlSize);
    }

    public static PaginationHtmlHelper Pagination<TModel>(this IHtmlHelper<TModel> htmlHelper)
    {
        return new PaginationHtmlHelper();
    } 

    /// <summary>
    /// Strongly typed method that checks is a property has an error on it. Useful for radiobutton or checkbox lists that aren't tightly bound to the model. Allowing you to add a custom html class
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="htmlHelper"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static bool HasErrorFor<TModel, TProperty>(
       this IHtmlHelper<TModel> htmlHelper,
       Expression<Func<TModel, TProperty>> expression)
    {
        string fullName = htmlHelper.NameFor(expression);
        return htmlHelper.ViewData.ModelState.GetFieldValidationState(fullName) == ModelValidationState.Invalid;
    }

    /// <summary>
    /// Gets css error class when there is an error on the model property
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="htmlHelper"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string ErrorClassFor<TModel, TProperty>(
       this IHtmlHelper<TModel> htmlHelper,
       Expression<Func<TModel, TProperty>> expression,
       string errorClassName = "is-invalid")
    {
        return HasErrorFor(htmlHelper, expression) ? errorClassName : "";
    }

    public static string ErrorMessageFor<TModel, TProperty>(
           this IHtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, TProperty>> expression)
    {
        string fullName = htmlHelper.NameFor(expression);
        var modelState = htmlHelper.ViewData.ModelState[fullName];
        return modelState != null && modelState.Errors.Count > 0 ? modelState.Errors.First().ErrorMessage : "";
    }

    public static int? MaxLengthFor<TModel, TProperty>(
         this IHtmlHelper<TModel> htmlHelper,
         Expression<Func<TModel, TProperty>> expression)
    {
        if (expression.Body is MemberExpression body)
        {
            object[] attrs = body.Member.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.StringLengthAttribute), false);

            if (attrs != null && attrs.Length > 0)
            {
                //Pull out the length value
                return ((System.ComponentModel.DataAnnotations.StringLengthAttribute)attrs[0]).MaximumLength;
            }
        }
        return null;
    }

    public static object? GetValueFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> accessor)
    {
        var model = htmlHelper.ViewData.Model;
        if (model == null || accessor == null) return null;
        var func = accessor.Compile();
        return func.Invoke(model);
    }
    
    public static IEnumerable<SelectListItem> WithBlank(this List<SelectListItem> selectListItems, string text = "") {
        selectListItems.Insert(0, new SelectListItem {Text = text, Value = ""});
        return selectListItems;
    }

    public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> source, Func<T, string> text, Func<T, string> value) {
        return source.Select(item => new SelectListItem {Text = text(item), Value = value(item)}).ToList();
    }
}
