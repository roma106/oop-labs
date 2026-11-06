using Itmo.ObjectOrientedProgramming.Lab1.Transport;

namespace Itmo.ObjectOrientedProgramming.Lab1.Routing;

public abstract record StageResult
{
    private StageResult() { }

    public sealed record Success(double StageTimeMinutes) : StageResult;

    public sealed record TruckIsTooFar() : StageResult;

    public sealed record StorageCantShip() : StageResult;

    public sealed record TruckCantLoad() : StageResult;
}

public interface IRouteStage
{
    StageResult DoStage(Truck truck);
}
