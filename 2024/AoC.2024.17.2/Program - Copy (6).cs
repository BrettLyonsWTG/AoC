static void Invoke(ref ulong rega, ref ulong regb, ref ulong regc, out uint outval, bool debug = false)
{
    StringBuilder? log = debug ? new() : null;

    log?.AppendLine($"sta:                   ; {rega,19:F0}; {regb,19:F0}; {regc,19:F0};");
    // [1] bst: A % 8->B
    regb = rega % 8;
    log?.AppendLine($"bst [2,4]: A % 8 -> B  ; {rega,19:F0}; {regb,19:F0}; {regc,19:F0};");
    // [2] bxl: B ^ 1->B
    regb ^= 1;
    log?.AppendLine($"bxl [1,1]: B ^ 1 -> B  ; {rega,19:F0}; {regb,19:F0}; {regc,19:F0};");
    // [3] cdv: A / 32->C
    regc = rega / 32;
    log?.AppendLine($"cdv [7,5]: A / 32 -> C ; {rega,19:F0}; {regb,19:F0}; {regc,19:F0};");
    // [4] bxc: B ^ C->C
    regc ^= regb;
    log?.AppendLine($"cdv [4,6]: B ^ C -> C  ; {rega,19:F0}; {regb,19:F0}; {regc,19:F0};");
    // [5] adv: A / 8->A
    rega /= 8;
    log?.AppendLine($"adv [0,3]: A / 8 -> A  ; {rega,19:F0}; {regb,19:F0}; {regc,19:F0};");
    // [6] bxl: B ^ 4->B
    regb ^= 4;
    log?.AppendLine($"bxl [1,4]: B ^ 4 -> B  ; {rega,19:F0}; {regb,19:F0}; {regc,19:F0};");
    // [7] out: B % 8->Out
    outval = (uint)(regb % 8);
    log?.AppendLine($"out [5,5]: B % 8 -> Out; {rega,19:F0}; {regb,19:F0}; {regc,19:F0}; {outval,4}");
    // [8] jnz: A is 0->Quit
    log?.AppendLine($"jnz: A is 0 -> Quit    ; {rega,19:F0}; {regb,19:F0}; {regc,19:F0};");

    if (debug)
        Console.WriteLine(log);
}

ulong rega = (ulong)(6 * Math.Pow(8, 8));
ulong regb = 0;
ulong regc = 0;

var outputs = new List<uint>();

while (rega > 0)
{
    Invoke(ref rega, ref regb, ref regc, out uint outval, true);
    outputs.Add(outval);
    Console.WriteLine();
}

Console.WriteLine(string.Join(',', outputs));

// Program: 2,4, 1,1, 7,5, 4,6, 0,3, 1,4, 5,5, 3,0
/*
[ 1] bst: A % 8 -> B
[ 2] bxl: B ^ 1 -> B
[ 3] cdv: A / 32 -> C
[ 4] bxc: B ^ C -> C
[ 5] adv: A / 8 -> A
[ 6] bxl: B ^ 4 -> B
[ 7] out: B % 8 -> Out
[ 8] jnz: A is 0 -> Quit
*/

