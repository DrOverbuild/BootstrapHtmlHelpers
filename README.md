Class retrieved from an extension of IHtmlHelper<TModel> that makes building Bootstrap forms mapped to MVC models super 
easy. Features:

* Each component is placed in a form group Div with Bootstrap class `mb-3`
* Support for validation and error messages
* Support for `DisplayAttribute`, including `Name`, `Prompt`, and `Description` (which is rendered below in a div element with the CSS class `form-text`).
* Adds the CSS class `is-required` to `form-label` if the property for the expression is required.
* Extensions for `Controller` and `ViewDataDictionary` to add Bootstrap alerts, and a `RenderAlerts` extension for IHtmlHelper to render alerts in the views.
* Pagination links

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
@{
    var bootstrap = Html.Bootstrap();
}
@bootstrap.TextBoxFor(m => m.ModelProperty)
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
@bootstrap.TextBoxFor(m => m.ModelProperty)
```

Textarea: 

```cshtml
@bootstrap.TextAreaFor(m => m.ModelProperty, 5, 10)
```

Textbox for passwords:

```cshtml
@bootstrap.PasswordFor(m => m.ModelPropety)
```

Datepicker:

```cshtml
@bootstrap.DatePickerFor(m => m.ModelProperty)
```

Yes/No Radio button group. This renders a group of radio buttons for boolean values. By default
the layout is horizontal, but if you'd rather have a vertical layout for this control, add
`RadioButtonLayout.Vertical`. This method supports nullable booleans too. If the value is null,
nothing will be selected by default.

```cshtml
@bootstrap.YesNoFor(m => m.BooleanModelProperty)
@bootstrap.YesNoFor(m => m.BooleanModelProperty, RadioButtonLayout.Vertical) @* vertical layout *@
```

Checkbox for boolean values:

```cshtml
@bootstrap.CheckboxFor(m => m.BooleanModelProperty)
```

`<select>` dropdown for any list of strings:

```cshtml
@bootstrap.DropDownListFor(m => m.Name, new List<SelectListItem> {
    new SelectListItem { Value = "value1", Text = "Display Value 1" },
    new SelectListItem { Value = "value2", Text = "Display Value 2" }, 
    new SelectListItem { Value = "value3", Text = "Display Value 3" } 
})
```

`<select>` dropdown for enums. This uses the built in `IHtmlHelper<TModel>.GetEnumSelectList<TEnum>()` 
method so this supports `DisplayAttribute.Name` right out of the box.

```cshtml
@bootstrap.EnumDropDownListFor(m => m.EnumModelProperty)
```

If the enum property is nullable and a blank option is required, use `NullabelEnumDropDownListFor`. This method supports
adding a custom text value for the empty first option, which is disabled by default, but can be endabled with the 
`emptyFirstOptionDisabled: false` parameter.

```cshtml
@bootstrap.NullableEnumDropDownListFor(m => m.NullableEnumModelProperty, emptyFirstValueText: "Choose...")

