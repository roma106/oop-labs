using Itmo.ObjectOrientedProgramming.Lab1.Location;
using Itmo.ObjectOrientedProgramming.Lab1.Routing;
using Itmo.ObjectOrientedProgramming.Lab1.Transport;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class RoutingTests
{
    [Fact]
    public void SuccessRouteSheet()
    {
        var start = new Coordinates(0, 0);
        var middle = new Coordinates(0, 1);
        var finish = new Coordinates(0, 2);

        var truck = new Truck(10, 60, start);

        var routeSheet = new RouteSheet(
            new List<IRouteStage>
            {
                new MoveStage(middle),
                new MoveStage(finish),
            });

        RouteSheet.RouteSheetResult result =
            routeSheet.DoRouteSheet(truck);

        RouteSheet.RouteSheetResult.Success success = Assert.IsType<RouteSheet.RouteSheetResult.Success>(result);

        double expectedTime = (
            start.DistanceKm(middle) +
            middle.DistanceKm(finish)) / truck.Speed * 60;

        Assert.Equal(expectedTime, success.Time, 6);
        Assert.Same(finish, truck.Coordinates);
    }
}