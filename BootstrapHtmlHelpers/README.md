Extensions for IHtmlHelper<TModel> that makes building Bootstrap forms mapped to MVC models super easy. Features:

* Each component is placed in a form group Div with Bootstrap class `mb-3`
* Support for validation and error messages
* Support for `DisplayAttribute`, including `Name`, `Prompt`, and `Description` (which is rendered below in a div element with the CSS class `form-text`).
* Adds the CSS class `is-required` to `form-label` if the property for the expression is required.

## Example

Given the following model:

```cs
public class ExampleViewModel
{
    [Display(Prompt = "Placeholder", Description = "Property description")]
    [Required(ErrorMessage = "Invalid message")]
    public string ModelProperty { get; set; }
}
```

To build a simple text box:

```cshtml
@Html.BsTextBoxFor(m => m.ModelProperty)
```

With the model state calculated to be invalid, the above renders out to:

```cshtml
<div class="mb-3">
    <label class="form-label is-required" for="Name">Name</label>
    <input class="form-control is-invalid " data-val="true" data-val-required="Invalid message" id="Name" name="Name" placeholder="Placeholder" type="text" value="">
    <div class="invalid-feedback field-validation-valid" data-valmsg-for="Name" data-valmsg-replace="true">
        Invalid messge
    </div>
    <div class="form-text">
        Property description
    </div>
</div>
```

## All components

Basic textbox:

```cshtml
@Html.BsTextBoxFor(m => m.ModelProperty)
```

Textbox for passwords:

```cshtml
@Html.BsPasswordFor(m => m.ModelPropety)
```

Datepicker:

```cshtml
@Html.BsDatePickerFor(m => m.ModelProperty)
```

Yes/No Radio button group. This renders a group of radio buttons for boolean values. By default
the layout is horizontal, but if you'd rather have a vertical layout for this control, add
`RadioButtonLayout.Vertical`. This method supports nullable booleans too. If the value is null,
nothing will be selected by default.

```cshtml
@Html.BsYesNoFor(m => m.BooleanModelProperty)
@Html.BsYesNoFor(m => m.BooleanModelProperty, RadioButtonLayout.Vertical) @* vertical layout *@
```

Checkbox for boolean values:

```cshtml
@Html.BsCheckboxFor(m => m.BooleanModelProperty)
```

`<select>` dropdown for any list of strings:

```cshtml
@Html.BsDropDownListFor(m => m.Name, new List<SelectListItem> {
    new SelectListItem { Value = "value1", Text = "Display Value 1" },
    new SelectListItem { Value = "value2", Text = "Display Value 2" }, 
    new SelectListItem { Value = "value3", Text = "Display Value 3" } 
})
```

`<select>` dropdown for enums. This uses the built in `IHtmlHelper<TModel>.GetEnumSelectList<TEnum>()` 
method so this supports `DisplayAttribute.Name` right out of the box.

```cshtml
@Html.BsEnumDropDownListFor(m => m.EnumModelProperty)
```

If the enum property is nullable and a blank option is required, use:

```cshtml
@Html.BsNullableEnumDropDownListFor(m => m.NullableEnumModelProperty)
```

## HTML Attributes
All extensions that support the anonymous object for adding additional HTML attributes also 
support passing HTML attributes of type `IDictionary<string,object>`. Note that passing 
`IDictionary<string,string>` will exhibit untintended behavior.

```cshtml
@Html.BsYesNoFor(m => m.BooleanModelProperty, new Dictionary<string, object> {
    { "data-attribute", "attribute value" }
})
```

## Required Indicator
Labels for the form groups are automatically given the CSS class `is-required` if the property
of the model is required. To add a required indicator, typically a red asterisk, add this CSS:

```css
.form-label.is-required::after {
  content: '*';
  color: var(--bs-danger);
  padding-left: 0.10rem;
}
```