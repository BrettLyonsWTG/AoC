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

void PrintRanges((int x, int y) sa, int ra, (int x, int y) sb, int rb, List<(int x, int y)> gaps)
{
    var minx = Math.Min(sa.x - ra, sb.x - rb);
    var miny = Math.Min(sa.y - ra, sb.y - rb);
    var maxx = Math.Max(sa.x + ra, sb.x + rb);
    var maxy = Math.Max(sa.y + ra, sb.y + rb);

    for (int y = miny; y <= maxy; y++)
    {
        for (int x = minx; x <= maxx; x++)
        {
            var is_a = Math.Abs(sa.x - x) + Math.Abs(sa.y - y) <= ra;
            var is_b = Math.Abs(sb.x - x) + Math.Abs(sb.y - y) <= rb;
            var is_gap = gaps.Contains((x, y));
            Console.ForegroundColor = is_gap ? ConsoleColor.Red : ConsoleColor.White;
            Console.Write(is_gap ? '@' : is_a ? 'A' : is_b ? 'B' : '.');
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

var uncovered = ranges.SelectMany(r =>
    ranges.Except([r])
        .Select(o => (o.s, o.r, d: Math.Abs(o.s.x - r.s.x) + Math.Abs(o.s.y - r.s.y) - r.r - o.r - 1))
        .OrderBy(o => o.d)
        .Where(o => o.d == 1)
        .SelectMany(n =>
        {
            var dx = n.s.x - r.s.x;
            var dy = n.s.y - r.s.y;
            var vx = dx > 0 ? 1 : -1;
            var vy = dy > 0 ? 1 : -1;

            (int x, int y) start = (vx, vy) switch
            {
                (1, 1) => (r.s.x + Math.Min(dx, r.r + 1), n.s.y - Math.Min(dy, n.r + 1)),
                (1, -1) => (r.s.x + Math.Min(dx, r.r + 1), n.s.y + Math.Min(-dy, n.r + 1)),
                (-1, 1) => (r.s.x - Math.Min(-dx, r.r + 1), n.s.y - Math.Min(dy, n.r + 1)),
                (-1, -1) => (r.s.x - Math.Min(-dx, r.r + 1), n.s.y + Math.Min(-dy, n.r + 1)),
                _ => throw new NotImplementedException()
            };

            return Enumerable.Range(0, Math.Min(Math.Abs(dx), Math.Abs(dy)) + 1)
                .Select(i => (x: start.x - i * vx, y: start.y + i * vy))
                .Where(u => !ranges.Any(r => Math.Abs(r.s.x - u.x) + Math.Abs(r.s.y - u.y) <= r.r));
        })
    ).Distinct().Single();

long result = uncovered.x * 4000000L + uncovered.y;

Console.WriteLine(new { result });