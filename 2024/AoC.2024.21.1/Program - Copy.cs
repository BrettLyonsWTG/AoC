var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var codes = File.ReadAllLines(file).Select(c => (code: c, num: int.Parse(c[..3]))).ToList();

long complexity = 0;

foreach (var (code, num) in codes)
{
    var pos = (x: 2, y: 3);
    var presses = new List<char>();

    for (var i = 0; i < 4; i++)
    {
        (int x, int y) next = code[i] switch
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

        if (next.x > pos.x) presses.AddRange(Enumerable.Repeat('>', next.x - pos.x));
        if (next.y < pos.y) presses.AddRange(Enumerable.Repeat('^', pos.y - next.y));
        if (next.x < pos.x) presses.AddRange(Enumerable.Repeat('<', pos.x - next.x));
        if (next.y > pos.y) presses.AddRange(Enumerable.Repeat('v', next.y - pos.y));
        presses.Add('A');

        pos = next;
    }

    var nextPresses = GetNextPresses(presses);
    var lastPresses = GetNextPresses(nextPresses);

    Console.WriteLine(new string([.. lastPresses]));
    Console.WriteLine(new string([.. nextPresses]));
    Console.WriteLine(new string([.. presses]));
    Console.WriteLine($"{code}: {new string([.. lastPresses])}");
    Console.WriteLine();

    complexity += lastPresses.Count * num;
}

Console.WriteLine(new { complexity });

List<char> GetNextPresses(List<char> lastPresses)
{
    var pos = (x: 2, y: 0);
    var presses = new List<char>();

    for (var i = 0; i < lastPresses.Count; i++)
    {
        (int x, int y) next = lastPresses[i] switch
        {
            'A' => (2, 0),
            '^' => (1, 0),
            '<' => (0, 1),
            'v' => (1, 1),
            '>' => (2, 1),
            _ => throw new InvalidOperationException()
        };

        if (next.x > pos.x) presses.AddRange(Enumerable.Repeat('>', next.x - pos.x));
        if (next.y > pos.y) presses.AddRange(Enumerable.Repeat('v', next.y - pos.y));
        if (next.x < pos.x) presses.AddRange(Enumerable.Repeat('<', pos.x - next.x));
        if (next.y < pos.y) presses.AddRange(Enumerable.Repeat('^', pos.y - next.y));
        presses.Add('A');
        pos = next;
    }

    return presses;
}

// 253278 is too high