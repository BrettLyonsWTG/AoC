var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var grid = File.ReadLines(file).SelectMany((line, y) => line.Select((c, x) => (x, y, c))).ToDictionary(g => (g.x, g.y), g => g.c);

var maxx = grid.Max(g => g.Key.x);
var maxy = grid.Max(g => g.Key.y);
var grida = grid.Where(g => g.Value is 'A' && g.Key.x > 0 && g.Key.x < maxx && g.Key.y > 0 && g.Key.y < maxy).Select(g => g.Key).ToList();

var r = grida.Where(g => grid[(g.x + 1, g.y - 1)] is 'M' && grid[(g.x + 1, g.y + 1)] is 'M' && grid[(g.x - 1, g.y - 1)] is 'S' && grid[(g.x - 1, g.y + 1)] is 'S').ToList();
var l = grida.Where(g => grid[(g.x - 1, g.y - 1)] is 'M' && grid[(g.x - 1, g.y + 1)] is 'M' && grid[(g.x + 1, g.y - 1)] is 'S' && grid[(g.x + 1, g.y + 1)] is 'S').ToList();
var d = grida.Where(g => grid[(g.x - 1, g.y + 1)] is 'M' && grid[(g.x + 1, g.y + 1)] is 'M' && grid[(g.x - 1, g.y - 1)] is 'S' && grid[(g.x + 1, g.y - 1)] is 'S').ToList();
var u = grida.Where(g => grid[(g.x - 1, g.y - 1)] is 'M' && grid[(g.x + 1, g.y - 1)] is 'M' && grid[(g.x - 1, g.y + 1)] is 'S' && grid[(g.x + 1, g.y + 1)] is 'S').ToList();

for (var y = 0; y <= maxy; y++)
{
    for (var x = 0; x <= maxx; x++)
    {
        var cross = new[] { (x, y), (x + 1, y + 1), (x + 1, y - 1), (x - 1, y + 1), (x - 1, y - 1) };
        if (r.Intersect(cross).Any())
            Console.Write(grid[(x, y)]);
        else
        if (l.Intersect(cross).Any())
            Console.Write(grid[(x, y)]);
        else
        if (d.Intersect(cross).Any())
            Console.Write(grid[(x, y)]);
        else
        if (u.Intersect(cross).Any())
            Console.Write(grid[(x, y)]);
        else
            Console.Write('.');
    }
    Console.WriteLine();
}

var result = r.Count + l.Count + d.Count + u.Count;

Console.WriteLine(new { result });
