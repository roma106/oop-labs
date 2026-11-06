using Itmo.ObjectOrientedProgramming.Lab1.Location;
using Itmo.ObjectOrientedProgramming.Lab1.Product;
using Itmo.ObjectOrientedProgramming.Lab1.Storaging;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class StorageTests
{
    [Fact]
    public void SuccessRecieve()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        Storage.RecieveResult result = storage.Recieve(
            new List<UnitAmount> { chairs, lamp });

        Assert.IsType<Storage.RecieveResult.Success>(result);
        Assert.Equal(10, UnitAmount.CountTotalUnitsSize(storage.Stock));
        Assert.Equal(2, storage.Stock.Count);
        Assert.Equal(3, storage.Stock.Single(u => u.Sku.Id == 1).Quantity);
        Assert.Equal(1, storage.Stock.Single(u => u.Sku.Id == 2).Quantity);
    }

    [Fact]
    public void NotEnoughCapacityRecieve()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 2);

        Storage.RecieveResult result = storage.Recieve(
            new List<UnitAmount> { chairs, lamp });

        Assert.IsType<Storage.RecieveResult.NotEnoughCapacity>(result);
        Assert.Empty(storage.Stock);
    }

    [Fact]
    public void NoUnitsRecieve()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        Storage.RecieveResult result = storage.Recieve(
            new List<UnitAmount> { });

        Assert.IsType<Storage.RecieveResult.NoUnitsToRecieve>(result);
        Assert.Empty(storage.Stock);
    }

    [Fact]
    public void DuplicatedRecieve()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        Storage.RecieveResult result = storage.Recieve(
            new List<UnitAmount> { lamp, lamp, lamp });

        Storage.RecieveResult result2 = storage.Recieve(
            new List<UnitAmount> { lamp, lamp });

        Assert.IsType<Storage.RecieveResult.Success>(result);
        Assert.IsType<Storage.RecieveResult.Success>(result2);
        Assert.Equal(5, UnitAmount.CountTotalUnitsSize(storage.Stock));

        UnitAmount unit = Assert.Single(storage.Stock);
        Assert.Equal(2, unit.Sku.Id);
        Assert.Equal(5, unit.Quantity);
    }

    [Fact]
    public void NotEnoughCapacityAfterRecieve()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 2);

        storage.Recieve(new List<UnitAmount> { chairs });

        Storage.RecieveResult result = storage.Recieve(
            new List<UnitAmount> { lamp });

        Assert.IsType<Storage.RecieveResult.NotEnoughCapacity>(result);
        Assert.Equal(9, UnitAmount.CountTotalUnitsSize(storage.Stock));

        UnitAmount unit = Assert.Single(storage.Stock);
        Assert.Equal(3, unit.Quantity);
    }

    [Fact]
    public void FullShip()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        storage.Recieve(new List<UnitAmount> { chairs, lamp });

        Storage.ShipResult result = storage.Ship(
            new List<UnitAmount> { chairs, lamp });

        Assert.IsType<Storage.ShipResult.Success>(result);
        Assert.Empty(storage.Stock);
    }

    [Fact]
    public void PartShip()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        storage.Recieve(new List<UnitAmount> { chairs, lamp });

        Storage.ShipResult result = storage.Ship(
            new List<UnitAmount>
            {
                new UnitAmount(new Sku(1, "Office Chair", 3), 2),
            });

        Assert.IsType<Storage.ShipResult.Success>(result);
        Assert.Equal(4, UnitAmount.CountTotalUnitsSize(storage.Stock));
        Assert.Equal(2, storage.Stock.Count);
        Assert.Equal(1, storage.Stock.Single(u => u.Sku.Id == 1).Quantity);
        Assert.Equal(1, storage.Stock.Single(u => u.Sku.Id == 2).Quantity);
    }

    [Fact]
    public void ShipMoreThanStored()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        storage.Recieve(new List<UnitAmount> { chairs, lamp });

        Storage.ShipResult result = storage.Ship(
            new List<UnitAmount>
            {
                new UnitAmount(new Sku(2, "Table lamp", 1), 10),
            });

        Assert.IsType<Storage.ShipResult.InvalidQuantityForSomeUnits>(result);
        Assert.Equal(2, storage.Stock.Count);
        Assert.Equal(10, UnitAmount.CountTotalUnitsSize(storage.Stock));
        Assert.Equal(3, storage.Stock.Single(u => u.Sku.Id == 1).Quantity);
        Assert.Equal(1, storage.Stock.Single(u => u.Sku.Id == 2).Quantity);
    }

    [Fact]
    public void EmptyShip()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        storage.Recieve(new List<UnitAmount> { chairs, lamp });

        Storage.ShipResult result = storage.Ship(
            new List<UnitAmount> { });

        Assert.IsType<Storage.ShipResult.NoUnitsToShip>(result);
        Assert.Equal(2, storage.Stock.Count);
        Assert.Equal(10, UnitAmount.CountTotalUnitsSize(storage.Stock));
    }

    [Fact]
    public void AtomicShip()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        storage.Recieve(new List<UnitAmount> { chairs, lamp });

        Storage.ShipResult result = storage.Ship(
            new List<UnitAmount>
            {
                new UnitAmount(chairs.Sku, 2),
                new UnitAmount(lamp.Sku, 5),
            });

        Assert.IsType<Storage.ShipResult.InvalidQuantityForSomeUnits>(result);
        Assert.Equal(10, UnitAmount.CountTotalUnitsSize(storage.Stock));
        Assert.Equal(3, storage.Stock.Single(u => u.Sku.Id == 1).Quantity);
        Assert.Equal(1, storage.Stock.Single(u => u.Sku.Id == 2).Quantity);
    }

    [Fact]
    public void DuplicatedShip()
    {
        var storage = new Storage(
            1,
            10,
            new Coordinates(0, 0),
            [new Worker(4, 2, 2)]);

        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 5);

        storage.Recieve(new List<UnitAmount> { lamp });

        Storage.ShipResult result = storage.Ship(
            new List<UnitAmount>
            {
                new UnitAmount(lamp.Sku, 2),
                new UnitAmount(lamp.Sku, 2),
            });

        Assert.IsType<Storage.ShipResult.Success>(result);

        UnitAmount unit = Assert.Single(storage.Stock);
        Assert.Equal(2, unit.Sku.Id);
        Assert.Equal(1, unit.Quantity);
    }

    [Fact]
    public void ZeroCapacityStorage()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Storage(1, 0, new Coordinates(0, 0), [new Worker(4, 2, 2)]));
    }

    [Fact]
    public void NegativeIdStorage()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Storage(-1, 10, new Coordinates(0, 0), [new Worker(4, 2, 2)]));
    }
}
