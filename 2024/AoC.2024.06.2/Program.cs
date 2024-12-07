var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var grid = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (x, y, c))).ToDictionary(g => (g.x, g.y), g => g.c);
var maxx = grid.Keys.Max(g => g.x);
var maxy = grid.Keys.Max(g => g.y);
var start = grid.Single(g => g.Value == '^').Key;
var obstacles = grid.Where(g => g.Value == '#').Select(g => g.Key).ToList();

var starting = GetPositions(obstacles);
(List<((int x, int y) pos, char dir)> positions, ((int x, int y) pos, char dir) loop) lastTest = default;
(int x, int y) lastObstacle = default;
int result = 0;
var path = starting.positions.Select(p => p.pos).Distinct().ToList();
int count = 0;

foreach (var obstacle in path)
{
    Console.WriteLine($"{++count}/{path.Count}");
    var test = GetPositions(obstacles.Append(obstacle).ToList());
    if (test.loop != default)
    {
        result++;
        lastTest = test;
        lastObstacle = obstacle;
    }
}

PrintPositions(lastTest.positions!, lastObstacle, lastTest.loop);
Console.WriteLine(new { result });

(List<((int x, int y) pos, char dir)> positions, ((int x, int y) pos, char dir) loop) GetPositions(List<(int x, int y)> obstacles)
{
    var positions = new List<((int x, int y) pos, char dir)>();
    var pos = (pos: (start.x, start.y), dir: '^');

    while (true)
    {
        var next = GetPathNext(pos, obstacles);
        if (next.dir != pos.dir)
            if (obstacles.Contains(next.pos))
            {
                pos.dir = pos.dir switch
                {
                    '^' => '╦',
                    'v' => '╩',
                    '<' => '╠',
                    '>' => '╣',
                    _ => throw new InvalidOperationException()
                };
                next = pos.dir switch
                {
                    '╦' => ((pos.pos.x, pos.pos.y + 1), 'v'),
                    '╩' => ((pos.pos.x, pos.pos.y - 1), '^'),
                    '╠' => ((pos.pos.x + 1, pos.pos.y), '>'),
                    '╣' => ((pos.pos.x - 1, pos.pos.y), '<'),
                    _ => throw new InvalidOperationException()
                };  
            }
            else
            {
                pos.dir = pos.dir switch
                {
                    '^' => '┌',
                    'v' => '┘',
                    '<' => '└',
                    '>' => '┐',
                    _ => throw new InvalidOperationException()
                };
            }
        if (positions.Contains(pos))
        {
            return (positions, pos);
        }
        positions.Add(pos);
        if (next.pos.x < 0 || next.pos.x > maxx || next.pos.y < 0 || next.pos.y > maxy) break;
        pos = next;
    }
    return (positions, default);
}

void PrintPositions(List<((int x, int y) pos, char dir)> positions, (int x, int y) obstacle, ((int x, int y) pos, char dir) loop = default)
{
    var defaultColor = Console.ForegroundColor;
    var loopPath = loop == default ? [] : positions[positions.IndexOf(loop)..].Select(p => p.pos).Distinct().ToList();
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x <= maxx; x++)
        {
            if (loopPath.Contains((x, y)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = defaultColor;
            }
            var paths = positions.Where(p => p.pos.x == x && p.pos.y == y).Select(p => p.dir).ToArray();
            Console.Write((x, y) == start
                ? '^'
                : (x, y) == obstacle
                    ? 'O'
                    : paths.Length switch
                    {
                        0 => grid[(x, y)],
                        1 when paths[0] is '<' or '>' or '╠' or '╣' => '─',
                        1 when paths[0] is '^' or 'v' or '╦' or '╩' => '│',
                        1 => paths[0],
                        > 1 when paths.All(p => p is '<' or '>') => '─',
                        > 1 when paths.All(p => p is '^' or 'v') => '│',
                        > 1 when paths.All(p => p is '^' or 'v' or '└' or '╠') => '├',
                        > 1 when paths.All(p => p is '^' or 'v' or '┐' or '╣') => '┤',
                        > 1 when paths.All(p => p is '<' or '>' or '┘' or '╩') => '┴',
                        > 1 when paths.All(p => p is '<' or '>' or '┌' or '╦') => '┬',
                        _ => '┼'
                    });
        }
        Console.WriteLine();
    }
    Console.WriteLine();
    Console.ForegroundColor = defaultColor;
}

((int x, int y) pos, char dir) GetPathNext(((int x, int y) pos, char dir) path, List<(int x, int y)> obstacles)
{
    var next = path.dir switch
    {
        '^' => (pos: (path.pos.x, path.pos.y - 1), path.dir),
        '>' => (pos: (path.pos.x + 1, path.pos.y), path.dir),
        'v' => (pos: (path.pos.x, path.pos.y + 1), path.dir),
        '<' => (pos: (path.pos.x - 1, path.pos.y), path.dir),
        _ => throw new InvalidOperationException()
    };
    if (obstacles.Contains(next.pos))
    {
        next = path.dir switch
        {
            '^' => (pos: (path.pos.x + 1, path.pos.y), '>'),
            '>' => (pos: (path.pos.x, path.pos.y + 1), 'v'),
            'v' => (pos: (path.pos.x - 1, path.pos.y), '<'),
            '<' => (pos: (path.pos.x, path.pos.y - 1), '^'),
            _ => throw new InvalidOperationException()
        };
    }
    return next;
}
