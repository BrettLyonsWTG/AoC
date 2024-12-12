var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var plots = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (p: (x, y), c))).ToDictionary(g => g.p, g => g.c);

var regions = new List<(char type, List<(int x, int y)> plots, int border)>();

var types = plots.GroupBy(p => p.Value).Select(t => (t.Key, plots: t.Select(p => p.Key).ToList())).ToList();

foreach (var type in types)
{
    while (type.plots.Count > 0)
    {
        var region = new List<(int x, int y)> { type.plots[0] };
        type.plots.RemoveAt(0);
        (int x, int y)[] neighbours = [];
        do
        {
            neighbours = region.SelectMany(p => new[] { (p.x - 1, p.y), (p.x + 1, p.y), (p.x, p.y - 1), (p.x, p.y + 1) })
                .Intersect(type.plots).ToArray();
            region.AddRange(neighbours);
            type.plots.RemoveAll(neighbours.Contains);
        } while (neighbours.Length > 0);
        var border = region.Sum(p => 4 - new[] { (p.x - 1, p.y), (p.x + 1, p.y), (p.x, p.y - 1), (p.x, p.y + 1) }.Intersect(region).Count());
        regions.Add((type.Key, region, border));

    }
}

var total = regions.Sum(r => r.plots.Count * r.border);
Console.WriteLine(new { total });
