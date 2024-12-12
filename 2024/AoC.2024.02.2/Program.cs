var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var result = File.ReadLines(file).Count(l =>
{
    if (Debugger.IsAttached) Console.WriteLine(new { l });
    var levels = l.Split().Select(int.Parse).ToArray();

    return Enumerable.Range(0, levels.Length).Any(x =>
    {
        int? vector = null;
        var sample = levels.Take(x).Concat(levels.Skip(x + 1)).ToArray();
        if (Debugger.IsAttached) Console.WriteLine(new { x, sample = string.Join(',', sample) });
        return Enumerable.Range(0, sample.Length - 1).All(i =>
        {
            vector ??= sample[i + 1] - sample[i];
            var safe = vector switch
            {
                > 0 => sample[i + 1] - sample[i] is > 0 and <= 3,
                < 0 => sample[i + 1] - sample[i] is < 0 and >= -3,
                _ => false
            };
            if (Debugger.IsAttached) Console.WriteLine(new { i, vector, safe });
            return safe;
        });
    });
});

Console.WriteLine(new { result });