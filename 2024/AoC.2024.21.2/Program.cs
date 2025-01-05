using System.Runtime.InteropServices;

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

static (int x, int y) GetDirPos(byte button) => (DirPad)button switch
{
    DirPad.Enter => (2, 0),
    DirPad.Up => (1, 0),
    DirPad.Left => (0, 1),
    DirPad.Down => (1, 1),
    DirPad.Right => (2, 1),
    _ => throw new InvalidOperationException()
};

static IEnumerable<byte> GetNumPresses(char button, ref (int x, int y) pos)
{
    var next = GetNumPos(button);

    var presses = Enumerable.Empty<byte>();

    if (pos.y == 3 && next.x == 0)
    {
        presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Up, pos.y - next.y));
        presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Left, pos.x - next.x));
    }
    else if (pos.x == 0 && next.y == 3)
    {
        presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Right, next.x - pos.x));
        presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Down, next.y - pos.y));
    }
    else
    {
        if (next.x < pos.x) presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Left, pos.x - next.x));
        if (next.y < pos.y) presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Up, pos.y - next.y));
        if (next.y > pos.y) presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Down, next.y - pos.y));
        if (next.x > pos.x) presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Right, next.x - pos.x));
    }
    presses = presses.Append((byte)DirPad.Enter);

    pos = next;
    return presses;
}

IEnumerable<byte> GetDirPresses(byte button, ref (int x, int y) pos)
{
    var next = GetDirPos(button);

    var presses = Enumerable.Empty<byte>();
    if (next.y > pos.y) presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Down, next.y - pos.y));
    if (next.x < pos.x) presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Left, pos.x - next.x));
    if (next.x > pos.x) presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Right, next.x - pos.x));
    if (next.y < pos.y) presses = presses.Concat(Enumerable.Repeat((byte)DirPad.Up, pos.y - next.y));
    presses = presses.Append((byte)DirPad.Enter);

    pos = next;
    return presses;
}

long GetPresses(string code)
{
    var pos = GetNumPos('A');
    var presses = code.SelectMany(c => GetNumPresses(c, ref pos)).ToArray();

    for (int i = 0; i < 25; i++)
    {
        GC.Collect();
        pos = GetDirPos((byte)DirPad.Enter);
        using var mem = new MemoryStream();
        foreach (var press in presses)
        {
            var dirPresses = GetDirPresses(press, ref pos);
            mem.Write(dirPresses.ToArray());
        }
        presses = mem.ToArray();
        Console.WriteLine($"{code}: {i + 1}={presses.Count()}");
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

enum DirPad : byte
{
    Enter = 0,
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4,
}
