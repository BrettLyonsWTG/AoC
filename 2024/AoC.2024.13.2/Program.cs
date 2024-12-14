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
    //var p = (long.Parse(matchp.Groups["ix"].Value), long.Parse(matchp.Groups["iy"].Value));
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

// 41046276509329 is too low
// 41046276509329
// 6 = 45943640842967
// 3 = 82261957837868
// 2 = 82261957837868
// 1 = 83136251707811

//Prize: X =            10000000012748, Y = 10000000012176
//((26, 66), (67, 21), (10000000012748,     10000000012176))
//Prize: X =            10000000018641, Y = 10000000010279
//((69, 23), (27, 71), (10000000018641,     10000000010279))


//Console.WriteLine(m);
//Console.WriteLine(new { am, ac, bm, bc, ix, iy, ia, ib });

//List<long> scores = [];
//long minx = (long)Math.Floor(ix / m.a.x) - 1;

//for (long i = minx; i < minx + 3; i++)
//{
//    var a = (i, x: m.a.x * i, y: m.a.y * i);
//    var dx = (m.p.x - a.x);
//    if (dx % m.b.x is 0)
//    {
//        var b = dx / m.b.x;
//        var c = (a, b, i: (x: a.x, y: a.y), p: (x: a.x + b * m.b.x, y: a.y + b * m.b.y));
//        if (c.p == m.p)
//        {
//            scores.Add(i * 3 + b);
//        }
//    }
//}

//var score = scores.DefaultIfEmpty().Min();
//Console.WriteLine(new { score });

//tokens += scores.DefaultIfEmpty().Min();


//Button A: X + 94, Y + 34
//Button B: X + 22, Y + 67
//Prize: X = 8400, Y = 5400

//((double x, double y) a, (double x, double y) b, (double x, double y) p) m = ((2, 1), (1, 2), (4, 4));
//((double x, double y) a, (double x, double y) b, (double x, double y) p) m = machines[0];

//// Line 1: y = m1 * x + c1
//double m1 = m.a.y / m.a.x;
//double c1 = 0;

//// Line 2: y = m2 * x + c2
//double m2 = m.b.y / m.b.x;
//double c2 = m.p.y - (m2 * m.p.x);

//double x = (c2 - c1) / (m1 - m2);
//double y = m1 * x + c1;

//double ia = x / m.a.x;
//double ib = (m.p.x - x) / m.b.x;

//Console.WriteLine(new { m1, c1, m2, c2, x, y, ia, ib });

//if (intersection != null)
//{
//    Console.WriteLine($"Intersection at: ({intersection.Value.x}, {intersection.Value.y})");
//}
//else
//{
//    Console.WriteLine("The lines are parallel and do not intersect.");
//}

//static (double x, double y)? FindIntersection(double m1, double c1, double m2, double c2)
//{
//    // Check if lines are parallel
//    if (m1 == m2)
//    {
//        return null; // Lines are parallel
//    }

//    // Calculate intersection point
//    double x = (c2 - c1) / (m1 - m2);
//    double y = m1 * x + c1;

//    return (x, y);
//}

//Explanation:
//Define the lines: Each line is represented in the slope-intercept form ( y = mx + c ).
//Check for parallel lines: If the slopes ( m1 ) and ( m2 ) are equal, the lines are parallel and do not intersect.
//Calculate the intersection: If the lines are not parallel, solve for ( x ) and ( y ) using the formulas:
//(x = \frac{c2 - c1}{ m1 - m2} )
//(y = m1 \cdot x + c1 )

//This code will output the intersection point of the two lines if they intersect, or indicate that the lines are parallel if they do not intersect.






//var m = machines[0];

//Console.WriteLine(m);
//Console.WriteLine(BY(m.p.x));
//Console.WriteLine(BY(m.p.x - 1));
//Console.WriteLine(BY(m.p.x - 2));
//Console.WriteLine(BY(m.p.x - 22));

//double AY(double x) => m.a.y / m.a.x * x;
//double BY(double x) => m.p.y - m.b.y / m.b.x * (m.p.x - x);


//double ABX() => (m.p.y - m.b.y / m.b.x * (m.p.x - x)) / (m.a.y / m.a.x);

//var tokens = machines.Sum(m => Enumerable.Range(0, (int)Math.Min(m.p.x / m.a.x, m.p.y / m.a.y) + 1).Select(a => (a, x: m.a.x * a, y: m.a.y * a))
//        .Select(a => (a, dx: m.p.x - a.x))
//        .Where(a => a.dx % m.b.x is 0)
//        .Select(a => (a.a.a, b: a.dx / m.b.x))
//        .Select(c => (c.a, c.b, p: (x: c.a * m.a.x + c.b * m.b.x, y: c.a * m.a.y + c.b * m.b.y)))
//        .Where(c => c.p == m.p)
//        .Select(c => c.a * 3 + c.b)
//        .DefaultIfEmpty()
//        .Min());

//double tokens = 0;

//var m = machines[0];

//var max = Math.Min(m.p.x / m.b.x, m.p.y / m.b.y) + 1;

//for (double i = 0; i < length; i++)
//{

//}

//machines.Sum(m => Enumerable.Range(0, ).Select(a => (a, x: m.a.x * a, y: m.a.y * a))
//        .Select(a => (a, dx: m.p.x - a.x))
//        .Where(a => a.dx % m.b.x is 0)
//        .Select(a => (a.a.a, b: a.dx / m.b.x))
//        .Select(c => (c.a, c.b, p: (x: c.a * m.a.x + c.b * m.b.x, y: c.a * m.a.y + c.b * m.b.y)))
//        .Where(c => c.p == m.p)
//        .Select(c => c.a * 3 + c.b)
//        .DefaultIfEmpty()
//        .Min());


//Console.WriteLine(new { tokens, max });
