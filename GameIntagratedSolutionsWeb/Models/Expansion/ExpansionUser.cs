using Clave.Expressionify;
using IntelliTrackSolutionsWeb.EFModels;
using BCrypted = BCrypt.Net.BCrypt;

namespace IntelliTrackSolutionsWeb.Models.Expansion;

public static partial class ExpansionUser
{
    [Expressionify]
    public static bool IsValidUser(this User tUser, User vUser)
    {
        return tUser.Login != vUser.Login &&
               tUser.Password != vUser.Password &&
               tUser.IdUser != vUser.IdUser;
    }

    [Expressionify]
    public static bool IsValidPassword(this User user, ServiceSystemContext context, string password)
    {
        return BCrypted.Verify(password, context.DecryptPassword(user.Password));
    }
}