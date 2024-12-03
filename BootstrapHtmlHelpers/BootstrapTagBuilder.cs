using System.Collections;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;

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

    #region Component Group Builders

    public IHtmlContent TextBoxFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        string? format = null,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
    {
        var textbox = TextBoxControlFor(expression, htmlAttributes, format);
        return FormGroupFor(expression, textbox, labelHtmlAttributes, containerHtmlAttributes, validationHtmlAttributes);
    }

    public IHtmlContent TextAreaFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        int rows,
        int columns,
        object? inputAttributes = null,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
    {
        var textarea = TextAreaControlFor(expression, rows, columns, inputAttributes);
        return FormGroupFor(expression, textarea, labelHtmlAttributes, containerHtmlAttributes, validationHtmlAttributes);
    }

    public IHtmlContent PasswordFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? inputAttributes = null,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
    {
        var password = PasswordControlFor(expression, inputAttributes);
        return FormGroupFor(expression, password, labelHtmlAttributes, containerHtmlAttributes, validationHtmlAttributes);
    }

    public IHtmlContent DatePickerFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? inputAttributes = null,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
    {
        var textbox = DatePickerControlFor(expression, inputAttributes);
        return FormGroupFor(expression, textbox, labelHtmlAttributes, containerHtmlAttributes, validationHtmlAttributes);
    }

    public IHtmlContent YesNoFor(
        Expression<Func<TModel, bool>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Horizontal,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
    {
        var control = YesNoControlFor(expression, layout);
        return FormGroupFor(expression, control, labelHtmlAttributes, containerHtmlAttributes, validationHtmlAttributes);
    }

    public IHtmlContent YesNoFor(
        Expression<Func<TModel, bool?>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Horizontal,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
    {
        var control = YesNoControlFor(expression, layout);
        return FormGroupFor(expression, control, labelHtmlAttributes, containerHtmlAttributes, validationHtmlAttributes);
    }

    public IHtmlContent CheckboxFor(
        Expression<Func<TModel, bool>> expression,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
    {
        var validationAttrs = ConvertAnonymousObjectIfNeeded(validationHtmlAttributes, "invalid-feedback");
        var errorMessage = _html.ValidationMessageFor(expression, null, validationAttrs, "div");

        var cssClasses = new StringBuilder("form-check-input ");
        if (IsInvalid(expression)) cssClasses.Append(InvalidClass).Append(' ');
        var checkbox = _html.CheckBoxFor(expression, new { @class = cssClasses.ToString() });

        var labelHtmlAttributesDict = ConvertAnonymousObjectIfNeeded(labelHtmlAttributes, "form-check-label");
        var label = _html.LabelFor(expression, labelHtmlAttributesDict);

        var checkboxDiv = new TagBuilder("div");
        checkboxDiv.AddCssClass("form-check mb-3");
        checkboxDiv.AddAttributes(ConvertAnonymousObjectIfNeeded(containerHtmlAttributes, "form-check mb-3"));
        checkboxDiv.InnerHtml.AppendHtml(checkbox);
        checkboxDiv.InnerHtml.AppendHtml(label);

        if (errorMessage != null) checkboxDiv.InnerHtml.AppendHtml(errorMessage);
        checkboxDiv.AppendElement("div", DescriptionFor(expression), "form-text");

        return checkboxDiv;
    }

    public IHtmlContent CheckboxGroupFor<TProperty>(
        Expression<Func<TModel, IEnumerable<TProperty>>> expression,
        string[]? selectedItems = null,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
        where TProperty : struct, Enum
    {
        var control = CheckboxGroupControlFor(expression, selectedItems);
        return FormGroupFor(expression, control, labelHtmlAttributes, containerHtmlAttributes, validationHtmlAttributes);
    }

    public IHtmlContent DropDownListFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        IEnumerable<SelectListItem> items,
        bool displayEmptyFirstValue = true,
        string? emptyFirstValueText = null,
        bool emptyFirstValueDisabled = true,
        object? selectHtmlAttributes = null,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
    {
        var control = DropDownListControlFor(expression, items, displayEmptyFirstValue, emptyFirstValueText,
            emptyFirstValueDisabled, selectHtmlAttributes);
        return FormGroupFor(expression, control, labelHtmlAttributes, containerHtmlAttributes,
            validationHtmlAttributes);
    }

    public IHtmlContent EnumDropDownListFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? selectHtmlAttributes = null,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null) where TProperty : struct, Enum
    {
        var control = EnumDropDownListControlFor(expression, selectHtmlAttributes);
        return FormGroupFor(expression, control,
            labelHtmlAttributes: labelHtmlAttributes,
            containerHtmlAttributes: containerHtmlAttributes,
            validationHtmlAttributes: validationHtmlAttributes);
    }

    public IHtmlContent NullableEnumDropDownListFor<TProperty>(
        Expression<Func<TModel, TProperty?>> expression,
        object? selectHtmlAttributes = null,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null,
        string? emptyFirstValueText = null,
        bool emptyFirstValueDisabled = true) where TProperty : struct, Enum
    {
        var control = NullableEnumDropDownListControlFor(expression, selectHtmlAttributes, emptyFirstValueText,
            emptyFirstValueDisabled);
        return FormGroupFor(expression, control,
            labelHtmlAttributes: labelHtmlAttributes,
            containerHtmlAttributes: containerHtmlAttributes,
            validationHtmlAttributes: validationHtmlAttributes);
    }

    public IHtmlContent EnumRadioGroupFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Vertical,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? radioGroupHtmlAttributes = null,
        object? validationHtmlAttributes = null) where TProperty : struct, Enum
    {
        var control = EnumRadioGroupControlFor(expression, layout, radioGroupHtmlAttributes);
        return FormGroupFor(expression, control,
            labelHtmlAttributes: labelHtmlAttributes,
            containerHtmlAttributes: containerHtmlAttributes,
            validationHtmlAttributes: validationHtmlAttributes);
    }

    public IHtmlContent NullableEnumRadioGroupFor<TProperty>(
        Expression<Func<TModel, TProperty?>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Vertical,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? radioGroupHtmlAttributes = null,
        object? validationHtmlAttributes = null) where TProperty : struct, Enum
    {
        var control = NullableEnumRadioGroupControlFor(expression, layout, radioGroupHtmlAttributes);
        return FormGroupFor(expression, control,
            labelHtmlAttributes: labelHtmlAttributes,
            containerHtmlAttributes: containerHtmlAttributes,
            validationHtmlAttributes: validationHtmlAttributes);
    }

    #endregion

    #region Individual Component Builders

    public IHtmlContent LabelFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null)
    {
        var metadata = MetadataFor(expression);
        var requiredCssClass = metadata.IsRequired ? "is-required" : "";
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes, $"form-label {requiredCssClass}");
        return _html.LabelFor(expression, attrsDict);
    }

    public IHtmlContent TextBoxControlFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? htmlAttributes = null,
        string? format = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(htmlAttributes);
        AddFormControlCssClassesFor(expression, attrsDict);
        return _html.TextBoxFor(expression, format, attrsDict);
    }

    public IHtmlContent TextAreaControlFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        int rows,
        int columns,
        object? inputAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(inputAttributes);
        AddFormControlCssClassesFor(expression, attrsDict);
        return _html.TextAreaFor(expression, rows, columns, attrsDict);
    }

    public IHtmlContent PasswordControlFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? inputAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(inputAttributes);
        AddFormControlCssClassesFor(expression, attrsDict);
        return _html.PasswordFor(expression, attrsDict);
    }

    public IHtmlContent DatePickerControlFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? inputAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(inputAttributes);
        attrsDict["type"] = "date";
        AddFormControlCssClassesFor(expression, attrsDict);
        return _html.TextBoxFor(expression, "{0:yyyy-MM-dd}", attrsDict);
    }

    public IHtmlContent YesNoControlFor(
        Expression<Func<TModel, bool>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Horizontal,
        object? radioGroupHtmlAttributes = null)
    {
        var idPrefix = _html.IdFor(expression);
        var name = _html.NameFor(expression);
        var value = _html.ValueFor(expression);

        var isInvalid = IsInvalid(expression);

        return RenderYesNoControl(layout, idPrefix, name, value, isInvalid, radioGroupHtmlAttributes);
    }

    /// <summary>
    /// To render the page with the form control initially blank, make the property a nullable boolean and add the
    /// [Required] attribute. Rendering the initial form this way is recommended for good UX. If a Yes/No control is
    /// required but a value is preselected, there's a greater chance of the user missing the control when filling the
    /// form. 
    /// </summary>
    /// <remarks>
    /// The YesNoControlFor overload that takes in the expression without the nullable boolean (<see cref="YesNoControlFor(System.Linq.Expressions.Expression{System.Func{TModel,bool}},BootstrapHtmlHelpers.RadioButtonLayout)"/>)
    /// is still provided because IHtmlHelper&lt;T&gt;.ValueFor throws an exception when passing a non-nullable
    /// boolean expression through this overload. 
    /// </remarks>
    public IHtmlContent YesNoControlFor(
        Expression<Func<TModel, bool?>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Horizontal,
        object? radioGroupHtmlAttributes = null)
    {
        var idPrefix = _html.IdFor(expression);
        var name = _html.NameFor(expression);
        var value = _html.ValueFor(expression);

        var isInvalid = IsInvalid(expression);

        return RenderYesNoControl(layout, idPrefix, name, value, isInvalid, radioGroupHtmlAttributes);
    }

    public IHtmlContent CheckboxGroupControlFor<TProperty>(
        Expression<Func<TModel, IEnumerable<TProperty>>> expression, string[]? selectedItems)
        where TProperty : struct, Enum
    {
        var cssClasses = new StringBuilder("form-check-input ");
        var isInvalid = IsInvalid(expression);
        if (isInvalid) cssClasses.Append(InvalidClass).Append(' ');
        var name = _html.NameFor(expression);

        var selectList = _html.GetEnumSelectList<TProperty>();
        HtmlContentBuilder builder = new HtmlContentBuilder();
        foreach (var item in selectList)
        {
            var input = new TagBuilder("input");
            input.AddCssClass(cssClasses.ToString());
            input.Attributes["type"] = "checkbox";
            input.Attributes["name"] = name;
            input.Attributes["id"] = $"{name}_{item.Value}";
            input.Attributes["value"] = item.Value;
            if (item.Disabled) input.Attributes["disabled"] = "disabled";
            if (selectedItems != null && selectedItems.Contains(item.Value)) input.Attributes["checked"] = "checked";


            var label = new TagBuilder("label");
            label.AddCssClass("form-check-label");
            label.Attributes["for"] = $"{name}_{item.Value}";
            label.InnerHtml.Append(item.Text);

            var checkboxDiv = new TagBuilder("div");
            checkboxDiv.AddCssClass("form-check");
            if (isInvalid) checkboxDiv.AddCssClass(InvalidClass);
            checkboxDiv.InnerHtml.AppendHtml(input);
            checkboxDiv.InnerHtml.AppendHtml(label);
            builder.AppendHtml(checkboxDiv);
        }

        return builder;
    }

    public IHtmlContent DropDownListControlFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        IEnumerable<SelectListItem> items,
        bool displayEmptyFirstValue = true,
        string? emptyFirstValueText = null,
        bool emptyFirstValueDisabled = true,
        object? selectHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(selectHtmlAttributes);
        AddFormControlCssClassesFor(expression, attrsDict, "form-select");
        var selectItems = items.ToList();

        if (displayEmptyFirstValue)
        {
            selectItems.Insert(0, new SelectListItem(
                text: emptyFirstValueText ?? "",
                value: string.Empty,
                selected: !selectItems.Any(i => i.Selected),
                disabled: emptyFirstValueDisabled));
        }

        return _html.DropDownListFor(expression, selectItems, attrsDict);
    }

    public IHtmlContent EnumDropDownListControlFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        object? selectHtmlAttributes = null) where TProperty : struct, Enum
    {
        var selectList =
            _html.GetEnumSelectList<TProperty>(); // great built in funtion... it knows which enum is already selected!
        return DropDownListControlFor(expression, selectList, displayEmptyFirstValue: false,
            selectHtmlAttributes: selectHtmlAttributes);
    }

    public IHtmlContent NullableEnumDropDownListControlFor<TProperty>(
        Expression<Func<TModel, TProperty?>> expression,
        object? selectHtmlAttributes = null,
        string? emptyFirstValueText = null,
        bool emptyFirstValueDisabled = true) where TProperty : struct, Enum
    {
        var selectList = _html.GetEnumSelectList<TProperty>();
        return DropDownListControlFor(expression, selectList,
            emptyFirstValueText: emptyFirstValueText,
            emptyFirstValueDisabled: emptyFirstValueDisabled,
            selectHtmlAttributes: selectHtmlAttributes);
    }

    public IHtmlContent EnumRadioGroupControlFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Vertical,
        object? radioGroupHtmlAttributes = null) where TProperty : struct, Enum
    {
        var selectList = _html.GetEnumSelectList<TProperty>().ToList();
        var idPrefix = _html.IdFor(expression);
        var name = _html.NameFor(expression);
        var modelValue = _html.ValueFor(expression);
        var selectedItem = selectList.FirstOrDefault(i =>
        {
            if (!Enum.TryParse(i.Value, out TProperty result)) return false;
            return modelValue == result.ToString();
        });
        if (selectedItem != null) selectedItem.Selected = true;
        var isInvalid = IsInvalid(expression);
        return RenderRadioControl(layout, selectList, idPrefix, name, isInvalid, radioGroupHtmlAttributes);
    }

    public IHtmlContent NullableEnumRadioGroupControlFor<TProperty>(
        Expression<Func<TModel, TProperty?>> expression,
        RadioButtonLayout layout = RadioButtonLayout.Vertical,
        object? radioGroupHtmlAttributes = null) where TProperty : struct, Enum
    {
        var selectList = _html.GetEnumSelectList<TProperty>().ToList();
        var idPrefix = _html.IdFor(expression);
        var name = _html.NameFor(expression);
        var modelValue = _html.ValueFor(expression);
        var selectedItem = selectList.FirstOrDefault(i =>
        {
            if (!Enum.TryParse(i.Value, out TProperty result)) return false;
            return modelValue == result.ToString();
        });
        if (selectedItem != null) selectedItem.Selected = true;
        var isInvalid = IsInvalid(expression);
        return RenderRadioControl(layout, selectList, idPrefix, name, isInvalid, radioGroupHtmlAttributes);
    }

    #endregion

    #region Utility Methods

    public IHtmlContent FormGroupFor<TProperty>(
        Expression<Func<TModel, TProperty>> expression,
        IHtmlContent control,
        object? labelHtmlAttributes = null,
        object? containerHtmlAttributes = null,
        object? validationHtmlAttributes = null)
    {
        var tagBuilder = new TagBuilder("div");
        tagBuilder.AddAttributes(ConvertAnonymousObjectIfNeeded(containerHtmlAttributes, $"mb-3"));

        var validationAttrs = ConvertAnonymousObjectIfNeeded(validationHtmlAttributes, "invalid-feedback");
        var errorMessage = _html.ValidationMessageFor(expression, null, validationAttrs, "div");

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

    public TagBuilder RenderRadioControl(RadioButtonLayout layout, IEnumerable<SelectListItem> items, string idPrefix,
        string name, bool isInvalid, object? radioGroupHtmlAttributes = null)
    {
        var attrsDict = ConvertAnonymousObjectIfNeeded(radioGroupHtmlAttributes);
        var radioDiv = new TagBuilder("div");
        var cssClass = new StringBuilder("d-flex ");

        if (isInvalid) cssClass.Append(InvalidClass);

        cssClass.Append(layout == RadioButtonLayout.Vertical ? " flex-column" : " gap-3");

        if (!attrsDict.TryAdd("class", cssClass.ToString()))
        {
            attrsDict["class"] = $"{attrsDict["class"]} {cssClass}";
        }

        radioDiv.AddAttributes(attrsDict);

        foreach (var item in items)
        {
            var radioBtn = RadioControlItem(
                id: $"{idPrefix}-{item.Value}",
                name: name,
                value: item.Value,
                text: item.Text,
                isChecked: item.Selected,
                isInvalid: isInvalid);
            radioDiv.InnerHtml.AppendHtml(radioBtn);
        }

        return radioDiv;
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

    private TagBuilder RenderYesNoControl(RadioButtonLayout layout, string idPrefix, string name, string value,
        bool isInvalid, object? radioGroupHtmlAttributes)
    {
        var yesNoItems = new SelectListItem[]
        {
            new("Yes", "True", value == "True"),
            new("No", "False", value == "False")
        };

        return RenderRadioControl(layout, yesNoItems, idPrefix, name, isInvalid, radioGroupHtmlAttributes);
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
        IDictionary<string, object> dict;
        if (obj is IDictionary<string, object> originalDict)
        {
            dict = new Dictionary<string, object>(originalDict);
        }
        else
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