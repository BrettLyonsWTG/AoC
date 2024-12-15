var file = Debugger.IsAttached ? "example.txt" : "input.txt";

int maxx = Debugger.IsAttached ? 11 : 101;
int maxy = Debugger.IsAttached ? 7 : 103;
var midx = (maxx - 1) / 2;
var midy = (maxy - 1) / 2;


var robots = File.ReadLines(file).Select(l =>
{
    var m = Regex.Match(l, @"p=(\d+),(\d+) v=(-?\d+),(-?\d+)");
    return (p: (x: int.Parse(m.Groups[1].Value), y: int.Parse(m.Groups[2].Value)), v: (x: int.Parse(m.Groups[3].Value), y: int.Parse(m.Groups[4].Value)));
}).ToList();

void PrintGrid()
{
    for (int y = 0; y < maxy; y++)
    {
        for (int x = 0; x < maxx; x++)
        {
            Console.Write(robots.Count(r => r.p == (x, y)) is int c && c > 0 ? c.ToString() : ".");
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

List<((int x, int y) p, (int x, int y) v)> prev = new();

for (int i = 0; i < 10000000; i++)
{
    for (int r = 0; r < robots.Count; r++)
    {
        var robot = robots[r];
        var nx = robot.p.x + robot.v.x;
        var ny = robot.p.y + robot.v.y;
        var nx2 = (int)(nx > 0 ? nx : nx + Math.Ceiling((double)Math.Abs(nx) / maxx) * maxx) % maxx;
        var ny2 = (int)(ny > 0 ? ny : ny + Math.Ceiling((double)Math.Abs(ny) / maxy) * maxy) % maxy;
        robots[r] = (p: (nx2, ny2), robot.v);
    }

    if (robots.Count(r => r.p.x < midx && r.p.y < midy) == robots.Count(r => r.p.x > midx && r.p.y < midy)
        && robots.Count(r => r.p.x < midx && r.p.y > midy) == robots.Count(r => r.p.x > midx && r.p.y > midy))
    {
        var left = robots.Where(r => r.p.x < midx).Select(r => r.p);
        var right = robots.Where(r => r.p.x > midx).Select(r => (maxx - 1 - r.p.x, r.p.y));
        var symmertric = left.Intersect(right).Count();
        if (symmertric >= 30)
        {
            PrintGrid();
            Console.WriteLine(new { i, symmertric, match = robots.SequenceEqual(prev) });
            Console.ReadLine();
            prev = robots.ToList();
        }
    }
}

PrintGrid();

// 25274 - 14871 = 10403

// 10403 is too high