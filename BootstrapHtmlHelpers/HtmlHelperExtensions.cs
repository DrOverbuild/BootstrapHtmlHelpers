using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BootstrapHtmlHelpers;

public static class HtmlHelperExtensions
{
    public const string InvalidClass = "is-invalid";

    #region HTML Helpers

    public static IHtmlContent BsLabelFor<TProperty, TModel>(this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null)
    {
        var expName = html.NameFor(expression);
        var requiredCssClass = html.ViewData.ModelExplorer.GetExplorerForProperty(expName).Metadata.IsRequired ? "is-required" : "";
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes, $"form-label {requiredCssClass}");
        return html.LabelFor(expression, attrsDict);
    }

    public static IHtmlContent BsTextBoxFor<TProperty, TModel>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        string? format = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        AddFormControlCssClassesFor(html, expression, attrsDict);
        var textbox = html.TextBoxFor(expression, format, attrsDict);
        return html.FormGroupFor(expression, textbox, labelHtmlAttributes);
    }
    
    public static IHtmlContent BsTextAreaFor<TProperty, TModel>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        int rows,
        int columns,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        AddFormControlCssClassesFor(html, expression, attrsDict);
        var textarea = html.TextAreaFor(expression, rows, columns, attrsDict);
        return html.FormGroupFor(expression, textarea, labelHtmlAttributes);
    }

    public static IHtmlContent BsPasswordFor<TProperty, TModel>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        AddFormControlCssClassesFor(html, expression, attrsDict);
        var password = html.PasswordFor(expression, attrsDict);
        return html.FormGroupFor(expression, password, labelHtmlAttributes);
    }

    public static IHtmlContent BsDatePickerFor<TProperty, TModel>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        attrsDict["type"] = "date";
        AddFormControlCssClassesFor(html, expression, attrsDict);
        var textbox = html.TextBoxFor(expression, "{0:yyyy-MM-dd}", attrsDict);
        return html.FormGroupFor(expression, textbox, labelHtmlAttributes);
    }

    public static IHtmlContent BsYesNoFor<TModel>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, bool?>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Horizontal,
        object? labelHtmlAttributes = null)
    {
        var idPrefix = html.IdFor(expression);
        var name = html.NameFor(expression);
        var value = html.ValueFor(expression);

        var isInvalid = html.IsInvalid(expression);

        var yesRadioBtn = RadioControlItem(html, id: $"{idPrefix}-Yes", name: name, value: true, text: "Yes",
            isChecked: value == "True", isInvalid: isInvalid);
        var noRadioBtn = RadioControlItem(html, id: $"{idPrefix}-No", name: name, value: false, text: "No",
            isChecked: value == "False", isInvalid: isInvalid);

        var radioDiv = new TagBuilder("div");
        if (layout == RadioButtonLayout.Horizontal) radioDiv.AddCssClass("d-flex gap-3");
        if (isInvalid) radioDiv.AddCssClass(InvalidClass);
        radioDiv.InnerHtml.AppendHtml(yesRadioBtn);
        radioDiv.InnerHtml.AppendHtml(noRadioBtn);
        return html.FormGroupFor(expression, radioDiv, labelHtmlAttributes);
    }

    public static IHtmlContent BsCheckboxFor<TModel>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, bool>> expression,
        object? labelHtmlAttributes = null)
    {
        var errorMessage = html.ValidationMessageFor(expression, null, new { @class = "invalid-feedback" }, "div");

        var cssClasses = new StringBuilder("form-check-input ");
        if (html.IsInvalid(expression)) cssClasses.Append(InvalidClass).Append(' ');
        var checkbox = html.CheckBoxFor(expression, new { @class = cssClasses.ToString() });

        var labelHtmlAttributesDict = ConvertAnonymousObjectIfNeeded(labelHtmlAttributes, "form-check-label");
        var label = html.LabelFor(expression, labelHtmlAttributesDict);

        var checkboxDiv = new TagBuilder("div");
        checkboxDiv.AddCssClass("form-check mb-3");
        checkboxDiv.InnerHtml.AppendHtml(checkbox);
        checkboxDiv.InnerHtml.AppendHtml(label);

        if (errorMessage != null) checkboxDiv.InnerHtml.AppendHtml(errorMessage);
        checkboxDiv.AppendElement("div", html.DescriptionFor(expression), "form-text");

        return checkboxDiv;
    }

    public static IHtmlContent BsDropDownListFor<TModel, TProperty>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        IEnumerable<SelectListItem> items,
        bool displayEmptyFirstValue = true,
        string? emptyFirstValueText = null,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        AddFormControlCssClassesFor(html, expression, attrsDict, "form-select");
        var selectItems = items.ToList();

        if (displayEmptyFirstValue)
        {
            selectItems.Insert(0, new SelectListItem() { Text = emptyFirstValueText ?? "" });
        }

        var control = html.DropDownListFor(expression, selectItems, attrsDict);
        return html.FormGroupFor(expression, control, labelHtmlAttributes);
    }

    public static IHtmlContent BsEnumDropDownListFor<TModel, TProperty>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null) where TProperty : struct, Enum
    {
        var selectList = html.GetEnumSelectList<TProperty>(); // great built in funtion... it knows which enum is already selected!
        return html.BsDropDownListFor(expression, selectList, displayEmptyFirstValue: false, htmlAttributes: htmlAttributes, labelHtmlAttributes: labelHtmlAttributes);
    }

    public static IHtmlContent BsNullableEnumDropDownListFor<TModel, TProperty>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty?>> expression,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null) where TProperty : struct, Enum
    {
        var selectList = html.GetEnumSelectList<TProperty>();
        return html.BsDropDownListFor(expression, selectList, htmlAttributes: htmlAttributes, labelHtmlAttributes: labelHtmlAttributes);
    }



    #endregion

    #region Utility Methods

    public static IHtmlContent FormGroupFor<TProperty, TModel>(
        this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        IHtmlContent control,
        object? labelHtmlAttributes = null)
    {
        var tagBuilder = new TagBuilder("div");
        tagBuilder.AddCssClass("mb-3");

        var errorMessage = html.ValidationMessageFor(expression, null, new { @class = "invalid-feedback" }, "div");

        tagBuilder.InnerHtml.AppendHtml(html.BsLabelFor(expression, labelHtmlAttributes));
        tagBuilder.InnerHtml.AppendHtml(control);
        if (errorMessage != null) tagBuilder.InnerHtml.AppendHtml(errorMessage);
        tagBuilder.AppendElement("div", html.DescriptionFor(expression), "form-text");
        return tagBuilder;
    }

    /// <summary>
    /// Prepares the CSS class list in the given HTML attribute dictionary.
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <param name="controlCssClass">Either form-control, form-select, or whatever other Bootstrap class the control should have</param>
    public static void AddFormControlCssClassesFor<TProperty, TModel>(
        IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression,
        IDictionary<string, object> htmlAttributes,
        string controlCssClass = "form-control")
    {
        var isInvalid = html.IsInvalid(expression);
        StringBuilder cssClass = new StringBuilder(controlCssClass).Append(' ');

        if (isInvalid) cssClass.Append(InvalidClass).Append(' ');
        if (htmlAttributes.TryGetValue("class", out var givenCssClass))
        {
            cssClass.Append(givenCssClass).Append(' ');
        }
        htmlAttributes["class"] = cssClass.ToString();
    }

    public static string? DescriptionFor<TProperty, TModel>(this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression)
    {
        string name = html.NameFor(expression);
        var explorer = html.ViewData.ModelExplorer.GetExplorerForProperty(name);
        return explorer.Metadata.Description;
    }

    public static bool IsInvalid<TProperty, TModel>(this IHtmlHelper<TModel> html,
        Expression<Func<TModel, TProperty>> expression)
    {
        var name = html.NameFor(expression);
        var state = html.ViewData.ModelState.GetValidationState(name);
        return state == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
    }

    private static TagBuilder RadioControlItem(IHtmlHelper html, string id, string name, object value, string text,
        bool disabled = false, bool isChecked = false, bool isInvalid = false)
    {
        var div = new TagBuilder("div");
        div.AddCssClass("form-check");
        var inputAttrs = new Dictionary<string, object>() {
            { "id", id },
            { "class", $"form-check-input {(isInvalid ? InvalidClass: "")}" }
        };

        if (disabled) inputAttrs["disabled"] = "";

        var input = html.RadioButton(name, value, isChecked, inputAttrs);
        var label = html.Label(name, text, new { @for = id, @class = $"form-check-label" });

        div.InnerHtml.AppendHtml(input);
        div.InnerHtml.AppendHtml(label);

        return div;
    }

    /// <summary>
    /// Converts an anonymous object to a dictionary. If the object is of type IDictionary already, the conversion is 
    /// skipped. Pass in a default CSS class string to pre-pend the CSS class to what already exists.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultCssClass"></param>
    /// <returns></returns>
    private static IDictionary<string, object> ConvertAnonymousObjectIfNeeded(object? obj, string? defaultCssClass = null)
    {
        if (obj is not IDictionary<string, object> dict) {
            dict = HtmlHelper.AnonymousObjectToHtmlAttributes(obj) ?? new Dictionary<string, object>();
        }

        if (string.IsNullOrEmpty(defaultCssClass)) return dict;

        if (dict.TryGetValue("class", out object? labelCssClass)) {
            dict["class"] = $"{defaultCssClass} {labelCssClass}";
        } else {
            dict["class"] = $"{defaultCssClass}";
        }

        return dict;
    }

    #endregion
}
