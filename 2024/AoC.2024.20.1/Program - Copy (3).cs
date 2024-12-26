using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;

var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);
var map = File.ReadLines(file)
    .SelectMany((l, y) => l.Select((c, x) => (c, p: (x, y))))
    .ToDictionary(m => m.p, m => m.c);

var start = map.Where(m => m.Value == 'S').Single().Key;
var end = map.Where(m => m.Value == 'E').Single().Key;
map[start] = '.';
map[end] = '.';
var paths = map.Where(m => m.Value == '.').Select(m => m.Key).ToHashSet();
var walls = map.Where(m => m.Value == '#').Select(m => m.Key).ToHashSet();
var maxx = map.Keys.Max(p => p.x);
var maxy = map.Keys.Max(p => p.y);

var current = new List<(int x, int y)> { start };
var done = new List<(int x, int y)>();
var moves = new List<((int x, int y) from, (int x, int y) to)>();
var moves_lock = new Lock();
var print_lock = new Lock();

while (current.Count > 0)
{
    var nextMoves = current.SelectMany(c => new[] { (c.x, c.y - 1), (c.x, c.y + 1), (c.x - 1, c.y), (c.x + 1, c.y) }
        .Except(done)
        .Except(walls)
        .Select(n => (from: c, to: n)))
        .ToArray();
    moves.AddRange(nextMoves);
    var nexts = nextMoves.Select(n => n.to).Distinct().ToList();
    done.AddRange(nexts);
    current = nexts;
}

void PrintMap(List<(int, int)> path, (int, int) cut = default)
{
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x <= maxx; x++)
        {
            Console.ForegroundColor = (x, y) == cut ? ConsoleColor.Green : path.Contains((x, y)) ? ConsoleColor.Red : ConsoleColor.White;
            Console.BackgroundColor = (x, y) == cut ? ConsoleColor.White : ConsoleColor.Black;
            Console.Write((x, y) == start ? 'S' : (x, y) == end ? 'E' : path.Contains((x, y)) ? 'O' : map[(x, y)]);
            Console.ResetColor();
        }
        Console.WriteLine();
    }
}

List<(int x, int y)> GetMoves((int x, int y) from, (int x, int y)[] to)
{
    var doneMoves = new List<((int x, int y) from, (int x, int y) to)>();
    var current = new List<(int x, int y)> { from };
    (int x, int y) dest = default;
    while (current.Count > 0)
    {
        ((int x, int y) from, (int x, int y) to)[] nextMoves = moves.Join(current, m => m.from, c => c, (m, _) => m)
            .Except(doneMoves)
            .ToArray();
        if (nextMoves.Length == 0)
        {
            lock (moves_lock)
            {
                nextMoves = current.SelectMany(c => new[] { (c.x, c.y - 1), (c.x, c.y + 1), (c.x - 1, c.y), (c.x + 1, c.y) }
                    .Select(n => (from: c, to: n)))
                    .ToArray();
                moves.AddRange(nextMoves.Except(moves));
            }
        }
        doneMoves.AddRange(nextMoves);
        if (nextMoves.Select(n => n.to).Intersect(to).Any())
        {
            dest = nextMoves.Select(n => n.to).Intersect(to).First();
            break;
        }
        current = nextMoves.Select(n => n.to).Distinct().ToList();
    }
    if (current.Count == 0)
    {
        return [];
    }
    else
    {
        var pos = from;
        var path = new List<(int x, int y)>() { from };
        while (pos != dest)
        {
            var next = doneMoves.First(m => m.from == pos).to;
            path.Add(next);
            pos = next;
        }
        return path;
    }
}

var fullPath = GetMoves(start, new[] { end });
//PrintMap(fullPath);
Console.WriteLine(fullPath.Count - 1);
Console.WriteLine();

var cuts = map
    .Where(m => m.Value is '#' && m.Key.x > 1 && m.Key.x < maxx - 1 && map[(m.Key.x - 1, m.Key.y)] == '.' && map[(m.Key.x + 1, m.Key.y)] == '.')
    .Select(m => (cut: m.Key, ends: new[] { (m.Key.x - 1, m.Key.y), (m.Key.x + 1, m.Key.y) }))
    .Concat(map
    .Where(m => m.Value is '#' && m.Key.y > 1 && m.Key.y < maxy - 1 && map[(m.Key.x, m.Key.y - 1)] == '.' && map[(m.Key.x, m.Key.y + 1)] == '.')
    .Select(m => (cut: m.Key, ends: new[] { (m.Key.x, m.Key.y - 1), (m.Key.x, m.Key.y + 1) })))
    .OrderBy(m => m.cut.y)
    .ThenBy(m => m.cut.x);

var result = 0;

Parallel.ForEach(cuts, cut =>
{
    var startToCut = GetMoves(start, cut.ends);
    var cutIn = startToCut.Last();
    var cutOut = cut.ends.Except([cutIn]).First();
    var cutToEnd = GetMoves(cutOut, [end]);
    var path = startToCut.Append(cut.cut).Concat(cutToEnd).ToList();
    var saving = fullPath.Count - path.Count;
    lock (print_lock)
    {
        if (saving >= 100)
        {
            Interlocked.Increment(ref result);
            //PrintMap(path, cut.cut);
            Console.WriteLine(new { cut.cut, result, len = path.Count - 1, saving = fullPath.Count - path.Count });
            //Console.WriteLine();
            //Console.ReadLine();
        }
        else
        {
            Console.WriteLine(new { cut.cut, result });
        }
    }
});

Console.WriteLine(new { result });