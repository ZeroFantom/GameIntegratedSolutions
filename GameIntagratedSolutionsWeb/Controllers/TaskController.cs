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
using Task = IntelliTrackSolutionsWeb.EFModels.Task;

namespace IntelliTrackSolutionsWeb.Controllers;

[Authorize(Policy = "OrganizationAdministrationPolicy")]
public class TaskController : AbstractController<RealApiDeviceHub, ServiceSystemContext>, IControllerBaseAction<Task>
{
    public TaskController(ServiceSystemContext dataBaseContext, IHubContext<RealApiDeviceHub> realApiContext) :
        base(dataBaseContext, realApiContext)
    {
    }

    public async Task<IActionResult> TaskTableData()
    {
        var tasks = await DataBaseContext.Tasks.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).Take(10).ToListAsync();
        var dataTable = new ModelDataTable<Task>(tasks);
        return View(dataTable);
    }

    [HttpPost]
    public async Task<IActionResult> TaskTableData([Bind("Models,Search,Skip,Take")] ModelDataTable<Task> tempUsers)
    {
        var query = DataBaseContext.Tasks.IncludePartialFullInfo()
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

    public async Task<IActionResult> Create()
    {
        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("IdTask,InformationSystemId,Title,Goal,DataRegistration,LastUpdate,Status,Deadline")] Task task)
    {
        if (ModelState.IsValid)
        {
            DataBaseContext.Add(task);
            await DataBaseContext.SaveChangesAsync();
            await this.NotifyRealApiDeviceUser(task.Title, "Создано задание организации",
                ExpansionControllerRelationApiDevice.ESendClaimType.Organization);
            return RedirectToAction(nameof(TaskTableData));
        }

        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(task);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var task = await DataBaseContext.Tasks.FirstOrDefaultAsync(x => x.IdTask == id);

        if (task == null) return NotFound();

        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("IdTask,InformationSystemId,Title,Goal,DataRegistration,LastUpdate,Status,Deadline")] Task task)
    {
        if (id != task.IdTask) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                DataBaseContext.Update(task);
                await DataBaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.IdTask))
                    return NotFound();
                throw;
            }

            await this.NotifyRealApiDeviceUser(task.Title, "Ред. задания организации",
                ExpansionControllerRelationApiDevice.ESendClaimType.Organization);
            return RedirectToAction(nameof(TaskTableData));
        }

        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(task);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var task = await DataBaseContext.Tasks
            .Include(t => t.InformationSystem)
            .FirstOrDefaultAsync(m => m.IdTask == id);
        if (task == null) return NotFound();

        return View(task);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var task = await DataBaseContext.Tasks.FirstOrDefaultAsync(x => x.IdTask == id);
        if (task != null) DataBaseContext.Tasks.Remove(task);

        await DataBaseContext.SaveChangesAsync();
        return RedirectToAction(nameof(TaskTableData));
    }

    private bool TaskExists(int id)
    {
        return DataBaseContext.Tasks.Any(e => e.IdTask == id);
    }
}