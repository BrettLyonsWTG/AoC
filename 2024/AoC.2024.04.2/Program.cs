var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var grid = File.ReadLines(file).SelectMany((line, y) => line.Select((c, x) => (x, y, c))).ToDictionary(g => (g.x, g.y), g => g.c);

var maxx = grid.Max(g => g.Key.x);
var maxy = grid.Max(g => g.Key.y);
var gridx = grid.Where(g => g.Value is 'X').Select(g => g.Key).ToList();

var r = gridx.Where(g => g.x < maxx - 2 && grid[(g.x + 1, g.y)] is 'M' && grid[(g.x + 2, g.y)] is 'A' && grid[(g.x + 3, g.y)] is 'S').ToList();
var l = gridx.Where(g => g.x > 2 && grid[(g.x - 1, g.y)] is 'M' && grid[(g.x - 2, g.y)] is 'A' && grid[(g.x - 3, g.y)] is 'S').ToList();
var d = gridx.Where(g => g.y < maxy - 2 && grid[(g.x, g.y + 1)] is 'M' && grid[(g.x, g.y + 2)] is 'A' && grid[(g.x, g.y + 3)] is 'S').ToList();
var u = gridx.Where(g => g.y > 2 && grid[(g.x, g.y - 1)] is 'M' && grid[(g.x, g.y - 2)] is 'A' && grid[(g.x, g.y - 3)] is 'S').ToList();
var dr = gridx.Where(g => g.x < maxx - 2 && g.y < maxy - 2 && grid[(g.x + 1, g.y + 1)] is 'M' && grid[(g.x + 2, g.y + 2)] is 'A' && grid[(g.x + 3, g.y + 3)] is 'S').ToList();
var dl = gridx.Where(g => g.x > 2 && g.y < maxy - 2 && grid[(g.x - 1, g.y + 1)] is 'M' && grid[(g.x - 2, g.y + 2)] is 'A' && grid[(g.x - 3, g.y + 3)] is 'S').ToList();
var ur = gridx.Where(g => g.x < maxx - 2 && g.y > 2 && grid[(g.x + 1, g.y - 1)] is 'M' && grid[(g.x + 2, g.y - 2)] is 'A' && grid[(g.x + 3, g.y - 3)] is 'S').ToList();
var ul = gridx.Where(g => g.x > 2 && g.y > 2 && grid[(g.x - 1, g.y - 1)] is 'M' && grid[(g.x - 2, g.y - 2)] is 'A' && grid[(g.x - 3, g.y - 3)] is 'S').ToList();

for (var y = 0; y <= maxy; y++)
{
    for (var x = 0; x <= maxx; x++)
    {
        if (r.Intersect([(x, y), (x - 1, y), (x - 2, y), (x - 3, y)]).Any())
            Console.Write(grid[(x, y)]);
        else
        if (l.Intersect([(x, y), (x + 1, y), (x + 2, y), (x + 3, y)]).Any())
            Console.Write(grid[(x, y)]);
        else
        if (d.Intersect([(x, y), (x, y - 1), (x, y - 2), (x, y - 3)]).Any())
            Console.Write(grid[(x, y)]);
        else
        if (u.Intersect([(x, y), (x, y + 1), (x, y + 2), (x, y + 3)]).Any())
            Console.Write(grid[(x, y)]);
        else
        if (dr.Intersect([(x, y), (x - 1, y - 1), (x - 2, y - 2), (x - 3, y - 3)]).Any())
            Console.Write(grid[(x, y)]);
        else
        if (dl.Intersect([(x, y), (x + 1, y - 1), (x + 2, y - 2), (x + 3, y - 3)]).Any())
            Console.Write(grid[(x, y)]);
        else
        if (ur.Intersect([(x, y), (x - 1, y + 1), (x - 2, y + 2), (x - 3, y + 3)]).Any())
            Console.Write(grid[(x, y)]);
        else
        if (ul.Intersect([(x, y), (x + 1, y + 1), (x + 2, y + 2), (x + 3, y + 3)]).Any())
            Console.Write(grid[(x, y)]);
        else
            Console.Write('.');
    }
    Console.WriteLine();
}

var result = r.Count + l.Count + d.Count + u.Count + dr.Count + dl.Count + ur.Count + ul.Count;

Console.WriteLine(new { result });

// 2466 is too low