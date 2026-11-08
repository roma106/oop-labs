using Itmo.ObjectOrientedProgramming.Lab1.Location;
using Itmo.ObjectOrientedProgramming.Lab1.Product;
using Itmo.ObjectOrientedProgramming.Lab1.Routing;
using Itmo.ObjectOrientedProgramming.Lab1.Storaging;
using Itmo.ObjectOrientedProgramming.Lab1.Transport;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class RouteSheetTests
{
    [Fact]
    public void SuccessRouteSheet()
    {
        var start = new Coordinates(0, 0);
        var finish = new Coordinates(0, 1);
        var worker = new Worker(10, 3, 2);
        var firstStorage = new Storage(1, 100, start, [worker]);
        var secondStorage = new Storage(2, 100, finish, [worker]);
        var truck = new Truck(100, 60, start);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        firstStorage.Recieve([chairs]);
        var routeSheet = new RouteSheet(
            new List<IRouteStage>
            {
                new ShipStage(firstStorage, [chairs]),
                new MoveStage(finish),
                new RecieveStage(secondStorage, [chairs]),
            });

        RouteSheet.RouteSheetResult result = routeSheet.DoRouteSheet(truck);

        RouteSheet.RouteSheetResult.Success success =
            Assert.IsType<RouteSheet.RouteSheetResult.Success>(result);
        double expectedTime = 19 + (start.DistanceKm(finish) / truck.Speed * 60) + 17;
        Assert.Equal(expectedTime, success.Time, 6);
        Assert.Same(finish, truck.Coordinates);
        Assert.Empty(firstStorage.Stock);
        Assert.Empty(truck.LoadedUnits);
        Assert.Equal(3, Assert.Single(secondStorage.Stock).Quantity);
    }

    [Fact]
    public void FailedRouteSheetStopsNextStages()
    {
        var start = new Coordinates(0, 0);
        var middle = new Coordinates(0, 1);
        var finish = new Coordinates(0, 2);
        var storage = new Storage(1, 100, start, [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, start);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 1);
        storage.Recieve([chairs]);
        var routeSheet = new RouteSheet(
            new List<IRouteStage>
            {
                new MoveStage(middle),
                new ShipStage(storage, [chairs]),
                new MoveStage(finish),
            });

        RouteSheet.RouteSheetResult result = routeSheet.DoRouteSheet(truck);

        RouteSheet.RouteSheetResult.Err error = Assert.IsType<RouteSheet.RouteSheetResult.Err>(result);
        Assert.IsType<StageResult.TruckIsTooFar>(error.Reason);
        Assert.Same(middle, truck.Coordinates);
        Assert.Equal(1, Assert.Single(storage.Stock).Quantity);
        Assert.Empty(truck.LoadedUnits);
    }

    [Fact]
    public void TwoMoveStagesRouteSheet()
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

        RouteSheet.RouteSheetResult result = routeSheet.DoRouteSheet(truck);

        RouteSheet.RouteSheetResult.Success success =
            Assert.IsType<RouteSheet.RouteSheetResult.Success>(result);
        double expectedTime = (start.DistanceKm(middle) + middle.DistanceKm(finish)) / truck.Speed * 60;
        Assert.Equal(expectedTime, success.Time, 6);
        Assert.Same(finish, truck.Coordinates);
    }

    [Fact]
    public void EmptyRouteSheet()
    {
        var coordinates = new Coordinates(0, 0);
        var truck = new Truck(10, 60, coordinates);
        var routeSheet = new RouteSheet(new List<IRouteStage>());

        RouteSheet.RouteSheetResult result = routeSheet.DoRouteSheet(truck);

        RouteSheet.RouteSheetResult.Success success =
            Assert.IsType<RouteSheet.RouteSheetResult.Success>(result);
        Assert.Equal(0.0, success.Time);
        Assert.Same(coordinates, truck.Coordinates);
    }
}
