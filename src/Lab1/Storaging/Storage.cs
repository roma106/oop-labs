using Itmo.ObjectOrientedProgramming.Lab1.Location;
using Itmo.ObjectOrientedProgramming.Lab1.Product;

namespace Itmo.ObjectOrientedProgramming.Lab1.Storaging;

public class Storage
{
    public int Id { get; }

    private readonly List<UnitAmount> _stock;

    public IReadOnlyCollection<UnitAmount> Stock => _stock.ToArray();

    public Coordinates Coordinates { get; }

    public double Capacity { get; }

    private readonly List<Worker> _workers;

    public IReadOnlyCollection<Worker> Workers => _workers.ToArray();

    private void RemoveFromStock(UnitAmount unit)
    {
        UnitAmount? instock = _stock.Find(u => u.Sku.Id == unit.Sku.Id);
        if (instock != null)
        {
            if (instock.Quantity - unit.Quantity > 0)
            {
                _stock.Add(new UnitAmount(unit.Sku, instock.Quantity - unit.Quantity));
            }

            _stock.Remove(instock);
        }
    }

    public Storage(int id, double capacity, Coordinates coordinates, IReadOnlyCollection<Worker> workers)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity, nameof(capacity));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id, nameof(id));
        ArgumentNullException.ThrowIfNull(coordinates, nameof(coordinates));
        ArgumentNullException.ThrowIfNull(workers, nameof(workers));
        Id = id;
        Capacity = capacity;
        Coordinates = coordinates;
        _stock = new List<UnitAmount>();
        _workers = new List<Worker>(workers);
    }

    public abstract record RecieveResult
    {
        private RecieveResult() { }

        public sealed record Success(double RecieveTime) : RecieveResult;

        public sealed record WorkersFailedToRecieve() : RecieveResult;

        public sealed record NotEnoughCapacity() : RecieveResult;

        public sealed record NoUnitsToRecieve() : RecieveResult;
    }

    public RecieveResult CheckRecieve(IReadOnlyCollection<UnitAmount> units)
    {
        ArgumentNullException.ThrowIfNull(units, nameof(units));
        if (units.Count == 0)
        {
            return new RecieveResult.NoUnitsToRecieve();
        }

        if (UnitAmount.CountTotalUnitsSize(units) > (Capacity - UnitAmount.CountTotalUnitsSize(_stock)))
        {
            return new RecieveResult.NotEnoughCapacity();
        }

        Timing.RecieveWorkResult workResult = Timing.RecieveWork(units, _workers);
        if (workResult is not Timing.RecieveWorkResult.Success success)
        {
            return new RecieveResult.WorkersFailedToRecieve();
        }

        return new RecieveResult.Success(success.WorkTime);
    }

    public RecieveResult Recieve(IReadOnlyCollection<UnitAmount> units)
    {
        RecieveResult result = CheckRecieve(units);

        if (result is not RecieveResult.Success)
        {
            return result;
        }

        foreach (UnitAmount unit in units)
        {
            int instock = _stock.FindIndex(u => u.Sku.Id == unit.Sku.Id);
            if (instock == -1)
            {
                _stock.Add(unit);
            }
            else
            {
                _stock[instock] = new UnitAmount(unit.Sku, unit.Quantity + _stock[instock].Quantity);
            }
        }

        return result;
    }

    public abstract record ShipResult
    {
        private ShipResult() { }

        public sealed record Success(double ShipTime) : ShipResult;

        public sealed record WorkersFailedToShip() : ShipResult;

        public sealed record InvalidQuantityForSomeUnits() : ShipResult;

        public sealed record NoUnitsToShip() : ShipResult;
    }

    public ShipResult CheckShip(IReadOnlyCollection<UnitAmount> units)
    {
        ArgumentNullException.ThrowIfNull(units);

        if (units.Count == 0)
        {
            return new ShipResult.NoUnitsToShip();
        }

        if (!ValidUnitsToShip(units))
        {
            return new ShipResult.InvalidQuantityForSomeUnits();
        }

        Timing.ShipWorkResult workResult = Timing.ShipWork(units, _workers);

        if (workResult is not Timing.ShipWorkResult.Success success)
        {
            return new ShipResult.WorkersFailedToShip();
        }

        return new ShipResult.Success(success.WorkTime);
    }

    public ShipResult Ship(IReadOnlyCollection<UnitAmount> units)
    {
        ShipResult result = CheckShip(units);

        if (result is not ShipResult.Success)
        {
            return result;
        }

        foreach (UnitAmount unit in units)
        {
            RemoveFromStock(unit);
        }

        return result;
    }

    private bool ValidUnitsToShip(IEnumerable<UnitAmount> units)
    {
        var qtys = units.GroupBy(
            unit => unit.Sku.Id,
            unit => unit,
            (id, uni) => new
            {
                Id = id,
                Sum = uni.Sum(uni => uni.Quantity),
            });

        return qtys.All(qty =>
        {
            UnitAmount? instock = _stock.FirstOrDefault(unit => unit.Sku.Id == qty.Id);

            return instock != null && instock.Quantity >= qty.Sum;
        });
    }
}