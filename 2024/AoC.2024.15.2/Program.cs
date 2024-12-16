var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);
var split = Array.IndexOf(lines, "");

var grid = lines[..split].SelectMany((l, y) => l.SelectMany((c, x) => new[] 
{ 
    (k: (x: x * 2, y), c: c is 'O' ? '[' : c), 
    (k: (x: x * 2 + 1, y), c: c is 'O' ? ']' : c is '@' ? '.' : c) 
})).ToDictionary(g => g.k, g => g.c);

var maxx = grid.Keys.Max(k => k.x);
var maxy = grid.Keys.Max(k => k.y);
var robot = grid.Single(g => g.Value == '@').Key;
grid[robot] = '.';

void PrintGrid(List<(int x, int y)>? highlight = null, ConsoleColor color = ConsoleColor.White)
{
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x <= maxx; x++)
        {
            var p = (x, y);
            if (highlight?.Contains(p) ?? false) Console.ForegroundColor = color;
            Console.Write(p == robot ? '@' : grid[p]);
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

PrintGrid();

var moves = string.Concat(lines[(split + 1)..]).ToCharArray();

foreach (var m in moves)
{
    var next = m switch
    {
        '^' => (x: robot.x, y: robot.y - 1),
        'v' => (x: robot.x, y: robot.y + 1),
        '<' => (x: robot.x - 1, y: robot.y),
        '>' => (x: robot.x + 1, y: robot.y),
        _ => throw new InvalidOperationException()
    };

    if (grid[next] is not '#')
    {
        if (grid[next] is not ('[' or ']'))
        {
            robot = next;
        }
        else if (m is ('<' or '>'))
        {
            var space = (m switch
            {
                '<' => grid.Where(g => g.Key.x < robot.x && g.Key.y == robot.y).OrderByDescending(g => g.Key.x),
                '>' => grid.Where(g => g.Key.x > robot.x && g.Key.y == robot.y).OrderBy(g => g.Key.x),
                _ => throw new InvalidOperationException()
            }).TakeWhile(g => g.Value is not '#').FirstOrDefault(g => g.Value is '.');
            if (space.Value is not default(char))
            {
                var dir = m is '<' ? 1 : -1;
                for (int x = space.Key.x + dir; m is '<' ? x <= robot.x : x >= robot.x ; x += dir)
                {
                    grid[(x - dir, robot.y)] = grid[(x, robot.y)];
                }
                robot = next;
            }
        }
        else if (m is ('^' or 'v'))
        {
            List<(int x, int y)> nexts = [next, (grid[next] is '[' ? next.x + 1 : next.x - 1, next.y)];
            List<(int x, int y)> pushing = [];
            var dir = m is '^' ? -1 : 1;

            while (nexts.Count > 0 && !nexts.Any(n => grid[n] is '#'))
            {
                pushing.AddRange(nexts);
                nexts = nexts.Select(n => (n.x, y: n.y + dir))
                    .SelectMany(n => ((int x, int y)[])(grid[n] switch
                    {
                        '[' => [n, (n.x + 1, n.y)],
                        ']' => [(n.x - 1, n.y), n],
                        '#' => [n],
                        _ => []
                    })).Distinct().ToList();
            }

            if (nexts.Count is 0)
            {
                if (Debugger.IsAttached) Console.WriteLine(m);
                if (Debugger.IsAttached) PrintGrid(pushing, ConsoleColor.Green);
                var vals = pushing.Join(grid, p => p, g => g.Key, (p, g) => g).ToDictionary();
                pushing.ForEach(p => grid[p] = '.');
                pushing.ForEach(p => grid[(p.x, p.y + dir)] = vals[p]);
                var pushed = pushing.Select(p => (p.x, p.y + dir)).ToList();
                robot = next;
                if (Debugger.IsAttached) PrintGrid(pushed, ConsoleColor.Red);
                if (Debugger.IsAttached) Debugger.Break();
            }
        }
    }
}

PrintGrid();

var result = grid.Where(g => g.Value is '[').Sum(g => g.Key.x + g.Key.y * 100);

Console.WriteLine(new { result });
