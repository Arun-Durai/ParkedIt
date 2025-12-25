using ParkedIt.Interfaces;
using ParkedIt.Models;
using ParkedIt.Services;

namespace ParkedIt;

/// <summary>
/// Main entry point for the Parking Management System console application.
/// WHY: Demonstrates the system in action with a simple console UI for testing entry/exit flows.
/// </summary>
class Program
{
    private static IParkingManager? _parkingManager;
    private static Dictionary<string, ParkingTicket> _tickets = new();

    static async Task Main(string[] args)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("   Parking Management System");
        Console.WriteLine("   Configuration-Driven Backend");
        Console.WriteLine("========================================\n");

        try
        {
            // Initialize services using dependency injection pattern
            // WHY: Manual DI for console app - in production, would use DI container (e.g., Microsoft.Extensions.DependencyInjection)
            InitializeServices();

            // Load and display parking lot information
            await DisplayParkingLotInfoAsync();

            // Main menu loop
            await RunMainMenuAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Initializes all services with their dependencies.
    /// WHY: Sets up the dependency chain manually - demonstrates clean architecture with dependency inversion.
    /// </summary>
    static void InitializeServices()
    {
        // Configuration file path
        // WHY: Tries multiple path locations to find config file (works in both development and production)
        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "parkingConfig.json");
        
        // If running from bin directory, try project root
        if (!File.Exists(configPath))
        {
            var projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..");
            configPath = Path.Combine(projectRoot, "Config", "parkingConfig.json");
        }

        // If still not found, try current directory
        if (!File.Exists(configPath))
        {
            configPath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "parkingConfig.json");
        }

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found. Tried: {configPath}");
        }

        // Create layout provider (JSON file-based)
        var layoutProvider = new JsonParkingLayoutProvider(configPath);

        // Create rule engine
        var ruleEngine = new InstitutionRuleEngine();

        // Create pricing strategy
        var pricingStrategy = new StandardPricingStrategy();

        // Create pricing service
        var pricingService = new PricingService(pricingStrategy);

        // Create layout service
        var layoutService = new LayoutService(layoutProvider);

        // Create parking manager service
        _parkingManager = new ParkingManagerService(
            layoutService,
            pricingService,
            ruleEngine,
            layoutProvider);

        Console.WriteLine("✅ Services initialized successfully\n");
    }

    /// <summary>
    /// Displays parking lot information loaded from configuration.
    /// WHY: Shows that configuration is loaded correctly and displays current state.
    /// </summary>
    static async Task DisplayParkingLotInfoAsync()
    {
        if (_parkingManager == null) return;

        var availability = await _parkingManager.GetAvailabilityAsync();

        Console.WriteLine("📊 Parking Lot Status:");
        Console.WriteLine($"   Total Capacity: {availability.TotalCapacity} spots");
        Console.WriteLine($"   Available: {availability.Available} spots");
        Console.WriteLine($"   Occupied: {availability.Occupied} spots");
        Console.WriteLine();

        if (availability.AvailableByType.Count > 0)
        {
            Console.WriteLine("   Available by Type:");
            foreach (var kvp in availability.AvailableByType)
            {
                Console.WriteLine($"      {kvp.Key}: {kvp.Value} spots");
            }
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Main menu loop for user interaction.
    /// WHY: Provides simple console UI for testing parking operations.
    /// </summary>
    static async Task RunMainMenuAsync()
    {
        while (true)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   Main Menu");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Vehicle Entry");
            Console.WriteLine("2. Vehicle Exit");
            Console.WriteLine("3. Check Availability");
            Console.WriteLine("4. View Active Tickets");
            Console.WriteLine("5. Exit Application");
            Console.Write("\nSelect option (1-5): ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await HandleVehicleEntryAsync();
                        break;
                    case "2":
                        await HandleVehicleExitAsync();
                        break;
                    case "3":
                        await DisplayParkingLotInfoAsync();
                        break;
                    case "4":
                        DisplayActiveTickets();
                        break;
                    case "5":
                        Console.WriteLine("\n👋 Thank you for using Parking Management System!");
                        return;
                    default:
                        Console.WriteLine("\n❌ Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    /// <summary>
    /// Handles vehicle entry flow.
    /// WHY: Demonstrates entry process: collect vehicle info, find spot, generate ticket.
    /// </summary>
    static async Task HandleVehicleEntryAsync()
    {
        if (_parkingManager == null)
        {
            Console.WriteLine("❌ Parking manager not initialized.");
            return;
        }

        Console.WriteLine("\n========================================");
        Console.WriteLine("   Vehicle Entry");
        Console.WriteLine("========================================");

        // Collect vehicle information
        Console.Write("Enter License Plate: ");
        var licensePlate = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            Console.WriteLine("❌ License plate is required.");
            return;
        }

        Console.WriteLine("\nVehicle Types:");
        var vehicleTypes = Enum.GetValues<VehicleType>();
        for (int i = 0; i < vehicleTypes.Length; i++)
        {
            Console.WriteLine($"   {i + 1}. {vehicleTypes[i]}");
        }
        Console.Write("Select Vehicle Type (1-{0}): ", vehicleTypes.Length);
        var vehicleTypeChoice = Console.ReadLine();

        if (!int.TryParse(vehicleTypeChoice, out var vehicleTypeIndex) ||
            vehicleTypeIndex < 1 || vehicleTypeIndex > vehicleTypes.Length)
        {
            Console.WriteLine("❌ Invalid vehicle type selection.");
            return;
        }

        var vehicleType = vehicleTypes[vehicleTypeIndex - 1];

        // Optional: Preferred spot type
        Console.WriteLine("\nPreferred Spot Type (optional):");
        Console.WriteLine("   0. Any");
        var spotTypes = Enum.GetValues<SpotType>();
        for (int i = 0; i < spotTypes.Length; i++)
        {
            Console.WriteLine($"   {i + 1}. {spotTypes[i]}");
        }
        Console.Write("Select Preferred Spot Type (0-{0}): ", spotTypes.Length);
        var spotTypeChoice = Console.ReadLine();

        SpotType? preferredSpotType = null;
        if (int.TryParse(spotTypeChoice, out var spotTypeIndex) && spotTypeIndex > 0 && spotTypeIndex <= spotTypes.Length)
        {
            preferredSpotType = spotTypes[spotTypeIndex - 1];
        }

        // Create vehicle info
        var vehicle = new VehicleInfo
        {
            LicensePlate = licensePlate,
            Type = vehicleType
        };

        // Process entry
        Console.WriteLine("\n🔄 Processing entry...");
        var ticket = await _parkingManager.EnterParkingAsync(vehicle, preferredSpotType);

        if (ticket == null)
        {
            Console.WriteLine("❌ No available spots found for this vehicle.");
            return;
        }

        // Store ticket
        _tickets[ticket.TicketId] = ticket;

        // Display ticket information
        Console.WriteLine("\n✅ Vehicle entered successfully!");
        Console.WriteLine($"   Ticket ID: {ticket.TicketId}");
        Console.WriteLine($"   License Plate: {ticket.Vehicle.LicensePlate}");
        Console.WriteLine($"   Vehicle Type: {ticket.Vehicle.Type}");
        Console.WriteLine($"   Entry Time: {ticket.EntryTime:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   Assigned Spot: {ticket.SpotLocation?.DisplayName ?? "N/A"}");
    }

    /// <summary>
    /// Handles vehicle exit flow.
    /// WHY: Demonstrates exit process: validate ticket, calculate charge, free spot.
    /// </summary>
    static async Task HandleVehicleExitAsync()
    {
        if (_parkingManager == null)
        {
            Console.WriteLine("❌ Parking manager not initialized.");
            return;
        }

        Console.WriteLine("\n========================================");
        Console.WriteLine("   Vehicle Exit");
        Console.WriteLine("========================================");

        if (_tickets.Count == 0)
        {
            Console.WriteLine("❌ No active tickets found.");
            return;
        }

        // Display active tickets
        Console.WriteLine("\nActive Tickets:");
        var ticketList = _tickets.Values.ToList();
        for (int i = 0; i < ticketList.Count; i++)
        {
            var t = ticketList[i];
            Console.WriteLine($"   {i + 1}. {t.TicketId} - {t.Vehicle.LicensePlate} ({t.Vehicle.Type}) - {t.SpotLocation?.DisplayName}");
        }

        Console.Write("\nEnter Ticket ID or number (1-{0}): ", ticketList.Count);
        var input = Console.ReadLine() ?? string.Empty;

        string ticketId;
        if (int.TryParse(input, out var index) && index >= 1 && index <= ticketList.Count)
        {
            ticketId = ticketList[index - 1].TicketId;
        }
        else
        {
            ticketId = input;
        }

        if (!_tickets.ContainsKey(ticketId))
        {
            Console.WriteLine("❌ Ticket not found.");
            return;
        }

        // Process exit
        Console.WriteLine("\n🔄 Processing exit...");
        var exitResult = await _parkingManager.ExitParkingAsync(ticketId);

        if (exitResult == null)
        {
            Console.WriteLine("❌ Failed to process exit. Ticket not found.");
            return;
        }

        // Remove from active tickets
        _tickets.Remove(ticketId);

        // Display exit information
        Console.WriteLine("\n✅ Vehicle exited successfully!");
        Console.WriteLine($"   Ticket ID: {exitResult.Ticket.TicketId}");
        Console.WriteLine($"   License Plate: {exitResult.Ticket.Vehicle.LicensePlate}");
        Console.WriteLine($"   Entry Time: {exitResult.Ticket.EntryTime:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   Exit Time: {exitResult.Ticket.ExitTime:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   Parking Duration: {FormatDuration(exitResult.ParkingDuration)}");
        Console.WriteLine($"   Total Charge: ${exitResult.TotalCharge:F2}");
    }

    /// <summary>
    /// Displays all active parking tickets.
    /// WHY: Allows users to view currently parked vehicles.
    /// </summary>
    static void DisplayActiveTickets()
    {
        Console.WriteLine("\n========================================");
        Console.WriteLine("   Active Tickets");
        Console.WriteLine("========================================");

        if (_tickets.Count == 0)
        {
            Console.WriteLine("No active tickets.");
            return;
        }

        foreach (var ticket in _tickets.Values)
        {
            var duration = DateTime.Now - ticket.EntryTime;
            Console.WriteLine($"\n   Ticket ID: {ticket.TicketId}");
            Console.WriteLine($"   License Plate: {ticket.Vehicle.LicensePlate}");
            Console.WriteLine($"   Vehicle Type: {ticket.Vehicle.Type}");
            Console.WriteLine($"   Spot: {ticket.SpotLocation?.DisplayName ?? "N/A"}");
            Console.WriteLine($"   Entry Time: {ticket.EntryTime:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"   Duration: {FormatDuration(duration)}");
        }
    }

    /// <summary>
    /// Formats a TimeSpan as a human-readable duration string.
    /// WHY: Helper method for displaying parking duration in a user-friendly format.
    /// </summary>
    static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalHours >= 1)
        {
            return $"{duration.Hours}h {duration.Minutes}m";
        }
        return $"{duration.Minutes}m";
    }
}
