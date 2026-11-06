namespace Itmo.ObjectOrientedProgramming.Lab1.Location;

public class Coordinates
{
    public decimal Longitude { get; }

    public decimal Latitude { get; }

    public Coordinates(decimal longitude, decimal latitude)
    {
        if (longitude < -180.0m || longitude > 180.0m)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude), "longitude is invalid");
        }

        if (latitude < -90.0m || latitude > 90.0m)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "latitude is invalid");
        }

        Longitude = longitude;
        Latitude = latitude;
    }

    public double DistanceKm(Coordinates destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        const double earthRadiusKm = 6371.0;

        // radians
        double startLatitude = (double)Latitude * Math.PI / 180.0;
        double startLongitude = (double)Longitude * Math.PI / 180.0;

        double endLatitude = (double)destination.Latitude * Math.PI / 180.0;
        double endLongitude = (double)destination.Longitude * Math.PI / 180.0;

        double latitudeDifference = endLatitude - startLatitude;
        double longitudeDifference = endLongitude - startLongitude;

        double sinLatitude = Math.Sin(latitudeDifference / 2.0);
        double sinLongitude = Math.Sin(longitudeDifference / 2.0);

        double a = (sinLatitude * sinLatitude) +
                   (Math.Cos(startLatitude) * Math.Cos(endLatitude) * sinLongitude * sinLongitude);

        a = Math.Clamp(a, 0.0, 1.0);

        double centralAngle = 2.0 * Math.Atan2(
            Math.Sqrt(a),
            Math.Sqrt(1.0 - a));

        double distanceKm = earthRadiusKm * centralAngle;

        return distanceKm;
    }
}