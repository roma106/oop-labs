using Itmo.ObjectOrientedProgramming.Lab1.Location;
using Itmo.ObjectOrientedProgramming.Lab1.Product;
using Itmo.ObjectOrientedProgramming.Lab1.Transport;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class TruckTests
{
    [Fact]
    public void SuccessLoad()
    {
        var truck = new Truck(10, 50, new Coordinates(0, 0));
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        Truck.LoadResult result = truck.Load(new List<UnitAmount> { chairs, lamp });

        Assert.IsType<Truck.LoadResult.Success>(result);
        Assert.Equal(10, UnitAmount.CountTotalUnitsSize(truck.LoadedUnits));
        Assert.Equal(2, truck.LoadedUnits.Count);
        Assert.Equal(1, truck.LoadedUnits.ElementAt(0).Sku.Id);
        Assert.Equal(2, truck.LoadedUnits.ElementAt(1).Sku.Id);
    }

    [Fact]
    public void NotEnoughCapacityLoad()
    {
        var truck = new Truck(10, 50, new Coordinates(0, 0));
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 2);

        Truck.LoadResult result = truck.Load(new List<UnitAmount> { chairs, lamp });

        Assert.IsType<Truck.LoadResult.NotEnoughCapacity>(result);
        Assert.Equal(0, UnitAmount.CountTotalUnitsSize(truck.LoadedUnits));
    }

    [Fact]
    public void NoUnitsLoad()
    {
        var truck = new Truck(10, 50, new Coordinates(0, 0));

        Truck.LoadResult result = truck.Load(new List<UnitAmount> { });

        Assert.IsType<Truck.LoadResult.NoUnitsToLoad>(result);
        Assert.Equal(0, UnitAmount.CountTotalUnitsSize(truck.LoadedUnits));
    }

    [Fact]
    public void DuplicatedLoad()
    {
        var truck = new Truck(10, 50, new Coordinates(0, 0));
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        Truck.LoadResult result = truck.Load(new List<UnitAmount> { lamp, lamp, lamp });
        Truck.LoadResult result2 = truck.Load(new List<UnitAmount> { lamp, lamp });

        Assert.IsType<Truck.LoadResult.Success>(result);
        Assert.IsType<Truck.LoadResult.Success>(result2);
        Assert.Equal(5, UnitAmount.CountTotalUnitsSize(truck.LoadedUnits));
        UnitAmount unit = Assert.Single(truck.LoadedUnits);
        Assert.Equal(2, unit.Sku.Id);
        Assert.Equal(5, unit.Quantity);
    }

    [Fact]
    public void FullUnload()
    {
        var truck = new Truck(10, 50, new Coordinates(0, 0));
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        truck.Load(new List<UnitAmount> { chairs, lamp });
        Truck.UnloadResult result = truck.Unload(new List<UnitAmount> { chairs, lamp });

        Assert.IsType<Truck.UnloadResult.Success>(result);
        Assert.Empty(truck.LoadedUnits);
    }

    [Fact]
    public void PartUnload()
    {
        var truck = new Truck(10, 50, new Coordinates(0, 0));
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        truck.Load(new List<UnitAmount> { chairs, lamp });
        Truck.UnloadResult result = truck.Unload(
            new List<UnitAmount>
            {
                new UnitAmount(new Sku(1, "Office Chair", 3), 2),
            });

        Assert.IsType<Truck.UnloadResult.Success>(result);
        Assert.Equal(4, UnitAmount.CountTotalUnitsSize(truck.LoadedUnits));
        Assert.Equal(2, truck.LoadedUnits.Count);
    }

    [Fact]
    public void UnloadMoreThanLoaded()
    {
        var truck = new Truck(10, 50, new Coordinates(0, 0));
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table Lamp", 1), 1);

        truck.Load(new List<UnitAmount> { chairs, lamp });
        Truck.UnloadResult result = truck.Unload(new List<UnitAmount>
        {
            new UnitAmount(new Sku(2, "Table Lamp", 1), 10),
        });

        Assert.IsType<Truck.UnloadResult.InvalidQuantityForSomeUnits>(result);
        Assert.Equal(2, truck.LoadedUnits.Count);
        Assert.Equal(3, truck.LoadedUnits.ElementAt(0).Quantity);
        Assert.Equal(1, truck.LoadedUnits.ElementAt(1).Quantity);
    }

    [Fact]
    public void EmptyUnload()
    {
        var truck = new Truck(10, 50, new Coordinates(0, 0));
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 3), 3);
        var lamp = new UnitAmount(new Sku(2, "Table lamp", 1), 1);

        truck.Load(new List<UnitAmount> { chairs, lamp });
        Truck.UnloadResult result = truck.Unload(new List<UnitAmount> { });

        Assert.IsType<Truck.UnloadResult.NoUnitsToUnload>(result);
        Assert.Equal(2, truck.LoadedUnits.Count);
    }

    [Fact]
    public void ZeroSpeedTruck()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Truck(10, 0, new Coordinates(0, 0)));
    }

    [Fact]
    public void NegativeCapacityTruck()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Truck(-100, 60, new Coordinates(0, 0)));
    }
}