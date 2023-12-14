using System.Security.Claims;
using IntelliTrackSolutionsWeb.EFModels;
using Task = IntelliTrackSolutionsWeb.EFModels.Task;

namespace IntelliTrackSolutionsWeb.Models.Expansion;

public static class ExpansionIQueryable
{
    public static IQueryable<User> ConstrainOrganization(this IQueryable<User> queryable, Claim? idIs)
    {
        return queryable.Where(x =>
            idIs != null && x.AccessLevel != null && int.Parse(idIs.Value) != 1
                ? x.AccessLevel.InformationSystemId == int.Parse(idIs.Value)
                : idIs != null && int.Parse(idIs.Value) == 1);
    }

    public static IQueryable<SystemObject> ConstrainOrganization(this IQueryable<SystemObject> queryable, Claim? idIs)
    {
        return queryable.Where(x =>
            idIs != null && int.Parse(idIs.Value) != 1
                ? x.InformationSystemId == int.Parse(idIs.Value)
                : idIs != null && int.Parse(idIs.Value) == 1);
    }

    public static IQueryable<AccessLevel> ConstrainOrganization(this IQueryable<AccessLevel> queryable, Claim? idIs)
    {
        return queryable.Where(x => idIs != null && int.Parse(idIs.Value) != 1
            ? x.InformationSystemId == int.Parse(idIs.Value)
            : idIs != null && int.Parse(idIs.Value) == 1);
    }

    public static IQueryable<Location> ConstrainOrganization(this IQueryable<Location> queryable, Claim? idIs)
    {
        return queryable.Where(x =>
            idIs != null && int.Parse(idIs.Value) != 1
                ? x.SystemObject!.InformationSystemId == int.Parse(idIs.Value)
                : idIs != null && int.Parse(idIs.Value) == 1);
    }

    public static IQueryable<Task> ConstrainOrganization(this IQueryable<Task> queryable, Claim? idIs)
    {
        return queryable.Where(x =>
            idIs != null && int.Parse(idIs.Value) != 1
                ? x.InformationSystemId == int.Parse(idIs.Value)
                : idIs != null && int.Parse(idIs.Value) == 1);
    }

    public static IQueryable<Chat> ConstrainOrganization(this IQueryable<Chat> queryable, Claim? idIs)
    {
        return queryable.Where(x =>
            idIs != null && int.Parse(idIs.Value) != 1
                ? x.InformationSystemId == int.Parse(idIs.Value)
                : idIs != null && int.Parse(idIs.Value) == 1);
    }

    public static IQueryable<Role> ConstrainOrganization(this IQueryable<Role> queryable, Claim? idIs)
    {
        return queryable.Where(x =>
            idIs != null && int.Parse(idIs.Value) != 1
                ? x.AccessLevel!.InformationSystemId == int.Parse(idIs.Value)
                : idIs != null && int.Parse(idIs.Value) == 1);
    }

    public static IQueryable<InformationSystem> ConstrainOrganization(this IQueryable<InformationSystem> queryable,
        Claim? idIs)
    {
        return queryable.Where(x =>
            idIs != null && int.Parse(idIs.Value) != 1
                ? x.IdInformationSystem == int.Parse(idIs.Value)
                : idIs != null && int.Parse(idIs.Value) == 1);
    }
}