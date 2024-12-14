var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var machines = new List<((int x, int y) a, (int x, int y) b, (int x, int y) p)>();
var lines = File.ReadAllLines(file);
for (int i = 0; i < lines.Length; i += 4)
{
    var matcha = Regex.Match(lines[i], @"Button A: X\+(?<x>\d+), Y\+(?<y>\d+)");
    var a = (int.Parse(matcha.Groups["x"].Value), int.Parse(matcha.Groups["y"].Value));
    var matchb = Regex.Match(lines[i + 1], @"Button B: X\+(?<x>\d+), Y\+(?<y>\d+)");
    var b = (int.Parse(matchb.Groups["x"].Value), int.Parse(matchb.Groups["y"].Value));
    var matchp = Regex.Match(lines[i + 2], @"Prize: X=(?<x>\d+), Y=(?<y>\d+)");
    var p = (int.Parse(matchp.Groups["x"].Value), int.Parse(matchp.Groups["y"].Value));
    machines.Add((a, b, p));
}

var tokens = machines.Sum(m => Enumerable.Range(0, Math.Min(m.p.x / m.a.x, m.p.y / m.a.y) + 1).Select(a => (a, x: m.a.x * a, y: m.a.y * a))
        .Select(a => (a, dx: m.p.x - a.x))
        .Where(a => a.dx % m.b.x is 0)
        .Select(a => (a.a.a, b: a.dx / m.b.x))
        .Select(c => (c.a, c.b, i: (x: c.a * m.a.x, y: c.a * m.a.y), p: (x: c.a * m.a.x + c.b * m.b.x, y: c.a * m.a.y + c.b * m.b.y)))
        .Where(c => c.p == m.p)
        .Select(c => { Console.WriteLine(c); return c.a * 3 + c.b; })
        .DefaultIfEmpty()
        .Min());

Console.WriteLine(new { tokens });
