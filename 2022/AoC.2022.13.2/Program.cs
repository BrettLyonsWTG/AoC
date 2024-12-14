using ILoggerFactory logFactory = LoggerFactory.Create(builder => builder
    .SetMinimumLevel(Debugger.IsAttached ? LogLevel.Debug : LogLevel.Information)
    .AddSimpleConsole(options => options.SingleLine = true));
ILogger log = logFactory.CreateLogger(typeof(Program).Assembly.GetName().Name!);
string file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);
var packets = new List<(string left, string right)>();
for (int i = 0; i < lines.Length; i += 3) { packets.Add((lines[i], lines[i + 1])); }

var result = packets.Select((p, i) =>
{
    var left = ParsePacket(p.left);
    var right = ParsePacket(p.right);
    log.LogDebug("== Pair {i} ==", i);
    log.LogDebug("- Compare {left}", p.left);
    log.LogDebug("       vs {right}", p.right);
    var r = (i: i + 1, c: ComparePackets(left, right) is not 1);
    log.LogDebug("");
    return r;
})
.Where(p => p.c).Sum(p => p.i);

log.LogInformation("The sum of these indices is {result}.", result);

int ComparePackets(PacketData left, PacketData right, int indent = 0)
{
    int result = 0;

    if (indent > 0)
        log.LogDebug("{indent}- Compare {left} vs {right}", new string(' ', indent), left, right);
    if (left.Value.HasValue && right.Value.HasValue)
    {
        result = Comparer<int>.Default.Compare(left.Value.Value, right.Value.Value);
        if (result is -1)
            log.LogDebug("{indent}- Left side is smaller, so inputs are in the right order", new string(' ', indent + 2));
        else if (result is 1)
            log.LogDebug("{indent}- Right side is smaller, so inputs are not in the right order", new string(' ', indent + 2));
    }
    else if (left.Value.HasValue)
    {
        log.LogDebug("{indent}- Mixed types; convert left to [{left}] and retry comparison", new string(' ', indent), left);
        result = ComparePackets(new PacketData { List = [left] }, right, indent + 2);
    }
    else if (right.Value.HasValue)
    {
        log.LogDebug("{indent}- Mixed types; convert right to [{right}] and retry comparison", new string(' ', indent), right);
        result = ComparePackets(left, new PacketData { List = [right] }, indent + 2);
    }
    else
    {
        int i = 0;
        for (; i < Math.Min(left.List.Count, right.List.Count); i++)
        {
            result = ComparePackets(left.List[i], right.List[i], indent + 2);
            if (result != 0)
                break;
        }

        if (result == 0)
        {
            if (right.List.Count < left.List.Count)
            {
                result = 1;
                log.LogDebug("{indent}- Right side ran out of items, so inputs are not in the right order", new string(' ', indent + 2));
            }
            else if (left.List.Count < right.List.Count)
            {
                result = -1;
                log.LogDebug("{indent}- Left side ran out of items, so inputs are in the right order", new string(' ', indent + 2));
            }
        }
    }
    return result;
}

static PacketData ParsePacket(ReadOnlySpan<char> packet, bool sub = false)
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
            var subPacket = ParsePacket(packet[result.Size..], true);
            if (!sub)
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
