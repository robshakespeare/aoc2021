namespace AoC.Day16;

public class Day16Solver : SolverBase
{
    public override string DayName => "Packet Decoder";

    public override long? SolvePart1(PuzzleInput input)
    {
        using var reader = new BitsReader(input);

        var topPacket = ReadPacket(reader);

        return topPacket.TotalPacketVersion;
    }

    public static Packet ReadPacket(BitsReader reader)
    {
        var (packetVersion, packetTypeId) = reader.ReadHeader();

        long? literal = null;
        var subPackets = new List<Packet>();

        if (packetTypeId == 4)
        {
            // Packets with type ID 4 represent a literal value
            literal = reader.ReadLiteral();
        }
        else
        {
            // Every other type of packet (any packet with a type ID other than 4) represent an operator
            // that performs some calculation on one or more sub-packets contained within
            var lengthTypeId = reader.ReadNextBitAsInt();

            if (lengthTypeId == 0)
            {
                // the next 15 bits are a number that represents the total length in bits of the sub-packets contained by this packet
                var subPacketsBitLength = reader.ReadNumber(15);

                var end = reader.CountOfBitsRead + subPacketsBitLength;

                while (reader.CountOfBitsRead < end)
                {
                    subPackets.Add(ReadPacket(reader));
                }
            }
            else
            {
                // the next 11 bits are a number that represents the number of sub-packets immediately contained by this packet
                var numOfSubPackets = reader.ReadNumber(11);

                for (var i = 0; i < numOfSubPackets; i++)
                {
                    subPackets.Add(ReadPacket(reader));
                }
            }
        }

        return new Packet(packetVersion, packetTypeId, literal, subPackets);
    }

    public record Packet(int PacketVersion, int PacketTypeId, long? Literal, IReadOnlyList<Packet> SubPackets)
    {
        public int TotalPacketVersion => PacketVersion + SubPackets.Sum(x => x.TotalPacketVersion);
    }

    public class BitsReader : IDisposable
    {
        private readonly IEnumerator<Bit> _bitReader;

        public int CountOfBitsRead { get; set; }

        public BitsReader(PuzzleInput input)
        {
            _bitReader = ReadBits(ReadBytes(input)).GetEnumerator();
        }

        public void Dispose() => _bitReader.Dispose();

        public (int packetVersion, int packetTypeId) ReadHeader() => (ReadNumber(3), ReadNumber(3));

        public long ReadLiteral()
        {
            var bits = new List<Bit>();

            bool keepReading;
            do
            {
                keepReading = Read();
                bits.AddRange(Read(4));
            } while (keepReading);

            return BitsToLong(bits);
        }

        public int ReadNumber(int bitLength) => BitsToInt(Read(bitLength));

        public int ReadNextBitAsInt() => Read().IsSet ? 1 : 0;

        private Bit Read()
        {
            if (!_bitReader.MoveNext())
            {
                throw new InvalidOperationException("End of transmission reached");
            }

            CountOfBitsRead++;
            return _bitReader.Current;
        }

        private IEnumerable<Bit> Read(int numBits)
        {
            for (var i = 0; i < numBits; i++)
            {
                yield return Read();
            }
        }

        private static int BitsToInt(IEnumerable<Bit> bits) => Convert.ToInt32(string.Join("", bits), 2);

        private static long BitsToLong(IEnumerable<Bit> bits) => Convert.ToInt64(string.Join("", bits), 2);

        private static IEnumerable<byte> ReadBytes(PuzzleInput input) => input.ToString().Select(chr => Convert.ToByte(chr.ToString(), 16));

        private static IEnumerable<Bit> ReadBits(IEnumerable<byte> bytes) =>
            bytes.SelectMany(b => Convert.ToString(b, 2).PadLeft(4, '0').Select(c => new Bit(c is '1')));

        private readonly record struct Bit(bool IsSet)
        {
            public override string ToString() => IsSet ? "1" : "0";

            public static implicit operator bool(Bit bit) => bit.IsSet;
        }
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    
}
