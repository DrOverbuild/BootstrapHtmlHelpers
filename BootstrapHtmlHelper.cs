using BootstrapExtensions.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;

namespace BootstrapExtensions.HtmlHelpers;

public enum FormControlSize
{
    Small,
    Default,
    Large
}

public enum Direction
{
    Horizontal,
    Vertical
}

public class BootstrapHtmlHelper<TModel>
{
    private IHtmlHelper<TModel> html;
    private FormControlSize FormControlSize;

    public BootstrapHtmlHelper(IHtmlHelper<TModel> _html, FormControlSize formControlSize = FormControlSize.Default)
    {
        FormControlSize = formControlSize;
        html = _html;
    }
    public string InputSize
    {
        get
        {
            return this.FormControlSize switch
            {
                FormControlSize.Small => "form-control-sm ",
                FormControlSize.Large => "form-control-lg ",
                _ => "",
            };
        }
    }

    public IHtmlContent TextBoxFor<TProperty>(Expression<Func<TModel, TProperty>> expression, string cssClass = "",
        string? ariaLabel = null, string? stringFormat = null, int? maxLength = null, object? htmlAttributes = null)
    {
        string? type = GetHtml5InputType<TProperty>();

        var customAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        customAttributes.Add("class", cssClass + " " + InputSize + "form-control " + html.ErrorClassFor(expression));

        if (type != null) customAttributes.Add("type", type);

        if (type == "number") customAttributes.Add("step", "any");

        maxLength ??= html.MaxLengthFor(expression);

        if (maxLength.HasValue && maxLength.Value > 0) customAttributes.Add("maxLength", maxLength.Value);

        if (!string.IsNullOrEmpty(ariaLabel)) customAttributes.Add("aria-label", ariaLabel);

        if (!string.IsNullOrWhiteSpace(cssClass) && (cssClass.Contains("datepicker") || cssClass.Contains("datetimepicker")))
        {
            customAttributes.Add("autocomplete", "off");
        }

        if (type == "number" && maxLength != null)
        {
            customAttributes.Add("oninput", "javascript: if (this.value.length > this.maxLength) this.value = this.value.slice(0, this.maxLength);");
        }

        return html.TextBoxFor(expression, stringFormat, customAttributes);
    }

    private static string? GetHtml5InputType<TProperty>()
    {
        var mainType = typeof(TProperty);
        var nullableType = Nullable.GetUnderlyingType(mainType);
        TypeCode typeCode;
        if (nullableType != null)
        {
            typeCode = Type.GetTypeCode(nullableType);
        }
        else
        {
            typeCode = Type.GetTypeCode(mainType);
        }

        return typeCode switch
        {
            TypeCode.Byte or
            TypeCode.SByte or
            TypeCode.UInt16 or
            TypeCode.UInt32 or
            TypeCode.UInt64 or
            TypeCode.Int16 or
            TypeCode.Int32 or
            TypeCode.Int64 or
            TypeCode.Decimal or
            TypeCode.Double or
            TypeCode.Single => "number",
            _ => null,
        };
    }

    public IHtmlContent TextAreaFor<TProperty>(Expression<Func<TModel, TProperty>> expression, int rows, int cols,
        string? ariaLabel = null)
    {
        return html.TextAreaFor(expression, rows, cols, new
        {
            @class = InputSize + "form-control " + html.ErrorClassFor(expression),
            maxLength = html.MaxLengthFor(expression),
            aria_label = ariaLabel
        });
    }

    public IHtmlContent PasswordFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
    {
        return html.PasswordFor(expression, new
        {
            @class = InputSize + "form-control " + html.ErrorClassFor(expression),
            maxLength = html.MaxLengthFor(expression)
        });
    }

    public IHtmlContent DropDownListFor<TProperty>(Expression<Func<TModel, TProperty>> expression,
        IEnumerable<SelectListItem> items, bool displayEmptyFirstValue = true, string? ariaLabel = null,
        string? dataToggleElementId = null, object? dataDisplayValue = null, string? emptyFirstValueText = "",
        object? htmlAttributes = null)
    {
        var customAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        customAttributes.Add("class", InputSize + "form-control " + html.ErrorClassFor(expression));
        customAttributes.Add("aria-label", ariaLabel);
        customAttributes.Add("data-display-value", dataDisplayValue);
        customAttributes.Add("data-toggle-element-id", dataToggleElementId);

        var innerItems = items.ToList();
        if (displayEmptyFirstValue)
            innerItems.Insert(0, new SelectListItem() { Text = emptyFirstValueText, Value = "" });
        return html.DropDownListFor(expression, innerItems, customAttributes);
    }

