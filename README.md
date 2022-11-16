# E-Paper Image Creator
A command-line tool to convert images to and from C/C++ source files for use with E-Paper displays in an embedded or IoT environment.

## Usage Example
Let's say you have a [epd4in2b](https://www.waveshare.com/product/4.2inch-e-paper-module-b.htm), which is a 3-color 4.2 inch e-paper display module, and you want to create an image for it:

First, we create an image with the same size as the display, so 400x300 pixels. Since an e-paper display only supports a couple of colors, it's best to design your picture around those colors. EPIC will automatically round the colors to the most similar color that the display can show, so they do not need to be exact. Let's save it as `image.png` in the same folder as the EPIC application.

Next, we need to find the right profile for the device. If your device is in [the list](#profiles), you can copy or remember the name for the next step. In our case this is `waveshare/epd4in2b`. If a profile does not exist, you can create one yourself, as described in [Custom Profiles](#custom-profiles)

Now, we can convert the image to a source file that you can use on your E-paper display. We can do this by opening a terminal in the folder where EPIC is stored and running the following command:

    EPIC build image.png -p waveshare/epd4in2b

This command will create the file `image.cpp` using the `build` command. If you want the file to have a different name, for example `image_data.c`, you can do so by adding it with the `-o` parameter:

    EPIC build image.png -p waveshare/epd4in2b -o image_data.c

If you want to see if the image converted succesfully, or you want to restore a picture from code, you can also extract the image using the `extract` command, as seen here:

    EPIC extract image_data.c -p waveshare/epd4in2b

## Profiles
The following devices already have profiles:

### Adafruit
 - todo

### Lolin
 - `lolin/wemos_2_13_tri_color` - 2.13 Inch 104x212  R/B/W
 - `lolin/wemos_2_13_tri_color_ssd1680` - 2.13 Inch 122x250  R/B/W

### Seeed Studio
 - `seeed/grove_triple_1_54` - 1.54 Inch 152x152  R/B/W
 - `seeed/grove_triple_2_13` - 2.13 Inch 104x212  R/B/W

### Waveshare
 - `waveshare/epd1in02d` - 1.02 Inch 128x80  B/W
 
 - `waveshare/epd1in53` - 1.54 Inch 200x200  B/W
 - `waveshare/epd1in53b` - 1.54 Inch 200x200  R/B/W
 - `waveshare/epd1in53c` - 1.54 Inch 152x152  R/B/W
 
 - `waveshare/epd1in64g` - 1.64 Inch 168x168  R/Y/B/W

 - `waveshare/epd2in13` - 2.13 Inch 122x250  R/B/W
 
 - `waveshare/epd4in2` - 4.2 Inch 400x300  B/W
 - `waveshare/epd4in2b` - 4.2 Inch 400x300  R/B/W

## Custom Profiles
It's usually fastest to start off with a profile of a device that is very similar. Often screens are sold under multiple names and even more often they share the same type of drivers. Once you have picked a profile, copy it and modify its parameters until it looks right. A good way to do this is to take an example project from the manufacturer's website and see what image you get when you use the `extract` command. If you want to be extra sure, you can also use the following command to run some tests to see if the profile `manufacturer/myepd` makes sense:

    EPIC validate manufacturer/myepd


