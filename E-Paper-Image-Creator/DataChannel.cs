using System;

namespace Zkwip.EPIC
{
    internal class DataChannel
    {
        private readonly byte[] _data;
        private readonly Profile _profile;

        public DataChannel(Profile p, int id)
        {
            ByteCount = ComputeLenght(p);
            ChannelId = id;

            _data = new byte[ByteCount];
            _profile = p;
        }

        public DataChannel(Profile p, int id, string text)
        {
            ByteCount = ComputeLenght(p);
            ChannelId = id;

            _data = new byte[ByteCount];
            _profile = p;

            FillFromText(text);
        }

        private static int ComputeLenght(Profile p)
        {
            var pixels = p.Width * p.Height;
            return ((pixels - 1) / 8) + 1;
        }

        public int ByteCount { get; }
        public int ChannelId { get; }
        public string ChannelName => _profile.ChannelNames[ChannelId];

        public bool this[int x, int y]
        {
            get => GetPixel(x, y);
            set => SetPixel(x, y, value);
        }

        public byte this[int i]
        {
            get => _data[i];
        }

        private void SetPixel(int x, int y, bool value)
        {
            int index = GetByteIndex(x, y);
            byte b = _data[index];
            var bitmask = GetBitMask(x, y);
            b |= bitmask;

            if (!value) b -= bitmask;

            _data[index] = b;
        }

        private bool GetPixel(int x, int y)
        {
            int index = GetByteIndex(x, y);
            byte b = _data[index];
            var bitmask = GetBitMask(x, y);
            return (b & bitmask) > 0;
        }

        private int GetByteIndex(int x, int y) => (x + _profile.Width * y) / 8;

        private byte GetBitMask(int x, int y)
        {
            var position = (x + _profile.Width * y) % 8;

            if (_profile.BigEndian)
                position = 7 - position;

            byte bitmask = (byte)(1 << position);
            return bitmask;
        }

        private void FillFromText(string text)
        {
            var cursor = 0;
            for (int i = 0; i < ByteCount; i++)
            {
                cursor = text.IndexOf("0x", cursor);

                if (cursor == -1)
                    throw new Exception($"Channel {ChannelName} has the wrong number of bytes: {i} instead of the expected {ByteCount}");

                cursor += 2;

                if (cursor >= text.Length)
                    throw new Exception($"Channel {ChannelName} has the wrong number of bytes: End of array is reached at index {i} instead of {ByteCount}");

                string code = text.Substring(cursor, 2);

                _data[i] = Convert.ToByte(code, 16);

                cursor += 2;
            }
        }
    }
}