var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var grid = File.ReadAllLines(file)
    .SelectMany((line, y) => line.Select((h, x) => new { x, y, h = h - '0' }))
    .ToDictionary(g => (g.x, g.y), g => g.h);
int maxx = grid.Keys.Max(k => k.x);
int maxy = grid.Keys.Max(k => k.y);

var best = grid.Max(tree =>
{
    int i, up = 0, down = 0, left = 0, right = 0;

    for (i = tree.Key.y; i > 0; i--) { up++; if (grid[(tree.Key.x, i - 1)] >= tree.Value) break; }
    for (i = tree.Key.y; i < maxy; i++) { down++; if (grid[(tree.Key.x, i + 1)] >= tree.Value) break; }
    for (i = tree.Key.x; i > 0; i--) { left++; if (grid[(i - 1, tree.Key.y)] >= tree.Value) break; }
    for (i = tree.Key.x; i < maxx; i++) { right++; if (grid[(i + 1, tree.Key.y)] >= tree.Value) break; }

    return up * down * left * right;
});

Console.WriteLine(new { best });
