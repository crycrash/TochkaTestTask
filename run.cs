using System.Text.RegularExpressions;

namespace Tochka;

internal abstract class HotelCapacity
{
    private static bool CheckCapacity(int maxCapacity, List<Guest> guests)
    {
        List<(DateTime date, int change)> events = [];

        foreach (var guest in guests)
        {
            var checkIn = DateTime.Parse(guest.CheckIn);
            var checkOut = DateTime.Parse(guest.CheckOut);

            events.Add((checkIn, +1));
            events.Add((checkOut, -1));
        }

        events.Sort((a, b) => 
            a.date.CompareTo(b.date) != 0 
                ? a.date.CompareTo(b.date) 
                : a.change.CompareTo(b.change));

        var currentGuests = 0;

        foreach (var e in events)
        {
            currentGuests += e.change;
            if (currentGuests > maxCapacity)
                return false;
        }

        return true;
    }

    class Guest
    {
        public string Name { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
    }

    static void Main()
    {
        var maxCapacity = int.Parse(Console.ReadLine());
        var n = int.Parse(Console.ReadLine());

        var guests = new List<Guest>();

        for (var i = 0; i < n; i++)
        {
            var line = Console.ReadLine();
            if (line == null) continue;
            var guest = ParseGuest(line);
            guests.Add(guest);
        }

        var result = CheckCapacity(maxCapacity, guests);
        Console.WriteLine(result ? "True" : "False");
    }

    private static Guest ParseGuest(string json)
    {
        var guest = new Guest();

        var nameMatch = Regex.Match(json, "\"name\"\\s*:\\s*\"([^\"]+)\"");
        if (nameMatch.Success)
            guest.Name = nameMatch.Groups[1].Value;

        var checkInMatch = Regex.Match(json, "\"check-in\"\\s*:\\s*\"([^\"]+)\"");
        if (checkInMatch.Success)
            guest.CheckIn = checkInMatch.Groups[1].Value;

        var checkOutMatch = Regex.Match(json, "\"check-out\"\\s*:\\s*\"([^\"]+)\"");
        if (checkOutMatch.Success)
            guest.CheckOut = checkOutMatch.Groups[1].Value;

        return guest;
    }
}