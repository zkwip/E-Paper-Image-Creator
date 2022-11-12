using SixLabors.ImageSharp;

namespace Zkwip.EPIC;

public static class StandardProfiles
{
    public static readonly Profile BlackWhiteRed = new()
    {
        Width = 400,
        Height = 300,
        BigEndian = true,

        FlipHorizontal = false,
        FlipVertical = false,

        Channels = new Channel[]
        {
            new Channel()
            {
                CName = "IMAGE_RED",
                Color = Color.Red,
                Distance = 100,
                StoreInverted = true
            },
            new Channel()
            {
                CName = "IMAGE_BLACK",
                Color = Color.Black,
                Distance = 300,
                StoreInverted = true
            },
        },

        Background = Color.White
    };

}