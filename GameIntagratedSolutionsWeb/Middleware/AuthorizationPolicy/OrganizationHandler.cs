﻿using System.Security.Claims;
using IntelliTrackSolutionsWeb.Models.Expansion;
using Microsoft.AspNetCore.Authorization;

namespace IntelliTrackSolutionsWeb.Middleware.AuthorizationPolicy;

public class OrganizationHandler : AuthorizationHandler<OrganizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, OrganizationRequirement requirement)
    {
        var organization = context.User.GetClaimIssuer(ClaimTypes.System);
        var access = context.User.GetClaimIssuer(ClaimTypes.PrimaryGroupSid);

        if (organization is null) return Task.CompletedTask;

        if (requirement.Permission == null)
            context.Succeed(requirement);
        else
            foreach (var requirementRole in requirement.Permission)
                if (access!.Value == requirementRole.ToString())
                    context.Succeed(requirement);

        return Task.CompletedTask;
    }
}