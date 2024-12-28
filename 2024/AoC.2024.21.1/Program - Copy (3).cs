var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var codes = File.ReadAllLines(file).Select(c => (code: c, num: int.Parse(c[..3]))).ToList();

List<char> GetNumPresses(char button, ref (int x, int y) pos)
{
    (int x, int y) next = button switch
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
    var presses = new List<char>();
    if (next.x > pos.x) presses.AddRange(Enumerable.Repeat('>', next.x - pos.x));
    if (next.x < pos.x) presses.AddRange(Enumerable.Repeat('<', pos.x - next.x));
    if (next.y > pos.y) presses.AddRange(Enumerable.Repeat('>', next.y - pos.y));
    if (next.y < pos.y) presses.AddRange(Enumerable.Repeat('<', pos.y - next.y));

    return presses;
}

IEnumerable<List<char>> GetCombos(IEnumerable<char> input, IEnumerable<char> done)
{
    bool noInput = true;

    foreach (var i in input)
    {
        noInput = false;
        foreach (var combo in GetCombos(input.Except([i]), done.Append(i)))
        {
            yield return combo;
        }
    }

    if (noInput)
    {
        yield return done.ToList();
    }
}

//var pos = (2, 3);
//var presses = GetNumPresses('3', ref pos);

var combos = GetCombos(['1', '2', '3', '4'], []).ToList();

foreach (var combo in combos)
{
    Console.WriteLine(string.Join(", ", combo));
}


//long complexity = 0;

//foreach (var (code, num) in codes)
//{
//    var pos = (x: 2, y: 3);
//    var presses = new List<char>();

//    for (var i = 0; i < 4; i++)
//    {
//        (int x, int y) next = code[i] switch
//        {
//            'A' => (2, 3),
//            '0' => (1, 3),
//            '1' => (0, 2),
//            '2' => (1, 2),
//            '3' => (2, 2),
//            '4' => (0, 1),
//            '5' => (1, 1),
//            '6' => (2, 1),
//            '7' => (0, 0),
//            '8' => (1, 0),
//            '9' => (2, 0),
//            _ => throw new InvalidOperationException()
//        };

//        while (pos != next)
//        {
//            var press = (next.x - pos.x, next.y - pos.y) switch
//            {
//                ( > 0, _) => '>',
//                (_, < 0) => '^',
//                (_, > 0) when pos.x > 0 || pos.y < 2 => 'v',
//                ( < 0, _) when pos.x > 1 || pos.y < 3 => '<',
//                _ => throw new InvalidOperationException()
//            };
//            pos = press switch
//            {
//                '^' => (pos.x, pos.y - 1),
//                '<' => (pos.x - 1, pos.y),
//                'v' => (pos.x, pos.y + 1),
//                '>' => (pos.x + 1, pos.y),
//                _ => throw new InvalidOperationException()
//            };
//            presses.Add(press);
//        }

//        presses.Add('A');
//    }

//    var nextPresses = GetForDirKeypad(presses);
//    var lastPresses = GetForDirKeypad(nextPresses);

//    Console.WriteLine(new string([.. lastPresses]));
//    Console.WriteLine(new string([.. nextPresses]));
//    Console.WriteLine(new string([.. presses]));
//    Console.WriteLine($"{code}: {new string([.. lastPresses])}");
//    Console.WriteLine();

//    complexity += lastPresses.Count * num;
//}

//Console.WriteLine(new { complexity });

//List<char> GetForDirKeypad(List<char> inPresses)
//{
//    var pos = (x: 2, y: 0);
//    var outPresses = new List<char>();

//    for (var i = 0; i < inPresses.Count; i++)
//    {
//        (int x, int y) next = inPresses[i] switch
//        {
//            'A' => (2, 0),
//            '^' => (1, 0),
//            '<' => (0, 1),
//            'v' => (1, 1),
//            '>' => (2, 1),
//            _ => throw new InvalidOperationException()
//        };

