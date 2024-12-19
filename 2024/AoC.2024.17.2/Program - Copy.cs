var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);

ulong rega = ulong.Parse(lines[0][12..]);
ulong regb = ulong.Parse(lines[1][12..]);
ulong regc = ulong.Parse(lines[2][12..]);

var ops = lines[4][9..].Split(',').Select(uint.Parse).ToArray();

static uint? Invoke(uint[] ops, ref uint inst, ref ulong rega, ref ulong regb, ref ulong regc, out string desc, bool debug = false)
{
    var op = ops[inst];

    ulong oper = op is 1 or 3 or 4
        ? ops[inst + 1]
        : ops[inst + 1] switch { var o and <= 3 => o, 4 => rega, 5 => regb, 6 => regc, _ => throw new Exception() };

    uint? oval = null;
    var dval = new StringBuilder(debug ? $"[{(inst / 2) + 1,2}] " : "");
    inst += 2;

    switch (op)
    {
        case 0:
            if (debug) dval.Append($"adv: {rega} / {Math.Pow(2, oper)}");
            rega = (ulong)(rega / Math.Pow(2, oper));
            if (debug) dval.Append($" = {rega}  (rega)");
            break;
        case 1:
            if (debug) dval.Append($"bxl: {regb} ^ {oper}");
            regb = regb ^ oper;
            if (debug) dval.Append($" = {regb} (regb)");
            break;
        case 2:
            if (debug) dval.Append($"bst: {oper} % 8");
            regb = oper % 8;
            if (debug) dval.Append($" = {regb} (regb)");
            break;
        case 3:
            if (debug) dval.Append($"jnz: {rega} is 0");
            if (rega is not 0)
            {
                inst = (uint)oper;
                if (debug) dval.Append($" -> {inst}");
            }
            else
            {
                if (debug) dval.Append(" (noop)");
            }
            break;
        case 4:
            if (debug) dval.Append($"bxc: {regb} ^ {regc}");
            regb = regb ^ regc;
            if (debug) dval.Append($" = {regb} (regb)");
            break;
        case 5:
            if (debug) dval.Append($"out: {oper} % 8");
            oval = (uint?)(oper % 8);
            if (debug) dval.Append($" => {oval}");
            break;
        case 6:
            if (debug) dval.Append($"bdv: {rega} / {Math.Pow(2, oper)}");
            regb = (ulong)(rega / Math.Pow(2, oper));
            if (debug) dval.Append($" = {regb}  (regb)");
            break;
        case 7:
            if (debug) dval.Append($"cdv: {rega} / {Math.Pow(2, oper)}");
            regc = (ulong)(rega / Math.Pow(2, oper));
            if (debug) dval.Append($" = {regc}  (regc)");
            break;
        default:
            if (debug) dval.Append("noop");
            break;
    }
    desc = dval.ToString();
    return oval;
}

rega = 9200740906;
var incr = 2U;
var bestlen = 0;
while (true)
{
    List<uint> outputs = [];

    {
        uint intr = 0;
        ulong rega2 = rega, regb2 = rega, regc2 = regc;
        List<string> cmds = [];
        while (intr < ops.Length)
        {
            var target = ops[outputs.Count];
            var o = Invoke(ops, ref intr, ref rega2, ref regb2, ref regc2, out string desc);
            cmds.Add(desc);
            if (o.HasValue)
            {
                outputs.Add(o.Value);
                if (o != target || outputs.Count == ops.Length)
                    break;
            }
        }
    }

    if (outputs.Count > bestlen)
    {
        bestlen = outputs.Count;

        uint intr = 0;
        ulong rega2 = rega, regb2 = rega, regc2 = regc;
        List<string> cmds = [];
        int debugoutput = 0;
        while (debugoutput < outputs.Count)
        {
            if (Invoke(ops, ref intr, ref rega2, ref regb2, ref regc2, out string desc, true) is not null) debugoutput++;
            cmds.Add(desc);
        }
        Console.WriteLine(new { rega, rega2, regb2, regc2, intr, output = string.Join(',', outputs) });
        for (int i = 0; i < cmds.Count; i++)
            Console.WriteLine($"({i + 1,3}) {cmds[i]}");
        if (outputs.SequenceEqual(ops))
            break;
        //Console.ReadLine();
        Console.WriteLine();
    }
    rega += incr;
}
Console.WriteLine(new { rega });

