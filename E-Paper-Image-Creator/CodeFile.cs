using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zkwip.EPIC
{
    internal class CodeFile
    {
        //private readonly Profile _profile;
        private readonly OutputBlock[] _blocks;

        private readonly int _entries;
        private readonly int _channels;
        private readonly int _blockBits;

        private readonly bool _interleaved;

        private readonly string[] _blockNames; 

        private CodeFile(Profile profile)
        {
            _channels = profile.Channels;
            _entries = profile.Width * profile.Height;

            _interleaved = profile.Interleaved;
            _blockBits = _entries * (_interleaved ? _channels: 1);

            _blockNames = profile.BlockNames;
            _blocks = new OutputBlock[_blockNames.Length];

            for (int i = 0; i < _blocks.Length; i++)
            {
                _blocks[i] = new OutputBlock(_blockBits, _blockNames[i], profile.MsbFirst);
            }
        }

        internal CodeFile(Profile profile, IEnumerable<bool[]> pixels) : this(profile)
        {
            int j = 0;

            foreach (var bits in pixels)
            {
                SetPixelBits(j, bits);
                j++;
            }
        }

        internal CodeFile(Profile profile, string content) : this(profile)
        {
            var cursor = 0;
            foreach (var _ in _blocks)
            {
                var block = OutputBlock.FromText(ref cursor, content, _blockBits, profile.MsbFirst);
                _blocks[FindBlockId(block.Name)] = block;
            }
        }

        internal IEnumerable<bool[]> GetAllPixels()
        {
            for (int i = 0; i < _entries; i++)
            {
                yield return GetPixelBits(i);
            }
        }

        private void SetPixelBits(int pixel, bool[] bits)
        {
            if (pixel >= _entries || pixel < 0)
                throw new IndexOutOfRangeException("Pixel is outside the expected range");

            for (int c = 0; c < bits.Length; c++)
                _blocks[_interleaved ? 0 : c].SetBit(FindBitPosition(c, pixel), bits[c]);
        }

        internal bool[] GetPixelBits(int pixel)
        {
            if (pixel >= _entries || pixel < 0)
                throw new IndexOutOfRangeException("Pixel is outside the expected range");

            bool[] colorBits = new bool[_channels];

            for (int c = 0; c < _channels; c++)
                colorBits[c] = _blocks[_interleaved ? 0 : c].GetBit(FindBitPosition(c, pixel));

            return colorBits;
        }

        private int FindBitPosition(int channel, int pixel)
        {
            if (_interleaved)
                return (pixel * _channels) + channel;

            return pixel;

        }

        private int FindBlockId(string name)
        {
            if (_interleaved) 
                return 0;

            // Match by name
            for (int c = 0; c < _channels; c++)
            {
                if (name == _blockNames[c])
                    return c;
            }

            throw new ProfileMismatchException($"Could not find array literal with name \"{name}\"");
        }

        private static string GenerateFileStart()
        {
            return $"// Image data converted by EPIC\n";
        }

        internal string BuildImageCode(bool disableProgmem)
        {
            string res = GenerateFileStart();

            for (int c = 0; c < _blocks.Length; c++)
                res += _blocks[c].GenerateLiteral(disableProgmem);

            return res;
        }
    }
}