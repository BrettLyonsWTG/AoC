string file = Debugger.IsAttached ? "example.txt" : "input.txt";
int row = Debugger.IsAttached ? 10 : 2000000;

List<((int x, int y) s, (int x, int y) b, int r)> ranges = [];

foreach (string line in File.ReadLines(file))
{
    var match = Regex.Match(line, @"Sensor at x=(\d+), y=(\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");
    var sensor = (x: int.Parse(match.Groups[1].Value), y: int.Parse(match.Groups[2].Value));
    var beacon = (x: int.Parse(match.Groups[3].Value), y: int.Parse(match.Groups[4].Value));
    var range = (sensor, beacon, Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y));
    ranges.Add(range);
}

var parts = ranges.Where(r => row >= r.s.y - r.r && row <= r.s.y + r.r)
    .SelectMany(r =>
    {
        var len = r.r - Math.Abs(r.s.y - row);
        return Enumerable.Range(r.s.x - len, len * 2 + 1);
    })
    .Distinct()
    .Except(ranges.Where(r => r.b.y == row)
    .Select(r => r.b.x))
    .Count();

Console.WriteLine(new { parts });