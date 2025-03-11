using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BootstrapHtmlHelpers;

public static class ModelStateDictionaryExtensions
{
    /// <summary>
    /// Returns a list of validation messages for a given ModelStateDictionary entry. Returns an empty string array if
    /// there are no validation errors for the state.
    /// </summary>
    /// <param name="modelState">The model state dictionary</param>
    /// <param name="key">Key of the entry for the validation messages</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Thrown if <paramref name="key"/> does not exist in the ModelStateDictionary</exception>
    public static string[] GetErrorMessages(this ModelStateDictionary modelState, string key)
    {
        var modelStateItem =  modelState[key];
        if (modelStateItem == null) throw new KeyNotFoundException($"No model state entry found for key {key}");
        return modelStateItem.Errors.Select(e => e.ErrorMessage).ToArray();
    }
}