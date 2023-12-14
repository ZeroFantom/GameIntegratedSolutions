using System.Security.Claims;
using IntelliTrackSolutionsWeb.Controllers.Abstraction;
using IntelliTrackSolutionsWeb.EFModels;
using IntelliTrackSolutionsWeb.Models;
using IntelliTrackSolutionsWeb.Models.Expansion;
using IntelliTrackSolutionsWeb.SignalRHub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.Controllers;

[Authorize(Policy = "OrganizationAdministrationPolicy")]
public class RoleController : AbstractController<RealApiDeviceHub, ServiceSystemContext>, IControllerBaseAction<Role>
{
    public RoleController(ServiceSystemContext dataBaseContext, IHubContext<RealApiDeviceHub> realApiContext) : base(
        dataBaseContext, realApiContext)
    {
    }


    public async Task<IActionResult> RoleTableData()
    {
        var tasks = await DataBaseContext.Roles.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).Take(10).ToListAsync();
        var dataTable = new ModelDataTable<Role>(tasks);
        return View(dataTable);
    }

    [HttpPost]
    public async Task<IActionResult> RoleTableData([Bind("Models,Search,Skip,Take")] ModelDataTable<Role> tempUsers)
    {
        var query = DataBaseContext.Roles.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System));

        tempUsers.Models = await ((tempUsers.Skip, tempUsers.Take) switch
        {
            (0, 0) => query,
            (> 0, > 0) => query.Skip(tempUsers.Skip).Take(tempUsers.Take),
            (0, > 0) => query.Take(tempUsers.Take),
            (> 0, 0) => query.Skip(tempUsers.Skip)
        }).ToListAsync();

        if (!string.IsNullOrEmpty(tempUsers.Search))
            tempUsers.Models = tempUsers.Models.Where(user => user.ReflectionPropertiesSearch(tempUsers.Search))
                .ToList();

        return View(tempUsers);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var role = await DataBaseContext.Roles.FirstOrDefaultAsync(x => x.IdRole == id);
        if (role == null) return NotFound();
        ViewData["AccessLevels"] = await DataBaseContext.AccessLevels.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdRole,AccessLevelId,Title,Description")] Role role)
    {
        if (id != role.IdRole) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                DataBaseContext.Update(role);
                await DataBaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(role.IdRole))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(RoleTableData));
        }

        ViewData["AccessLevels"] = await DataBaseContext.AccessLevels.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(role);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var role = await DataBaseContext.Roles
            .Include(r => r.AccessLevel)
            .FirstOrDefaultAsync(m => m.IdRole == id);
        if (role == null) return NotFound();

        return View(role);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var role = await DataBaseContext.Roles.FirstOrDefaultAsync(x => x.IdRole == id);
        if (role != null) DataBaseContext.Roles.Remove(role);

        await DataBaseContext.SaveChangesAsync();
        return RedirectToAction(nameof(RoleTableData));
    }

    private bool RoleExists(int id)
    {
        return (DataBaseContext.Roles?.Any(e => e.IdRole == id)).GetValueOrDefault();
    }
}