//        while (pos != next)
//        {
//            var press = (next.x - pos.x, next.y - pos.y) switch
//            {
//                ( > 0, _) => '>',
//                (_, < 0) when pos.x > 0 => '^',
//                (_, > 0) => 'v',
//                ( < 0, _) => '<',
//                _ => throw new InvalidOperationException()
//            };
//            pos = press switch
//            {
//                '^' => (pos.x, pos.y - 1),
//                '<' => (pos.x - 1, pos.y),
//                'v' => (pos.x, pos.y + 1),
//                '>' => (pos.x + 1, pos.y),
//                _ => throw new InvalidOperationException()
//            };
//            outPresses.Add(press);
//        }

//        outPresses.Add('A');
//    }

//    return outPresses;
//}

//string ReverseForDirKeypad(ReadOnlySpan<char> inPresses)
//{
//    var pos = (x: 2, y: 0);
//    var outPresses = new List<char>();

//    for (var i = 0; i < inPresses.Length; i++)
//    {
//        if (inPresses[i] == 'A')
//        {
//            outPresses.Add(pos switch
//            {
//                (2, 0) => 'A',
//                (1, 0) => '^',
//                (0, 1) => '<',
//                (1, 1) => 'v',
//                (2, 1) => '>',
//                _ => throw new InvalidOperationException()
//            });
//            continue;
//        }
//        else
//        {
//            pos = inPresses[i] switch
//            {
//                '^' => (pos.x, pos.y - 1),
//                '<' => (pos.x - 1, pos.y),
//                'v' => (pos.x, pos.y + 1),
//                '>' => (pos.x + 1, pos.y),
//                _ => throw new InvalidOperationException()
//            };
//        }
//    }

//    return new string(outPresses.ToArray());
//}

//string ReverseForNumKeypad(ReadOnlySpan<char> inPresses)
//{
//    var pos = (x: 2, y: 3);
//    var outPresses = new List<char>();

//    for (var i = 0; i < inPresses.Length; i++)
//    {
//        if (inPresses[i] == 'A')
//        {
//            outPresses.Add(pos switch
//            {
//                (2, 3) => 'A',
//                (1, 3) => '0',
//                (0, 2) => '1',
//                (1, 2) => '2',
//                (2, 2) => '3',
//                (0, 1) => '4',
//                (1, 1) => '5',
//                (2, 1) => '6',
//                (0, 0) => '7',
//                (1, 0) => '8',
//                (2, 0) => '9',
//                _ => throw new InvalidOperationException()
//            });
//            continue;
//        }
//        else
//        {
//            pos = inPresses[i] switch
//            {
//                '^' => (pos.x, pos.y - 1),
//                '<' => (pos.x - 1, pos.y),
//                'v' => (pos.x, pos.y + 1),
//                '>' => (pos.x + 1, pos.y),
//                _ => throw new InvalidOperationException()
//            };
//        }
//    }

//    return new string(outPresses.ToArray());
//}

////{
////    var inPresses = "v<<A>>^AvA^Av<<A>>^AAv<A<A>>^AAvAA<^A>Av<A>^AA<A>Av<A<A>>^AAAvA<^A>A";
////    Console.WriteLine(inPresses);
////    var presses1 = ReverseForDirKeypad(inPresses);
////    Console.WriteLine(presses1);
////    var presses2 = ReverseForDirKeypad(presses1);
////    Console.WriteLine(presses2);
////    var presses3 = ReverseForNumKeypad(presses2);
////    Console.WriteLine(presses3);
////}
////{
////    var inPresses = "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A";
////    Console.WriteLine(inPresses);
////    var presses1 = ReverseForDirKeypad(inPresses);
////    Console.WriteLine(presses1);
////    var presses2 = ReverseForDirKeypad(presses1);
////    Console.WriteLine(presses2);
////    var presses3 = ReverseForNumKeypad(presses2);
////    Console.WriteLine(presses3);
////}
//// 253278 is too high
//// 242974 is too high