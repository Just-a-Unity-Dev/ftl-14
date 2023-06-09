using Content.Shared._FTL.Weapons;
using Robust.Client.GameObjects;
using Robust.Shared.Map;

namespace Content.Client._FTL.Weapons;

public sealed class WeaponTargetingBoundUserInterface : BoundUserInterface
{
    [Dependency] private EntityManager _entityManager = default!;
    private WeaponTargetingWindow? _window;

    public WeaponTargetingBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {

    }

    protected override void Open()
    {
        base.Open();
        _window?.Close();
        EntityUid? gridUid = null;

        if (IoCManager.Resolve<IEntityManager>().TryGetComponent<TransformComponent>(Owner, out var xform))
        {
            gridUid = xform.GridUid;
        }

        _window = new WeaponTargetingWindow(this, gridUid, xform?.Coordinates, xform?.LocalRotation);
        _window.OpenCentered();
        _window.OnClose += Close;
    }

    public void FireWeapon(EntityCoordinates entityCoordinates, EntityUid targetGrid)
    {
        var message = new FireWeaponSendMessage(entityCoordinates, targetGrid);

        SendMessage(message);
    }

    public void ScanButton(EntityUid targetGrid)
    {
        var message = new ShipScanRequestMessage(targetGrid);

        SendMessage(message);
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not WeaponTargetingUserInterfaceState msg)
            return;
        _window?.UpdateState(msg);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _window?.Dispose();
    }
}
