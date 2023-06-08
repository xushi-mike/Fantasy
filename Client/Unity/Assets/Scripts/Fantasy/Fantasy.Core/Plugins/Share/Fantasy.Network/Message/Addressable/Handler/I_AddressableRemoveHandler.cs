#if FANTASY_SERVER
namespace Fantasy.Core.Network;

public sealed class I_AddressableRemoveHandler : RouteRPC<Scene, I_AddressableRemove_Request, I_AddressableRemove_Response>
{
    protected override async FTask Run(Scene scene, I_AddressableRemove_Request request, I_AddressableRemove_Response response, Action reply)
    {
        await scene.GetComponent<AddressableManageComponent>().Remove(request.AddressableId);
    }
}
#endif