string file = Debugger.IsAttached ? "example.txt" : "input.txt";

HashSet<(int x, int y)> sensors = [];
HashSet<(int x, int y)> beacons = [];
HashSet<(int x, int y)> covered = [];

void PrintGrid()
{
    var minx = sensors.Concat(beacons).Concat(covered).Min(p => p.x);
    var miny = sensors.Concat(beacons).Concat(covered).Min(p => p.y);
    var maxx = sensors.Concat(beacons).Concat(covered).Max(p => p.x);
    var maxy = sensors.Concat(beacons).Concat(covered).Max(p => p.y);

    for (int y = miny; y <= maxy; y++)
    {
        for (int x = minx; x <= maxx; x++)
        {
            Console.ForegroundColor
                = sensors.Contains((x, y))
                    ? ConsoleColor.Red
                : beacons.Contains((x, y)) 
                    ? ConsoleColor.Green
                : covered.Contains((x, y))
                    ? ConsoleColor.Yellow
                : ConsoleColor.White;
            Console.Write(sensors.Contains((x, y)) ? 'S' : beacons.Contains((x, y)) ? 'B' : covered.Contains((x, y)) ? '#' : '.');
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

foreach (string line in File.ReadLines(file))
{
    Console.WriteLine(line);
    var match = Regex.Match(line, @"Sensor at x=(\d+), y=(\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");
    var sensor = (x: int.Parse(match.Groups[1].Value), y: int.Parse(match.Groups[2].Value));
    var beacon = (x: int.Parse(match.Groups[3].Value), y: int.Parse(match.Groups[4].Value));
    sensors.Add(sensor);
    beacons.Add(beacon);
    var range = Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);
    for (
        int x1 = sensor.x, x2 = sensor.x, y = sensor.y - range; 
        y <= sensor.y + range; 
        x1 += y < sensor.y ? -1 : 1, x2 += y < sensor.y ? 1 : -1, y++)
    {
        var covering = Enumerable.Range(x1, x2 - x1 + 1)
            .Select(x => (x, y))
            .Except(covered)
            .Except(sensors)
            .Except(beacons)
            .ToArray();
        foreach (var c in covering) { covered.Add(c); }
    }
    PrintGrid();
}