/*
{ rega = 73935405, rega2 = 0, regb2 = 1, regc2 = 0, intr = 14, output = 2,4,1,1,7,5,4,6,1 }
(  1) [ 1] bst: 73935405 % 8 = 5 (regb)
(  2) [ 2] bxl: 5 ^ 1 = 4 (regb)
(  3) [ 3] cdv: 73935405 / 16 = 4620962  (regc)
(  4) [ 4] bxc: 4 ^ 4620962 = 4620966 (regb)
(  5) [ 5] adv: 73935405 / 8 = 9241925  (rega)
(  6) [ 6] bxl: 4620966 ^ 4 = 4620962 (regb)
(  7) [ 7] out: 4620962 % 8 => 2
(  8) [ 8] jnz: 9241925 is 0 -> 0
(  9) [ 1] bst: 9241925 % 8 = 5 (regb)
( 10) [ 2] bxl: 5 ^ 1 = 4 (regb)
( 11) [ 3] cdv: 9241925 / 16 = 577620  (regc)
( 12) [ 4] bxc: 4 ^ 577620 = 577616 (regb)
( 13) [ 5] adv: 9241925 / 8 = 1155240  (rega)
( 14) [ 6] bxl: 577616 ^ 4 = 577620 (regb)
( 15) [ 7] out: 577620 % 8 => 4
( 16) [ 8] jnz: 1155240 is 0 -> 0
( 17) [ 1] bst: 1155240 % 8 = 0 (regb)
( 18) [ 2] bxl: 0 ^ 1 = 1 (regb)
( 19) [ 3] cdv: 1155240 / 2 = 577620  (regc)
( 20) [ 4] bxc: 1 ^ 577620 = 577621 (regb)
( 21) [ 5] adv: 1155240 / 8 = 144405  (rega)
( 22) [ 6] bxl: 577621 ^ 4 = 577617 (regb)
( 23) [ 7] out: 577617 % 8 => 1
( 24) [ 8] jnz: 144405 is 0 -> 0
( 25) [ 1] bst: 144405 % 8 = 5 (regb)
( 26) [ 2] bxl: 5 ^ 1 = 4 (regb)
( 27) [ 3] cdv: 144405 / 16 = 9025  (regc)
( 28) [ 4] bxc: 4 ^ 9025 = 9029 (regb)
( 29) [ 5] adv: 144405 / 8 = 18050  (rega)
( 30) [ 6] bxl: 9029 ^ 4 = 9025 (regb)
( 31) [ 7] out: 9025 % 8 => 1
( 32) [ 8] jnz: 18050 is 0 -> 0
( 33) [ 1] bst: 18050 % 8 = 2 (regb)
( 34) [ 2] bxl: 2 ^ 1 = 3 (regb)
( 35) [ 3] cdv: 18050 / 8 = 2256  (regc)
( 36) [ 4] bxc: 3 ^ 2256 = 2259 (regb)
( 37) [ 5] adv: 18050 / 8 = 2256  (rega)
( 38) [ 6] bxl: 2259 ^ 4 = 2263 (regb)
( 39) [ 7] out: 2263 % 8 => 7
( 40) [ 8] jnz: 2256 is 0 -> 0
( 41) [ 1] bst: 2256 % 8 = 0 (regb)
( 42) [ 2] bxl: 0 ^ 1 = 1 (regb)
( 43) [ 3] cdv: 2256 / 2 = 1128  (regc)
( 44) [ 4] bxc: 1 ^ 1128 = 1129 (regb)
( 45) [ 5] adv: 2256 / 8 = 282  (rega)
( 46) [ 6] bxl: 1129 ^ 4 = 1133 (regb)
( 47) [ 7] out: 1133 % 8 => 5
( 48) [ 8] jnz: 282 is 0 -> 0
( 49) [ 1] bst: 282 % 8 = 2 (regb)
( 50) [ 2] bxl: 2 ^ 1 = 3 (regb)
( 51) [ 3] cdv: 282 / 8 = 35  (regc)
( 52) [ 4] bxc: 3 ^ 35 = 32 (regb)
( 53) [ 5] adv: 282 / 8 = 35  (rega)
( 54) [ 6] bxl: 32 ^ 4 = 36 (regb)
( 55) [ 7] out: 36 % 8 => 4
( 56) [ 8] jnz: 35 is 0 -> 0
( 57) [ 1] bst: 35 % 8 = 3 (regb)
( 58) [ 2] bxl: 3 ^ 1 = 2 (regb)
( 59) [ 3] cdv: 35 / 4 = 8  (regc)
( 60) [ 4] bxc: 2 ^ 8 = 10 (regb)
( 61) [ 5] adv: 35 / 8 = 4  (rega)
( 62) [ 6] bxl: 10 ^ 4 = 14 (regb)
( 63) [ 7] out: 14 % 8 => 6
( 64) [ 8] jnz: 4 is 0 -> 0
( 65) [ 1] bst: 4 % 8 = 4 (regb)
( 66) [ 2] bxl: 4 ^ 1 = 5 (regb)
( 67) [ 3] cdv: 4 / 32 = 0  (regc)
( 68) [ 4] bxc: 5 ^ 0 = 5 (regb)
( 69) [ 5] adv: 4 / 8 = 0  (rega)
( 70) [ 6] bxl: 5 ^ 4 = 1 (regb)
( 71) [ 7] out: 1 % 8 => 1
*/
