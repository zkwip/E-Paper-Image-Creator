using System;
using System.Text;

namespace Zkwip.EPIC
{
    internal class OutputBlock
    {
        private readonly byte[] _data;
        private readonly int _byteCount;
        private readonly bool _msbFirst;
        private readonly int _explicitSize;

        public OutputBlock(int bits, string name, bool msbFirst, int explicitByteCount)
        {
            Name = name;
            _byteCount = BitToByteCount(bits);
            _msbFirst = msbFirst;

            _data = new byte[_byteCount];
            _explicitSize = explicitByteCount;
        }

        private static int BitToByteCount(int bits) => (bits - 1) / 8 + 1;

        public static OutputBlock FromText(ref int cursor, string content, int bits, bool msbFirst, int explicitByteCount)
        {
            string arrayText = ReadBlock(ref cursor, content, out string name, explicitByteCount, bits);
            var block = new OutputBlock(bits, name, msbFirst, explicitByteCount);

            block.FillFromText(SkipComments(arrayText));
            return block;
        }

        public string Name { get; }

        private static string ReadBlock(ref int cursor, string content, out string name, int explicitByteCount, int bits)
        {
            ReadTo(ref cursor, content, "const unsigned char");
            name = ReadTo(ref cursor, content, "[");
            var count = ReadTo(ref cursor, content, "]");
            ReadTo(ref cursor, content, "{");


            if (explicitByteCount > 0)
            {
                if (count.Length == 0)
                    throw new ProfileMismatchException($"Array literal {name} has no explicit byte count, but the profile does");
                var bytes = int.Parse(count);

                if (bytes < explicitByteCount)
                    throw new ProfileMismatchException($"Array literal {name} has the wrong number of bytes: {bytes} instead of the expected {explicitByteCount}");

            }

            if (explicitByteCount == 0 && count.Length > 0)
                throw new ProfileMismatchException($"Array literal {name} has an explicit byte count \"{count}\" while the profile does not expect it.");

            return ReadTo(ref cursor, content, "};");
        }

        private static string ReadTo(ref int cursor, string content, string handle)
        {
            var end = content.IndexOf(handle, cursor);
            if (end == -1)
                throw new FileReaderException($"Failed to find the handle \"{handle}\" in array literal");

            string text = content[cursor..end].Trim();
            cursor = end + handle.Length;

            return text;
        }
        private static string ReadToOrEnd(ref int cursor, string content, string handle)
        {
            var end = content.IndexOf(handle, cursor);
            if (end == -1)
                end = content.Length;

            string text = content[cursor..end].Trim();
            cursor = end + handle.Length;

            if (cursor > content.Length) 
                cursor = content.Length;

            return text;
        }

        private void FillFromText(string text)
        {
            var cursor = 0;
            for (int i = 0; i < _byteCount; i++)
            {
                cursor = text.IndexOf("0x", cursor);

                if (cursor == -1)
                    throw new ProfileMismatchException($"Array literal {Name} has the wrong number of bytes: {i} instead of the expected {_byteCount}");

                cursor += 2;

                if (cursor >= text.Length)
                    throw new ProfileMismatchException($"Array literal {Name} has the wrong number of bytes: End of array is reached at index {i} instead of {_byteCount}");

                string code = ReadToOrEnd(ref cursor, text,",");

                _data[i] = Convert.ToByte(code, 16);
            }

            if (_byteCount <= _explicitSize && text.IndexOf("0x", cursor) != -1)
                throw new ProfileMismatchException($"Array literal {Name} has the wrong number of bytes: End of array has not been reached at index {_byteCount}");
        }

        internal string GenerateLiteral(bool disableProgmem, int perLine = 16)
        {
            var builder = new StringBuilder();
            BuildLiteralString(disableProgmem,builder,perLine);
            return builder.ToString();
        }

        internal void BuildLiteralString(bool disableProgmem, StringBuilder builder, int perLine = 16)
        { 
            string bcs = (_explicitSize > 0) ? _byteCount.ToString() : "";
            builder.AppendLine($"\nconst unsigned char {Name}[{bcs}] {(disableProgmem ? "" : "PROGMEM ")}= {{");

            for (int i = 0; i < _byteCount; i++)
            {
                builder.Append(String.Format("0x{0:x2}, ", _data[i]));

                if (i % perLine == perLine - 1)
                    builder.AppendLine();
            }

            builder.AppendLine("};");
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

            if (_msbFirst)
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