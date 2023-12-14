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

[Authorize(Policy = "OrganizationPolicy")]
public class SystemObjectController : AbstractController<RealApiDeviceHub, ServiceSystemContext>,
    IControllerBaseAction<SystemObject>
{
    public SystemObjectController(ServiceSystemContext dataBaseContext, IHubContext<RealApiDeviceHub> realApiContext) :
        base(dataBaseContext, realApiContext)
    {
    }

    public async Task<IActionResult> SystemObjectTableData()
    {
        var systemObjects = await DataBaseContext.SystemObjects.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).Take(10).ToListAsync();
        var dataTable = new ModelDataTable<SystemObject>(systemObjects);
        return View(dataTable);
    }

    [HttpPost]
    public async Task<IActionResult> SystemObjectTableData(
        [Bind("Models,Search,Skip,Take")] ModelDataTable<SystemObject> systemObject)
    {
        var query = DataBaseContext.SystemObjects.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System));
        systemObject.Models = await ((systemObject.Skip, systemObject.Take) switch
        {
            (0, 0) => query,
            (> 0, > 0) => query.Skip(systemObject.Skip).Take(systemObject.Take),
            (0, > 0) => query.Take(systemObject.Take),
            (> 0, 0) => query.Skip(systemObject.Skip)
        }).ToListAsync();

        if (!string.IsNullOrEmpty(systemObject.Search))
            systemObject.Models = systemObject.Models
                .Where(user => user.ReflectionPropertiesSearch(systemObject.Search)).ToList();

        return View(systemObject);
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
        [Bind("IdSystemObject,Name,Description,Condition,DataRegistration,LastUpdate,InformationSystemId")]
        SystemObject systemObject)
    {
        if (ModelState.IsValid)
        {
            DataBaseContext.Add(systemObject);
            await DataBaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(SystemObjectTableData));
        }

        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(systemObject);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var systemObject = await DataBaseContext.SystemObjects.FirstOrDefaultAsync(x => x.IdSystemObject == id);
        if (systemObject == null) return NotFound();
        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(systemObject);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("IdSystemObject,Name,Description,Condition,DataRegistration,LastUpdate,InformationSystemId")]
        SystemObject systemObject)
    {
        if (id != systemObject.IdSystemObject) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                DataBaseContext.Update(systemObject);
                await DataBaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemObjectExists(systemObject.IdSystemObject))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(SystemObjectTableData));
        }

        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(systemObject);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var systemObject = await DataBaseContext.SystemObjects
            .Include(s => s.InformationSystem)
            .FirstOrDefaultAsync(m => m.IdSystemObject == id);
        if (systemObject == null) return NotFound();

        return View(systemObject);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var systemObject = await DataBaseContext.SystemObjects.FirstOrDefaultAsync(x => x.IdSystemObject == id);
        if (systemObject != null) DataBaseContext.SystemObjects.Remove(systemObject);

        await DataBaseContext.SaveChangesAsync();
        return RedirectToAction(nameof(SystemObjectTableData));
    }

    private bool SystemObjectExists(int id)
    {
        return DataBaseContext.SystemObjects.Any(e => e.IdSystemObject == id);
    }
}