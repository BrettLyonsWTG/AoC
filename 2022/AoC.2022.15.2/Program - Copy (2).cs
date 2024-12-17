using System.Collections.Immutable;

string file = Debugger.IsAttached ? "example.txt" : "input.txt";
int lim = Debugger.IsAttached ? 20 : 4000000;

List<((int x, int y) s, (int x, int y) b, int r)> ranges = [];

foreach (string line in File.ReadLines(file))
{
    var match = Regex.Match(line, @"Sensor at x=(\d+), y=(\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");
    var sensor = (x: int.Parse(match.Groups[1].Value), y: int.Parse(match.Groups[2].Value));
    var beacon = (x: int.Parse(match.Groups[3].Value), y: int.Parse(match.Groups[4].Value));
    var range = (sensor, beacon, Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y));
    ranges.Add(range);
}

var values = Enumerable.Range(0, lim + 1).ToImmutableHashSet();

for (int row = 0; row <= lim; row++)
{
    var parts = ranges
        .Where(r => r.s.y - r.r <= row && r.s.y + r.r >= row)
        .Select(r => (r.s, r: r.r - Math.Abs(r.s.y - row)))
        .Select(r => (r.s, left: Math.Max(r.s.x - r.r, 0), right: Math.Min(r.s.x + r.r, lim)))
        .Where(r => r.left <= lim && r.right >= 0)
        .SelectMany(r => Enumerable.Range(r.left, r.right - r.left + 1))
        .Distinct()
        //.Except(ranges.Where(r => r.b.y == row).Select(r => r.b.x))
        ;
    var open = values.Except(parts).ToList();
    if (open.Count > 0)
    {
        Console.WriteLine(new { row, open = string.Join(",", open) });
    }
    if (open.Count == 1)
    {
        Console.WriteLine(new { x = open[0], y = row, freq = open[0] * 4000000 + row });
        break;
    }
    if (row % 1 == 0)
    {
        Console.WriteLine(new { row });
    }
}