// Program: 2,4, 1,1, 7,5, 4,6, 0,3, 1,4, 5,5, 3,0
/*
 *  1 bst 4
 *  2 bxl 1
 *  3 cdv 5
 *  4 bxc  regb = regb ^ regc
 *  5 adv  rega / 8
 *  6 bxl  regb = regb ^ 4
 *  7 out  => regb % 8
 *  8 jnz  if rega = 0 goto 0
 */

/*
8 jnz rega=0
7 out regb % 8 = 0      a=0, b=0, c=0
6 bxl regb ^ 4 = 0      regb is multiple of 4 and 10
5 adv rega / 8
4 bxc regb = regb ^ regc    
*/

//{ rega = 40380970,  rega2 = 2, regb2 = 2, regc2 = 4, intr = 14, output = 2,4,1,1,7,5,4,2 }
//{ rega = 73935402,  rega2 = 0, regb2 = 1, regc2 = 0, intr = 14, output = 2,4,1,1,7,5,4,6,1 }
//{ rega = 610806314, rega2 = 0, regb2 = 1, regc2 = 0, intr = 14, output = 2,4,1,1,7,5,4,6,0,1 }

/*
{ rega = 5174209066, rega2 = 0, regb2 = 1, regc2 = 0, intr = 16, output = 2,4,1,1,7,5,4,6,0,3,1 }
(  1) bst: 5174209066 % 8 = 2 (regb)
(  2) bxl: 2 ^ 2 = 3 (regb)
(  3) cdv: 5174209066 / 8 = 646776133  (regc)
(  4) bxc: 3 ^ 646776133 = 646776134 (regb)
(  5) adv: 5174209066 / 8 = 646776133  (rega)
(  6) bxl: 646776134 ^ 646776134 = 646776130 (regb)
(  7) out: 646776130 % 8 => 2
(  8) jnz: 646776133 is 0 -> 0
(  9) bst: 646776133 % 8 = 5 (regb)
( 10) bxl: 5 ^ 5 = 4 (regb)
( 11) cdv: 646776133 / 16 = 40423508  (regc)
( 12) bxc: 4 ^ 40423508 = 40423504 (regb)
( 13) adv: 646776133 / 8 = 80847016  (rega)
( 14) bxl: 40423504 ^ 40423504 = 40423508 (regb)
( 15) out: 40423508 % 8 => 4
( 16) jnz: 80847016 is 0 -> 0
( 17) bst: 80847016 % 8 = 0 (regb)
( 18) bxl: 0 ^ 0 = 1 (regb)
( 19) cdv: 80847016 / 2 = 40423508  (regc)
( 20) bxc: 1 ^ 40423508 = 40423509 (regb)
( 21) adv: 80847016 / 8 = 10105877  (rega)
( 22) bxl: 40423509 ^ 40423509 = 40423505 (regb)
( 23) out: 40423505 % 8 => 1
( 24) jnz: 10105877 is 0 -> 0
( 25) bst: 10105877 % 8 = 5 (regb)
( 26) bxl: 5 ^ 5 = 4 (regb)
( 27) cdv: 10105877 / 16 = 631617  (regc)
( 28) bxc: 4 ^ 631617 = 631621 (regb)
( 29) adv: 10105877 / 8 = 1263234  (rega)
( 30) bxl: 631621 ^ 631621 = 631617 (regb)
( 31) out: 631617 % 8 => 1
( 32) jnz: 1263234 is 0 -> 0
( 33) bst: 1263234 % 8 = 2 (regb)
( 34) bxl: 2 ^ 2 = 3 (regb)
( 35) cdv: 1263234 / 8 = 157904  (regc)
( 36) bxc: 3 ^ 157904 = 157907 (regb)
( 37) adv: 1263234 / 8 = 157904  (rega)
( 38) bxl: 157907 ^ 157907 = 157911 (regb)
( 39) out: 157911 % 8 => 7
( 40) jnz: 157904 is 0 -> 0
( 41) bst: 157904 % 8 = 0 (regb)
( 42) bxl: 0 ^ 0 = 1 (regb)
( 43) cdv: 157904 / 2 = 78952  (regc)
( 44) bxc: 1 ^ 78952 = 78953 (regb)
( 45) adv: 157904 / 8 = 19738  (rega)
( 46) bxl: 78953 ^ 78953 = 78957 (regb)
( 47) out: 78957 % 8 => 5
( 48) jnz: 19738 is 0 -> 0
( 49) bst: 19738 % 8 = 2 (regb)
( 50) bxl: 2 ^ 2 = 3 (regb)
( 51) cdv: 19738 / 8 = 2467  (regc)
( 52) bxc: 3 ^ 2467 = 2464 (regb)
( 53) adv: 19738 / 8 = 2467  (rega)
( 54) bxl: 2464 ^ 2464 = 2468 (regb)
( 55) out: 2468 % 8 => 4
( 56) jnz: 2467 is 0 -> 0
( 57) bst: 2467 % 8 = 3 (regb)
( 58) bxl: 3 ^ 3 = 2 (regb)
( 59) cdv: 2467 / 4 = 616  (regc)
( 60) bxc: 2 ^ 616 = 618 (regb)
( 61) adv: 2467 / 8 = 308  (rega)
( 62) bxl: 618 ^ 618 = 622 (regb)
( 63) out: 622 % 8 => 6
( 64) jnz: 308 is 0 -> 0
( 65) bst: 308 % 8 = 4 (regb)
( 66) bxl: 4 ^ 4 = 5 (regb)
( 67) cdv: 308 / 32 = 9  (regc)
( 68) bxc: 5 ^ 9 = 12 (regb)
( 69) adv: 308 / 8 = 38  (rega)
( 70) bxl: 12 ^ 12 = 8 (regb)
( 71) out: 8 % 8 => 0
( 72) jnz: 38 is 0 -> 0
( 73) bst: 38 % 8 = 6 (regb)
( 74) bxl: 6 ^ 6 = 7 (regb)
( 75) cdv: 38 / 128 = 0  (regc)
( 76) bxc: 7 ^ 0 = 7 (regb)
( 77) adv: 38 / 8 = 4  (rega)
( 78) bxl: 7 ^ 7 = 3 (regb)
( 79) out: 3 % 8 => 3
( 80) jnz: 4 is 0 -> 0
( 81) bst: 4 % 8 = 4 (regb)
( 82) bxl: 4 ^ 4 = 5 (regb)
( 83) cdv: 4 / 32 = 0  (regc)
( 84) bxc: 5 ^ 0 = 5 (regb)
( 85) adv: 4 / 8 = 0  (rega)
( 86) bxl: 5 ^ 5 = 1 (regb)
( 87) out: 1 % 8 => 1
( 88) jnz: 0 is 0 (noop)
 */

