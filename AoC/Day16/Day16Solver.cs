namespace AoC.Day16;

public class Day16Solver : SolverBase
{
    public override string DayName => "Packet Decoder";

    public override long? SolvePart1(PuzzleInput input) => Packet.Decode(input).TotalPacketVersion;

    public override long? SolvePart2(PuzzleInput input) => Packet.Decode(input).Value;

    public record Packet(int PacketVersion, int PacketTypeId, long? Literal, IReadOnlyList<Packet> SubPackets)
    {
        public int TotalPacketVersion => PacketVersion + SubPackets.Sum(x => x.TotalPacketVersion);

        public long Value => Literal ?? PacketTypeId switch
        {
            0 => SubPackets.Sum(x => x.Value),
            1 => SubPackets.Aggregate(1L, (agg, x) => agg * x.Value),
            2 => SubPackets.Min(x => x.Value),
            3 => SubPackets.Max(x => x.Value),
            5 => SubPackets[0].Value > SubPackets[1].Value ? 1 : 0,
            6 => SubPackets[0].Value < SubPackets[1].Value ? 1 : 0,
            7 => SubPackets[0].Value == SubPackets[1].Value ? 1 : 0,
            _ => throw new InvalidOperationException("Invalid PacketTypeId " + PacketTypeId)
        };

        public static Packet Decode(PuzzleInput input) => ReadPacket(new BitsReader(input));

        private static Packet ReadPacket(BitsReader reader)
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
                var lengthTypeId = reader.ReadBit().IsSet ? 1 : 0;

                if (lengthTypeId == 0)
                {
                    // the next 15 bits are a number that represents the total length in bits of the sub-packets contained by this packet
                    var subPacketsBitLength = reader.ReadNumber(15);
                    var end = reader.BitPointer + subPacketsBitLength;

                    while (reader.BitPointer < end)
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
    }

    public class BitsReader
    {
        private readonly Bit[] _bits;

        public int BitPointer { get; private set; }
        public bool End => BitPointer >= _bits.Length;

        public BitsReader(PuzzleInput input) => _bits = BytesToBits(HexInputToBytes(input)).ToArray();

        public (int packetVersion, int packetTypeId) ReadHeader() => (ReadNumber(3), ReadNumber(3));

        public long ReadLiteral()
        {
            var bits = new List<Bit>();

            bool keepReading;
            do
            {
                keepReading = ReadBit();
                bits.AddRange(ReadBits(4));
            } while (keepReading);

            return BitsToLong(bits);
        }

        public int ReadNumber(int bitLength) => BitsToInt(ReadBits(bitLength));

        public Bit ReadBit()
        {
            if (End)
            {
                throw new InvalidOperationException("End of transmission reached");
            }

            return _bits[BitPointer++];
        }

        private IEnumerable<Bit> ReadBits(int numBits)
        {
            for (var i = 0; i < numBits; i++)
            {
                yield return ReadBit();
            }
        }

        private static int BitsToInt(IEnumerable<Bit> bits) => Convert.ToInt32(string.Join("", bits), 2);

        private static long BitsToLong(IEnumerable<Bit> bits) => Convert.ToInt64(string.Join("", bits), 2);

        private static IEnumerable<byte> HexInputToBytes(PuzzleInput input) => input.ToString().Select(chr => Convert.ToByte(chr.ToString(), 16));

        private static IEnumerable<Bit> BytesToBits(IEnumerable<byte> bytes) =>
            bytes.SelectMany(b => Convert.ToString(b, 2).PadLeft(4, '0').Select(c => new Bit(c is '1')));

        public readonly record struct Bit(bool IsSet)
        {
            public override string ToString() => IsSet ? "1" : "0";

            public static implicit operator bool(Bit bit) => bit.IsSet;
        }
    }
}
