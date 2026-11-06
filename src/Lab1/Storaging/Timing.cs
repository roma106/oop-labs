using Itmo.ObjectOrientedProgramming.Lab1.Product;

namespace Itmo.ObjectOrientedProgramming.Lab1.Storaging;

public class Timing
{
    private class WorkerProgress
    {
        public Worker Worker { get; }

        public double AvailableSize { get; private set; }

        public int Trips { get; private set; }

        public WorkerProgress(Worker worker)
        {
            Worker = worker;
            AvailableSize = worker.MaxSize;
            Trips = 0;
        }

        public void AddUnit(double size)
        {
            if (Trips == 0 || AvailableSize < size)
            {
                Trips++;
                AvailableSize = Worker.MaxSize;
            }

            AvailableSize -= size;
        }

        public double GetReceiveTime()
        {
            if (Trips == 0)
            {
                return 0;
            }

            return (Trips * Worker.LoadTime) + (2 * (Trips - 1) * Worker.MoveTime);
        }

        public double PredictReceiveTime(double size)
        {
            int predictedTrips = Trips;

            if (Trips == 0 || AvailableSize < size)
            {
                predictedTrips++;
            }

            if (predictedTrips == 0)
            {
                return 0;
            }

            return (predictedTrips * Worker.LoadTime) + (2 * (predictedTrips - 1) * Worker.MoveTime);
        }

        public double GetShipTime()
        {
            if (Trips == 0)
            {
                return 0;
            }

            return (Trips * Worker.LoadTime) + (((2 * Trips) - 1) * Worker.MoveTime);
        }

        public double PredictShipTime(double size)
        {
            int predictedTrips = Trips;

            if (Trips == 0 || AvailableSize < size)
            {
                predictedTrips++;
            }

            return (predictedTrips * Worker.LoadTime) + (((2 * predictedTrips) - 1) * Worker.MoveTime);
        }
    }

    public abstract record RecieveWorkResult
    {
        private RecieveWorkResult() { }

        public sealed record Success(double WorkTime) : RecieveWorkResult;

        public sealed record NoWorkers() : RecieveWorkResult;

        public sealed record SomeUnitsAreTooBigForAllWorkers() : RecieveWorkResult;
    }

    public static RecieveWorkResult RecieveWork(IReadOnlyCollection<UnitAmount> units, IReadOnlyCollection<Worker> workers)
    {
        ArgumentNullException.ThrowIfNull(units);
        ArgumentNullException.ThrowIfNull(workers);

        if (units.Count == 0)
        {
            return new RecieveWorkResult.Success(0);
        }

        if (workers.Count == 0)
        {
            return new RecieveWorkResult.NoWorkers();
        }

        var plans = workers
            .Select(worker => new WorkerProgress(worker))
            .ToList();

        var unitsMap = units
            .GroupBy(unit => unit.Sku.Id)
            .Select(group => new UnitAmount(
                group.First().Sku,
                group.Sum(unit => unit.Quantity))).OrderByDescending(u => u.Sku.Size).ToList();

        foreach (UnitAmount unit in unitsMap)
        {
            for (int i = 0; i < unit.Quantity; i++)
            {
                var validPlans = plans
                    .Where(plan => plan.Worker.MaxSize >= unit.Sku.Size)
                    .ToList();

                if (validPlans.Count == 0)
                {
                    return new RecieveWorkResult.SomeUnitsAreTooBigForAllWorkers();
                }

                WorkerProgress selected = validPlans
                    .OrderBy(plan => plan.PredictReceiveTime(unit.Sku.Size))
                    .First();

                selected.AddUnit(unit.Sku.Size);
            }
        }

        double totalTime = plans.Max(plan => plan.GetReceiveTime());

        return new RecieveWorkResult.Success(totalTime);
    }

    public abstract record ShipWorkResult
    {
        private ShipWorkResult() { }

        public sealed record Success(double WorkTime) : ShipWorkResult;

        public sealed record NoWorkers() : ShipWorkResult;

        public sealed record SomeUnitsAreTooBigForAllWorkers() : ShipWorkResult;
    }

    public static ShipWorkResult ShipWork(
        IReadOnlyCollection<UnitAmount> units,
        IReadOnlyCollection<Worker> workers)
    {
        ArgumentNullException.ThrowIfNull(units);
        ArgumentNullException.ThrowIfNull(workers);

        if (units.Count == 0)
        {
            return new ShipWorkResult.Success(0);
        }

        if (workers.Count == 0)
        {
            return new ShipWorkResult.NoWorkers();
        }

        var unitsMap = units
            .GroupBy(unit => unit.Sku.Id)
            .Select(group => new UnitAmount(
                group.First().Sku,
                group.Sum(unit => unit.Quantity)))
            .OrderByDescending(unit => unit.Sku.Size)
            .ToList();

        var plans = workers
            .Select(worker => new WorkerProgress(worker))
            .ToList();

        foreach (UnitAmount unit in unitsMap)
        {
            for (int i = 0; i < unit.Quantity; i++)
            {
                var validPlans = plans
                    .Where(plan => plan.Worker.MaxSize >= unit.Sku.Size)
                    .ToList();

                if (validPlans.Count == 0)
                {
                    return new ShipWorkResult.SomeUnitsAreTooBigForAllWorkers();
                }

                WorkerProgress selected = validPlans
                    .OrderBy(plan => plan.PredictShipTime(unit.Sku.Size))
                    .First();

                selected.AddUnit(unit.Sku.Size);
            }
        }

        double totalTime = plans.Max(plan => plan.GetShipTime());

        return new ShipWorkResult.Success(totalTime);
    }
}