/*
{ rega = 9200740906, rega2 = 0, regb2 = 5, regc2 = 1, intr = 14, output = 2,4,1,1,7,5,4,6,0,3,1,5 }
(  1) bst: 9200740906 % 8 = 2 (regb)
(  2) bxl: 2 ^ 2 = 3 (regb)
(  3) cdv: 9200740906 / 8 = 1150092613  (regc)
(  4) bxc: 3 ^ 1150092613 = 1150092614 (regb)
(  5) adv: 9200740906 / 8 = 1150092613  (rega)
(  6) bxl: 1150092614 ^ 1150092614 = 1150092610 (regb)
(  7) out: 1150092610 % 8 => 2
(  8) jnz: 1150092613 is 0 -> 0
(  9) bst: 1150092613 % 8 = 5 (regb)
( 10) bxl: 5 ^ 5 = 4 (regb)
( 11) cdv: 1150092613 / 16 = 71880788  (regc)
( 12) bxc: 4 ^ 71880788 = 71880784 (regb)
( 13) adv: 1150092613 / 8 = 143761576  (rega)
( 14) bxl: 71880784 ^ 71880784 = 71880788 (regb)
( 15) out: 71880788 % 8 => 4
( 16) jnz: 143761576 is 0 -> 0
( 17) bst: 143761576 % 8 = 0 (regb)
( 18) bxl: 0 ^ 0 = 1 (regb)
( 19) cdv: 143761576 / 2 = 71880788  (regc)
( 20) bxc: 1 ^ 71880788 = 71880789 (regb)
( 21) adv: 143761576 / 8 = 17970197  (rega)
( 22) bxl: 71880789 ^ 71880789 = 71880785 (regb)
( 23) out: 71880785 % 8 => 1
( 24) jnz: 17970197 is 0 -> 0
( 25) bst: 17970197 % 8 = 5 (regb)
( 26) bxl: 5 ^ 5 = 4 (regb)
( 27) cdv: 17970197 / 16 = 1123137  (regc)
( 28) bxc: 4 ^ 1123137 = 1123141 (regb)
( 29) adv: 17970197 / 8 = 2246274  (rega)
( 30) bxl: 1123141 ^ 1123141 = 1123137 (regb)
( 31) out: 1123137 % 8 => 1
( 32) jnz: 2246274 is 0 -> 0
( 33) bst: 2246274 % 8 = 2 (regb)
( 34) bxl: 2 ^ 2 = 3 (regb)
( 35) cdv: 2246274 / 8 = 280784  (regc)
( 36) bxc: 3 ^ 280784 = 280787 (regb)
( 37) adv: 2246274 / 8 = 280784  (rega)
( 38) bxl: 280787 ^ 280787 = 280791 (regb)
( 39) out: 280791 % 8 => 7
( 40) jnz: 280784 is 0 -> 0
( 41) bst: 280784 % 8 = 0 (regb)
( 42) bxl: 0 ^ 0 = 1 (regb)
( 43) cdv: 280784 / 2 = 140392  (regc)
( 44) bxc: 1 ^ 140392 = 140393 (regb)
( 45) adv: 280784 / 8 = 35098  (rega)
( 46) bxl: 140393 ^ 140393 = 140397 (regb)
( 47) out: 140397 % 8 => 5
( 48) jnz: 35098 is 0 -> 0
( 49) bst: 35098 % 8 = 2 (regb)
( 50) bxl: 2 ^ 2 = 3 (regb)
( 51) cdv: 35098 / 8 = 4387  (regc)
( 52) bxc: 3 ^ 4387 = 4384 (regb)
( 53) adv: 35098 / 8 = 4387  (rega)
( 54) bxl: 4384 ^ 4384 = 4388 (regb)
( 55) out: 4388 % 8 => 4
( 56) jnz: 4387 is 0 -> 0
( 57) bst: 4387 % 8 = 3 (regb)
( 58) bxl: 3 ^ 3 = 2 (regb)
( 59) cdv: 4387 / 4 = 1096  (regc)
( 60) bxc: 2 ^ 1096 = 1098 (regb)
( 61) adv: 4387 / 8 = 548  (rega)
( 62) bxl: 1098 ^ 1098 = 1102 (regb)
( 63) out: 1102 % 8 => 6
( 64) jnz: 548 is 0 -> 0
( 65) bst: 548 % 8 = 4 (regb)
( 66) bxl: 4 ^ 4 = 5 (regb)
( 67) cdv: 548 / 32 = 17  (regc)
( 68) bxc: 5 ^ 17 = 20 (regb)
( 69) adv: 548 / 8 = 68  (rega)
( 70) bxl: 20 ^ 20 = 16 (regb)
( 71) out: 16 % 8 => 0
( 72) jnz: 68 is 0 -> 0
( 73) bst: 68 % 8 = 4 (regb)
( 74) bxl: 4 ^ 4 = 5 (regb)
( 75) cdv: 68 / 32 = 2  (regc)
( 76) bxc: 5 ^ 2 = 7 (regb)
( 77) adv: 68 / 8 = 8  (rega)
( 78) bxl: 7 ^ 7 = 3 (regb)
( 79) out: 3 % 8 => 3
( 80) jnz: 8 is 0 -> 0
( 81) bst: 8 % 8 = 0 (regb)
( 82) bxl: 0 ^ 0 = 1 (regb)
( 83) cdv: 8 / 2 = 4  (regc)
( 84) bxc: 1 ^ 4 = 5 (regb)
( 85) adv: 8 / 8 = 1  (rega)
( 86) bxl: 5 ^ 5 = 1 (regb)
( 87) out: 1 % 8 => 1
( 88) jnz: 1 is 0 -> 0
( 89) bst: 1 % 8 = 1 (regb)
( 90) bxl: 1 ^ 1 = 0 (regb)
( 91) cdv: 1 / 1 = 1  (regc)
( 92) bxc: 0 ^ 1 = 1 (regb)
( 93) adv: 1 / 8 = 0  (rega)
( 94) bxl: 1 ^ 1 = 5 (regb)
( 95) out: 5 % 8 => 5
 */