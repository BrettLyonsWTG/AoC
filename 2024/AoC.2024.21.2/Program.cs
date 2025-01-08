var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var codes = File.ReadAllLines(file).Select(c => (code: c, num: int.Parse(c[..3]))).ToList();

static (int x, int y) GetNumPos(char button) => button switch
{
    'A' => (2, 3),
    '0' => (1, 3),
    '1' => (0, 2),
    '2' => (1, 2),
    '3' => (2, 2),
    '4' => (0, 1),
    '5' => (1, 1),
    '6' => (2, 1),
    '7' => (0, 0),
    '8' => (1, 0),
    '9' => (2, 0),
    _ => throw new InvalidOperationException()
};

static (int x, int y) GetDirPos(char button) => button switch
{
    'A' => (2, 0),
    '^' => (1, 0),
    '<' => (0, 1),
    'v' => (1, 1),
    '>' => (2, 1),
    _ => throw new InvalidOperationException()
};

static IEnumerable<char> GetNumPresses(char button, ref (int x, int y) pos)
{
    var next = GetNumPos(button);

    var presses = Enumerable.Empty<char>();

    if (pos.y == 3 && next.x == 0)
    {
        presses = presses.Concat(Enumerable.Repeat('^', pos.y - next.y));
        presses = presses.Concat(Enumerable.Repeat('<', pos.x - next.x));
    }
    else if (pos.x == 0 && next.y == 3)
    {
        presses = presses.Concat(Enumerable.Repeat('>', next.x - pos.x));
        presses = presses.Concat(Enumerable.Repeat('v', next.y - pos.y));
    }
    else
    {
        if (next.x < pos.x) presses = presses.Concat(Enumerable.Repeat('<', pos.x - next.x));
        if (next.y < pos.y) presses = presses.Concat(Enumerable.Repeat('^', pos.y - next.y));
        if (next.y > pos.y) presses = presses.Concat(Enumerable.Repeat('v', next.y - pos.y));
        if (next.x > pos.x) presses = presses.Concat(Enumerable.Repeat('>', next.x - pos.x));
    }
    presses = presses.Append('A');

    pos = next;
    return presses;
}

IEnumerable<char> GetDirPresses(char from, char to)
{
    var pos = GetDirPos(from);
    var next = GetDirPos(to);

    var presses = Enumerable.Empty<char>();
    if (next.y > pos.y) presses = presses.Concat(Enumerable.Repeat('v', next.y - pos.y));
    if (next.x < pos.x) presses = presses.Concat(Enumerable.Repeat('<', pos.x - next.x));
    if (next.x > pos.x) presses = presses.Concat(Enumerable.Repeat('>', next.x - pos.x));
    if (next.y < pos.y) presses = presses.Concat(Enumerable.Repeat('^', pos.y - next.y));

    return presses;
}

long GetPresses(string code)
{
    var pos = GetNumPos('A');
    var presses = code.SelectMany(c => GetNumPresses(c, ref pos)).ToArray();
    Console.WriteLine($"{code}: {new string(presses)}");

    for (int i = 0; i < 2; i++)
    {
        var last = 'A';
        presses = presses.SelectMany(c =>
        {
            var nextPresses = Enumerable.Empty<char>();
            if (last != c)
            {
                nextPresses = GetDirPresses(last, c);
                last = c;
            }
            return nextPresses.Append('A');
        }).ToArray();
        Console.WriteLine($"{code}: {i + 1}={new string(presses)}");
    }

    return presses.LongLength;
}

long total = 0;
foreach (var line in File.ReadLines(file))
{
    var presses = GetPresses(line);
    var num = int.Parse(line[..3]);
    Console.WriteLine($"{line}: {presses}");
    total += presses * num;
}
Console.WriteLine(total);

void GetDirPressesComp(string code)
{
    var presses = code.ToCharArray();
    Console.WriteLine($"{code}: {new string(presses)}");

    for (int i = 0; i < 4; i++)
    {
        var last = 'A';
        presses = presses.SelectMany(c =>
        {
            var nextPresses = Enumerable.Empty<char>();
            if (last != c)
            {
                nextPresses = GetDirPresses(last, c);
                last = c;
            }
            return nextPresses.Append('A');
        }).ToArray();
        Console.WriteLine($"{code}: {i + 1}={new string(presses)}");
    }
}

GetDirPressesComp("<");
