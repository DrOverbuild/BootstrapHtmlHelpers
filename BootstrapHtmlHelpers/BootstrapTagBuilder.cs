using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BootstrapHtmlHelpers;

public class BootstrapTagBuilder<TModel>
{
    public const string InvalidClass = "is-invalid";

    private readonly ModelExpressionProvider _expProv;
    private readonly IHtmlHelper<TModel> _html;

    public BootstrapTagBuilder(IHtmlHelper<TModel> html)
    {
        _html = html;
        _expProv = new ModelExpressionProvider(_html.MetadataProvider);
    }

    #region Component Builders

    public IHtmlContent LabelFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null)
    {
        var metadata = MetadataFor(expression);
        var requiredCssClass = metadata.IsRequired ? "is-required" : "";
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes, $"form-label {requiredCssClass}");
        return _html.LabelFor(expression, attrsDict);
    }

    public IHtmlContent TextBoxFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        string? format = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        AddFormControlCssClassesFor(expression, attrsDict);
        var textbox = _html.TextBoxFor(expression, format, attrsDict);
        return FormGroupFor(expression, textbox, labelHtmlAttributes);
    }

    public IHtmlContent TextAreaFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        int rows,
        int columns,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        AddFormControlCssClassesFor(expression, attrsDict);
        var textarea = _html.TextAreaFor(expression, rows, columns, attrsDict);
        return FormGroupFor(expression, textarea, labelHtmlAttributes);
    }

    public IHtmlContent PasswordFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        AddFormControlCssClassesFor(expression, attrsDict);
        var password = _html.PasswordFor(expression, attrsDict);
        return FormGroupFor(expression, password, labelHtmlAttributes);
    }

    public IHtmlContent DatePickerFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        attrsDict["type"] = "date";
        AddFormControlCssClassesFor(expression, attrsDict);
        var textbox = _html.TextBoxFor(expression, "{0:yyyy-MM-dd}", attrsDict);
        return FormGroupFor(expression, textbox, labelHtmlAttributes);
    }

    public IHtmlContent YesNoFor(
        Expression<Func<TModel, bool?>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Horizontal,
        object? labelHtmlAttributes = null)
    {
        var idPrefix = _html.IdFor(expression);
        var name = _html.NameFor(expression);
        var value = _html.ValueFor(expression);

        var isInvalid = IsInvalid(expression);

        var yesRadioBtn = RadioControlItem(id: $"{idPrefix}-Yes", name: name, value: true, text: "Yes",
            isChecked: value == "True", isInvalid: isInvalid);
        var noRadioBtn = RadioControlItem(id: $"{idPrefix}-No", name: name, value: false, text: "No",
            isChecked: value == "False", isInvalid: isInvalid);

        var radioDiv = new TagBuilder("div");
        if (layout == RadioButtonLayout.Horizontal) radioDiv.AddCssClass("d-flex gap-3");
        if (isInvalid) radioDiv.AddCssClass(InvalidClass);
        radioDiv.InnerHtml.AppendHtml(yesRadioBtn);
        radioDiv.InnerHtml.AppendHtml(noRadioBtn);
        return FormGroupFor(expression, radioDiv, labelHtmlAttributes);
    }

    public IHtmlContent CheckboxFor(
        Expression<Func<TModel, bool>> expression,
        object? labelHtmlAttributes = null)
    {
        var errorMessage = _html.ValidationMessageFor(expression, null, new { @class = "invalid-feedback" }, "div");

        var cssClasses = new StringBuilder("form-check-input ");
        if (IsInvalid(expression)) cssClasses.Append(InvalidClass).Append(' ');
        var checkbox = _html.CheckBoxFor(expression, new { @class = cssClasses.ToString() });

        var labelHtmlAttributesDict = ConvertAnonymousObjectIfNeeded(labelHtmlAttributes, "form-check-label");
        var label = _html.LabelFor(expression, labelHtmlAttributesDict);

        var checkboxDiv = new TagBuilder("div");
        checkboxDiv.AddCssClass("form-check mb-3");
        checkboxDiv.InnerHtml.AppendHtml(checkbox);
        checkboxDiv.InnerHtml.AppendHtml(label);

        if (errorMessage != null) checkboxDiv.InnerHtml.AppendHtml(errorMessage);
        checkboxDiv.AppendElement("div", DescriptionFor(expression), "form-text");

        return checkboxDiv;
    }

    public IHtmlContent DropDownListFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        IEnumerable<SelectListItem> items,
        bool displayEmptyFirstValue = true,
        string? emptyFirstValueText = null,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        AddFormControlCssClassesFor(expression, attrsDict, "form-select");
        var selectItems = items.ToList();

        if (displayEmptyFirstValue)
        {
            selectItems.Insert(0, new SelectListItem() { Text = emptyFirstValueText ?? "" });
        }

        var control = _html.DropDownListFor(expression, selectItems, attrsDict);
        return FormGroupFor(expression, control, labelHtmlAttributes);
    }

    public IHtmlContent EnumDropDownListFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null) where TProperty : struct, Enum
    {
        var selectList =
            _html.GetEnumSelectList<TProperty>(); // great built in funtion... it knows which enum is already selected!
        return DropDownListFor(expression, selectList, displayEmptyFirstValue: false, htmlAttributes: htmlAttributes,
            labelHtmlAttributes: labelHtmlAttributes);
    }

    public IHtmlContent NullableEnumDropDownListFor<TProperty>(
        Expression<Func<TModel, TProperty?>> expression,
        object? htmlAttributes = null,
        object? labelHtmlAttributes = null) where TProperty : struct, Enum
    {
        var selectList = _html.GetEnumSelectList<TProperty>();
        return DropDownListFor(expression, selectList, htmlAttributes: htmlAttributes,
            labelHtmlAttributes: labelHtmlAttributes);
    }

    #endregion

    #region Utility Methods

    public IHtmlContent FormGroupFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        IHtmlContent control,
        object? labelHtmlAttributes = null)
    {
        var tagBuilder = new TagBuilder("div");
        tagBuilder.AddCssClass("mb-3");

        var errorMessage = _html.ValidationMessageFor(expression, null, new { @class = "invalid-feedback" }, "div");

        tagBuilder.InnerHtml.AppendHtml(LabelFor(expression, labelHtmlAttributes));
        tagBuilder.InnerHtml.AppendHtml(control);
        if (errorMessage != null) tagBuilder.InnerHtml.AppendHtml(errorMessage);
        tagBuilder.AppendElement("div", DescriptionFor(expression), "form-text");
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
    public void AddFormControlCssClassesFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        IDictionary<string, object> htmlAttributes,
        string controlCssClass = "form-control")
    {
        var isInvalid = IsInvalid(expression);
        StringBuilder cssClass = new StringBuilder(controlCssClass).Append(' ');

        if (isInvalid) cssClass.Append(InvalidClass).Append(' ');
        if (htmlAttributes.TryGetValue("class", out var givenCssClass))
        {
            cssClass.Append(givenCssClass).Append(' ');
        }

        htmlAttributes["class"] = cssClass.ToString();
    }

    public string? DescriptionFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
    {
        var metadata = MetadataFor(expression);
        return metadata.Description;
    }

    public bool IsInvalid<TProperty>(Expression<Func<TModel, TProperty>> expression)
    {
        var name = _html.NameFor(expression);
        var state = _html.ViewData.ModelState.GetValidationState(name);
        return state == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
    }

    private TagBuilder RadioControlItem(string id, string name, object value, string text,
        bool disabled = false, bool isChecked = false, bool isInvalid = false)
    {
        var div = new TagBuilder("div");
        div.AddCssClass("form-check");
        var inputAttrs = new Dictionary<string, object>()
        {
            { "id", id },
            { "class", $"form-check-input {(isInvalid ? InvalidClass : "")}" }
        };

        if (disabled) inputAttrs["disabled"] = "";

        var input = _html.RadioButton(name, value, isChecked, inputAttrs);
        var label = _html.Label(name, text, new { @for = id, @class = $"form-check-label" });

        div.InnerHtml.AppendHtml(input);
        div.InnerHtml.AppendHtml(label);

        return div;
    }

    private ModelMetadata MetadataFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
    {
        return _expProv.CreateModelExpression(_html.ViewData, expression).Metadata;
    }

    /// <summary>
    /// Converts an anonymous object to a dictionary. If the object is of type IDictionary already, the conversion is 
    /// skipped. Pass in a default CSS class string to pre-pend the CSS class to what already exists.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultCssClass"></param>
    /// <returns></returns>
    private static IDictionary<string, object> ConvertAnonymousObjectIfNeeded(object? obj,
        string? defaultCssClass = null)
    {
        if (obj is not IDictionary<string, object> dict)
        {
            dict = HtmlHelper.AnonymousObjectToHtmlAttributes(obj) ?? new Dictionary<string, object>();
        }

        if (string.IsNullOrEmpty(defaultCssClass)) return dict;

        if (dict.TryGetValue("class", out object? labelCssClass))
        {
            dict["class"] = $"{defaultCssClass} {labelCssClass}";
        }
        else
        {
            dict["class"] = $"{defaultCssClass}";
        }

        return dict;
    }

    #endregion
}