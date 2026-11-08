namespace Itmo.ObjectOrientedProgramming.Lab1.Product;

public class Sku
{
    public int Id { get; }

    public string Name { get; }

    public double Size { get; } // m^3

    public Sku(int id, string name, double size)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size, nameof(size));
        Id = id;
        Name = name;
        Size = size;
    }
}