using Itmo.ObjectOrientedProgramming.Lab1.Transport;

namespace Itmo.ObjectOrientedProgramming.Lab1.Routing;

public class RouteSheet
{
    public IReadOnlyList<IRouteStage> Stages { get; }

    public RouteSheet(IReadOnlyCollection<IRouteStage> stages)
    {
        Stages = stages.ToList().AsReadOnly();
    }

    public abstract record RouteSheetResult
    {
        private RouteSheetResult() { }

        public sealed record Success(double Time) : RouteSheetResult;

        public sealed record Err(StageResult Reason) : RouteSheetResult;
    }

    public RouteSheetResult DoRouteSheet(Truck truck)
    {
        ArgumentNullException.ThrowIfNull(truck);
        double overallTime = 0;
        foreach (IRouteStage stage in Stages)
        {
            StageResult result = stage.DoStage(truck);
            if (result is not StageResult.Success success)
            {
                return new RouteSheetResult.Err(result);
            }

            overallTime += success.StageTimeMinutes;
        }

        return new RouteSheetResult.Success(overallTime);
    }
}