    public IHtmlContent EnumDropDownListFor<TEnum, TProperty>(Expression<Func<TModel, TProperty>> expression,
        bool displayEmptyFirstValue = true, string? ariaLabel = null, string? dataToggleElementId = null,
        object? dataDisplayValue = null, string? emptyFirstValueText = "", object? htmlAttributes = null)
        where TEnum : struct, Enum
    {
        return DropDownListFor(expression, html.GetEnumSelectList<TEnum>(), displayEmptyFirstValue, ariaLabel,
            dataToggleElementId, dataDisplayValue, emptyFirstValueText, htmlAttributes);
    }

    public IHtmlContent EnumDropDownList<TEnum>(string name, string value, bool displayEmptyFirstValue = true, 
        string? emptyFirstValueText = "", object? htmlAttributes = null)
        where TEnum : struct, Enum
    {
        var selectItems = html.GetEnumSelectList<TEnum>().ToList();
        if (displayEmptyFirstValue)
        {
            selectItems.Insert(0, new SelectListItem { Text = emptyFirstValueText, Value = "" });
        }

        var selectedItem = selectItems.FirstOrDefault(item => item.Value == value);

        if (selectedItem != null) selectedItem.Selected = true;

        return html.DropDownList(name, selectItems, htmlAttributes);
    }

    private IHtmlContent _controlWithFormGroup<TProperty>(Expression<Func<TModel, TProperty>> expression)
    {
        var tagBuilder = new TagBuilder("div");
        var label = html.LabelFor(expression);
        var input = html.TextBoxFor(expression);
        tagBuilder.InnerHtml.AppendHtml(label);
        tagBuilder.InnerHtml.AppendHtml(input);
        return tagBuilder;
    }

    public IHtmlContent CheckBoxFor(Expression<Func<TModel, bool>> expression, string? labelText = null,
        string? dataToggleElementId = null, object? dataDisplayValue = null)
    {
        var checkbox = html.CheckBoxFor(expression,
            new
            {
                @class = "form-check-input " + html.ErrorClassFor(expression),
                data_display_value = dataDisplayValue,
                data_toggle_element_id = dataToggleElementId
            });
        var label = html.LabelFor(expression, labelText, new { @class = "form-check-label" });

        var tagBuilder = new TagBuilder("div");
        tagBuilder.AddCssClass("mb-3 form-check");
        tagBuilder.InnerHtml.AppendHtml(checkbox);
        tagBuilder.InnerHtml.AppendHtml(label);
        return tagBuilder;
    }

