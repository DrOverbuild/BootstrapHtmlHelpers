using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BootstrapHtmlHelpers;

public static class ControllerExtensions
{
    public static void AddAlert(this Controller controller, string alertClass, string message)
    {
        controller.ViewData.AddAlert(alertClass, message);
    }

    public static void AddAlert(this ViewDataDictionary viewData, string alertClass, string message)
    {
        if (viewData["alerts"] is not List<Alert> alerts)
        {
            alerts = new List<Alert>();
            viewData["alerts"] = alerts;
        }
        
        alerts.Add(new Alert
        {
            AlertClass = alertClass,
            Message = message
        });   
    }
}

public struct Alert
{
    public const string Primary = "primary";
    public const string Secondary = "secondary";
    public const string Success = "success";
    public const string Danger = "danger";
    public const string Warning = "warning";
    public const string Info = "info";
    public const string Light = "light";
    public const string Dark = "dark";
    
    public string AlertClass { get; set; }
    public string Message { get; set; }
}