string file = Debugger.IsAttached ? "example.txt" : "input.txt";

var packets = File.ReadAllLines(file).Where(l => l.Length > 0).Select(l => new PacketData(l)).ToList();

var div1 = new PacketData("[[2]]");
var div2 = new PacketData("[[6]]");
packets.AddRange([div1, div2]);

packets.Sort();
packets.ForEach(Console.WriteLine);

Console.WriteLine(new { d1 = packets.IndexOf(div1) + 1, d2 = packets.IndexOf(div2) + 1 });
Console.WriteLine(new { key = (packets.IndexOf(div1) + 1L) * (packets.IndexOf(div2) + 1L) });


public class PacketData : IEquatable<PacketData>, IComparable<PacketData>
{
    public int Size { get; set; } = 0;
    public int? Value { get; set; } = null;
    public List<PacketData> List { get; set; } = [];

    public PacketData() { }

    public PacketData(ReadOnlySpan<char> packet, bool sub = false)
    {
        while (Size < packet.Length)
        {
            if (packet[Size] is ',')
            {
                Size++;
            }
            if (packet[Size] is '[')
            {
                Size++;
                var subPacket = new PacketData(packet[Size..], true);
                if (!sub)
                {
                    Size = subPacket.Size;
                    Value = subPacket.Value;
                    List = subPacket.List;
                }
                else
                {
                    List.Add(subPacket);
                }
                Size += subPacket.Size;
            }
            else if (packet[Size] is ']')
            {
                Size++;
                break;
            }
            else
            {
                var next = packet[Size..].IndexOfAnyExceptInRange('0', '9');
                if (next > 0)
                {
                    List.Add(new PacketData { Value = int.Parse(packet[Size..(Size + next)]), Size = next - Size });
                    Size += next;
                }
            }
        }
    }

    public int CompareTo(PacketData? other)
    {
        if (other is null)
            return 1;

        int result = 0;

        if (this.Value.HasValue && other.Value.HasValue)
        {
            result = Comparer<int>.Default.Compare(this.Value.Value, other.Value.Value);
        }
        else if (this.Value.HasValue)
        {
            result = new PacketData { List = [this] }.CompareTo(other);
        }
        else if (other.Value.HasValue)
        {
            result = this.CompareTo(new PacketData { List = [other] });
        }
        else
        {
            int i = 0;
            for (; i < Math.Min(this.List.Count, other.List.Count); i++)
            {
                result = this.List[i].CompareTo(other.List[i]);
                if (result != 0)
                    break;
            }

            if (result == 0)
            {
                if (other.List.Count < this.List.Count)
                {
                    result = 1;
                }
                else if (this.List.Count < other.List.Count)
                {
                    result = -1;
                }
            }
        }

        return result;
    }

    public bool Equals(PacketData? other) => CompareTo(other) == 0;

    public override string ToString() => Value?.ToString() ?? $"[{string.Join(",", List)}]";
}