    private IHtmlContent _formGroupFor<TProperty>(Expression<Func<TModel, TProperty>> expression, IHtmlContent control,
        string? labelText = null, string? helpText = null, string cssClass = "", bool showLabel = true)
    {
        var tagBuilder = new TagBuilder("div");
        tagBuilder.AddCssClass("form-group mb-3"); // Bootstrap 5 has deprecated class 'form-group' in favor of 'mb-3'
        tagBuilder.AddCssClass(cssClass);
        if (showLabel)
        {
            var label = html.LabelFor(expression, labelText, new { @class = "form-label" });
            tagBuilder.InnerHtml.AppendHtml(label);
        }

        tagBuilder.InnerHtml.AppendHtml(control);

        var errorMessage = html.ValidationMessageFor(expression, null, new { @class = "invalid-feedback" }, "div");

        if (errorMessage != null) tagBuilder.InnerHtml.AppendHtml(errorMessage);

        if (!string.IsNullOrEmpty(helpText))
        {
            var helpTextDiv = new TagBuilder("div");
            helpTextDiv.AddCssClass("form-text");
            helpTextDiv.InnerHtml.SetContent(helpText);
            tagBuilder.InnerHtml.AppendHtml(helpTextDiv);
        }

        return tagBuilder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="expression"></param>
    /// <param name="labelText"></param>
    /// <param name="helpText"></param>
    /// <param name="cssClass"></param>
    /// <param name="inputCssClass"></param>
    /// <param name="ariaLabel"></param>
    /// <param name="stringFormat">ex. {0:MM/dd/yyyy}</param>
    /// <param name="maxLength">Override StringLengthAttribute</param>
    /// <returns></returns>
    public IHtmlContent TextBoxFormGroupFor<TProperty>(Expression<Func<TModel, TProperty>> expression,
        string? labelText = null, string? helpText = null, string cssClass = "", string inputCssClass = "",
        string? ariaLabel = null, string? stringFormat = null, int? maxLength = null, object? htmlAttributes = null)
    {
        var textBox = TextBoxFor(expression, inputCssClass, ariaLabel, stringFormat, maxLength, htmlAttributes);
        return _formGroupFor(expression, textBox, labelText, helpText, cssClass);
    }

    public IHtmlContent TextAreaFormGroupFor<TProperty>(Expression<Func<TModel, TProperty>> expression, int rows,
        int cols, string? labelText = null, string? helpText = null, string cssClass = "", string? ariaLabel = null)
    {
        return _formGroupFor(expression, TextAreaFor(expression, rows, cols, ariaLabel), labelText, helpText, cssClass);
    }

    public IHtmlContent DropDownListWithFormGroupFor<TProperty>(Expression<Func<TModel, TProperty>> expression,
        IEnumerable<SelectListItem> items, string? labelText = null, string? helpText = null, string cssClass = "",
        bool displayEmptyFirstValue = true, string? ariaLabel = null, string? dataToggleElementId = null,
        object? dataDisplayValue = null, string emptyFirstValueText = "", bool showLabel = true,
        object? htmlAttributes = null)
    {
        var dropdown = DropDownListFor(expression, items, displayEmptyFirstValue, ariaLabel, dataToggleElementId, dataDisplayValue, emptyFirstValueText, htmlAttributes);
        return _formGroupFor(expression, dropdown, labelText, helpText, cssClass, showLabel);
    }

    public IHtmlContent EnumDropDownListFormGroupFor<TEnum>(Expression<Func<TModel, TEnum>> expression,
        string? labelText = null, string? helpText = null, string cssClass = "", bool displayEmptyFirstValue = true,
        string? ariaLabel = null, string? dataToggleElementId = null, object? dataDisplayValue = null,
        string? emptyFirstValueText = "", bool showLabel = true, object? htmlAttributes = null)
        where TEnum : struct, Enum
    {
        return _enumDropDownListFormGroupFor<TEnum, TEnum>(expression, labelText, helpText, cssClass,
            displayEmptyFirstValue, ariaLabel, dataToggleElementId, dataDisplayValue, emptyFirstValueText, showLabel,
            htmlAttributes);
    }

    public IHtmlContent EnumDropDownListFormGroupFor<TEnum>(Expression<Func<TModel, TEnum?>> expression,
        string? labelText = null, string? helpText = null, string cssClass = "", bool displayEmptyFirstValue = true,
        string? ariaLabel = null, string? dataToggleElementId = null, object? dataDisplayValue = null,
        string? emptyFirstValueText = "", bool showLabel = true, object? htmlAttributes = null)
        where TEnum : struct, Enum
    {
        return _enumDropDownListFormGroupFor<TEnum, TEnum?>(expression, labelText, helpText, cssClass,
            displayEmptyFirstValue, ariaLabel, dataToggleElementId, dataDisplayValue, emptyFirstValueText, showLabel,
            htmlAttributes);
    }

    private IHtmlContent _enumDropDownListFormGroupFor<TEnum, TProperty>(Expression<Func<TModel, TProperty>> expression,
        string? labelText, string? helpText, string cssClass, bool displayEmptyFirstValue,
        string? ariaLabel, string? dataToggleElementId, object? dataDisplayValue,
        string? emptyFirstValueText, bool showLabel, object? htmlAttributes)
        where TEnum : struct, Enum
    {
        var dropdown = EnumDropDownListFor<TEnum, TProperty>(expression, displayEmptyFirstValue, ariaLabel,
            dataToggleElementId, dataDisplayValue, emptyFirstValueText, htmlAttributes);
        return _formGroupFor(expression, dropdown, labelText, helpText, cssClass, showLabel);
    }

    /// <summary>
    /// Builds a horizontal set of radio buttons for an expression that returns an enumeration type. Each enum value is
    /// an option in the radio button set.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="expression"></param>
    /// <param name="labelText"></param>
    /// <param name="cssClass"></param>
    /// <param name="helpText"></param>
    /// <returns></returns>
    public IHtmlContent CustomRadioListFormGroupFor<TEnum>(Expression<Func<TModel, TEnum>> expression,
        Direction repeatDirection = Direction.Horizontal, string? labelText = null, string cssClass = "",
        string? helpText = null)
        where TEnum : struct, Enum
    {
        return _customRadioListFormGroupFor<TEnum, TEnum>(expression, repeatDirection, labelText, cssClass, helpText);
    }

    /// <summary>
    /// Same as CustomRadioListFormGroupFor but used when the expression returns a nullable enumeration type. Builds a 
    /// horizontal set of radio buttons for an expression that returns an enumeration type. Each enum value is
    /// an option in the radio button set.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="expression"></param>
    /// <param name="labelText"></param>
    /// <param name="cssClass"></param>
    /// <param name="helpText"></param>
    /// <returns></returns>
    public IHtmlContent CustomRadioListFormGroupFor<TEnum>(Expression<Func<TModel, TEnum?>> expression,
        Direction repeatDirection = Direction.Horizontal, string? labelText = null, string cssClass = "",
        string? helpText = null)
        where TEnum : struct, Enum
    {
        return _customRadioListFormGroupFor<TEnum, TEnum?>(expression, repeatDirection, labelText, cssClass, helpText);
    }

    private IHtmlContent _customRadioListFormGroupFor<TEnum, TProperty>(
        Expression<Func<TModel, TProperty>> expression, Direction repeatDirection, string? labelText, string cssClass, string? helpText)
        where TEnum : struct, Enum
    {
        var control = new TagBuilder("div");

        var items = html.GetEnumSelectList<TEnum>();
        var idPrefix = html.IdFor(expression);
        var name = html.NameFor(expression);

        var value = html.ValueFor(expression);
        var parseSucceeded = Enum.TryParse(value, out TEnum valueEnum);

        var counter = 0;

        foreach (var item in items)
        {
            counter++;
            var id = $"{idPrefix}-{counter}";

            // sometimes the enum name is used as the value in the model state, but the GetEnumSelectList() method sets
            // the value of each item to be the int value of the enum in a string, so we need to check for both
            var isChecked = value == item.Value || (parseSucceeded && ((int)(object)valueEnum).ToString() == item.Value);
            var radioBtn = _radioControlItem(id, name, item.Value, item.Text, item.Disabled, isChecked, repeatDirection);
            control.InnerHtml.AppendHtml(radioBtn);
        }

        return _formGroupFor(expression, control, labelText, helpText, cssClass);
    }

    private IHtmlContent _radioControlItem(string id, string name, object value, string text, bool disabled = false,
        bool isChecked = false, Direction repeatDirection = Direction.Horizontal, string cssClass = "",
        object? htmlAttrs = null)
    {
        var controlItem = new TagBuilder("div");
        controlItem.AddCssClass(repeatDirection == Direction.Horizontal ? "custom-control custom-radio" : "form-check");

        var inputAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttrs);
        inputAttrs.Add("class", $"custom-control-input {cssClass}");
        inputAttrs.Add("id", id);

        if (disabled) inputAttrs.Add("disabled", "");

        // we cannot use RadioButtonFor because it overwrites our checked attribute if we added it.
        var input = html.RadioButton(name, value, isChecked, inputAttrs);
        var label = html.Label(name, text, new
        {
            @for = id,
            @class = repeatDirection == Direction.Horizontal ? "custom-control-label" : "custom-control-description"
        });

        controlItem.InnerHtml.AppendHtml(input);
        controlItem.InnerHtml.AppendHtml(label);
        return controlItem;
    }

