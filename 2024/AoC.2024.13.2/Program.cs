var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var machines = new List<((long x, long y) a, (long x, long y) b, (long x, long y) p)>();
var lines = File.ReadAllLines(file);
for (int i = 0; i < lines.Length; i += 4)
{
    var matcha = Regex.Match(lines[i], @"Button A: X\+(?<ix>\d+), Y\+(?<iy>\d+)");
    var a = (long.Parse(matcha.Groups["ix"].Value), long.Parse(matcha.Groups["iy"].Value));
    var matchb = Regex.Match(lines[i + 1], @"Button B: X\+(?<ix>\d+), Y\+(?<iy>\d+)");
    var b = (long.Parse(matchb.Groups["ix"].Value), long.Parse(matchb.Groups["iy"].Value));
    var matchp = Regex.Match(lines[i + 2], @"Prize: X=(?<ix>\d+), Y=(?<iy>\d+)");
    var p = (long.Parse(matchp.Groups["ix"].Value) + 10000000000000, long.Parse(matchp.Groups["iy"].Value) + 10000000000000);
    machines.Add((a, b, p));
}

long tokens = 0;

foreach (var m in machines)
{
    double am = (double)m.a.y / (double)m.a.x;
    double ac = 0;

    double bm = (double)m.b.y / (double)m.b.x;
    double bc = m.p.y - (m.p.x * bm);

    double ix = (bc - ac) / (am - bm);
    double iy = am * ix;
    int round = 3;
    double ia = double.Round(ix / m.a.x, round, MidpointRounding.AwayFromZero);
    double ib = double.Round((m.p.x - ix) / m.b.x, round, MidpointRounding.AwayFromZero);

    if (double.IsInteger(ia) && double.IsInteger(ib))
    {
        Console.WriteLine(m);
        tokens += (long)(ia * 3 + ib);
    }
}

Console.WriteLine(new { tokens });
