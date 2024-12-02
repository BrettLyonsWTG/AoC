var file = Debugger.IsAttached ? "example.txt" : "input.txt";

int x = 1;

var result = GetOps(file).Select((op, p) =>
{
    int cycle = p + 1;
    int during = x * cycle;
    x = x + op;
    int after = x * cycle;
    return new { cycle, x, during, after };
}).Where(s => (s.cycle - 20) % 40 == 0)
.Select(s =>
{
    if (Debugger.IsAttached) Console.WriteLine(s);
    return s;
})
.Sum(s => s.during);

Console.WriteLine(new { result });

IEnumerable<int> GetOps(string file)
{
    foreach (var line in File.ReadLines(file))
    {
        switch (line)
        {
            case "noop":
                yield return 0;
                break;
            default:
                yield return 0;
                yield return int.Parse(line[5..]);
                break;
        }
    }
}