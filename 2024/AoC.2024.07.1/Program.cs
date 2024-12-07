var file = Debugger.IsAttached ? "example.txt" : "input.txt";

long result = 0;

foreach (var line in File.ReadAllLines(file))
{
    var value = long.Parse(line[..line.IndexOf(':')]);
    List<long> numbers = [];
    var numberSpan = line[(line.IndexOf(':') + 2)..].AsSpan();
    foreach (var split in numberSpan.Split(' '))
    {
        numbers.Add(long.Parse(numberSpan[split]));
    }

    var combos = new List<List<char>>([['+'], ['*']]);
    for (long i = 2; i < numbers.Count; i++)
    {
        combos = combos.Select(c => c.Append('+').ToList())
            .Concat(combos.Select(c => c.Append('*').ToList())).ToList();
    }

    foreach (var combo in combos)
    {
        long calc = numbers[0];
        for (int i = 0; i < combo.Count; i++)
        {
            if (combo[i] == '+')
            {
                calc += numbers[i + 1];
            }
            else
            {
                calc *= numbers[i + 1];
            }
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
