static bool Invoke(ref ulong rega, ref ulong regb, ref ulong regc, out uint outval)
{
    regb = rega % 8;
    regb ^= 1;
    regc = rega / 32;
    regc ^= regb;
    rega /= 8;
    regb ^= 4;
    outval = (uint)(regb % 8);
    return rega is 0;
}

ulong rega = (ulong)(6 * Math.Pow(8, 8));
ulong regb = 0;
ulong regc = 0;

var outputs = new List<uint>();

bool ended = false;
while (!ended)
{
    ended = Invoke(ref rega, ref regb, ref regc, out uint outval);
    outputs.Add(outval);
}

Console.WriteLine(string.Join(',', outputs));
