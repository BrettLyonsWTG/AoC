var file = Debugger.IsAttached ? "input.txt" : "input.txt";

var grid = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (p: (x, y), c))).ToDictionary(p => p.p, p => p.c);

var start = grid.Single(p => p.Value == 'S').Key;
var end = grid.Single(p => p.Value == 'E').Key;
grid[start] = 'a';
grid[end] = 'z';
var maxx = grid.Max(p => p.Key.x);
var maxy = grid.Max(p => p.Key.y);

int steps = 0;
var dones = new List<(int x, int y)>();
var moves = new List<((int x, int y) from, (int x, int y) to)>();
var spots = new (int x, int y)[] { start };

while (true)
{
    dones.AddRange(spots);

    var nexts = spots.SelectMany(s => 
        new (int x, int y)[] { (s.x + 1, s.y), (s.x - 1, s.y), (s.x, s.y + 1), (s.x, s.y - 1) }
            .Where(p => p.x >= 0 && p.y >= 0 && p.x <= maxx && p.y <= maxy)
            .Except(dones).Where(p => grid[p] - grid[s] <= 1)
            .Select(p => (from: s, to: p)))
        .ToArray();
    moves.AddRange(nexts);

    spots = nexts.Select(n => n.to).Distinct().ToArray();
    steps++;

    if (spots.Contains(end))
    {
        dones.AddRange(spots);
        break;
    }
}

var path = new List<(int x, int y)>();
var current = end;
while (current != start)
{
    path.Add(current);
    current = moves.First(m => m.to == current).from;
}
path.Add(start);

PrintPath(dones, path);
Console.WriteLine(new { steps });

void PrintPath(List<(int x, int y)> dones, List<(int x, int y)> path)
{
    var defaultColor = Console.ForegroundColor;
    for (var y = 0; y <= maxy; y++)
    {
        for (var x = 0; x <= maxx; x++)
        {
            var p = (x, y);
            if (path.Contains(p)) Console.ForegroundColor = ConsoleColor.Red;
            else if (dones.Contains(p)) Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(grid[p]);
            Console.ForegroundColor = defaultColor;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}
