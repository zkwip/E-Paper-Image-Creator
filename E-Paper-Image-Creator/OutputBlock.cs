using System;

namespace Zkwip.EPIC
{
    internal class OutputBlock
    {
        private readonly byte[] _data;
        public int ByteCount { get; }

        public bool BigEndian { get; }
        public string Name { get; }

        public OutputBlock(int byteCount, string name, bool bigEndian)
        {
            _data = new byte[byteCount];
            Name = name;
            ByteCount = byteCount;
            BigEndian = bigEndian;
        }

        internal void FillFromText(string text)
        {
            var cursor = 0;
            for (int i = 0; i < ByteCount; i++)
            {
                cursor = text.IndexOf("0x", cursor);

                if (cursor == -1)
                    throw new Exception($"Channel {Name} has the wrong number of bytes: {i} instead of the expected {ByteCount}");

                cursor += 2;

                if (cursor >= text.Length)
                    throw new Exception($"Channel {Name} has the wrong number of bytes: End of array is reached at index {i} instead of {ByteCount}");

                string code = text.Substring(cursor, 2);

                _data[i] = Convert.ToByte(code, 16);

                cursor += 2;
            }
        }

        internal string GenerateLiteral(bool disableProgmem, int perLine = 16)
        {
            string res = $"\nconst unsigned char {Name}[] {(disableProgmem ? "" : "PROGMEM ")}= {{\n";

            for (int i = 0; i < ByteCount; i++)
            {
                res += String.Format("0x{0:x2}, ", _data[i]);

                if (i % perLine == perLine - 1)
                    res += "\n";
            }

            return res + "};\n";
        }

        internal bool GetBit(int pos)
        {
            byte b = _data[pos/8];
            var bitmask = GetBitMask(pos);
            return (b & bitmask) > 0;
        }

        internal void SetBit(int pos, bool value)
        {
            byte b = _data[pos / 8];
            var bitmask = GetBitMask(pos);
            b |= bitmask;

            if (!value) b -= bitmask;

            _data[pos / 8] = b;
        }

        private byte GetBitMask(int index)
        {
            var position = index % 8;

            if (BigEndian)
                position = 7 - position;

            byte bitmask = (byte)(1 << position);
            return bitmask;
        }
    }
}