// optionally leave the empty first option enabled
@bootstrap.NullableEnumDropDownListFor(m => m.NullableEnumModelProperty, emptyFirstValueText: "Choose...", emptyFirstOptionDisabled: false)
```

Checkbox group for enums. As with `EnumDropDownListFor`, this uses the `IHtmlHelper<TModel>.GetEnumSelectList<TEnum>()`
for proper model metadata. This will render a group of checkboxes, and when the form is submitted, the model's property 
is set to the list of checked enum values. There is a limitation to reading the value of the model when rending a page.
Because of that limitation, it is best to pass the current value of the model property as a string array, shown below.

```cshtml
@bootstrap.CheckboxGroupFor(m => m.ListOfEnum, selectedItems: Model.ListOfEnum?.Select(o => ((int)o).ToString()).ToArray())
```

## Individual Components

All of the examples above will render what are known as "form groups," or form elements that are accompanied by a label,
description (form text), validation, and a bottom margin. If you are building a specialty form element and want the 
basic control without labels, form text, validation, or margin, use the control methods for each component listed above:

- `TextBoxControlFor(expression, htmlAttributes, format)`
- `TextAreaControlFor(expression, rows, columns, inputAttributes)`
- `PasswordControlFor(expression, inputAttributes)`
- `YesNoControlFor(expression, layout)`
- `CheckboxGroupControlFor(expression, selectedItems)`
- `DropDownListControlFor(expression, items, displayEmptyFirstValue, emptyFirstValueText, selectHtmlAttributes)`
- `EnumDropDownListControlFor(expression, selectHtmlAttributes)`
- `NullableEnumDropDownListControlFor(expression, selectHtmlAttributes)`

Use `LabelFor(expression, htmlAttributes)` to take advantage of Bootstrap labels in a custom form group.


## HTML Attributes
All extensions that support the anonymous object for adding additional HTML attributes also 
support passing HTML attributes of type `IDictionary<string,object>`. Note that passing 
`IDictionary<string,string>` will exhibit unintended behavior.

```cshtml
@bootstrap.TextBoxFor(m => m.Property, labelHtmlAttributes: new { id = "id" }) @* Adds HTML Attributes to the Label *@
@bootstrap.TextBoxFor(m => m.Property, containerHtmlAttributes: new { id = "id" }) @* Adds HTML Attributes to the containing <div> *@
@bootstrap.TextBoxFor(m => m.Property, htmlAttributes: new { id = "id" }) @* Adds HTML Attributes to the <input> element *@
@bootstrap.YesNoFor(m => m.BooleanModelProperty, new Dictionary<string, object> {
    { "data-attribute", "attribute value" }
}) @* Also adds HTML Attributes to the input *@
```

Many of the rendered HTML elements have default class names. When additional classes are provided through the given
HTML attributes object, the classes are appended to the default class list.

```cshtml
@bootstrap.TextBoxFor(m => m.Property, inputAttributes: new { @class = "custom-input" })
```

The resulting class list will be `form-control custom-input`.

There are three elements where elements can usually be applied:

- `inputAttributes` — The main input element. By default this input has the `form-control` class as well as whatever 
  attributes are applied via `IHtmlHelper<T>.TextBoxFor()`.
- `labelHtmlAttributes` — The label for the control. Usually this has the `form-label` CSS class as well as 
  `is-required` if the model metadata marks it as such. Other attributes are applied via `IHtmlHelper<T>.LabelFor()`.
- `containerHtmlAttributes` — The containing `<div>` element. This is usually a container with just a `mb-3` CSS class.

The following table lists out which components and methods support each attribute:

| Method                                | Input Attributes           | Label Attributes | Container Attributes |
|---------------------------------------|----------------------------|------------------|----------------------|
| `TextBoxFor`                          | ✅ (`htmlAttributes`)       | ✅                | ✅                    |  
| `TextAreaFor`                         | ✅                          | ✅                | ✅                    |
| `PasswordFor`                         | ✅                          | ✅                | ✅                    |
| `DatePickerFor`                       | ✅                          | ✅                | ✅                    |
| `YesNoFor`                            | ❌                          | ✅                | ✅                    |
| `CheckboxFor`                         | ❌                          | ✅                | ✅                    |
| `CheckboxGroupFor`                    | ❌                          | ✅                | ✅                    |
| `DropDownListFor`                     | ✅ (`selectHtmlAttributes`) | ✅                | ✅                    |
| `EnumDropDownListFor`                 | ✅ (`selectHtmlAttributes`) | ✅                | ✅                    |
| `NullableEnumDropDownListFor`         | ✅ (`selectHtmlAttributes`) | ✅                | ✅                    |
| `TextBoxControlFor`                   | ✅ (`htmlAttributes`)       | ❌                | ❌                    |
| `TextAreaControlFor`                  | ✅                          | ❌                | ❌                    |
| `PasswordControlFor`                  | ✅                          | ❌                | ❌                    |
| `YesNoControlFor`                     | ❌                          | ❌                | ❌                    |
| `CheckboxGroupControlFor`             | ❌                          | ❌                | ❌                    |
| `DropDownListControlFor`              | ✅ (`selectHtmlAttributes`) | ❌                | ❌                    |
| `EnumDropDownListControlFor`          | ✅ (`selectHtmlAttributes`) | ❌                | ❌                    |
| `NullableEnumDropDownListControlFor`  | ✅ (`selectHtmlAttributes`) | ❌                | ❌                    |
| `FormGroupFor`                        | ❌                          | ✅                | ✅                    |

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

## Alerts

Alerts can be added to views with the `AddAlert` extension. This extension is available on the Controller and ViewDataDictionary classes:

```csharp
# Within MVC Controller
AddAlert(BootstrapColor.Success, "Alert message");

