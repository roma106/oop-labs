namespace Itmo.ObjectOrientedProgramming.Lab1.Product;

public class UnitAmount
{
    public Sku Sku { get; }

    public int Quantity { get; }

    public UnitAmount(Sku sku, int quantity)
    {
        ArgumentNullException.ThrowIfNull(sku, nameof(sku));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
        Sku = sku;
        Quantity = quantity;
    }

    public static double CountTotalUnitsSize(IEnumerable<UnitAmount> units)
    {
        return units.Sum(sku => sku.Sku.Size * sku.Quantity);
    }
}