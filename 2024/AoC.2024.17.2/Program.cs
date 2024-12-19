static bool Invoke(ref ulong rega, ref ulong regb, ref ulong regc, out uint outval)
{
    regb = rega % 8;
    regb ^= 1;
    regc = (ulong)(rega / Math.Pow(2, regb));
    regb ^= regc;
    rega /= 8;
    regb ^= 4;
    outval = (uint)(regb % 8);
    return rega is 0;
}

ulong rega = 5;
ulong regb = 0;
ulong regc = 0;

//uint[] targets = [7, 3, 0, 5, 7, 1, 4, 0, 5];
//for (int t = targets.Length - 1; t >= 0; t--)
//{
//    ReverseInvoke(targets[t], ref rega, ref regb, ref regc);
//    Console.WriteLine(new { t = targets[t], rega, regb, regc });
//}

//Console.WriteLine(new { rega });

var outputs = new List<uint>();
bool ended = false;
rega = 28066687;
regb = 0;
regc = 0;
while (!ended)
{
    ended = Invoke(ref rega, ref regb, ref regc, out uint outval);
    Console.WriteLine(new { t = outval, rega, regb, regc });
    outputs.Add(outval);
}
Console.WriteLine(string.Join(',', outputs));
