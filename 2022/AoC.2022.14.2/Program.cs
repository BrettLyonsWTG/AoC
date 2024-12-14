string file = Debugger.IsAttached ? "example.txt" : "input.txt";

var paths = File.ReadLines(file).Select(l => l.Split(" -> ").Select(p => { var s = p.Split(',').Select(int.Parse).ToArray(); return (x: s[0], y: s[1]); }).ToList()).ToList();

var maxy = paths.Max(p => p.Max(p => p.y)) + 2;

var rocks = new HashSet<(int x, int y)>();
foreach (var path in paths)
{
    for (int i = 1; i < path.Count; i++)
    {
        if (path[i - 1].y == path[i].y)
        {
            var min = Math.Min(path[i - 1].x, path[i].x);
            var max = Math.Max(path[i - 1].x, path[i].x);
            for (int x = min; x <= max; x++)
            {
                rocks.Add((x, path[i].y));
            }
        }
        else
        {
            var min = Math.Min(path[i - 1].y, path[i].y);
            var max = Math.Max(path[i - 1].y, path[i].y);
            for (int y = min; y <= max; y++)
            {
                rocks.Add((path[i].x, y));
            }
        }
    }
}

var sand = (x: 0, y: 0);
var stopped = new HashSet<(int x, int y)>();

while (sand != (500, 0))
{
    sand = (500, 0);

    while (sand.y <= maxy)
    {
        var down = (x: sand.x, y: sand.y + 1);
        if (down.y != maxy && !rocks.Contains(down) && !stopped.Contains(down))
        {
            sand = down;
            continue;
        }
        var downleft = (x: sand.x - 1, y: sand.y + 1);
        if (downleft.y != maxy && !rocks.Contains(downleft) && !stopped.Contains(downleft))
        {
            sand = downleft;
            continue;
        }
        var downright = (x: sand.x + 1, y: sand.y + 1);
        if (downright.y != maxy && !rocks.Contains(downright) && !stopped.Contains(downright))
        {
            sand = downright;
            continue;
        }
        break;
    }

    if (sand.y <= maxy)
        stopped.Add(sand);
}

PrintGrid();
Console.WriteLine(new { stopped.Count });

void PrintGrid()
{
    var miny = 0;
    var minx = rocks.Concat(stopped).Min(p => p.x) - 1;
    var maxx = rocks.Concat(stopped).Max(p => p.x) + 1;
    for (int y = miny; y <= maxy; y++)
    {
        for (int x = minx; x <= maxx; x++)
        {
            Console.Write(
                y == maxy || rocks.Contains((x, y))
                ? '#'
                : stopped.Contains((x, y))
                ? 'o'
                : '.');
        }
        Console.WriteLine();
    }
}