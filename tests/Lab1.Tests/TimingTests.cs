using Itmo.ObjectOrientedProgramming.Lab1.Product;
using Itmo.ObjectOrientedProgramming.Lab1.Storaging;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class TimingTests
{
    [Fact]
    public void SuccessShipWork()
    {
        var worker = new Worker(10, 3, 2);
        var chairs = new UnitAmount(new Sku(1, "Chair", 6), 3);

        Timing.ShipWorkResult result = Timing.ShipWork(
            new List<UnitAmount> { chairs },
            new List<Worker> { worker });

        Timing.ShipWorkResult.Success success =
            Assert.IsType<Timing.ShipWorkResult.Success>(result);

        Assert.Equal(19, success.WorkTime);
    }

    [Fact]
    public void SuccessRecieveWork()
    {
        var worker = new Worker(10, 3, 2);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 3);

        Timing.RecieveWorkResult result = Timing.RecieveWork([chairs], [worker]);

        Timing.RecieveWorkResult.Success success =
            Assert.IsType<Timing.RecieveWorkResult.Success>(result);
        Assert.Equal(17.0, success.WorkTime);
    }

    [Fact]
    public void ParallelShipWork()
    {
        var firstWorker = new Worker(6, 3, 2);
        var secondWorker = new Worker(6, 3, 2);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 2);

        Timing.ShipWorkResult result = Timing.ShipWork(
            [chairs], [firstWorker, secondWorker]);

        Timing.ShipWorkResult.Success success =
            Assert.IsType<Timing.ShipWorkResult.Success>(result);
        Assert.Equal(5.0, success.WorkTime);
    }

    [Fact]
    public void NoWorkersRecieveWork()
    {
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 1);

        Timing.RecieveWorkResult result = Timing.RecieveWork([chairs], []);

        Assert.IsType<Timing.RecieveWorkResult.NoWorkers>(result);
    }

    [Fact]
    public void NoWorkersShipWork()
    {
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 1);

        Timing.ShipWorkResult result = Timing.ShipWork([chairs], []);

        Assert.IsType<Timing.ShipWorkResult.NoWorkers>(result);
    }

    [Fact]
    public void TooBigShipWork()
    {
        var worker = new Worker(5, 3, 2);
        var chairs = new UnitAmount(new Sku(1, "Office Chair", 6), 1);

        Timing.ShipWorkResult result = Timing.ShipWork([chairs], [worker]);

        Assert.IsType<Timing.ShipWorkResult.SomeUnitsAreTooBigForAllWorkers>(result);
    }

    [Fact]
    public void EmptyShipWork()
    {
        Timing.ShipWorkResult result = Timing.ShipWork([], []);

        Timing.ShipWorkResult.Success success =
            Assert.IsType<Timing.ShipWorkResult.Success>(result);
        Assert.Equal(0.0, success.WorkTime);
    }
}