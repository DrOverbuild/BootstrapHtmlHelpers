using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BootstrapHtmlHelpers;

public static class ControllerExtensions
{
    public static void AddAlert(this Controller controller, BootstrapColor alertClass, string message)
    {
        controller.ViewData.AddAlert(alertClass, message);
    }

    public static void AddAlert(this ViewDataDictionary viewData, BootstrapColor alertClass, string message)
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


    public static void AddAlert(this ITempDataDictionary tempData, BootstrapColor alertClass, string message)
    {
        if (tempData["alerts"] is not List<string> alerts)
        {
            alerts = new List<string>();
            tempData["alerts"] = alerts;
        }

        alerts.Add($"<div class=\"alert alert-{alertClass.ToString().ToLowerInvariant()}\" role=\"alert\">{message}</div>"); 
    }
}

public struct Alert
{ 
    public BootstrapColor AlertClass { get; set; }
    public string Message { get; set; }
}

