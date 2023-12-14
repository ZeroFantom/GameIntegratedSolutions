using IntelliTrackSolutionsWeb.EFModels;
using Microsoft.EntityFrameworkCore;
using Task = IntelliTrackSolutionsWeb.EFModels.Task;

namespace IntelliTrackSolutionsWeb.Models.Expansion;

public static class ExpansionDbSet
{
    public static async Task<User?> FindUser(this DbSet<User> users, ServiceSystemContext context, User user,
        bool detail = false)
    {
        return (await (detail ? users.IncludesDetailAuthentification() : users.IncludesLiteAuthentification())
                .Where(x => x.Login == user.Login).ToListAsync())
            .FirstOrDefault(x => x.IsValidPassword(context, user.Password));
    }

    public static async Task<User?> FindUser(this DbSet<User> users, ServiceSystemContext context, string login,
        string password, bool detail = false)
    {
        return (await (detail ? users.IncludesDetailAuthentification() : users.IncludesLiteAuthentification())
                .Where(x => x.Login == login).ToListAsync())
            .FirstOrDefault(x => x.IsValidPassword(context, password));
    }


    public static IQueryable<User> IncludesLiteAuthentification(this DbSet<User> users)
    {
        return users.Include(x => x.TwoFactorAuthentification);
    }

    public static IQueryable<User> IncludesDetailAuthentification(this DbSet<User> users)
    {
        return users.IncludesLiteAuthentification()
            .Include(x => x.InformationUser)
            .Include(x => x.AccessLevel)
            .ThenInclude(x => x.InformationSystem)
            .Include(x => x.AccessLevel)
            .ThenInclude(x => x.Role);
    }

    public static IQueryable<User> IncludePartialFullInfo(this DbSet<User> users)
    {
        return users
            .IncludesLiteAuthentification()
            .Include(x => x.InformationUser)
            .Include(x => x.AccessLevel)
            .ThenInclude(x => x.Role)
            .Include(x => x.AccessLevel)
            .ThenInclude(x => x.InformationSystem)
            .ThenInclude(x => x.Chats)
            .ThenInclude(x => x.Messages);
    }

    public static IQueryable<AccessLevel> IncludePartialFullInfo(this DbSet<AccessLevel> usersAccessLevels)
    {
        return usersAccessLevels
            .Include(x => x.Role)
            .Include(x => x.InformationSystem);
    }

    public static IQueryable<Location> IncludePartialFullInfo(this DbSet<Location> locations)
    {
        return locations.Include(x => x.SystemObject);
    }

    public static IQueryable<InformationSystem> IncludePartialFullInfo(this DbSet<InformationSystem> informationSystems)
    {
        return informationSystems
            .Include(x => x.Chats)
            .ThenInclude(x => x.Messages)
            .ThenInclude(x => x.Author)
            .Include(x => x.Tasks)
            .Include(x => x.AccessLevels)
            .ThenInclude(x => x.Role)
            .Include(x => x.AccessLevels)
            .ThenInclude(x => x.Users)
            .Include(x => x.SystemObjects)
            .ThenInclude(x => x.Locations);
    }

    public static IQueryable<SystemObject> IncludePartialFullInfo(this DbSet<SystemObject> systemObjects)
    {
        return systemObjects
            .Include(x => x.InformationSystem)
            .Include(x => x.Locations);
    }

    public static IQueryable<Role> IncludePartialFullInfo(this DbSet<Role> roles)
    {
        return roles
            .Include(x => x.AccessLevel)
            .ThenInclude(x => x.InformationSystem);
    }

    public static IQueryable<Chat> IncludePartialFullInfo(this DbSet<Chat> chats)
    {
        return chats.Include(x => x.InformationSystem)
            .Include(x => x.Messages)
            .ThenInclude(x => x.Author);
    }

    public static IQueryable<Task> IncludePartialFullInfo(this DbSet<Task> tasks)
    {
        return tasks.Include(x => x.InformationSystem);
    }
}