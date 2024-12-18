var file = Debugger.IsAttached ? "example2.txt" : "input.txt";

var track = File.ReadLines(file)
    .SelectMany((l, y) => l.Select((c, x) => (c, p: (x, y))))
    .GroupBy(t => t.c)
    .ToDictionary(g => g.Key, g => g.Select(t => t.p).ToList());

var walls = track['#'];
var paths = track['.'];
var start = track['S'].Single();
var end = track['E'].Single();
var max = walls.Max();

void PrintTrack(List<((int, int) p, char c)> path)
{
    var pathVals = path.ToDictionary(p => p.p, p => p.c);
    for (int y = 0; y <= max.y; y++)
    {
        for (int x = 0; x <= max.x; x++)
        {
            Console.ForegroundColor = walls.Contains((x, y)) ? ConsoleColor.Yellow : pathVals.ContainsKey((x, y)) ? ConsoleColor.DarkRed : ConsoleColor.White;
            Console.BackgroundColor = pathVals.ContainsKey((x, y)) ? ConsoleColor.White : ConsoleColor.Black;
            Console.Write((x, y) == start ? 'S' : (x, y) == end ? 'E' : walls.Contains((x, y)) ? '#' : pathVals.TryGetValue((x, y), out char c) ? c : '.');
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

var bests = new Dictionary<((int x, int y) p, char c), (int cost, List<((int x, int y) p, char c)> path)>();

List<(((int x, int y) p, char c) key, (int cost, List<((int x, int y) p, char c)> path) value)> queue = [((start, '>'), (0, [(start, '>')]))];

while (true)
{
    var l = queue.OrderBy(q => q.value.cost).First();
    queue.Remove(l);
    List<(((int x, int y) p, char c) key, (int cost, List<((int x, int y) p, char c)> path) value)> nexts =
        new[]
        {
            (p: (l.key.p.x + 1, l.key.p.y), c: '>' ),
            (p: (l.key.p.x, l.key.p.y + 1), c: 'v' ),
            (p: (l.key.p.x - 1, l.key.p.y), c: '<' ),
            (p: (l.key.p.x, l.key.p.y - 1), c: '^' )
        }
        .ExceptBy(walls, n => n.p)
        .ExceptBy(l.value.path.Select(p => p.p), n => n.p)
        .Select(n => (n, path: l.value.path.Append(n).ToList()))
        .Select(n => (key: n.n, value: (cost: l.value.cost + (n.n.c == l.value.path.Last().c ? 1 : 1001), n.path)))
        .Where(n => !bests.ContainsKey(n.key) || bests[n.key].cost > n.value.cost)
        .ToList();

    foreach (var next in nexts.GroupBy(n => n.key).Select(n => (key: n.Key, n.OrderByDescending(v => v.value.cost).First().value)))
    {
        bests[next.key] = next.value;
    }

    if (nexts.Any(n => n.key.p == end))
        break;

    queue.AddRange(nexts);
}

var best = bests.Where(b => b.Key.p == end).OrderBy(b => b.Value.cost).First();
PrintTrack(best.Value.path);
Console.WriteLine(new { best.Value.cost });
