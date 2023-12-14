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
public class ChatController : AbstractController<RealApiDeviceHub, ServiceSystemContext>, IControllerBaseAction<Chat>
{
    public ChatController(ServiceSystemContext dataBaseContext, IHubContext<RealApiDeviceHub> realApiContext) :
        base(dataBaseContext, realApiContext)
    {
    }

    public async Task<IActionResult> ChatTableData()
    {
        var chats = await DataBaseContext.Chats.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).Take(10).ToListAsync();
        var dataTable = new ModelDataTable<Chat>(chats);
        return View(dataTable);
    }

    [HttpPost]
    public async Task<IActionResult> ChatTableData([Bind("Models,Search,Skip,Take")] ModelDataTable<Chat> tempUsers)
    {
        var query = DataBaseContext.Chats.IncludePartialFullInfo()
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
    public async Task<IActionResult> Create([Bind("IdChat,Title,InformationSystemId")] Chat chat)
    {
        if (ModelState.IsValid)
        {
            DataBaseContext.Add(chat);
            await DataBaseContext.SaveChangesAsync();
            return RedirectToAction(nameof(ChatTableData));
        }

        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(chat);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var chat = await DataBaseContext.Chats.FirstOrDefaultAsync(x => x.IdChat == id);
        if (chat == null) return NotFound();
        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(chat);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdChat,Title,InformationSystemId")] Chat chat)
    {
        if (id != chat.IdChat) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                DataBaseContext.Update(chat);
                await DataBaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatExists(chat.IdChat))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(ChatTableData));
        }

        ViewData["InformationSystems"] = await DataBaseContext.InformationSystems.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View(chat);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var chat = await DataBaseContext.Chats
            .Include(c => c.InformationSystem)
            .FirstOrDefaultAsync(m => m.IdChat == id);
        if (chat == null) return NotFound();

        return View(chat);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var chat = await DataBaseContext.Chats.FirstOrDefaultAsync(x => x.IdChat == id);
        if (chat != null) DataBaseContext.Chats.Remove(chat);

        await DataBaseContext.SaveChangesAsync();
        return RedirectToAction(nameof(ChatTableData));
    }

    private bool ChatExists(int id)
    {
        return DataBaseContext.Chats.Any(e => e.IdChat == id);
    }
}