using Itmo.ObjectOrientedProgramming.Lab1.Location;
using Itmo.ObjectOrientedProgramming.Lab1.Product;

namespace Itmo.ObjectOrientedProgramming.Lab1.Transport;

public class Truck
{
    public double Capacity { get; } // m^3

    public double Speed { get; }

    public Coordinates Coordinates { get; private set; }

    private readonly List<UnitAmount> _loadedUnits;

    private void AddToLoaded(UnitAmount unit)
    {
        int loaded = _loadedUnits.FindIndex(u => u.Sku.Id == unit.Sku.Id);
        if (loaded == -1)
        {
            _loadedUnits.Add(unit);
        }
        else
        {
            _loadedUnits[loaded] = new UnitAmount(unit.Sku, unit.Quantity + _loadedUnits[loaded].Quantity);
        }
    }

    private void RemoveFromLoaded(UnitAmount unit)
    {
        UnitAmount? loaded = _loadedUnits.Find(u => u.Sku.Id == unit.Sku.Id);
        if (loaded != null)
        {
            if (loaded.Quantity - unit.Quantity > 0)
            {
                _loadedUnits.Add(new UnitAmount(unit.Sku, loaded.Quantity - unit.Quantity));
            }

            _loadedUnits.Remove(loaded);
        }
    }

    public IReadOnlyCollection<UnitAmount> LoadedUnits => _loadedUnits.ToArray();

    public Truck(double cap, double speed, Coordinates coordinates)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cap, nameof(cap));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(speed, nameof(speed));
        ArgumentNullException.ThrowIfNull(coordinates);

        Capacity = cap;
        Speed = speed;
        Coordinates = coordinates;
        _loadedUnits = new List<UnitAmount>();
    }

    public abstract record LoadResult
    {
        private LoadResult() { }

        public sealed record Success() : LoadResult;

        public sealed record NotEnoughCapacity() : LoadResult;

        public sealed record NoUnitsToLoad() : LoadResult;
    }

    public LoadResult CheckLoad(IReadOnlyCollection<UnitAmount> units)
    {
        ArgumentNullException.ThrowIfNull(units);

        if (units.Count == 0)
        {
            return new LoadResult.NoUnitsToLoad();
        }

        if (UnitAmount.CountTotalUnitsSize(units) > Capacity - UnitAmount.CountTotalUnitsSize(_loadedUnits))
        {
            return new LoadResult.NotEnoughCapacity();
        }

        return new LoadResult.Success();
    }

    public LoadResult Load(IReadOnlyCollection<UnitAmount> units)
    {
        LoadResult result = CheckLoad(units);

        if (result is not LoadResult.Success)
        {
            return result;
        }

        foreach (UnitAmount unit in units)
        {
            AddToLoaded(unit);
        }

        return new LoadResult.Success();
    }

    public abstract record UnloadResult
    {
        private UnloadResult() { }

        public sealed record Success() : UnloadResult;

        public sealed record InvalidQuantityForSomeUnits() : UnloadResult;

        public sealed record NoUnitsToUnload() : UnloadResult;
    }

    public UnloadResult CheckUnload(IReadOnlyCollection<UnitAmount> units)
    {
        ArgumentNullException.ThrowIfNull(units);

        if (units.Count == 0)
        {
            return new UnloadResult.NoUnitsToUnload();
        }

        if (!ValidUnitsToUnload(units))
        {
            return new UnloadResult.InvalidQuantityForSomeUnits();
        }

        return new UnloadResult.Success();
    }

    public UnloadResult Unload(IReadOnlyCollection<UnitAmount> units)
    {
        UnloadResult result = CheckUnload(units);
        if (result is not UnloadResult.Success)
        {
            return result;
        }

        foreach (UnitAmount unit in units)
        {
            RemoveFromLoaded(unit);
        }

        return result;
    }

    public double MoveTo(Coordinates dest)
    {
        double time = Coordinates.DistanceKm(dest) / Speed;
        Coordinates = dest;
        return time;
    }

    private bool ValidUnitsToUnload(IEnumerable<UnitAmount> units)
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
            UnitAmount? loaded = _loadedUnits.FirstOrDefault(unit => unit.Sku.Id == qty.Id);

            return loaded != null && loaded.Quantity >= qty.Sum;
        });
    }
}