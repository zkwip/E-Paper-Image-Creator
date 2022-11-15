# E-Paper Image Creator
A command-line tool to convert images to and from C/C++ source files for use with E-Paper displays in an embedded or IoT environment.

## Usage Example
Let's say you have a [epd4in2b](https://www.waveshare.com/product/4.2inch-e-paper-module-b.htm), which is a 3-color 4.2 inch e-paper display module, and you want to create an image for it:

First, we create an image with the same size as the display, so 400x300 pixels. let's save it as `image.png`.

Next, we need to find the right profile for the device. In our case this is `waveshare/epd4in2b`. If a profile does not exist, you can create one yourself, as described in [Custom Profiles](#Custom_Profiles)

Now, we can convert the image to a source file that you can use on your E-paper display, by running the following command:

    EPIC build image.png -p waveshare/epd4in2b

This command will create the file `image.cpp` using the `build` command. If you want the file to have a different name, for example `image_data.c`, you can run the following command

    EPIC build image.png -p waveshare/epd4in2b -o image_data.c

If you want to see if the image converted succesfully, or you want to restore a picture from code, you can also extract the image using the `extract` command, as seen here:

    EPIC extract image_data.c -p waveshare/epd4in2b

## Profiles
The following devices already have profiles:

### Lolin
 - `wemos_2_13_tri_color` - 2.13 Inch 104x212  R/B/W
 - `wemos_2_13_tri_color_ssd1680` - 2.12 Inch 112x250  R/B/W

### Seeed Studio
 - `grove_triple_1_54` - 1.54 Inch 152x152  R/B/W
 - `grove_triple_2_13` - 2.13 Inch 104x212  R/B/W

### Waveshare
 - `epd1in02d` - 1.02 Inch 128x80  B/W
 
 - `epd1in53` - 1.54 Inch 200x200  B/W
 - `epd1in53b` - 1.54 Inch 200x200  R/B/W
 - `epd1in53c` - 1.54 Inch 152x152  R/B/W
 
 - `epd1in64g` - 1.64 Inch 168x168  R/Y/B/W
 
 - `epd4in2` - 4.2 Inch 400x300  B/W
 - `epd4in2b` - 4.2 Inch 400x300  R/B/W

## Custom Profiles

todo
