using Itmo.ObjectOrientedProgramming.Lab1.Location;
using Itmo.ObjectOrientedProgramming.Lab1.Transport;

namespace Itmo.ObjectOrientedProgramming.Lab1.Routing;

public class MoveStage : IRouteStage
{
    public Coordinates Destination { get; }

    public MoveStage(Coordinates destination)
    {
        ArgumentNullException.ThrowIfNull(destination, nameof(destination));

        Destination = destination;
    }

    public StageResult DoStage(Truck truck)
    {
        ArgumentNullException.ThrowIfNull(truck, nameof(truck));
        double moveTimeHours = truck.MoveTo(Destination);

        return new StageResult.Success(moveTimeHours * 60);
    }
}