using System;
using System.Collections.Generic;

namespace Zkwip.EPIC
{
    internal class CodeFile
    {
        private readonly OutputBlock[] _blocks;

        private readonly int _entries;
        private readonly int _channels;
        private readonly int _blockBits;

        private readonly bool _interleaved;

        private readonly string[] _blockNames; 

        private CodeFile(Profile profile)
        {
            _channels = profile.Channels;
            _entries = profile.Entries;

            _interleaved = profile.Interleaved;
            _blockBits = _entries * (_interleaved ? _channels: 1);

            _blockNames = profile.BlockNames;
            _blocks = new OutputBlock[_blockNames.Length];

            for (int i = 0; i < _blocks.Length; i++)
            {
                _blocks[i] = new OutputBlock(_blockBits, _blockNames[i], profile.MsbFirst, profile.ExplicitSize);
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
            var left = _blocks.Length;
            var cursor = 0;

            try
            {
                while (left > 0)
                {
                    var block = OutputBlock.FromText(ref cursor, content, _blockBits, profile.MsbFirst, profile.ExplicitSize);
                    int id = FindBlockId(block.Name);

                    if (id != -1)
                    {
                        _blocks[id] = block;
                        left--;
                    }
                }
            }
            catch (FileReaderException)
            {
                for(int i=0;i<_blocks.Length;i++)
                {
                    if (_blocks[i] is null)
                        throw new ProfileMismatchException($"Literal {_blockNames[i]}was not found in the code file.");

                }
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
            //if (_interleaved) 
            //    return 0;

            // Match by name
            for (int c = 0; c < _blockNames.Length; c++)
            {
                if (name == _blockNames[c])
                    return c;
            }

            return -1;
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