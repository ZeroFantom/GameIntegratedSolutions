using IntelliTrackSolutionsWeb.Controllers.Abstraction;
using IntelliTrackSolutionsWeb.EFModels;
using IntelliTrackSolutionsWeb.SignalRHub;
using Microsoft.AspNetCore.SignalR;

namespace IntelliTrackSolutionsWeb.Controllers;

public class InformationUsersController : AbstractController<RealApiDeviceHub, ServiceSystemContext>,
    IControllerBaseAction<InformationUser>
{
    public InformationUsersController(ServiceSystemContext dataBaseContext,
        IHubContext<RealApiDeviceHub> realApiContext) : base(dataBaseContext, realApiContext)
    {
    }
}