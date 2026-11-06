using Itmo.ObjectOrientedProgramming.Lab1.Product;
using Itmo.ObjectOrientedProgramming.Lab1.Storaging;
using Itmo.ObjectOrientedProgramming.Lab1.Transport;

namespace Itmo.ObjectOrientedProgramming.Lab1.Routing;

public class ShipStage : IRouteStage
{
    private const double MaxDistanceKm = 0.01;

    public Storage Storage { get; }

    public IReadOnlyCollection<UnitAmount> Units { get; }

    public ShipStage(Storage storage, IReadOnlyCollection<UnitAmount> units)
    {
        ArgumentNullException.ThrowIfNull(storage, nameof(storage));
        ArgumentNullException.ThrowIfNull(units, nameof(units));

        Storage = storage;
        Units = units;
    }

    public StageResult DoStage(Truck truck)
    {
        ArgumentNullException.ThrowIfNull(truck, nameof(truck));

        if (Storage.Coordinates.DistanceKm(truck.Coordinates) > MaxDistanceKm)
        {
            return new StageResult.TruckIsTooFar();
        }

        if (Storage.CheckShip(Units) is not Storage.ShipResult.Success shipRes)
        {
            return new StageResult.StorageCantShip();
        }

        if (truck.CheckLoad(Units) is not Truck.LoadResult.Success)
        {
            return new StageResult.TruckCantLoad();
        }

        Storage.Ship(Units);
        truck.Load(Units);

        return new StageResult.Success(shipRes.ShipTime);
    }
}