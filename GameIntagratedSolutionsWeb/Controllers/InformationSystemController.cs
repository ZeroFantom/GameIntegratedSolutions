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

public class InformationSystemController : AbstractController<RealApiDeviceHub, ServiceSystemContext>,
    IControllerBaseAction<InformationSystem>
{
    public InformationSystemController(ServiceSystemContext context, IHubContext<RealApiDeviceHub> realApiContext) :
        base(context, realApiContext)
    {
    }

    [Authorize]
    public async Task<IActionResult> Monitoring()
    {
        if (int.TryParse(User.GetClaimIssuer(ClaimTypes.System)?.Value, out var isId))
            return isId switch
            {
                1 => View(await DataBaseContext.InformationSystems.IncludePartialFullInfo().ToListAsync()),
                _ => View(new List<InformationSystem>
                {
                    (await DataBaseContext.InformationSystems.IncludePartialFullInfo().FirstOrDefaultAsync(
                        x => x.IdInformationSystem == isId))!
                })
            };

        return View();
    }

    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> InformationSystemTableData()
    {
        var informationSystem =
            await DataBaseContext.InformationSystems.IncludePartialFullInfo().Take(10).ToListAsync();
        var dataTable = new ModelDataTable<InformationSystem>(informationSystem);
        return View(dataTable);
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> InformationSystemTableData(
        [Bind("Models,Search,Skip,Take")] ModelDataTable<InformationSystem> informationSystem)
    {
        var query = DataBaseContext.InformationSystems.IncludePartialFullInfo();
        informationSystem.Models = await ((informationSystem.Skip, informationSystem.Take) switch
        {
            (0, 0) => query,
            (> 0, > 0) => query.Skip(informationSystem.Skip).Take(informationSystem.Take),
            (0, > 0) => query.Take(informationSystem.Take),
            (> 0, 0) => query.Skip(informationSystem.Skip)
        }).ToListAsync();

        if (!string.IsNullOrEmpty(informationSystem.Search))
            informationSystem.Models = informationSystem.Models
                .Where(user => user.ReflectionPropertiesSearch(informationSystem.Search)).ToList();

        return View(informationSystem);
    }

    [Authorize(Policy = "AdminPolicy")]
    public IActionResult Create()
    {
        return View("Administration/Create");
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("IdInformationSystem,Title,Description,ApiKey")]
        InformationSystem informationSystem)
    {
        if (ModelState.IsValid)
        {
            DataBaseContext.Add(informationSystem);
            await DataBaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(InformationSystemTableData));
        }

        return View("Administration/Create", informationSystem);
    }

    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var informationSystem =
            await DataBaseContext.InformationSystems.FirstOrDefaultAsync(x => x.IdInformationSystem == id);
        if (informationSystem == null) return NotFound();

        return View(informationSystem);
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("IdInformationSystem,Title,Description,ApiKey")]
        InformationSystem informationSystem)
    {
        if (id != informationSystem.IdInformationSystem) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                DataBaseContext.Update(informationSystem);
                await DataBaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InformationSystemExists(informationSystem.IdInformationSystem))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(InformationSystemTableData));
        }

        return View(informationSystem);
    }

    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var informationSystem = await DataBaseContext.InformationSystems
            .FirstOrDefaultAsync(m => m.IdInformationSystem == id);
        if (informationSystem == null) return NotFound();

        return View("Administration/Delete", informationSystem);
    }

    [HttpPost]
    [ActionName("Delete")]
    [Authorize(Policy = "AdminPolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var informationSystem =
            await DataBaseContext.InformationSystems.FirstOrDefaultAsync(x => x.IdInformationSystem == id);
        if (informationSystem != null) DataBaseContext.InformationSystems.Remove(informationSystem);

        await DataBaseContext.SaveChangesAsync();
        return RedirectToAction(nameof(InformationSystemTableData));
    }

    private bool InformationSystemExists(int id)
    {
        return DataBaseContext.InformationSystems.Any(e => e.IdInformationSystem == id);
    }
}