# With view data:
ViewData.AddAlert(BootstrapColor.Success, "Alert message");
```

Alerts can be added to TempData to save the alert for the next request. Useful when showing an alert after redirecting 
to another action:

```charp
TempData.AddAlert(BootstrapColor.Success, "Alert Message");
```

Alert types are available for each Bootstrap color classes:

```csharp
public enum BootstrapColor
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Light,
    Dark
}
```

In your views, render your alerts as follows: 

```cshtml
@Html.RenderAlerts()
```

To prevent rendering views from TempData, pass `false`:

```cshtml
@Html.RenderAlerts(false)
```

Note: The content of the alerts are not HTML encoded. RAW HTML will be rendered. Encode any user-generated strings you 
pass to `AddAlert`.

## Pagination
This package features the `BsPagination` class with methods for rendering Bootstrap pagination links.

```cshtml
@BsPagination.Paginate(5, 10, page => Url.Action("Index", "Home", new { page })!)
```

The above will render the following:

```html
<nav aria-label="Page navigation">
    <ul class="pagination justify-content-center ">
        <li class="page-item "><a class="page-link" href="/?page=4">Previous</a></li>
        <li class="page-item "><a class="page-link" href="/?page=1">1</a></li>
        <li class="page-item"><span class="page-text">…</span></li>
        <li class="page-item "><a class="page-link" href="/?page=4">4</a></li>
        <li class="page-item active"><a class="page-link" href="/?page=5">5</a></li>
        <li class="page-item "><a class="page-link" href="/?page=6">6</a></li>
        <li class="page-item"><span class="page-text">…</span></li>
        <li class="page-item "><a class="page-link" href="/?page=10">10</a></li>
        <li class="page-item "><a class="page-link" href="/?page=6">Next</a></li>
    </ul>
</nav>
```

`Paginate` takes the following parameters:

```cs
int currentPage, int totalPages, Func<int, string> buildHref, string ulClasses = "", string nonLinkClass = "page-text"
```
| Parameter Name | Description                                                                                                                                                               |
|----------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `currentPage`  | The current page. This page will be indicated with the `active` CSS class.                                                                                                |
| `totalPages`   | Total pages available. If you have the count of total items, the total number of pages can be calculated with `BsPagination.TotalPages(totalItems, itemsPerPage)`         |
| `buildHref`    | A lambda function that takes an integer as a parameter and returns a string. This method is called for each page item. Use this to build the href URL for each page.      |
| `ulClasses`    | CSS classes for the `<ul>` element that is rendered. The `<ul>` element has `pagination justify-content-center`. The value of this parameter is added to this class list. |
| `nonLinkClass` | The class for the `<span>` element that is rendered for non-link page items. By default the class is `page-text`.                                                         |

### Non-link Page Items
This pagination method renders a page item with an ellipsis character and no links. This indicates separation between 
the first page and the previous page as well as separation between the next page and the last page. However, Bootstrap
does not have good styling for this type of element, so we recommend adding this to your CSS:

```css
.page-text {
    position: relative;
    display: block;
    padding: var(--bs-pagination-padding-y) var(--bs-pagination-padding-x);
    font-size: var(--bs-pagination-font-size);
    background-color: var(--bs-pagination-bg);
    border: var(--bs-pagination-border-width) solid var(--bs-pagination-border-color);
    color: var(--bs-secondary);
}

.page-item:not(:first-child) .page-text {
    margin-left: calc(var(--bs-border-width) * -1);
}
```

### Calculation Utilities
`BsPagination` provides three utility methods for making calculations related to pagination:

| Method                                          | Description                                                                                                                                                     |
|-------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `TotalPages(int totalItems, int itemsPerPage)`  | Calculate the number of pages based on the total number of items.                                                                                               |
| `MinMax(int page, int totalPages)`              | Returns `page` unless it is below zero or above the value of `totalPages`, in which case it will return those values.                                           |
| `Offset(int page, int perPage, int totalPages)` | Returns the SQL OFFSET value (or LINQ `Skip()` value) based on the number of items per page. `MinMax` is called in this method, so validation is taken care of. |