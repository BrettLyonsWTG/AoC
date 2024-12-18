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
var digx = (int)Math.Ceiling(Math.Log10(max.x));
var digy = (int)Math.Ceiling(Math.Log10(max.y));

void PrintTrack(List<((int, int) p, char c)> path, (int x, int y) junction = default)
{
    var pathVals = path.GroupBy(p => p.p).ToDictionary(p => p.Key, p => p.GroupBy(l => l.c).OrderByDescending(c => c.Count()).First().Key);

    for (int p = digx - 1; p >= 0; p--)
    {
        Console.Write(" ".PadLeft(digy + 1));
        var s = (int)Math.Pow(10, p);
        for (int x = 0; x <= max.x; x++)
        {
            if (x >= s && (x % 5 == 0 || x == max.x))
            {
                Console.Write($"{x / s % 10}");
            }
            else
            {
                Console.Write(' ');
            }
        }
        Console.WriteLine();
    }

    for (int y = 0; y <= max.y; y++)
    {
        Console.Write($"{{0,{digy}}} ", y);
        for (int x = 0; x <= max.x; x++)
        {
            Console.ForegroundColor = walls.Contains((x, y)) ? ConsoleColor.Yellow : (x, y) == junction ? ConsoleColor.Red : pathVals.ContainsKey((x, y)) ? ConsoleColor.Black : ConsoleColor.White;
            Console.BackgroundColor = pathVals.ContainsKey((x, y)) ? ConsoleColor.Gray : ConsoleColor.Black;
            Console.Write(walls.Contains((x, y)) ? '#' : (x, y) == junction ? '@' : pathVals.TryGetValue((x, y), out char c) ? c : ' ');
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        Console.WriteLine();
    }
    if (junction != default)
    {
        Console.WriteLine(new { junction });
    }
    Console.WriteLine();
}

List<(((int x, int y) p, char d) key, int cost, List<((int x, int y) p, char d)> path)> startqueue = [((start, '>'), 0, [(start, '>')])];
List<(((int x, int y) p, char d) key, int cost, List<((int x, int y) p, char d)> path)> endqueue
    = new ((int x, int y) p, char d, char e, int c)[]
    {
        ((end.x + 1, end.y), 'v', '<', 1001),
        ((end.x + 1, end.y), '<', '<', 1),
        ((end.x + 1, end.y), '^', '<', 1001),
        ((end.x - 1, end.y), 'v', '>', 1001),
        ((end.x - 1, end.y), '>', '>', 1),
        ((end.x - 1, end.y), '^', '>', 1001),
        ((end.x, end.y + 1), '>', '^', 1001),
        ((end.x, end.y + 1), '^', '^', 1),
        ((end.x, end.y + 1), '<', '^', 1001),
        ((end.x, end.y - 1), '>', 'v', 1001),
        ((end.x, end.y - 1), 'v', 'v', 1),
        ((end.x, end.y - 1), '<', 'v', 1001),
    }.Where(e =>
        e.p.x >= 0 && e.p.y >= 0 &&
        e.p.x <= max.x && e.p.y <= max.y &&
        !walls.Contains(e.p) &&
        !walls.Contains(e.d switch
        {
            '<' => (e.p.x - 1, e.p.y),
            '>' => (e.p.x + 1, e.p.y),
            '^' => (e.p.x, e.p.y - 1),
            'v' => (e.p.x, e.p.y + 1),
            _ => e.p
        })).Select(e => (key: (e.p, e.d), e.c, path: new[] { (end, e.e), (e.p, e.d) }.ToList()))
        .ToList();

var starts = new Dictionary<((int x, int y) p, char d), (int cost, List<List<((int x, int y) p, char d)>> paths)>();
Dictionary<((int x, int y) p, char d), (int cost, List<List<((int x, int y) p, char d)>> paths)> ends
    = endqueue.ToDictionary(e => e.key, e => (e.cost, new[] { e.path }.ToList()));

while (startqueue.Count > 0 || endqueue.Count > 0)
{
    if (endqueue.Count > 0)
    {
        var current = endqueue.OrderBy(e => e.cost).First();
        endqueue.Remove(current);

        char[] nextdirs = current.key.d switch
        {
            '>' => ['>', '^', 'v'],
            '<' => ['<', '^', 'v'],
            'v' => ['v', '<', '>'],
            '^' => ['^', '<', '>'],
            _ => throw new InvalidOperationException()
        };

        List<(((int x, int y) p, char d) key, int cost, List<((int x, int y) p, char d)> path)> nexts =
            nextdirs
                .Select(d => (p: d switch
                {
                    '>' => (current.key.p.x - 1, current.key.p.y),
                    '<' => (current.key.p.x + 1, current.key.p.y),
                    'v' => (current.key.p.x, current.key.p.y - 1),
                    '^' => (current.key.p.x, current.key.p.y + 1),
                    _ => throw new InvalidOperationException()
                }, d, c: d == current.key.d ? 1 : 1001))
                .ExceptBy(walls, n => n.p)
                .ExceptBy(current.path.Select(p => p.p), n => n.p)
                .Select(n => (key: (n.p, n.d), cost: current.cost + n.c, path: current.path.Append((n.p, n.d)).ToList()))
                .Where(n => !ends.TryGetValue(n.key, out var p) || p.cost >= n.cost)
                .ToList();

        foreach (var next in nexts)
        {
            if (ends.TryGetValue(next.key, out var last))
            {
                if (next.cost < last.cost)
                {
                    last.cost = next.cost;
                    last.paths.Clear();
                }
                last.paths.Add(next.path);
            }
            else
            {
                ends[next.key] = (next.cost, [next.path]);
            }

            if (!starts.ContainsKey(next.key))
            {
                endqueue.Add((next.key, next.cost, next.path));
            }
        }
    }

    if (startqueue.Count > 0)
    {
        var current = startqueue.OrderBy(e => e.cost).First();
        startqueue.Remove(current);

        char[] nextdirs = current.key.d switch
        {
            '>' => ['>', '^', 'v'],
            '<' => ['<', '^', 'v'],
            'v' => ['v', '<', '>'],
            '^' => ['^', '<', '>'],
            _ => throw new InvalidOperationException()
        };

        List<(((int x, int y) p, char d) key, int cost, List<((int x, int y) p, char d)> path)> nexts =
            nextdirs
                .Select(d => (p: d switch
                {
                    '>' => (current.key.p.x + 1, current.key.p.y),
                    '<' => (current.key.p.x - 1, current.key.p.y),
                    'v' => (current.key.p.x, current.key.p.y + 1),
                    '^' => (current.key.p.x, current.key.p.y - 1),
                    _ => throw new InvalidOperationException()
                }, d, c: d == current.key.d ? 1 : 1001))
                .ExceptBy(walls, n => n.p)
                .ExceptBy(current.path.Select(p => p.p), n => n.p)
                .Select(n => (key: (n.p, n.d), cost: current.cost + n.c, path: current.path.Append((n.p, n.d)).ToList()))
                .Where(n => !starts.TryGetValue(n.key, out var p) || p.cost >= n.cost)
                .ToList();

        foreach (var next in nexts)
        {
            if (starts.TryGetValue(next.key, out var last))
            {
                if (next.cost < last.cost)
                {
                    last.cost = next.cost;
                    last.paths.Clear();
                }
                last.paths.Add(next.path);
            }
            else
            {
                starts[next.key] = (next.cost, [next.path]);
            }

            if (!ends.ContainsKey(next.key))
            {
                startqueue.Add((next.key, next.cost, next.path));
            }
        }
    }
}

var bests = starts.Join(ends, s => s.Key, e => e.Key, (s, e) => (junction: s.Key.p, cost: s.Value.cost + e.Value.cost, s: s.Value.paths, e: e.Value.paths))
    .GroupBy(j => j.cost)
    .OrderBy(j => j.Key).First();

foreach (var best in bests)
{
    PrintTrack(best.s.SelectMany(s => s).Concat(best.e.SelectMany(e => e)).ToList(), best.junction);
}

var bestpaths = bests.SelectMany(b => b.s.SelectMany(s => s).Select(p => p.p).Concat(b.e.SelectMany(e => e).Select(p => p.p))).Distinct().ToList();
PrintTrack(bestpaths.Select(b => (b, 'O')).ToList());
Console.WriteLine(new { bestpaths = bestpaths.Count() });
