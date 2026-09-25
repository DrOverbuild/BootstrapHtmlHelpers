namespace BootstrapHtmlHelpers;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class HtmlAttributeAttribute: Attribute
{
    public string Name { get; set; }
    public string Value { get; set; }
    
    public HtmlAttributeAttribute(string name, string value)
    {
        Name = name;
        Value = value;
    }
}