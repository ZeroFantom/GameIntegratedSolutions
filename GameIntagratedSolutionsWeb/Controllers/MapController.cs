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
public class MapController : AbstractController<RealApiDeviceHub, ServiceSystemContext>, IControllerBaseAction<Location>
{
    public MapController(ServiceSystemContext dataBaseContext, IHubContext<RealApiDeviceHub> realApiContext) : base(
        dataBaseContext, realApiContext)
    {
    }

    [Authorize]
    public async Task<IActionResult> Map()
    {
        ViewData["Locations"] = await DataBaseContext.Locations.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();

        return View();
    }

    public async Task<IActionResult> LocationTableData()
    {
        var systemObjects = await DataBaseContext.Locations.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).Take(10).ToListAsync();
        var dataTable = new ModelDataTable<Location>(systemObjects);
        return View(dataTable);
    }

    [HttpPost]
    public async Task<IActionResult> LocationTableData(
        [Bind("Models,Search,Skip,Take")] ModelDataTable<Location> locationObject)
    {
        var query = DataBaseContext.Locations.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System));
        locationObject.Models = await ((locationObject.Skip, locationObject.Take) switch
        {
            (0, 0) => query,
            (> 0, > 0) => query.Skip(locationObject.Skip).Take(locationObject.Take),
            (0, > 0) => query.Take(locationObject.Take),
            (> 0, 0) => query.Skip(locationObject.Skip)
        }).ToListAsync();

        if (!string.IsNullOrEmpty(locationObject.Search))
            locationObject.Models = locationObject.Models
                .Where(user => user.ReflectionPropertiesSearch(locationObject.Search)).ToList();

        return View(locationObject);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["SystemObjects"] = await DataBaseContext.SystemObjects.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdLocation,SystemObjectId,Latitude,Longitude")] Location location)
    {
        if (ModelState.IsValid)
        {
            DataBaseContext.Add(location);
            await DataBaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(LocationTableData));
        }

        ViewData["SystemObjects"] = await DataBaseContext.SystemObjects.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(location);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var location = await DataBaseContext.Locations.FirstOrDefaultAsync(x => x.IdLocation == id);
        if (location == null) return NotFound();

        ViewData["SystemObjects"] = await DataBaseContext.SystemObjects.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(location);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("IdLocation,SystemObjectId,Latitude,Longitude")] Location location)
    {
        if (id != location.IdLocation) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                DataBaseContext.Update(location);
                await DataBaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(location.IdLocation))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(LocationTableData));
        }

        ViewData["SystemObjects"] = await DataBaseContext.SystemObjects.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(location);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var location = await DataBaseContext.Locations
            .Include(l => l.SystemObject)
            .FirstOrDefaultAsync(m => m.IdLocation == id);
        if (location == null) return NotFound();

        return View(location);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var location = await DataBaseContext.Locations.FirstOrDefaultAsync(x => x.IdLocation == id);
        if (location != null) DataBaseContext.Locations.Remove(location);

        await DataBaseContext.SaveChangesAsync();
        return RedirectToAction(nameof(LocationTableData));
    }

    private bool LocationExists(int id)
    {
        return DataBaseContext.Locations.Any(e => e.IdLocation == id);
    }
}