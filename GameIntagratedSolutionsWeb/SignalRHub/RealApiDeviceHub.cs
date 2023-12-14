using System.Text.Json;
using IntelliTrackSolutionsWeb.EFModels;
using IntelliTrackSolutionsWeb.Models.Expansion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace IntelliTrackSolutionsWeb.SignalRHub;

public class RealApiDeviceHub : Hub
{
    private readonly ServiceSystemContext _context;

    public RealApiDeviceHub(ServiceSystemContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Метод проверяет существование пользователя на основе логина и пароля.
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task AuthentificationValidate(string login, string password)
    {
        var userVerify = await _context.Users.FindUser(_context, login, password);
        await Clients.Caller.SendAsync("ReceiveAuthentificationValidate", userVerify != null);
    }

    /// <summary>
    ///     Отправка авторизационных данных для мобильного приложения.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public async Task AuthentificationUserMobile()
    {
        try
        {
            var userVerify = await _context.Users.IncludePartialFullInfo()
                .FirstOrDefaultAsync(x => x.IdUser.ToString() == Context.UserIdentifier);

            foreach (var chat in userVerify!.AccessLevel!.InformationSystem!.Chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Access:{userVerify.AccessLevel.IdAccessLevel}");
                await Groups.AddToGroupAsync(Context.ConnectionId,
                    $"Group:{userVerify.AccessLevel.InformationSystem.IdInformationSystem}");
                await Groups.AddToGroupAsync(Context.ConnectionId, chat.IdChat.ToString());
            }

            await Clients.Caller.SendAsync("ReceiveUserMobile", JsonSerializer.Serialize(userVerify));
        }
        catch (Exception)
        {
            //ignore
        }
    }

    /// <summary>
    ///     Метод перенаправляет сообщение по соответствующим чатам, а также добавляет его в базу данных.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task InviteChatMessage(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();

        await MessageChat(message);
    }

    /// <summary>
    ///     Метод отправляет сообщение в чат соответствующей группы пользователей.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task MessageChat(Message message)
    {
        await Clients.Group(message.ChatId.ToString())
            .SendAsync("ReceiveChatMessage", JsonSerializer.Serialize(message));
    }
}