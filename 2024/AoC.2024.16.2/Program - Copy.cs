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

void PrintTrackBests(List<List<((int, int) p, char c)>> bests)
{
    var pathVals = bests.SelectMany(b => b.Select(i => i.p)).ToHashSet();
    for (int y = 0; y <= max.y; y++)
    {
        for (int x = 0; x <= max.x; x++)
        {
            Console.ForegroundColor = walls.Contains((x, y)) ? ConsoleColor.Yellow : pathVals.Contains((x, y)) ? ConsoleColor.DarkRed : ConsoleColor.White;
            Console.BackgroundColor = pathVals.Contains((x, y)) ? ConsoleColor.White : ConsoleColor.Black;
            Console.Write((x, y) == start ? 'S' : (x, y) == end ? 'E' : walls.Contains((x, y)) ? '#' : pathVals.Contains((x, y)) ? 'O' : '.');
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

var partialbests = new Dictionary<((int x, int y) p, char c), int>();
var bests = new List<List<((int x, int y) p, char c)>>();
var bestcost = int.MaxValue;
var counter = 0;

List<(((int x, int y) p, char c) key, (int cost, List<((int x, int y) p, char c)> path) value)> queue = [((start, '>'), (0, [(start, '>')]))];

while (queue.Count > 0)
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
        .Where(n => n.value.cost <= bestcost && (!partialbests.TryGetValue(n.key, out var p) || p >= n.value.cost))
        .ToList();

    foreach (var next in nexts)
    {
        partialbests[next.key] = next.value.cost;
        if (next.key.p == end)
        {
            if (next.value.cost < bestcost)
            {
                bests.Clear();
                bests.Add(next.value.path);
                bestcost = next.value.cost;
            }
            else if (next.value.cost == bestcost)
            {
                bests.Add(next.value.path);
            }
            PrintTrack(next.value.path);
            Console.WriteLine(new { next.value.cost });
            Console.ReadLine();
        }

        if (++counter % 100000 == 0)
        {
            PrintTrack(next.value.path);
            Console.WriteLine(new { counter, partialbests = partialbests.Count });
        }
    }

    queue.AddRange(nexts);
}

PrintTrackBests(bests);
Console.WriteLine(new { onbest = bests.SelectMany(b => b.Select(i => i.p)).Distinct().Count() });
