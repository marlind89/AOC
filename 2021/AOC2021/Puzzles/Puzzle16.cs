namespace AOC2021.Puzzles
{
    internal class Puzzle16 : Puzzle<long>
    {
        private int _versionSum;

        protected override void Solve(string[] lines)
        {
            var bitReader = new BitReader(string.Join(null, lines[0]
                .Select(c => Convert.ToString(Convert.ToInt32("" + c, 16), 2).PadLeft(4, '0'))));

            var result = ParsePacket(bitReader);

            One = _versionSum;
            Two = result;
        }

        private long ParsePacket(BitReader bitReader)
        {
            var version = bitReader.ReadNext(3);
            var type = bitReader.ReadNext(3);

            _versionSum += version;

            if (type == 4)
            {
                return GetPayloadForPacketType4(bitReader);
            }
            
            var packets = new List<long>();
            if (bitReader.ReadNext(1) == 0)
            {
                var length = bitReader.ReadNext(15);

                var prevIdx = bitReader.CurrentIdx;
                while ((bitReader.CurrentIdx - prevIdx) != length)
                {
                    packets.Add(ParsePacket(bitReader));
                }
            }
            else
            {
                packets.AddRange(Enumerable.Range(0, bitReader.ReadNext(11))
                    .Select(_ => ParsePacket(bitReader)));
            }

            return type switch
            {
                0 => packets.Sum(),
                1 => packets.Aggregate(1L, (a, b) => a * b),
                2 => packets.Min(),
                3 => packets.Max(),
                5 => packets[0] > packets[1] ? 1 : 0,
                6 => packets[0] < packets[1] ? 1 : 0,
                7 => packets[0] == packets[1] ? 1 : 0,
                _ => 0
            };
        }

        private static long GetPayloadForPacketType4(BitReader reader)
        {
            var groups = new List<long>();
            while (true)
            {
                var group = reader.ReadNext(5);
                groups.Add(group & 0xF);

                if ((group & 0x10) == 0)
                {
                    break;
                }
            }

            groups.Reverse();

            long result = 0;
            for (int i = 0; i < groups.Count; i++)
            {
                result |= groups[i] << (i * 4);
            }

            return result;
        }

        private class BitReader
        {
            private readonly string _packet;
            public int CurrentIdx { get; private set; }
            public BitReader(string packet)
            {
                _packet = packet;
            }

            public int ReadNext(int bitSize) => Convert.ToInt32(_packet[CurrentIdx..(CurrentIdx += bitSize)], 2);
        }
    }
}
