﻿var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);
var split = Array.IndexOf(lines, "");

var grid = lines[..split].SelectMany((l, y) => l.Select((c, x) => (k: (x, y), c))).ToDictionary(g => g.k, g => g.c);

var maxx = grid.Keys.Max(k => k.x);
var maxy = grid.Keys.Max(k => k.y);
var robot = grid.Single(g => g.Value == '@').Key;
grid[robot] = '.';

void PrintGrid()
{
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x <= maxx; x++)
        {
            var p = (x, y);
            Console.Write(p == robot ? '@' : grid[p]);
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
        '^' => (robot.x, robot.y - 1),
        'v' => (robot.x, robot.y + 1),
        '<' => (robot.x - 1, robot.y),
        '>' => (robot.x + 1, robot.y),
        _ => throw new InvalidOperationException()
    };

    if (grid[next] is not '#')
    {
        if (grid[next] is not 'O')
        {
            robot = next;
        }
        else
        {
            var space = (m switch
            {
                '^' => grid.Where(g => g.Key.x == robot.x && g.Key.y < robot.y).OrderByDescending(g => g.Key.y),
                'v' => grid.Where(g => g.Key.x == robot.x && g.Key.y > robot.y).OrderBy(g => g.Key.y),
                '<' => grid.Where(g => g.Key.x < robot.x && g.Key.y == robot.y).OrderByDescending(g => g.Key.x),
                '>' => grid.Where(g => g.Key.x > robot.x && g.Key.y == robot.y).OrderBy(g => g.Key.x),
                _ => throw new InvalidOperationException()
            }).TakeWhile(g => g.Value is not '#').FirstOrDefault(g => g.Value is '.');
            if (space.Value is not default(char))
            {
                grid[space.Key] = 'O';
                robot = next;
                grid[robot] = '.';
            }
        }
    }

    if (Debugger.IsAttached) Console.WriteLine(m);
    if (Debugger.IsAttached) PrintGrid();
    //if (Debugger.IsAttached) Console.ReadLine();
}

PrintGrid();

var result = grid.Where(g => g.Value is 'O').Sum(g => g.Key.x + g.Key.y * 100);

Console.WriteLine(new { result });
