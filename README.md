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
The following devices profiles are included out of the box, grouped by brand and ordered by size. Profile names generally follow the manufacturers naming converted to lower case and sometimes shortened.

### Adafruit
 - `adafruit/thinkink_154_mono_d27` - 1.54 Inch 200x200 B/W
 - `adafruit/thinkink_154_mono_d67` - 1.54 Inch 200x200 B/W
 - `adafruit/thinkink_154_mono_m10` - 1.54 Inch 152x152 B/W
 - `adafruit/thinkink_154_tri_rw` - 1.54 Inch 152x152 R/B/W
 - `adafruit/thinkink_154_tri_z17` - 1.54 Inch 152x152 R/B/W
 - `adafruit/thinkink_154_tri_z90` - 1.54 Inch 200x200 R/B/W
  
 - `adafruit/thinkink_213_mono_b72` - 2.13 Inch 250x122 B/W
 - `adafruit/thinkink_213_mono_b73` - 2.13 Inch 250x122 B/W
 - `adafruit/thinkink_213_mono_bn` - 2.13 Inch 250x122 B/W
 - `adafruit/thinkink_213_mono_m21` - 2.13 Inch 212x104 B/W
 - `adafruit/thinkink_213_tri_rw` - 2.13 Inch 250x122 R/B/W
 - `adafruit/thinkink_213_tri_z16` - 2.13 Inch 212x104 R/B/W

 - `adafruit/thinkink_270_tri_c44` - 2.7 Inch 264x176 R/B/W
 - `adafruit/thinkink_270_tri_z70` - 2.7 Inch 264x176 R/B/W

 - `adafruit/thinkink_290_mono_bn` - 2.9 Inch 296x128 B/W
 - `adafruit/thinkink_290_mono_m06` - 2.9 Inch 296x128 B/W
 - `adafruit/thinkink_290_tri_rh` - 2.9 Inch 296x128 R/B/W
 - `adafruit/thinkink_290_tri_z10` - 2.9 Inch 296x128 R/B/W
 - `adafruit/thinkink_290_tri_z13` - 2.9 Inch 296x128 R/B/W
 - `adafruit/thinkink_290_tri_z94` - 2.9 Inch 296x128 R/B/W

 - `adafruit/thinkink_420_mono_bn` - 4.2 Inch 400x300 B/W
 - `adafruit/thinkink_420_mono_m06` - 4.2 Inch 400x300 B/W
 - `adafruit/thinkint_420_tri_rw` - 4.2 Inch 400x300 R/B/W
 - `adafruit/thinkint_420_tri_z21` - 4.2 Inch 400x300 R/B/W

### Lolin
 - `lolin/wemos_2_13_tri_color` - 2.13 Inch 104x212  R/B/W
 - `lolin/wemos_2_13_tri_color_ssd1680` - 2.13 Inch 122x250  R/B/W

### Seeed Studio
 - `seeed/grove_triple_1_54` - 1.54 Inch 152x152  R/B/W
 - `seeed/grove_triple_2_13` - 2.13 Inch 104x212  R/B/W

### Waveshare
 - `waveshare/epd1in02d` - 1.02 Inch 128x80  B/W
 
 - `waveshare/epd1in54` - 1.54 Inch 200x200  B/W
 - `waveshare/epd1in54b` - 1.54 Inch 200x200  R/B/W
 - `waveshare/epd1in54c` - 1.54 Inch 152x152  R/B/W
 
 - `waveshare/epd1in64g` - 1.64 Inch 168x168  R/Y/B/W

 - `waveshare/epd2in13` - 2.13 Inch 250x122  B/W
 - `waveshare/epd2in13_v3` - 2.13 Inch 250x122  B/W
 - `waveshare/epd2in13b` - 2.13 Inch 212x104  R/B/W
 - `waveshare/epd2in13b_v3` - 2.13 Inch 212x104  R/B/W
 - `waveshare/epd2in13b_v4` - 2.13 Inch 250x122  R/B/W
 - `waveshare/epd2in13c` - 2.13 Inch 212x104  Y/B/W
 - `waveshare/epd2in13d` - 2.13 Inch 212x104  B/W

 - `waveshare/epd2in36g` - 2.36 Inch 168x168  R/Y/B/W

 - `waveshare/epd2in66` - 2.66 Inch 296x152  B/W
 - `waveshare/epd2in66b` - 2.66 Inch 296x152  R/B/W

 - `waveshare/epd2in7` - 2.7 Inch 264x176  B/W
 - `waveshare/epd2in7_v2` - 2.7 Inch 264x176  B/W
 - `waveshare/epd2in7b` - 2.7 Inch 264x176  R/B/W
 - `waveshare/epd2in7b_v2` - 2.7 Inch 264x176  R/B/W

 - `waveshare/epd2in9` - 2.9 Inch 296x128  B/W
 - `waveshare/epd2in9_v2` - 2.9 Inch 296x128  B/W
 - `waveshare/epd2in9b` - 2.9 Inch 296x128  R/B/W
 - `waveshare/epd2in9b_v3` - 2.9 Inch 296x128  R/B/W *
 - `waveshare/epd2in9c` - 2.9 Inch 296x128  Y/B/W
 - `waveshare/epd2in9d` - 2.9 Inch 296x128  B/W
 
 - `waveshare/epd3in0g` - 3.0 Inch 400x168  R/Y/B/W

 - `waveshare/epd3in52` - 3.52 Inch 360x240  B/W

 - `waveshare/epd3in7` - 3.7 Inch 480x280  B/W
  
 - `waveshare/epd4in01f` - 4.01 Inch 640x400  7-color

 - `waveshare/epd4in2` - 4.2 Inch 400x300  B/W
 - `waveshare/epd4in2b` - 4.2 Inch 400x300  R/B/W
 - `waveshare/epd4in2b_v2` - 4.2 Inch 400x300  R/B/W
 - `waveshare/epd4in2c` - 4.2 Inch 400x300  Y/B/W

 - `waveshare/epd4in37g` - 4.37 Inch 512x368  R/Y/B/W

