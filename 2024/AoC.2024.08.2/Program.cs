var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var map = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (p: (x, y), c))).ToDictionary(g => g.p, g => g.c);
var maxx = map.Keys.Max(p => p.x);
var maxy = map.Keys.Max(p => p.y);

var frequencies = map.Where(m => m.Value is not '.').GroupBy(m => m.Value).ToDictionary(g => g.Key, g => g.Select(a => a.Key).ToList());
HashSet<(int x, int y)> antinodes = new();

foreach (var f in frequencies)
{
    Console.WriteLine(new { f.Key, f.Value.Count });
    var antenna = f.Value.OrderBy(a => a.y).ThenBy(a => a.x).ToList();
    for (var a = 0; a < antenna.Count - 1; a++)
    {
        for (var b = a + 1; b < antenna.Count; b++)
        {
            var distx = antenna[b].x - antenna[a].x;
            var disty = antenna[b].y - antenna[a].y;
            for (int y = antenna[a].y, x = antenna[a].x; y >= 0 && x >= 0 && x <= maxx; y -= disty, x -= distx)
            {
                antinodes.Add((x, y));
            }
            for (int y = antenna[b].y, x = antenna[b].x; y <= maxy && x >= 0 && x <= maxx; y += disty, x += distx)
            {
                antinodes.Add((x, y));
            }
        }
    }
}

PrintGrid();
Console.WriteLine(new { antinodes.Count });

void PrintGrid()
{
    var defaultColor = Console.ForegroundColor;
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x <= maxx; x++)
        {
            if (antinodes.Contains((x, y)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(map[(x, y)] is '.' ? '#' : map[(x, y)]);
                Console.ForegroundColor = defaultColor;
            }
            else
            {
                Console.Write(map[(x, y)]);
            }
        }
        Console.WriteLine();
    }
}



