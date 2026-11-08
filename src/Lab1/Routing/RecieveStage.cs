using Itmo.ObjectOrientedProgramming.Lab1.Product;
using Itmo.ObjectOrientedProgramming.Lab1.Storaging;
using Itmo.ObjectOrientedProgramming.Lab1.Transport;

namespace Itmo.ObjectOrientedProgramming.Lab1.Routing;

public class RecieveStage : IRouteStage
{
    private const double MaxDistanceKm = 0.01;

    public Storage Storage { get; }

    public IReadOnlyCollection<UnitAmount> Units { get; }

    public RecieveStage(Storage storage, IReadOnlyCollection<UnitAmount> units)
    {
        ArgumentNullException.ThrowIfNull(storage, nameof(storage));
        ArgumentNullException.ThrowIfNull(units, nameof(units));

        Storage = storage;
        Units = units.ToArray();
    }

    public StageResult DoStage(Truck truck)
    {
        ArgumentNullException.ThrowIfNull(truck, nameof(truck));

        if (Storage.Coordinates.DistanceKm(truck.Coordinates) > MaxDistanceKm)
        {
            return new StageResult.TruckIsTooFar();
        }

        if (truck.CheckUnload(Units) is not Truck.UnloadResult.Success)
        {
            return new StageResult.TruckCantUnload();
        }

        if (Storage.CheckRecieve(Units) is not Storage.RecieveResult.Success shipRes)
        {
            return new StageResult.StorageCantRecieve();
        }

        truck.Unload(Units);
        Storage.Recieve(Units);

        return new StageResult.Success(shipRes.RecieveTime);
    }
}