var file = Debugger.IsAttached ? "example.txt" : "input.txt";

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

var starts = new Dictionary<((int x, int y) p, char d), (int cost, List<List<((int x, int y) p, char d)>> paths)>();
var ends = new Dictionary<((int x, int y) p, char d), (int cost, List<List<((int x, int y) p, char d)>> paths)>();

var bestcost = int.MaxValue;
var bests = new Dictionary<((int x, int y) p, char d), (List<List<((int x, int y) p, char d)>> starts, List<List<((int x, int y) p, char d)>> ends)>();

List<(((int x, int y) p, char d) key, (int c, int cost, List<((int x, int y) p, char d)> path) value)> startqueue = [((start, '>'), (0, 0, [(start, '>')]))];

var endings = new ((int x, int y) p, char d, char e, int c)[]
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
        }))
    .ToArray();

List<(((int x, int y) p, char d) key, (int c, int cost, List<((int x, int y) p, char d)> path) value)> endqueue 
    = endings.Select(e => (key: (e.p, e.d), value: (e.c, e.c, path: new[] { (e.p, e.d), (end, e.e) }.ToList()))).ToList();

while (startqueue.Count > 0 || endqueue.Count > 0)
{
    if (startqueue.Count > 0)
    {
        var curstart = startqueue.OrderBy(q => q.value.cost).First();
        startqueue.Remove(curstart);
        List<(((int x, int y) p, char d) key, (int c, int cost, List<((int x, int y) p, char d)> path) value)> nextstarts =
            new[]
            {
            (p: (curstart.key.p.x + 1, curstart.key.p.y), d: '>' ),
            (p: (curstart.key.p.x, curstart.key.p.y + 1), d: 'v' ),
            (p: (curstart.key.p.x - 1, curstart.key.p.y), d: '<' ),
            (p: (curstart.key.p.x, curstart.key.p.y - 1), d: '^' )
            }
            .ExceptBy(walls, n => n.p)
            .ExceptBy(curstart.value.path.Select(p => p.p), n => n.p)
            .Select(n => (n, path: curstart.value.path.Append(n).ToList(), c: n.d == curstart.value.path.Last().d ? 1 : 1001))
            .Select(n => (key: n.n, value: (n.c, cost: curstart.value.cost + n.c, n.path)))
            .Where(n => n.value.cost <= bestcost && (!starts.TryGetValue(n.key, out var p) || p.cost >= n.value.cost))
            .ToList();

        foreach (var nextstart in nextstarts.ExceptBy(ends.Keys, s => s.key))
        {
            var exists = starts.TryGetValue(nextstart.key, out var e);
            if (!exists || nextstart.value.cost <= e.cost)
            {
                if (!exists || nextstart.value.cost < e.cost)
                {
                    starts[nextstart.key] = (nextstart.value.cost, [nextstart.value.path]);
                }
                else
                {
                    starts[nextstart.key].paths.Add(nextstart.value.path);
                }
                if (ends.ContainsKey(nextstart.key))
                {
                    var cost = nextstart.value.cost + ends[nextstart.key].cost;
                    if (cost <= bestcost)
                    {
                        if (cost < bestcost)
                        {
                            bestcost = cost;
                            bests.Clear();
                        }

                        if (!bests.ContainsKey(nextstart.key))
                        {
                            bests[nextstart.key] = ([nextstart.value.path], ends[nextstart.key].paths);
                        }
                        else
                        {
                            bests[nextstart.key].starts.Add(nextstart.value.path);
                        }
                    }
                }
                else
                {
                    startqueue.Add(nextstart);
                }
            }
        }
    }

    if (endqueue.Count > 0)
    {
        var curend = endqueue.OrderBy(q => q.value.cost).First();
        endqueue.Remove(curend);
        List<(((int x, int y) p, char d) key, (int c, int cost, List<((int x, int y) p, char d)> path) value)> nextends =
            new[]
            {
                (p: (curend.key.p.x + 1, curend.key.p.y), d: 'v', c: 1001),
                (p: (curend.key.p.x + 1, curend.key.p.y), d: '<', c: 1),
                (p: (curend.key.p.x + 1, curend.key.p.y), d: '^', c: 1001),
                (p: (curend.key.p.x - 1, curend.key.p.y), d: 'v', c: 1001),
                (p: (curend.key.p.x - 1, curend.key.p.y), d: '>', c: 1),
                (p: (curend.key.p.x - 1, curend.key.p.y), d: '^', c: 1001),
                (p: (curend.key.p.x, curend.key.p.y + 1), d: '>', c: 1001),
                (p: (curend.key.p.x, curend.key.p.y + 1), d: '^', c: 1),
                (p: (curend.key.p.x, curend.key.p.y + 1), d: '<', c: 1001),
                (p: (curend.key.p.x, curend.key.p.y - 1), d: '>', c: 1001),
                (p: (curend.key.p.x, curend.key.p.y - 1), d: 'v', c: 1),
                (p: (curend.key.p.x, curend.key.p.y - 1), d: '<', c: 1001),
            }
            .ExceptBy(walls, n => n.p)
            .ExceptBy(curend.value.path.Select(p => p.p), n => n.p)
            .Select(n => (n, path: curend.value.path.Append((n.p, n.d)).ToList(), n.c))
            .Select(n => (key: (n.n.p, n.n.d), value: (n.c, cost: curend.value.cost + n.c, n.path)))

            .Where(n => n.value.cost <= bestcost && (!ends.TryGetValue(n.key, out var p) || p.cost >= n.value.cost))

            .ToList();

        foreach (var nextend in nextends.ExceptBy(ends.Keys, s => s.key))
        {
            var exists = ends.TryGetValue(nextend.key, out var e);
            if (!exists || nextend.value.cost <= e.cost)
            {
                if (!exists || nextend.value.cost < e.cost)
                {
                    ends[nextend.key] = (nextend.value.cost, [nextend.value.path]);
                }
                else
                {
                    ends[nextend.key].paths.Add(nextend.value.path);
                }
                if (starts.ContainsKey(nextend.key))
                {
                    var cost = nextend.value.cost + starts[nextend.key].cost;
                    if (cost <= bestcost)
                    {
                        PrintTrackBests([starts[nextend.key].paths.First(), nextend.value.path]);
                        Console.WriteLine(new { nextend.key, cost });
                        Console.ReadLine();

                        if (cost < bestcost)
                        {
                            bestcost = cost;
                            bests.Clear();
                        }

                        if (!bests.ContainsKey(nextend.key))
                        {
                            bests[nextend.key] = (starts[nextend.key].paths, [nextend.value.path]);
                        }
                        else
                        {
                            bests[nextend.key].starts.Add(nextend.value.path);
                        }
                    }
                }
                else
                {
                    endqueue.Add(nextend);
                }
            }
        }
    }
}

foreach (var b in bests)
{
    PrintTrack(b.Value.starts[0]);
    PrintTrackBests(b.Value.starts.Concat(b.Value.ends).ToList());
    Console.WriteLine(new { b.Key });
}

Console.WriteLine(new { bestcost });
