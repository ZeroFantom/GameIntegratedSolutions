using Microsoft.AspNetCore.Authorization;

namespace IntelliTrackSolutionsWeb.Middleware.AuthorizationPolicy;

public class OrganizationAccessRequirement : IAuthorizationRequirement
{
    public OrganizationAccessRequirement(int organization, int[]? permission = null)
    {
        Permission = permission;
        Organization = organization;
    }

    public int Organization { get; }
    public int[]? Permission { get; }
}