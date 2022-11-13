namespace Zkwip.EPIC
{
    internal static class CodeFileReader
    {
        internal static DataChannel[] ExtractDataChannels(string content, Profile profile)
        {
            DataChannel[] channels = new DataChannel[profile.Channels];

            const string arrayPrefix = "const unsigned char";
            const string arraySuffix = "[] = {";
            const string arrayClosure = "};";

            var cursor = 0;

            foreach (string _ in profile.ChannelNames)
            {
                cursor = content.IndexOf(arrayPrefix, cursor);
                if (cursor == -1)
                    break;

                cursor += arrayPrefix.Length;

                var nameEnd = content.IndexOf(arraySuffix, cursor);
                var name = content[cursor..nameEnd].Trim();
                cursor = nameEnd + arraySuffix.Length;

                var arrayEnd = content.IndexOf(arrayClosure, cursor);
                string arrayText = content[cursor..arrayEnd];
                cursor = nameEnd + arrayClosure.Length;
                for (int c = 0; c < profile.Channels; c++)
                {
                    if (name == profile.ChannelNames[c])
                    {
                        // Console.WriteLine($"Found channel {name} with data length of {arrayData.Length} characters.");
                        channels[c] = new DataChannel(profile, c, SkipComments(arrayText));
                    }
                }
            }
            return channels;
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