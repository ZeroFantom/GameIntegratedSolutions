using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IntelliTrackSolutionsWeb.EFModels_Partial.Converter;

public enum TaskStatus
{
    Completed,
    PendingExecution,
    NotCompleted
}

public enum ObjectCondition
{
    Faulty,
    Serviceable,
    Disabled
}

public class TaskStatusConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : Enum
{
    public TaskStatusConverter() : base(
        status => status.ToString(),
        value => (TEnum)Enum.Parse(typeof(TEnum), value))
    {
    }
}