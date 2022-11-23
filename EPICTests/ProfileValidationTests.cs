using FluentAssertions;

namespace Zkwip.EPIC.Tests
{
    public class ProfileValidationTests
    {
        public static IEnumerable<Object[]> Profiles()
        {
            foreach (var path in Directory.GetFiles("Profiles", "*.json", SearchOption.AllDirectories))
            {
                int start = path.LastIndexOf("Examples") + "Examples".Length + 1;
                int end = path.LastIndexOf(".");
                string profile = path[start..end];

                yield return new Object[] { profile };
            }
        }
        public static IEnumerable<Object[]> Examples()
        {
            foreach (var example in Directory.GetFiles("Examples", "*.*", SearchOption.AllDirectories))
            {
                int start = example.LastIndexOf("Examples") + "Examples".Length + 1;
                int end = example.LastIndexOf(".");
                string profile = example[start..end];

                yield return new Object[] { profile, example };
            }
        }

        [Theory]
        [MemberData(nameof(Profiles))]
        public void AllProfiles_Should_BeValid(string profile) 
        {
            Action act = () => ImageCreator.ValidateProfile(profile);

            act.Should().NotThrow();
        }

        [Theory]
        [MemberData(nameof(Examples))]
        public void AllExample_Should_BeExtractable(string profile, string file) 
        {
            Action act = () => ImageCreator.Extract(file, $"Extractions\\{profile}.png", profile, true, 0);

            act.Should().NotThrow();
        }
    }
}