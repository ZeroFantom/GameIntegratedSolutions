using System.Globalization;
using System.Security.Claims;
using IntelliTrackSolutionsWeb.Controllers.Abstraction;
using IntelliTrackSolutionsWeb.EFModels;
using IntelliTrackSolutionsWeb.Models;
using IntelliTrackSolutionsWeb.Models.Expansion;
using IntelliTrackSolutionsWeb.SignalRHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.Controllers;

public class UsersController : AbstractController<RealApiDeviceHub, ServiceSystemContext>, IControllerBaseAction<User>
{
    public UsersController(ServiceSystemContext context, IHubContext<RealApiDeviceHub> mobileContext) : base(context,
        mobileContext)
    {
    }

    #region Методы администрирования

    /// <summary>
    ///     Метод отображения таблицы пользовательских аккаунтов.
    /// </summary>
    /// <returns></returns>
    [Authorize(Policy = "OrganizationAdministrationPolicy")]
    public async Task<IActionResult> UserTableData()
    {
        var users = await DataBaseContext.Users.IncludesDetailAuthentification()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).Take(10).ToListAsync();
        var dataTable = new ModelDataTable<User>(users);
        return View("Administration/UserTableData", dataTable);
    }

    /// <summary>
    ///     Метод отображения таблицы пользовательских аккаунтов.
    /// </summary>
    /// <param name="tempUsers"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "OrganizationAdministrationPolicy")]
    public async Task<IActionResult> UserTableData([Bind("Models,Search,Skip,Take")] ModelDataTable<User> tempUsers)
    {
        var query = DataBaseContext.Users.IncludesDetailAuthentification()
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

        return View("Administration/UserTableData", tempUsers);
    }

    /// <summary>
    ///     Метод редактирования пользователя на стороне администрации.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Policy = "OrganizationAdministrationPolicy")]
    public async Task<IActionResult> AdminEdit(int? id)
    {
        if (id == null) return NotFound();

        var user = await DataBaseContext.Users.FindAsync(id);
        if (user == null) return NotFound();

        ViewData["AccessLevels"] = await DataBaseContext.AccessLevels.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View("Administration/Edit", user);
    }

    /// <summary>
    ///     Метод редактирования пользователя на стороне администрации.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "OrganizationAdministrationPolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminEdit(int id, [Bind("IdUser,Login,Password,AccessLevelId")] User user)
    {
        if (id != user.IdUser) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var tempUser = await DataBaseContext.Users.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdUser == user.IdUser);

                if (user.Password != tempUser!.Password)
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                DataBaseContext.Users.Update(user);
                await DataBaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.IdUser)) return NotFound();

                throw;
            }

            return RedirectToAction(nameof(UserTableData));
        }

        ViewData["AccessLevels"] = await DataBaseContext.AccessLevels.IncludePartialFullInfo()
            .ConstrainOrganization(User.GetClaimIssuer(ClaimTypes.System)).ToListAsync();
        return View("Administration/Edit", user);
    }

    /// <summary>
    ///     Метод удаления пользователя по id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Policy = "OrganizationAdministrationPolicy")]
    public async Task<IActionResult> AdminDelete(int? id)
    {
        if (id == null) return NotFound();

        var user = await DataBaseContext.Users
            .Include(u => u.AccessLevel)
            .FirstOrDefaultAsync(m => m.IdUser == id);
        if (user == null) return NotFound();

        return View("Administration/Delete", user);
    }

    /// <summary>
    ///     Метод удаления пользователя по id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "OrganizationAdministrationPolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminDeleteConfirmed([Bind("IdUser")] User user)
    {
        var userI = await DataBaseContext.Users.FirstOrDefaultAsync(x => x.IdUser == user.IdUser);

        if (userI != null)
        {
            DataBaseContext.Users.Remove(userI);
            await DataBaseContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(UserTableData));
    }

    /// <summary>
    ///     Метод проверяет существование пользователя в бд.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool UserExists(int id)
    {
        return DataBaseContext.Users.Any(e => e.IdUser == id);
    }

    #endregion

    /// <summary>
    ///     Метод проверки авторизации, в случае успеха возвращает модальное окно на тех страницах к которым доступ технически
    ///     закрыт.
    /// </summary>
    private async void IsAuth()
    {
        if (await this.IsAuthUser())
            ViewData["ModalWindow"] = new ModalWindow("Предупреждение",
                "Вы не можете воспользоваться некоторыми страницами будучи авторизованным!\nВы подтверждаете выход из текущего аккаунта?",
                true, DefaultButtonTemplate.AuthYesNo);
    }

    /// <summary>
    ///     Метод прогрузки страницы регистрации при обращении с url или обновлении.
    /// </summary>
    /// <returns></returns>
    public IActionResult Create()
    {
        IsAuth();

        ViewData["Error"] = null;
        return View();
    }

    /// <summary>
    ///     Метод регистрации на сайте, в случае отправки формы.
    /// </summary>
    /// <param name="user">регистрируемый поользователь</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdUser,Login,Password")] User user)
    {
        var firstName = Request.Form["first_name"];
        var lastName = Request.Form["last_name"];

        if (!ModelState.IsValid)
        {
            ViewData["Error"] = null;
            return View(user);
        }

        if (firstName == "" || lastName == "")
        {
            ViewData["Error"] = "заполните пожалуйста все поля!";
            return View(user);
        }

        user.InformationUser = new InformationUser
        {
            FirstName = firstName!,
            LastName = lastName!
        };

        try
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await DataBaseContext.Users.AddAsync(user);
            await DataBaseContext.SaveChangesAsync();

            return View(nameof(Login));
        }
        catch (Exception e)
        {
            if (e.InnerException != null && e.InnerException.Message.Contains("Duplicate"))
                ViewData["Error"] = "такой пользователь уже существует!";
            else
                ViewData["Error"] = "ошибка во время обработки запроса, попробуйте позже!";

            return View(user);
        }
    }

    /// <summary>
    ///     Метод прогрузки и отображения страницы редактирования профиля.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public async Task<IActionResult> Edit()
    {
        return View("Profile",
            await DataBaseContext.Users.Include(x => x.InformationUser).FirstOrDefaultAsync(x =>
                x.IdUser.ToString() == User.GetClaimIssuer(ClaimTypes.Sid)!.Value));
    }


    /// <summary>
    ///     Метод прогрузки и отображения страницы редактирования профиля.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public IActionResult EditChanges(User user)
    {
        return View("Profile", user);
    }

    /// <summary>
    ///     Метод редактирования профиля, форма информации об аккаунте.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("IdUser,Login,Password")] User model)
    {
        var user = await DataBaseContext.Users.Include(x => x.InformationUser)
            .FirstOrDefaultAsync(x => x.IdUser.ToString() == User.GetClaimIssuer(ClaimTypes.Sid)!.Value);

        var firstName = Request.Form["firstName"];
        var lastName = Request.Form["lastName"];
        var middleName = Request.Form["middleName"];

        if (!ModelState.IsValid) return View("Profile", model);

        user!.InformationUser!.FirstName = firstName;
        user.InformationUser.LastName = lastName;
        user.InformationUser.MiddleName = middleName;

        user.Login = model.Login;

        if (!user.IsValidPassword(DataBaseContext, model.Password))
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

        DataBaseContext.Users.Update(user);
        await DataBaseContext.SaveChangesAsync();
        await this.NotifyRealApiDeviceUser("Данные аккаунта изменены!", "Пользовательские данные");
        return EditChanges(user);
    }

    /// <summary>
    ///     Методы выхода из аккаунта.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    ///     Метод прогрузки страницы авторизации при обращении с url или обновлении.
    /// </summary>
    /// <returns></returns>
    public IActionResult Login()
    {
        IsAuth();

        ViewData["Error"] = null;
        ViewData["CodeIsEnable"] ??= false;
        return View();
    }

    /// <summary>
    ///     Метод авторизации на сайте.
    /// </summary>
    /// <param name="user">авторизируемый пользователь</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([Bind("IdUser,Login,Password")] User user)
    {
        var code = Request.Form["code"];

        if (!ModelState.IsValid)
        {
            ViewData["Error"] = null;
            return View(user);
        }

        User? userVerify;

        try
        {
            userVerify = await DataBaseContext.Users.FindUser(DataBaseContext, user);
        }
        catch (Exception ex)
        {
            ViewData["Error"] = "ошибка во время обработки запроса, попробуйте позже!";

            return View(user);
        }

        if (userVerify == null)
        {
            ViewData["Error"] = "Неверный логин или пароль!";
            return View(user);
        }

        if (userVerify.TwoFactorAuthentification != null)
        {
            ViewData["CodeIsEnable"] = true;

            if (code == "")
            {
                ViewData["Error"] = "Пожалуйста введите код!";
            }
            else if (userVerify.TwoFactorAuthentification.Code != code)
            {
                ViewData["Error"] = "Неверный двухфакторный код!";
            }
            else
            {
                HttpContext.Session.SetInt32("UserId", userVerify.IdUser);

                return RedirectToAction(nameof(SuccessAuth));
            }
        }
        else
        {
            HttpContext.Session.SetInt32("UserId", userVerify.IdUser);

            return RedirectToAction(nameof(SuccessAuth));
        }

        return View(user);
    }

    /// <summary>
    ///     Промежуточный метод во избежание проблем с обновлением навигационной панели.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public IActionResult SuccessAuth()
    {
        return View();
    }

    /// <summary>
    ///     Метод скачивания политики конфиденциальности.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult SavePrivacy()
    {
        return File("file/Privacy.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "Privacy.docx");
    }

    /// <summary>
    ///     Метод изменения автара пользователя на странице.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AvatarChanges()
    {
        var user = await DataBaseContext.Users.Include(x => x.InformationUser)
            .FirstOrDefaultAsync(x => x.IdUser.ToString() == User.GetClaimIssuer(ClaimTypes.Sid)!.Value);
        var avatarNew = Request.Form["avatar"].ToString();
        if (!string.IsNullOrEmpty(avatarNew) && Uri.TryCreate(avatarNew, UriKind.RelativeOrAbsolute, out var uriAvatar))
        {
            var req = new HttpClient();
            var responseMessage = await req.GetAsync(uriAvatar);
            var isImage = responseMessage.Content.Headers.ContentType?.MediaType?.ToLower(CultureInfo.InvariantCulture)
                .StartsWith("image/");
            if (isImage != null && isImage.Value)
            {
                user!.InformationUser!.Avatar = avatarNew;
                await this.NotifyRealApiDeviceUser("Аватар изменён!", "Пользовательские данные");
                DataBaseContext.InformationUsers.Update(user.InformationUser!);
                await DataBaseContext.SaveChangesAsync();
            }
        }

        return EditChanges(user);
    }
}