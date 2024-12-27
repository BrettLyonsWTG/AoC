var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);

ulong rega = ulong.Parse(lines[0][12..]);
ulong regb = ulong.Parse(lines[1][12..]);
ulong regc = ulong.Parse(lines[2][12..]);

var ops = lines[4][9..].Split(',').Select(uint.Parse).ToArray();
uint inst = 0;

static uint? Invoke(uint[] ops, ref uint inst, ref ulong rega, ref ulong regb, ref ulong regc)
{
    var op = ops[inst];

    ulong oper = op is 1 or 3 or 4
        ? ops[inst + 1]
        : ops[inst + 1] switch { var o and <= 3 => o, 4 => rega, 5 => regb, 6 => regc, _ => throw new Exception() };

    inst += 2;

    switch (op)
    {
        case 0:
            rega = (ulong)(rega / Math.Pow(2, oper));
            break;
        case 1:
            regb = regb ^ oper;
            break;
        case 2:
            regb = oper % 8;
            break;
        case 3:
            if (rega is not 0)
                inst = (uint)oper;
            break;
        case 4:
            regb = regb ^ regc;
            break;
        case 5:
            return (uint?)oper % 8;
        case 6:
            regb = (ulong)(rega / Math.Pow(2, oper));
            break;
        case 7:
            regc = (ulong)(rega / Math.Pow(2, oper));
            break;
    }
    return null;
}

List<uint> outputs = [];

while (inst < ops.Length)
{
    if (Invoke(ops, ref inst, ref rega, ref regb, ref regc) is uint o)
        outputs.Add(o);
}

Console.WriteLine(new { inst, rega, regb, regc, output = string.Join(',', outputs) });
