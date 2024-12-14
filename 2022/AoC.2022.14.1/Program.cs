string file = Debugger.IsAttached ? "example.txt" : "input.txt";

var paths = File.ReadLines(file).Select(l => l.Split(" -> ").Select(p => { var s = p.Split(',').Select(int.Parse).ToArray(); return (x: s[0], y: s[1]); }).ToList()).ToList();

var minx = paths.Min(p => p.Min(p => p.x)) - 1;
var miny = 0;
var maxx = paths.Max(p => p.Max(p => p.x)) + 1;
var maxy = paths.Max(p => p.Max(p => p.y));
Console.WriteLine(new { minx, maxx, miny, maxy });

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

var sand = (x: 500, y: 0);
var stopped = new HashSet<(int x, int y)>();

while (sand.y <= maxy)
{
    sand = (x: 500, y: 0);

    while (sand.y <= maxy)
    {
        var down = (sand.x, sand.y + 1);
        if (!rocks.Contains(down) && !stopped.Contains(down))
        {
            sand = down;
            continue;
        }
        var downleft = (sand.x - 1, sand.y + 1);
        if (!rocks.Contains(downleft) && !stopped.Contains(downleft))
        {
            sand = downleft;
            continue;
        }
        var downright = (sand.x + 1, sand.y + 1);
        if (!rocks.Contains(downright) && !stopped.Contains(downright))
        {
            sand = downright;
            continue;
        }
        break;
    }

    if (sand.y <= maxy)
        stopped.Add(sand);

    //PrintGrid();
    //Console.ReadLine();
}

PrintGrid();
Console.WriteLine(new { stopped.Count });

void PrintGrid()
{
    for (int y = miny; y <= maxy; y++)
    {
        for (int x = minx; x <= maxx; x++)
        {
            Console.Write(
                rocks.Contains((x, y)) 
                ? '#'
                : stopped.Contains((x, y))
                ? 'o'
                : '.');
        }
        Console.WriteLine();
    }
}