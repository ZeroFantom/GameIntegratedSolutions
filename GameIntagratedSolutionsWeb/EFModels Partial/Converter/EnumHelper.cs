using Microsoft.AspNetCore.Mvc.Rendering;

namespace IntelliTrackSolutionsWeb.EFModels_Partial.Converter;

public static class EnumHelper
{
    public static IEnumerable<SelectListItem> GetSelectList<TEnum>()
        where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text = e.ToString()
            });
    }
}