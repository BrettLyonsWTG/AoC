var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var plots = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (p: (x, y), c))).ToDictionary(g => g.p, g => g.c);

var regions = new List<(char type, List<(int x, int y)> plots, int sides)>();

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
        var topleft = region.Count(p => !region.Contains((p.x - 1, p.y)) && !region.Contains((p.x, p.y - 1)));
        var bottomright = region.Count(p => !region.Contains((p.x + 1, p.y)) && !region.Contains((p.x, p.y + 1)));
        var innertopright = region.Count(p => region.Contains((p.x + 1, p.y)) && region.Contains((p.x, p.y - 1)) && !region.Contains((p.x + 1, p.y - 1)));
        var innerbottomleft = region.Count(p => region.Contains((p.x - 1, p.y)) && region.Contains((p.x, p.y + 1)) && !region.Contains((p.x - 1, p.y + 1)));

        regions.Add((type.Key, region, (topleft + bottomright + innertopright + innerbottomleft) * 2));

    }
}

var total = regions.Sum(r => r.plots.Count * r.sides);
Console.WriteLine(new { total });
