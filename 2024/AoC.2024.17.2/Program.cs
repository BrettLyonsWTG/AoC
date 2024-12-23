var file = "input.txt";

var lines = File.ReadAllLines(file);

long rega = long.Parse(lines[0][12..]);

var ops = lines[4][9..].Split(',').Select(int.Parse).ToArray();

static int Invoke(ref long rega, out string? log, bool debug = false)
{
    var logger = debug ? new StringBuilder() : null;

    logger?.AppendLine($"sta     : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega})");

    int regb = (int)(rega & 7);
    logger?.AppendLine($"B=A&7   : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb})");

    regb = regb ^ 1;
    logger?.AppendLine($"B=B^1   : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb})");

    int regc = (int)((rega >> regb) & 7);
    logger?.AppendLine($"C=A>>B  : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb}) C=[{Convert.ToString(regc, 2).PadLeft(3, '0')}] ({regc})");

    regb ^= regc;
    logger?.AppendLine($"B=B^C   : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb}) C=[{Convert.ToString(regc, 2).PadLeft(3, '0')}] ({regc})");

    regb = regb ^ 4;
    logger?.AppendLine($"B=B^4   : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb})");

    rega = rega >> 3;
    logger?.AppendLine($"A=A>>3  : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb}) C=[{Convert.ToString(regc, 2).PadLeft(3, '0')}] ({regc})");

    int outval = regb & 7;
    logger?.AppendLine($"out     : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) O=[{Convert.ToString(outval, 2).PadLeft(3, '0')}] ({outval})");

    log = logger?.ToString();
    return outval;
}

static void ReverseInvoke(ref long rega, int regb, out string? log, bool debug = false)
{
    var logger = debug ? new StringBuilder() : null;

    logger?.AppendLine($"out     : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) O=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb})");

    rega = rega << 3;
    logger?.AppendLine($"A=A<<3  : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb})");

    regb = regb ^ 4;
    logger?.AppendLine($"B=B^4   : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb})");

    rega = rega + regb ^ 4;
    var regc = rega + regb >> regb & 7;
    if ()
    logger?.AppendLine($"B=B^1   : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb}) C=[{Convert.ToString(regc, 2).PadLeft(3, '0')}] ({regc})");

    logger?.AppendLine($"A=A+B   : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega}) B=[{Convert.ToString(regb, 2).PadLeft(3, '0')}] ({regb})");

    logger?.AppendLine($"sta     : A=[{Convert.ToString(rega, 2).PadLeft(3, '0')}] ({rega})");

    log = logger?.ToString();
}

for (int i = 8; i < 9; i++)
{
    rega = i;
    var outval = Invoke(ref rega, out string? log, true);
    if (log != null) Console.WriteLine(log);

    ReverseInvoke(ref rega, outval, out log, true);
    if (log != null) Console.WriteLine(log);
}