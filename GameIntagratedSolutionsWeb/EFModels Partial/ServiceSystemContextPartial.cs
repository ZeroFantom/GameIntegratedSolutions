using Clave.Expressionify;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels_Partial;

public static class ConfiguredServiceSystemContext
{
    public static void ConfigurationMySql(this DbContextOptionsBuilder options)
    {
        options.UseMySql("Name=IntelliTrackSolutionsDB", ServerVersion.Parse("8.0.30 mysql"),
                x => x.UseNetTopologySuite())
            .UseExpressionify(o => o.WithEvaluationMode(ExpressionEvaluationMode.FullCompatibilityButSlow))
            .UseAllCheckConstraints()
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
}