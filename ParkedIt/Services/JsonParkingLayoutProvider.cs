using System.Text.Json;
using ParkedIt.Interfaces;
using ParkedIt.Models;
using ParkedIt.Utils;

namespace ParkedIt.Services;

/// <summary>
/// JSON file-based implementation of IParkingLayoutProvider.
/// WHY: Implements interface to load parking configuration from JSON files.
/// This can be easily replaced with a database provider later without changing dependent code.
/// </summary>
public class JsonParkingLayoutProvider : IParkingLayoutProvider
{
    private readonly string _configFilePath;

    /// <summary>
    /// Constructor that accepts path to JSON configuration file.
    /// WHY: Allows configuration file path to be specified, enabling different configs for different environments.
    /// </summary>
    public JsonParkingLayoutProvider(string configFilePath)
    {
        _configFilePath = configFilePath ?? throw new ArgumentNullException(nameof(configFilePath));
    }

    /// <summary>
    /// Loads parking lot structure from JSON configuration file.
    /// WHY: Deserializes JSON into domain models, mapping JSON structure to domain structure.
    /// </summary>
    public async Task<ParkingLot> LoadParkingLotAsync()
    {
        if (!File.Exists(_configFilePath))
        {
            throw new FileNotFoundException($"Configuration file not found: {_configFilePath}");
        }

        var jsonContent = await File.ReadAllTextAsync(_configFilePath);
        var config = JsonSerializer.Deserialize<ParkingConfigJson>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (config == null)
        {
            throw new InvalidOperationException("Failed to deserialize configuration file.");
        }

        // Map JSON models to domain models
        return MapToDomainModel(config);
    }

    /// <summary>
    /// Saves parking lot state back to JSON file.
    /// WHY: Persists spot availability changes (though in production, this would go to a database).
    /// </summary>
    public async Task SaveParkingLotAsync(ParkingLot parkingLot)
    {
        // In a real implementation, we would serialize the parking lot back to JSON
        // For now, we'll just note that this would update the configuration file
        // In production, spot availability would be stored in a database, not the config file
        await Task.CompletedTask;
    }

    /// <summary>
    /// Maps JSON configuration models to domain models.
    /// WHY: Separates data transfer objects (JSON) from domain models, enabling clean architecture.
    /// </summary>
    private ParkingLot MapToDomainModel(ParkingConfigJson config)
    {
        // Map institution
        var institution = new Institution
        {
            Id = config.Institution.Id,
            Name = config.Institution.Name,
            Type = ParseInstitutionType(config.Institution.Type),
            IsFreeParking = config.Institution.IsFreeParking,
            FreeMinutes = config.Institution.FreeMinutes,
            BaseRate = config.Institution.BaseRate,
            HourlyRate = config.Institution.HourlyRate,
            Rules = new ParkingRules
            {
                MaxParkingHours = config.Institution.Rules.MaxParkingHours,
                HasVipSpots = config.Institution.Rules.HasVipSpots,
                HasStaffSpots = config.Institution.Rules.HasStaffSpots,
                HasAccessibleSpots = config.Institution.Rules.HasAccessibleSpots,
                AllowedVehicleTypes = config.Institution.Rules.AllowedVehicleTypes
                    .Select(ParseVehicleType)
                    .ToList(),
                TimeBasedRules = config.Institution.Rules.TimeBasedRules
            }
        };

        // Map parking lot structure
        var parkingLot = new ParkingLot
        {
            Id = config.ParkingLot.Id,
            Name = config.ParkingLot.Name,
            Institution = institution,
            Floors = config.ParkingLot.Floors.Select(f => new Floor
            {
                Id = f.Id,
                Name = f.Name,
                IsEnabled = f.IsEnabled,
                Sections = f.Sections.Select(s => new Section
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsEnabled = s.IsEnabled,
                    Spots = s.Spots.Select(sp => new Spot
                    {
                        Id = sp.Id,
                        Type = ParseSpotType(sp.Type),
                        Status = ParseAvailabilityStatus(sp.Status),
                        IsEnabled = sp.IsEnabled,
                        AllowedVehicleTypes = sp.AllowedVehicleTypes
                            .Select(ParseVehicleType)
                            .ToList()
                    }).ToList()
                }).ToList()
            }).ToList()
        };

        return parkingLot;
    }

    /// <summary>
    /// Parses institution type string to enum.
    /// WHY: Handles string-to-enum conversion with error handling.
    /// </summary>
    private InstitutionType ParseInstitutionType(string type)
    {
        return Enum.TryParse<InstitutionType>(type, true, out var result)
            ? result
            : throw new ArgumentException($"Invalid institution type: {type}");
    }

    /// <summary>
    /// Parses vehicle type string to enum.
    /// </summary>
    private VehicleType ParseVehicleType(string type)
    {
        return Enum.TryParse<VehicleType>(type, true, out var result)
            ? result
            : throw new ArgumentException($"Invalid vehicle type: {type}");
    }

    /// <summary>
    /// Parses spot type string to enum.
    /// </summary>
    private SpotType ParseSpotType(string type)
    {
        return Enum.TryParse<SpotType>(type, true, out var result)
            ? result
            : throw new ArgumentException($"Invalid spot type: {type}");
    }

    /// <summary>
    /// Parses availability status string to enum.
    /// </summary>
    private AvailabilityStatus ParseAvailabilityStatus(string status)
    {
        return Enum.TryParse<AvailabilityStatus>(status, true, out var result)
            ? result
            : throw new ArgumentException($"Invalid availability status: {status}");
    }
}

