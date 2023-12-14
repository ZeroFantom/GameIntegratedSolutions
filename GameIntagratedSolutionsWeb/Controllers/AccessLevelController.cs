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
public class AccessLevelController : AbstractController<RealApiDeviceHub, ServiceSystemContext>,
    IControllerBaseAction<InformationSystem>
{
    public AccessLevelController(ServiceSystemContext context, IHubContext<RealApiDeviceHub> realApiContext) :
        base(context, realApiContext)
    {
    }

    public async Task<IActionResult> AccessLevelTableData()
    {
        var systemObjects = await DataBaseContext.AccessLevels.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).Take(10).ToListAsync();
        var dataTable = new ModelDataTable<AccessLevel>(systemObjects);
        return View(dataTable);
    }

    [HttpPost]
    public async Task<IActionResult> AccessLevelTableData(
        [Bind("Models,Search,Skip,Take")] ModelDataTable<AccessLevel> accessLevel)
    {
        var query = DataBaseContext.AccessLevels.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System));

        accessLevel.Models = await ((accessLevel.Skip, accessLevel.Take) switch
        {
            (0, 0) => query,
            (> 0, > 0) => query.Skip(accessLevel.Skip).Take(accessLevel.Take),
            (0, > 0) => query.Take(accessLevel.Take),
            (> 0, 0) => query.Skip(accessLevel.Skip)
        }).ToListAsync();

        if (!string.IsNullOrEmpty(accessLevel.Search))
            accessLevel.Models = accessLevel.Models.Where(user => user.ReflectionPropertiesSearch(accessLevel.Search))
                .ToList();

        return View(accessLevel);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("IdAccessLevel,Permission,InformationSystemId")] AccessLevel accessLevel)
    {
        if (ModelState.IsValid)
        {
            DataBaseContext.Add(accessLevel);
            await DataBaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(AccessLevelTableData));
        }

        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(accessLevel);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var accessLevel = await DataBaseContext.AccessLevels.FirstOrDefaultAsync(x => x.IdAccessLevel == id);
        if (accessLevel == null) return NotFound();
        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(accessLevel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("IdAccessLevel,Permission,InformationSystemId")] AccessLevel accessLevel)
    {
        if (id != accessLevel.IdAccessLevel) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                DataBaseContext.Update(accessLevel);
                await DataBaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccessLevelExists(accessLevel.IdAccessLevel))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(AccessLevelTableData));
        }

        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(accessLevel);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var accessLevel = await DataBaseContext.AccessLevels
            .Include(a => a.InformationSystem)
            .FirstOrDefaultAsync(m => m.IdAccessLevel == id);

        if (accessLevel == null) return NotFound();

        return View(accessLevel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var accessLevel = await DataBaseContext.AccessLevels.FirstOrDefaultAsync(x => x.IdAccessLevel == id);

        if (accessLevel != null)
        {
            DataBaseContext.AccessLevels.Remove(accessLevel);
            await DataBaseContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(AccessLevelTableData));
    }

    private bool AccessLevelExists(int id)
    {
        return DataBaseContext.AccessLevels.Any(e => e.IdAccessLevel == id);
    }
}