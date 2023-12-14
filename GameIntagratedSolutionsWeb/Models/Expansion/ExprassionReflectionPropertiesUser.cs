namespace IntelliTrackSolutionsWeb.Models.Expansion;

public static class ExpansionReflectionPropertiesSearch
{
    public static bool ReflectionPropertiesSearch<TModel>(this TModel model, string search)
    {
        return typeof(TModel).GetProperties().ToList()
            .Any(property => property.GetValue(model)?.ToString()?.Contains(search) ?? false);
    }
}