var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var grid = File.ReadAllLines(file)
    .SelectMany((line, y) => line.Select((h, x) => new { x, y, h = h - '0' }))
    .ToDictionary(g => (g.x, g.y), g => g.h);
int maxx = grid.Keys.Max(k => k.x);
int maxy = grid.Keys.Max(k => k.y);

var visibile = grid.Count(tree =>
{
    grid.Where(g => g.Key.x == tree.Key.x && g.Key.y < tree.Key.y).OrderByDescending(g => g.Key.y).TakeWhile(g => g.Value).ToList().ForEach(g => Console.WriteLine(g));

    var axes = grid.Where(forest => forest.Key.x == tree.Key.x || forest.Key.y == tree.Key.y);
    return axes.Where(left => left.Key.x < tree.Key.x).All(left => left.Value < tree.Value)
        || axes.Where(right => right.Key.x > tree.Key.x).All(right => right.Value < tree.Value)
        || axes.Where(up => up.Key.y < tree.Key.y).All(up => up.Value < tree.Value)
        || axes.Where(down => down.Key.y > tree.Key.y).All(down => down.Value < tree.Value);
});

Console.WriteLine(new { visibile });
