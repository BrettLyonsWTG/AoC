var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);
var packets = new List<(string left, string right)>();
for (int i = 0; i < lines.Length; i += 3) { packets.Add((lines[i], lines[i + 1])); }

var result = packets.Select((p, i) =>
    {
        Console.WriteLine(new { i = i + 1, p.left, p.right });
        var r = (i: i + 1, c: ComparePackets(ParsePacket(p.left), ParsePacket(p.right)) is not 1);
        //Console.WriteLine(new { r, p.left, p.right });
        return r;
    })
    .Where(p => p.c).Sum(p => p.i);

Console.WriteLine(new { result });

int ComparePackets(PacketData left, PacketData right)
{
    int result = 0;

    if (left.Value.HasValue && right.Value.HasValue)
        result = Comparer<int>.Default.Compare(left.Value.Value, right.Value.Value);
    else
    {
        var leftVals = left.Value.HasValue ? [left] : left.List;
        var rigthVals = right.Value.HasValue ? [right] : right.List;

        int i = 0;
        for (; i < Math.Min(leftVals.Count, rigthVals.Count); i++)
        {
            result = ComparePackets(leftVals[i], rigthVals[i]);
            if (result != 0)
                break;
        }

        if (result == 0)
            result = rigthVals.Count < leftVals.Count ? 1 : 0;
    }
    Console.WriteLine($"  left : {left}");
    Console.WriteLine($"  right: {right}");
    Console.WriteLine($"       = {result}");
    return result;
}

static PacketData ParsePacket(ReadOnlySpan<char> packet)
{
    var result = new PacketData();

    while (result.Size < packet.Length)
    {
        if (packet[result.Size] is ',')
        {
            result.Size++;
        }
        if (packet[result.Size] is '[')
        {
            result.Size++;
            var subPacket = ParsePacket(packet[result.Size..]);
            if (result.Size == 1)
            {
                result = subPacket;
            }
            else
            {
                result.List.Add(subPacket);
            }
            result.Size += subPacket.Size;
        }
        else if (packet[result.Size] is ']')
        {
            result.Size++;
            break;
        }
        else
        {
            var next = packet[result.Size..].IndexOfAnyExceptInRange('0', '9');
            if (next > 0)
            {
                result.List.Add(new PacketData { Value = int.Parse(packet[result.Size..(result.Size + next)]), Size = next - result.Size });
                result.Size += next;
            }
        }
    }

    return result;
}

class PacketData
{
    public int Size { get; set; } = 0;
    public int? Value { get; set; } = null;
    public List<PacketData> List { get; set; } = [];

    public override string ToString() => Value?.ToString() ?? $"[{string.Join(",", List)}]";
}

// 3920 is too low