var file = Debugger.IsAttached ? "example.txt" : "input.txt";

long result = 0;

foreach (var line in File.ReadAllLines(file))
{
    var value = long.Parse(line[..line.IndexOf(':')]);
    List<(long n, long p)> numbers = [];
    var numberSpan = line[(line.IndexOf(':') + 2)..].AsSpan();
    foreach (var split in numberSpan.Split(' '))
    {
        numbers.Add((long.Parse(numberSpan[split]), split.GetOffsetAndLength(numberSpan.Length).Length));
    }

    var combos = new List<List<char>>([['+'], ['*'], ['|']]);
    for (long i = 2; i < numbers.Count; i++)
    {
        combos = combos.Select(c => c.Append('+').ToList())
            .Concat(combos.Select(c => c.Append('*').ToList()))
            .Concat(combos.Select(c => c.Append('|').ToList())).ToList();
    }

    foreach (var combo in combos)
    {
        long calc = numbers[0].n;
        for (int i = 0; i < combo.Count; i++)
        {
            calc = combo[i] switch
            {
                '+' => calc + numbers[i + 1].n,
                '*' => calc * numbers[i + 1].n,
                '|' => calc * (long)Math.Pow(10, numbers[i + 1].p) + numbers[i + 1].n,
                _ => throw new NotImplementedException()
            };
        }
        if (calc == value)
        {
            Console.WriteLine($"{line} > {string.Join(',', combo)}");
            result += value;
            break;
        }
    }
}

Console.WriteLine(new { result });
