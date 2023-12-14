using Microsoft.AspNetCore.Authorization;

namespace IntelliTrackSolutionsWeb.Middleware.AuthorizationPolicy;

public class OrganizationRequirement : IAuthorizationRequirement
{
    public OrganizationRequirement()
    {
    }

    public OrganizationRequirement(int[] permission)
    {
        Permission = permission;
    }

    public int[]? Permission { get; }
}