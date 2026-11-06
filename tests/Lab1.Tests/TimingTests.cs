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
}