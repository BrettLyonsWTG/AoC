var file = Debugger.IsAttached ? "example.txt" : "input.txt";

int p = 0;
int x = 1;

foreach (var op in GetOps(file))
{
    Console.Write(p - x is >= -1 and <= 1 ? '#' : '.');
    x = x + op;
    if (++p % 40 == 0)
    {
        p = 0;
        Console.WriteLine();
    }
}


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