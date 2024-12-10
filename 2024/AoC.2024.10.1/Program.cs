var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var map = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (p: (x, y), c))).ToDictionary(g => g.p, g => g.c - '0');
var maxx = map.Keys.Max(p => p.x);
var maxy = map.Keys.Max(p => p.y);

var score = 0;

foreach (var th in map.Where(m => m.Value is 0).Select(m => m.Key))
{
    var paths = new List<(int x, int y)>();
    int height = 0;
    List<(int x, int y)> spots = [th];
    while (spots.Count > 0 && height < 9)
    {
        paths.AddRange(spots);
        spots = spots.SelectMany(s => map.Where(m => (m.Key.x - s.x, m.Key.y - s.y, m.Value - height) switch { (1 or -1, 0, 1) or (0, 1 or -1, 1) => true, _ => false })
                .Select(m => m.Key)).Distinct().ToList();
        height++;
    }
    paths.AddRange(spots);
    score += spots.Count;

    //for (int y = 0; y < maxx; y++)
    //{
    //    for (int x = 0; x < maxy; x++)
    //    {
    //        Console.Write(paths.Contains((x, y)) ? map[(x, y)].ToString() : ".");
    //    }
    //    Console.WriteLine();
    //}
    Console.WriteLine(spots.Count);
}

Console.WriteLine(new { score });