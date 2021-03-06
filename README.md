# srtm2tiff

srtm2tiff is an open-source command line tool to convert NASA HGT elevation files from the SRTM Shuttle radar mission to TIFF raster files.

SRTM format has two variants:
- SRTM1 files are sampled with a 1 arc-second distance between samples, having 3601 square samples per file.
- SRTM3 files are sampled with a 3 arc-second distance between samples, having 1201 square samples per file.

Elevation data is stored as a 16 bit signed integer, ranging from -32767 to 32767 meters.

> Documentation provided by the US government can be checked out at http://dds.cr.usgs.gov/srtm/version1/Documentation/SRTM_Topo.txt

## Usage ##

To convert a single HGT file to TIFF, use the command:

```srtm2tiff.exe fileName.hgt```

HGT files can be converted in batches. To convert several files at the same time, insert the filenames consecutively:

```srtm2tiff.exe file1.hgt file2.hgt file3.hgt ···```

TIFF files will be written on the same directory where the original HGT file resides.

Unless specified, srtm2tiff will not produce any console output. To generate a verbose output with information regarding the conversion state and the format of the input files, use the "verbose" option:

```srtm2tiff.exe -verbose file1.hgt file2.hgt ···```

## Building ##

The application was developed and built using Microsoft Visual Studio Express 2013. It should build under Mono or under other versions of Visual Studio, albeit it is untested.

The application uses the LibTIFF .NET binding from BitMiracle. You can obtain the source needed to build it here:

https://github.com/BitMiracle/libtiff.net

The srtm2tiff_vs15 directory includes a Visual Studio 2015 solution ready to build the project.

## Binaries ##

If you want to download a compiled version of the tool, you can do so at:

> http://cesar.jumbledevs.net/bin/srtm2tiff.zip

The binary was compiled with Visual Studio 2013 and is untested on any other platform but Windows 8.1 64-bit.

## Contributing ##

The application is licensed under the Lesser GNU public license. Feel free to use any part of the application in your projects as long as you quote the original project somewhere. Feel also free to make a pull request or comment about any bug or doubt.


