var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var map = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (p: (x, y), c))).ToDictionary(g => g.p, g => g.c - '0');
var maxx = map.Keys.Max(p => p.x);
var maxy = map.Keys.Max(p => p.y);

var score = 0;

foreach (var th in map.Where(m => m.Value is 0).Select(m => m.Key))
{
    List<List<(int x, int y)>> paths = [[th]];
    int height = 0;
    while (paths.Count > 0 && height < 9)
    {
        paths = paths.GroupBy(p => p.Last())
            .SelectMany(s => map.Where(m => m.Value == height + 1 && (m.Key.x - s.Key.x, m.Key.y - s.Key.y) is (-1, 0) or (1, 0) or (0, 1) or (0, -1))
                .SelectMany(m => s.Select(n => n.Append(m.Key).ToList()))).ToList();
        height++;
    }
    score += paths.Count;
    Console.WriteLine(paths.Count);
}

Console.WriteLine(new { score });