﻿using System;
using System.Threading.Channels;

namespace Zkwip.EPIC
{
    internal class OutputBlock
    {
        private readonly byte[] _data;
        private int _byteCount;

        public OutputBlock(int bits, string name, bool msbFirst)
        {
            Name = name;
            _byteCount = (bits - 1) / 8 + 1;
            MsbFirst = msbFirst;

            _data = new byte[_byteCount];
        }

        public static OutputBlock FromText(ref int cursor, string content, int bits, bool msbFirst)
        {
            string arrayText = ReadBlock(ref cursor, content, out string name);
            var block = new OutputBlock(bits, name, msbFirst);

            block.FillFromText(SkipComments(arrayText));
            return block;
        }

        public bool MsbFirst { get; }
        public string Name { get; }

        private static string ReadBlock(ref int cursor, string content, out string name)
        {
            ReadTo(ref cursor, content, "const unsigned char");
            name = ReadTo(ref cursor, content, "[");
            ReadTo(ref cursor, content, "{");
            return ReadTo(ref cursor, content, "};");
        }

        private static string ReadTo(ref int cursor, string content, string handle)
        {
            var end = content.IndexOf(handle, cursor);
            if (end == -1)
                throw new ProfileMismatchException($"Failed to find the handle \"{handle}\" in array literal");

            string text = content[cursor..end].Trim();
            cursor = end + handle.Length;

            return text;
        }

        private void FillFromText(string text)
        {
            var cursor = 0;
            for (int i = 0; i < _byteCount; i++)
            {
                cursor = text.IndexOf("0x", cursor);

                if (cursor == -1)
                    throw new Exception($"Array literal {Name} has the wrong number of bytes: {i} instead of the expected {_byteCount}");

                cursor += 2;

                if (cursor >= text.Length)
                    throw new Exception($"Array literal {Name} has the wrong number of bytes: End of array is reached at index {i} instead of {_byteCount}");

                string code = text.Substring(cursor, 2);

                _data[i] = Convert.ToByte(code, 16);

                cursor += 2;
            }

            if (text.IndexOf("0x", cursor) != -1)
                throw new Exception($"Array literal {Name} has the wrong number of bytes: End of array has not been reached at index {_byteCount}");
        }

        internal string GenerateLiteral(bool disableProgmem, int perLine = 16)
        {
            string res = $"\nconst unsigned char {Name}[] {(disableProgmem ? "" : "PROGMEM ")}= {{\n";

            for (int i = 0; i < _byteCount; i++)
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

            if (MsbFirst)
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