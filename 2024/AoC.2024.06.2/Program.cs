var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var grid = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (x, y, c))).ToDictionary(g => (g.x, g.y), g => g.c);
var maxx = grid.Keys.Max(g => g.x);
var maxy = grid.Keys.Max(g => g.y);
var start = grid.Single(g => g.Value == '^').Key;

var starting = GetPositions((int.MinValue, int.MinValue));

HashSet<(int x, int y, char d)> lastPositions = [];
(int x, int y) lastObstacle = (int.MinValue, int.MinValue);
int result = 0;

foreach (var pos in starting.positions.Where(p => p.x < maxx && p.d is '>' or '┌' && grid.Any(g => g.Key.x == p.x && g.Key.y > p.y && g.Value is '#')))
{
    var obstacle = (pos.x + 1, pos.y);
    var test = GetPositions(obstacle);
    if (test.looping)
    {
        result++;
        lastPositions = test.positions;
        lastObstacle = obstacle;
    }
}

foreach (var pos in starting.positions.Where(p => p.x > 0 && p.d is '<' or '┘' && grid.Any(g => g.Key.x == p.x && g.Key.y < p.y && g.Value is '#')))
{
    var obstacle = (pos.x - 1, pos.y);
    var test = GetPositions(obstacle);
    if (test.looping)
    {
        result++;
        lastPositions = test.positions;
        lastObstacle = obstacle;
    }
}

foreach (var pos in starting.positions.Where(p => p.y < maxy && p.d is 'v' or '┐' && grid.Any(g => g.Key.x < p.x && g.Key.y == p.y && g.Value is '#')))
{
    var obstacle = (pos.x, pos.y + 1);
    var test = GetPositions(obstacle);
    if (test.looping)
    {
        result++;
        lastPositions = test.positions;
        lastObstacle = obstacle;
    }
}

foreach (var pos in starting.positions.Where(p => p.y > 0 && p.d is '^' or '└' && grid.Any(g => g.Key.x > p.x && g.Key.y == p.y && g.Value is '#')))
{
    var obstacle = (pos.x, pos.y - 1);
    var test = GetPositions(obstacle);
    if (test.looping)
    {
        result++;
        lastPositions = test.positions;
        lastObstacle = obstacle;
    }
}

PrintPositions(lastPositions, lastObstacle);
Console.WriteLine(new { result });

(HashSet<(int x, int y, char d)> positions, bool looping) GetPositions((int x, int y) obstacle)
{
    var positions = new HashSet<(int x, int y, char d)>();
    bool looping = false;
    var pos = (start.x, start.y, d: '^');

    while (true)
    {
        var next = GetNext(pos);
        pos.d = grid.TryGetValue((next.x, next.y), out char c) && c is '#' || (next.x, next.y) == obstacle
            ? pos.d switch { '^' => '┌', 'v' => '┘', '<' => '└', '>' => '┐', _ => throw new InvalidOperationException() } : pos.d;
        if (positions.Contains(pos))
        {
            looping = true;
            break;
        }
        else
        {
            positions.Add(pos);
        }
        if (next.x < 0 || next.x > maxx || next.y < 0 || next.y > maxy) break;

        if (pos.d is '┌' or '┘' or '└' or '┐')
        {
            pos.d = pos.d switch
            {
                '└' => '^',
                '┌' => '>',
                '┐' => 'v',
                '┘' => '<',
                _ => pos.d
            };
            next = GetNext(pos);
        }
        pos = (next.x, next.y, pos.d);
    }
    return (positions, looping);
}

void PrintPositions(HashSet<(int x, int y, char d)> positions, (int x, int y) obstacle)
{
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x < maxx; x++)
        {
            var paths = positions.Where(p => p.x == x && p.y == y).Select(p => p.d).ToArray();
            Console.Write((x, y) == start
                ? '^'
                : (x, y) == obstacle
                    ? 'O'
                    : paths.Length switch
                    {
                        0 => grid[(x, y)],
                        1 when paths[0] is '<' or '>' => '─',
                        1 when paths[0] is '^' or 'v' => '│',
                        1 => paths[0],
                        _ => '┼'
                    });
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

static (int x, int y, char d) GetNext((int x, int y, char d) pos)
{
    return pos.d switch
    {
        '^' => (pos.x, pos.y - 1, pos.d),
        '>' => (pos.x + 1, pos.y, pos.d),
        'v' => (pos.x, pos.y + 1, pos.d),
        '<' => (pos.x - 1, pos.y, pos.d),
        _ => throw new InvalidOperationException()
    };
}

// 2258 is too high
// 2120 is too low
// 2126 is too low
