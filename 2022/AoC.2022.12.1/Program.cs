﻿var file = Debugger.IsAttached ? "input.txt" : "input.txt";

var grid = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (p: (x, y), c))).ToDictionary(p => p.p, p => p.c);

var start = grid.Single(p => p.Value == 'S').Key;
var end = grid.Single(p => p.Value == 'E').Key;
grid[start] = 'a';
grid[end] = 'z';
var maxx = grid.Max(p => p.Key.x);
var maxy = grid.Max(p => p.Key.y);

var paths = new Dictionary<(int x, int y), List<(int x, int y)>> { [start] = [start] };
var work = new List<(int x, int y)> { start };

var reported = 0;

while (work.Count > 0)
{
    var cur = work.OrderByDescending(w => grid[w]).ThenBy(w => Math.Abs(end.x - w.x) + Math.Abs(end.y - w.y)).First();
    work.Remove(cur);
    var path = paths[cur];

    var nexts = new (int x, int y)[] { (cur.x + 1, cur.y), (cur.x - 1, cur.y), (cur.x, cur.y + 1), (cur.x, cur.y - 1) }
        .Except(paths[cur])
        .Where(n => n.x >= 0 && n.x <= maxx && n.y >= 0 && n.y <= maxy && grid[(n.x, n.y)] - grid[cur] <= 1)
        .ToList();

    foreach (var next in nexts)
    {
        if (paths.TryGetValue(next, out var prev))
        {
            if (path.Count < prev.Count)
            {
                prev.Clear();
                prev.AddRange(path);
                prev.Add(next);
                if (next != end)
                {
                    work.Add(next);
                    //if (prev.Count > reported)
                    {
                        reported = prev.Count;
                        PrintPath(prev);
                        Console.ReadKey(true);
                    }
                }
            }
        }
        else
        {
            paths[next] = new List<(int x, int y)>(path) { next };
            if (next != end)
            {
                work.Add(next);
                //if (paths[next].Count > reported)
                {
                    reported = paths[next].Count;
                    PrintPath(paths[next]);
                    Console.ReadKey(true);
                }
            }
        }
    }
}

PrintPath(paths[end]);

Console.WriteLine(paths[end].Count - 1);

void PrintPath(List<(int x, int y)> path)
{
    var defaultColor = Console.ForegroundColor;

    for (var y = 0; y <= maxy; y++)
    {
        for (var x = 0; x <= maxx; x++)
        {
            var p = (x, y);
            if (path.Contains(p))
            {
                if (p == path.Last())
                    Console.Write('*');
                else
                {
                    var n = path[path.IndexOf(p) + 1];
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write((n.x - p.x, n.y - p.y) switch
                    {
                        (1, 0) => '>',
                        (-1, 0) => '<',
                        (0, 1) => 'v',
                        (0, -1) => '^',
                        _ => throw new InvalidOperationException()
                    });
                }
            }
            else
            {
                if (paths.ContainsKey(p)) Console.ForegroundColor = ConsoleColor.Green;
                else if (work.Contains(p)) Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(grid[p]);
            }
            Console.ForegroundColor = defaultColor;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}