    public IHtmlContent YesNoFor<TProperty>(Expression<Func<TModel, TProperty>> expression, string? labelText = null,
        string cssClass = "", Direction repeatDirection = Direction.Horizontal, string? dataToggleElementId = null,
        object? dataDisplayValue = null, string? helpText = null)
    {
        var errorClass = html.ErrorClassFor(expression);
        var idForPrefix = html.IdFor(expression);
        var nameFor = html.NameFor(expression);
        var htmlAttrs = new
        {
            data_display_value = dataDisplayValue,
            data_toggle_element_id = dataToggleElementId,
        };

        var yesRadioBtn = _radioControlItem(id: $"{idForPrefix}-Yes", name: nameFor, value: true, text: "Yes",
            cssClass: errorClass, repeatDirection: repeatDirection, htmlAttrs: htmlAttrs);
        var noRadioBtn = _radioControlItem(id: $"{idForPrefix}-No", name: nameFor, value: false, text: "No",
            cssClass: errorClass, repeatDirection: repeatDirection, htmlAttrs: htmlAttrs);

        var checkBoxList = new TagBuilder("div");
        checkBoxList.AddCssClass(errorClass);
        checkBoxList.InnerHtml.AppendHtml(yesRadioBtn);
        checkBoxList.InnerHtml.AppendHtml(noRadioBtn);

        return _formGroupFor(expression, checkBoxList, labelText, cssClass);
    }
}