Profiles marked with a * appeared to have code in the manufacturer's example that does not match the specification in their documentation. In this case we made an educated guess which of the two was right. Because of this, the profile specifications and/or example files might be different from what's found on the manufacturer's website.

## Planned support in future versions

### Waveshare
 - `waveshare/epd5in65f` - 5.65 Inch 600x448 7-color

 - `waveshare/epd5in83` - 5.83 Inch 648x480  B/W
 - `waveshare/epd5in83_v2` - 5.83 Inch 648x480  B/W
 - `waveshare/epd5in83b` - 5.83 Inch 648x480  R/B/W
 - `waveshare/epd5in83b_v2` - 5.83 Inch 648x480  R/B/W
 - `waveshare/epd5in83c` - 5.83 Inch 648x480  Y/B/W
 
 - `waveshare/epd7in3f` - 7.3 Inch 800x480  7-color
 - `waveshare/epd7in3g` - 7.3 Inch 800x480  R/Y/B/W

 - `waveshare/epd7in5` - 7.5 Inch 800x480  B/W
 - `waveshare/epd7in5_v2` - 7.5 Inch 800x480  B/W
 - `waveshare/epd7in5b` - 7.5 Inch 800x480  R/B/W
 - `waveshare/epd7in5b_v2` - 7.5 Inch 800x480  R/B/W
 - `waveshare/epd7in5c` - 7.5 Inch 640x384  Y/B/W
 - `waveshare/epd7in5_hd` - 7.5 Inch 880x528  B/W
 - `waveshare/epd7in5b_hd` - 7.5 Inch 880x528  R/B/W

## Custom Profiles
It's usually fastest to start off with a profile of a device that is very similar. Often screens are sold under multiple names and even more often they share the same type of drivers. Once you have picked a profile, copy it and modify its parameters until it looks right. A good way to do this is to take an example project from the manufacturer's website and see what image you get when you use the `extract` command. If you want to be extra sure, you can also use the following command to run some tests to see if the profile `manufacturer/myepd` makes sense:

    EPIC validate manufacturer/myepd

## Available commands and parameters

### Build
This command will create a code file from an image. It requires an image file as argument.

    EPIC build "image.png"

This command can use the following parameters to modify its behaviour:

 - `-o "out.cpp"` - This parameter sets the name of the output file. If this is ommitted the output will have the same name as the image, but with the `.cpp` extension 
 - `-p "waveshare/epd2in13"` - This parameter sets the name of the profile. You can pick one that matches your display from the list in [Profiles](#profiles). 
 - `-r 1, --rotate 1` - This parameter sets the amount of rotation between the image and the device's coordinate space. This is added onto the rotation defined in the profile itself. 1 means it's rotated by 90 degrees, 2 means 180 degrees. Negative values turn counter-clockwise.
 - `--flip-horizontal` - This parameter flips the image horizontally.
 - `--flip-vertical` - This parameter flips the image vertically.
 - `--no-progmem` - This parameter removes the processor-specific PROGMEM macro from the output source file, which is not always used for devices other than arduino.
 - `-O, --override` - This parameter allows the application to write the output file, even if it already exists.

### Extract
This command will create a code file from an image. It requires an image file as argument.

    EPIC extract "image.cpp"

This command can use the following parameters to modify its behaviour:

 - `-o "out.cpp"` - This parameter sets the name of the output file. If this is ommitted the output will have the same name as the source file, but with the `.png` extension 
 - `-p "waveshare/epd2in13"` - This parameter sets the name of the profile. You can pick one that matches your display from the list in [Profiles](#profiles). 
 - `-r 1, --rotate 1` - This parameter sets the amount of rotation between the image and the device's coordinate space. This is added onto the rotation defined in the profile itself. 1 means it's rotated by 90 degrees, 2 means 180 degrees. Negative values turn counter-clockwise.
 - `--flip-horizontal` - This parameter flips the image horizontally.
 - `--flip-vertical` - This parameter flips the image vertically.
 - `-O, --override` - This parameter allows the application to write the output file, even if it already exists.

### Validate
This command is used to validate a profile. 

    EPIC validate "waveshare/epd2in13"

