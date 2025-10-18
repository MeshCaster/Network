namespace Core.Domain.ValueObjects;

/// <summary>
/// Value object for geographic coordinates
/// </summary>
public record Location
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double? Altitude { get; init; }
    
    public Location(double latitude, double longitude, double? altitude = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
        
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));
        
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
    }
    
    /// <summary>
    /// Calculate distance to another location in meters (Haversine formula)
    /// </summary>
    public double DistanceTo(Location other)
    {
        const double earthRadius = 6371000; // meters
        
        var lat1Rad = Latitude * Math.PI / 180;
        var lat2Rad = other.Latitude * Math.PI / 180;
        var deltaLat = (other.Latitude - Latitude) * Math.PI / 180;
        var deltaLon = (other.Longitude - Longitude) * Math.PI / 180;
        
        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return earthRadius * c;
    }
}