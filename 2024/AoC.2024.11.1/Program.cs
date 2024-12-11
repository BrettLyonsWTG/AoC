var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var stones = File.ReadAllText(file).Trim().Split().Select(long.Parse).ToList();

if (Debugger.IsAttached) Console.WriteLine($"Initial arrangement:");
if (Debugger.IsAttached) Console.WriteLine(string.Join(" ", stones));
if (Debugger.IsAttached) Console.WriteLine();

for (int b = 1; b <= (Debugger.IsAttached ? 6 : 75); b++)
{
    Console.WriteLine(new { b, stones.Count });
    for (int s = 0; s < stones.Count; s++)
    {
        if (stones[s] == 0)
        {
            stones[s] = 1;
            continue;
        }
        var digits = stones[s].ToString().Length;
        if (digits % 2 == 0)
        {
            var before = stones[s];
            var div = (long)Math.Pow(10, digits / 2);
            var left = stones[s] / div;
            var right = stones[s] % div;
            stones[s] = left;
            stones.Insert(++s, right);
            continue;
        }

        stones[s] *= 2024;
    }
    if (Debugger.IsAttached) Console.WriteLine($"After {b} blinks:");
    if (Debugger.IsAttached) Console.WriteLine(string.Join(" ", stones));
    if (Debugger.IsAttached) Console.WriteLine();
}

Console.WriteLine(new { stones.Count });
