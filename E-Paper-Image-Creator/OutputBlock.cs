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

        public OutputBlock(ref int cursor, string content, int blockLength, bool bigEndian)
        {
            string arrayText = ReadBlock(ref cursor, content, out string name);

            _data = new byte[blockLength];
            Name = name;
            BigEndian = bigEndian;
            ByteCount = blockLength;

            FillFromText(SkipComments(arrayText));
        }

        private static string ReadBlock(ref int cursor, string content, out string name)
        {
            const string arrayNamePrefix = "const unsigned char";
            const string arrayNameSuffix = "[]";
            const string arrayOpening = "{";
            const string arrayClosure = "};";

            // Find "const unsign.."
            cursor = content.IndexOf(arrayNamePrefix, cursor);
            if (cursor == -1)
                throw new ParseException("Failed to find the array literal");

            cursor += arrayNamePrefix.Length;

            // Capture name
            var nameEnd = content.IndexOf(arrayNameSuffix, cursor);
            if (nameEnd == -1)
                throw new ParseException("Failed to find the array literal");

            name = content[cursor..nameEnd].Trim();
            cursor = nameEnd + arrayNameSuffix.Length;

            // Find start of the array literal
            var arrayStart = content.IndexOf(arrayOpening, cursor);
            if (arrayStart == -1)
                throw new ParseException("Failed to find the array literal");

            cursor = arrayStart + arrayOpening.Length;

            // Capture the literal
            var arrayEnd = content.IndexOf(arrayClosure, cursor);
            if (arrayEnd == -1)
                throw new ParseException("Failed to find the array literal");

            string arrayText = content[cursor..arrayEnd];
            cursor = arrayEnd + arrayClosure.Length;
            return arrayText;
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

        private static string SkipComments(string input)
        {
            input = input.ToLower();
            var cursor = 0;

            while (true)
            {
                if (cursor >= input.Length - 2)
                    break;

                if (input.Substring(cursor, 2) == "//")
                {
                    input = Slice(input, cursor, input.IndexOf("\n", cursor));
                    continue;
                }

                if (input.Substring(cursor, 2) == "/*")
                {
                    input = Slice(input, cursor, input.IndexOf("*/", cursor));
                    continue;
                }

                cursor++;
            }

            return input;
        }

        private static string Slice(string input, int firstCut, int secondCut)
        {
            if (secondCut < 0)
                secondCut += input.Length;

            return input[..firstCut] + input[secondCut..];
        }
    }
}