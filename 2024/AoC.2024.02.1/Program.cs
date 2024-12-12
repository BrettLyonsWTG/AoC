var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var result = File.ReadLines(file).Count(l =>
{
    var levels = l.Split().Select(int.Parse).ToArray();
    int? vector = null;
    if (Debugger.IsAttached) Console.WriteLine(new { l });
    return Enumerable.Range(0, levels.Length - 1).All(i =>
    {
        vector ??= levels[i + 1] - levels[i];
        var safe = vector switch
        {
            > 0 => levels[i + 1] - levels[i] is > 0 and <= 3,
            < 0 => levels[i + 1] - levels[i] is < 0 and >= -3,
            _ => false
        };
        if (Debugger.IsAttached) Console.WriteLine(new { i, vector, safe });
        return safe;
    });
});

Console.WriteLine(new { result });