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
            Console.ForegroundColor = robots.Any(r => r.p == (x, y)) ? ConsoleColor.Green : ConsoleColor.White;
            Console.Write(robots.Any(r => r.p == (x, y)) ? '@' : "·");
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

for (int i = 0; i < 10404; i++)
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

    var orphans = robots.Count(r => !robots.Any(n => n.p == (r.p.x, r.p.y - 1) || n.p == (r.p.x, r.p.y + 1) || n.p == (r.p.x - 1, r.p.y) || n.p == (r.p.x + 1, r.p.y)));
    if (i is 8278 or 8279 or 8280)
    {
        PrintGrid();
        Console.WriteLine(new { seconds = i + 1, orphans });
        if (i is 8280)
            break;
    }
}
