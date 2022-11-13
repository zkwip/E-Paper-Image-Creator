# E-Paper Image Creator (under developement)

A tool to convert images into C++ code files for E-Paper displays.
Currently, it uses windows-only but these are going to be phased out in order to make it cross-platform

## Usage Examples
A few quick examples:

``EPIC.exe build image.png`` will build a file called ``image.cpp`` for use with your arduino projects.

``EPIC.exe build image.png -o output.cpp`` will build a file called ``output.cpp`` for use with your arduino projects.

``EPIC.exe build image.png -p waveshare/epd4in2b`` builds an output.cpp file for use with your arduino projects using the profile in ``Profiles/waveshare/epd4in2b.json``.

``EPIC.exe extract image_data.cpp`` recreates the image file stored in the code file. 

``EPIC.exe extract image_data.cpp -p waveshare/epd4in2b`` recreates the file using the profile ``Profiles/waveshare/epd4in2b.json``.

``EPIC.exe extract image_data.cpp -o picture.png`` recreates the file using the profile and writes it to ``picture.png``.

More features and parameters are documented in

``EPIC.exe --help``

## Profiles
The following devices already have profiles:

### Waveshare
 - epd1in02d - 1.02 Inch 128x80  B/W
 
 - epd1in53 - 1.54 Inch 200x200  B/W
 - epd1in53b - 1.54 Inch 200x200  B/R/W
 - epd1in53c - 1.54 Inch 152x152  B/R/W
 
 - epd1in64g - 1.54 Inch 168x168  B/R/Y/W
 
 - epd4in2 - 4.2 Inch 400x300  B/W
 - epd4in2b - 4.2 Inch 400x300  B/R/W
