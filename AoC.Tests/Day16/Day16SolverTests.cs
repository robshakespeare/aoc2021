using AoC.Day16;
using FluentAssertions.Execution;
using static AoC.Day16.Day16Solver;

namespace AoC.Tests.Day16;

public class Day16SolverTests
{
    private readonly Day16Solver _sut = new();

    [TestCase("D2FE28", 6, 4)]
    [TestCase("38006F45291200", 1, 6)]
    [TestCase("EE00D40C823060", 7, 3)]
    public void BitsReader_ReadHeader_Tests(string input, int expectedVersion, int expectedTypeId)
    {
        using var reader = new BitsReader(input);

        // ACT
        var (packetVersion, packetTypeId) = reader.ReadHeader();

        // ASSERT
        packetVersion.Should().Be(expectedVersion);
        packetTypeId.Should().Be(expectedTypeId);
    }

    [Test]
    public void PacketExample1_RepresentsLiteralValue_2021()
    {
        using var reader = new BitsReader("D2FE28");

        // ACT
        var (packetVersion, packetTypeId) = reader.ReadHeader();
        var literal = reader.ReadLiteral();

        // ASSERT
        using (new AssertionScope())
        {
            packetVersion.Should().Be(6);
            packetTypeId.Should().Be(4);
            literal.Should().Be(2021);
        }
    }

    [Test]
    public void ReadPacketExample_LengthTypeId_0_ThatContains2SubPackets()
    {
        using var reader = new BitsReader("38006F45291200");

        // ACT
        var result = ReadPacket(reader);

        // ASSERT
        using (new AssertionScope())
        {
            result.Literal.Should().BeNull();
            result.SubPackets.Should().HaveCount(2);
            result.SubPackets[0].Literal.Should().Be(10);
            result.SubPackets[1].Literal.Should().Be(20);
        }
    }

    [Test]
    public void ReadPacketExample_LengthTypeId_1_ThatContains3SubPackets()
    {
        using var reader = new BitsReader("EE00D40C823060");

        // ACT
        var result = ReadPacket(reader);

        // ASSERT
        using (new AssertionScope())
        {
            result.Literal.Should().BeNull();
            result.SubPackets.Should().HaveCount(3);
            result.SubPackets[0].Literal.Should().Be(1);
            result.SubPackets[1].Literal.Should().Be(2);
            result.SubPackets[2].Literal.Should().Be(3);
        }
    }

    [TestCase("8A004A801A8002F478", 16)]
    [TestCase("620080001611562C8802118E34", 12)]
    [TestCase("C0015000016115A2E0802F182340", 23)]
    [TestCase("A0016C880162017C3686B18A3D4780", 31)]
    public void Part1Examples(string input, int expectedResult)
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(input);

        // ASSERT
        part1ExampleResult.Should().Be(expectedResult);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(989);
    }

    [TestCase("C200B40A82", 3)]
    [TestCase("04005AC33890", 54)]
    [TestCase("880086C3E88112", 7)]
    [TestCase("CE00C43D881120", 9)]
    [TestCase("D8005AC2A8F0", 1)]
    [TestCase("F600BC2D8F", 0)]
    [TestCase("9C005AC2F8F0", 0)]
    [TestCase("9C0141080250320F1802104A08", 1)]
    public void Part2Examples(string input, long expectedResult)
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(input);

        // ASSERT
        part2ExampleResult.Should().Be(expectedResult);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(7936430475134);
    }
}
