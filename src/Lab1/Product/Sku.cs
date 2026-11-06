namespace Itmo.ObjectOrientedProgramming.Lab1.Product;

public class Sku
{
    public int Id { get; }

    public string Name { get; }

    public double Size { get; } // m^3

    public Sku(int id, string name, double size)
    {
        Id = id;
        Name = name;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size, nameof(size));
        Size = size;
    }
}