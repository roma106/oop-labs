using Itmo.ObjectOrientedProgramming.Lab1.Location;
using Itmo.ObjectOrientedProgramming.Lab1.Product;
using Itmo.ObjectOrientedProgramming.Lab1.Routing;
using Itmo.ObjectOrientedProgramming.Lab1.Storaging;
using Itmo.ObjectOrientedProgramming.Lab1.Transport;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class RoutingStageTests
{
    [Fact]
    public void SuccessShipStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 100, coordinates, [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        storage.Recieve([chairs]);
        var stage = new ShipStage(storage, [chairs]);

        StageResult result = stage.DoStage(truck);

        StageResult.Success success = Assert.IsType<StageResult.Success>(result);
        Assert.Equal(19.0, success.StageTimeMinutes);
        Assert.Empty(storage.Stock);
        Assert.Equal(3, Assert.Single(truck.LoadedUnits).Quantity);
    }

    [Fact]
    public void PartShipStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 100, coordinates, [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        storage.Recieve([chairs]);
        var stage = new ShipStage(storage, [new UnitAmount(chairs.Sku, 2)]);

        StageResult result = stage.DoStage(truck);

        StageResult.Success success = Assert.IsType<StageResult.Success>(result);
        Assert.Equal(12.0, success.StageTimeMinutes);
        Assert.Equal(1, Assert.Single(storage.Stock).Quantity);
        Assert.Equal(2, Assert.Single(truck.LoadedUnits).Quantity);
    }

    [Fact]
    public void SuccessRecieveStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 100, coordinates, [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        truck.Load([chairs]);
        var stage = new RecieveStage(storage, [chairs]);

        StageResult result = stage.DoStage(truck);

        StageResult.Success success = Assert.IsType<StageResult.Success>(result);
        Assert.Equal(17.0, success.StageTimeMinutes);
        Assert.Empty(truck.LoadedUnits);
        Assert.Equal(3, Assert.Single(storage.Stock).Quantity);
    }

    [Fact]
    public void PartRecieveStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 100, coordinates, [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        truck.Load([chairs]);
        var stage = new RecieveStage(storage, [new UnitAmount(chairs.Sku, 2)]);

        StageResult result = stage.DoStage(truck);

        StageResult.Success success = Assert.IsType<StageResult.Success>(result);
        Assert.Equal(10.0, success.StageTimeMinutes);
        Assert.Equal(1, Assert.Single(truck.LoadedUnits).Quantity);
        Assert.Equal(2, Assert.Single(storage.Stock).Quantity);
    }

    [Fact]
    public void TooFarShipStage()
    {
        var storage = new Storage(1, 100, new Coordinates(0, 0), [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, new Coordinates(0, 0.0001m));
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        storage.Recieve([chairs]);
        var stage = new ShipStage(storage, [chairs]);

        StageResult result = stage.DoStage(truck);

        Assert.IsType<StageResult.TruckIsTooFar>(result);
        Assert.Equal(3, Assert.Single(storage.Stock).Quantity);
        Assert.Empty(truck.LoadedUnits);
    }

    [Fact]
    public void TooFarRecieveStage()
    {
        var storage = new Storage(1, 100, new Coordinates(0, 0), [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, new Coordinates(0, 0.0001m));
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        truck.Load([chairs]);
        var stage = new RecieveStage(storage, [chairs]);

        StageResult result = stage.DoStage(truck);

        Assert.IsType<StageResult.TruckIsTooFar>(result);
        Assert.Equal(3, Assert.Single(truck.LoadedUnits).Quantity);
        Assert.Empty(storage.Stock);
    }

    [Fact]
    public void TruckCantLoadStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 100, coordinates, [new Worker(10, 3, 2)]);
        var truck = new Truck(10, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        storage.Recieve([chairs]);
        var stage = new ShipStage(storage, [chairs]);

        StageResult result = stage.DoStage(truck);

        Assert.IsType<StageResult.TruckCantLoad>(result);
        Assert.Equal(3, Assert.Single(storage.Stock).Quantity);
        Assert.Empty(truck.LoadedUnits);
    }

    [Fact]
    public void StorageCantShipStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 100, coordinates, [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        var stage = new ShipStage(storage, [chairs]);

        StageResult result = stage.DoStage(truck);

        Assert.IsType<StageResult.StorageCantShip>(result);
        Assert.Empty(storage.Stock);
        Assert.Empty(truck.LoadedUnits);
    }

    [Fact]
    public void StorageCantRecieveStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 10, coordinates, [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);
        truck.Load([chairs]);
        var stage = new RecieveStage(storage, [chairs]);

        StageResult result = stage.DoStage(truck);

        Assert.IsType<StageResult.StorageCantRecieve>(result);
        Assert.Empty(storage.Stock);
        Assert.Equal(3, Assert.Single(truck.LoadedUnits).Quantity);
    }

    [Fact]
    public void TruckCantUnloadStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 100, coordinates, [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 1);
        var stage = new RecieveStage(storage, [chairs]);

        StageResult result = stage.DoStage(truck);

        Assert.IsType<StageResult.TruckCantUnload>(result);
        Assert.Empty(storage.Stock);
        Assert.Empty(truck.LoadedUnits);
    }

    [Fact]
    public void WorkersCantRecieveStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 100, coordinates, [new Worker(5, 3, 2)]);
        var truck = new Truck(100, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 1);
        truck.Load([chairs]);
        var stage = new RecieveStage(storage, [chairs]);

        StageResult result = stage.DoStage(truck);

        Assert.IsType<StageResult.StorageCantRecieve>(result);
        Assert.Empty(storage.Stock);
        Assert.Equal(1, Assert.Single(truck.LoadedUnits).Quantity);
    }

    [Fact]
    public void InvalidManifestShipStage()
    {
        var coordinates = new Coordinates(0, 0);
        var storage = new Storage(1, 100, coordinates, [new Worker(10, 3, 2)]);
        var truck = new Truck(100, 60, coordinates);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 2);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);
        storage.Recieve([chairs, lamp]);
        var stage = new ShipStage(storage, [chairs, new UnitAmount(lamp.Sku, 5)]);

        StageResult result = stage.DoStage(truck);

        Assert.IsType<StageResult.StorageCantShip>(result);
        Assert.Equal(2, storage.Stock.Single(unit => unit.Sku.Id == 1).Quantity);
        Assert.Equal(1, storage.Stock.Single(unit => unit.Sku.Id == 2).Quantity);
        Assert.Empty(truck.LoadedUnits);
    }
}
