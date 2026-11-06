using Itmo.ObjectOrientedProgramming.Lab1.Product;

namespace Itmo.ObjectOrientedProgramming.Lab1.Storaging;

public class Worker
{
    public double MaxSize { get; }

    public double LoadTime { get; }

    public double MoveTime { get; }

    public Worker(double maxSize, double loadTime, double moveTime)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxSize, nameof(maxSize));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(loadTime, nameof(loadTime));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(moveTime, nameof(moveTime));
        MaxSize = maxSize;
        LoadTime = loadTime;
        MoveTime = moveTime;
    }
}