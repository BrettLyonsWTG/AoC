var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);
var packets = new List<(string left, string right)>();
for (int i = 0; i < lines.Length; i += 3) { packets.Add((lines[i], lines[i + 1])); }

var result = 0;


PacketData ParsePacket(ReadOnlySpan<char> packet)
{

}


class PacketData
{
    public int? Val { get; set; }
    public List<PacketData>? List { get; set; } = null;
}

//for (int i = 0; i < packets.Count; i++)
//{
//    var start = packets[i];

//    if (ComparePackets(packets[i].left, packets[i].right))
//        result += i + 1;
//}

//static bool ComparePackets(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
//{
//    Console.WriteLine(new { left = left.ToString(), right = right.ToString() });

//    if (right.Length is 0) return left.Length is 0;
//    if (left.Length is 0) return true;
//    if (left[0] is '[' && !right.ContainsAny(['[', ']', ',']))
//        return ComparePackets(left[1..^-1], right);
//    if (!left.ContainsAny(['[', ']', ',']) && right[0] is '[')
//        return ComparePackets(left, right[1..^-1]);
//    if (left[0] is '[' && right[0] is '[')
//        return ComparePackets(left[1..^-1], right[1..^-1]);

//    int l = 0, r = 0;
//    while (l < left.Length && r < right.Length)
//    {

//    }

//    return false;
//}
