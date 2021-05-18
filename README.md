# Freesat
This Visual Studio 2019 project is a simple .NET Core 3.1 console application to create the files that are required on a Windows Media Center PC to allow a quick and successful TV Signal Setup for a single DVBS Satellite. (The original WMC procedure is obsolete.) In particular it creates an MXF file that includes tuning parameters. The primary target is the FTA channels, including Freesat, at 28.2 E, but configuration for any single DVBS satellite is possible.

In addition to the source and resources, this repository also includes, for reference, sample published build output (the console application) and sample output from running the console application (the files required on the HTPC). The latter are in the Publish\Target subfolder, and include a readme. The remainder of the Publish folder is the sample console application.

After building and publishing the console application, the three text files in the Publish\Resources subfolder (not the project Resources folder) should be edited, replaced or deleted as follows before running.

DVBViewer.csv is the tuning parameters file. It is a tab-delimited text file with a header row. There is a column with the literal text to be written to the MXF file for each of the service attributes 'name' and 'service ID'; each of the transponder numerical attributes 'carrier frequency', 'symbol rate', 'original network ID' and 'transport stream ID'; and the satellite attribute 'position east'. There is also a column with the text string that specifies the transponder polarization. Other columns in the file are ignored.

EPG123Client.csv is optional, and if present it is as copied to the clipboard from the EPG123 Client Guide Tool. It is a tab-delimited text file with a header row. Columns other than 'Call Sign', 'Number' and 'MatchName' are ignored. The last of these columns has colon-separated fields that are used to match with a service in the tuning parameters file. The first field is a textual prefix and the others are numerical. A match occurs when the MatchName has the prefix 'DVBS', the first number agrees with the satellite attribute 'position east', and the last number agrees with the service attribute 'service ID' (e.g. in 'DVBS:282:10847:2:2050:6941' the relevant fields are 'DVBS', '282' and '6941', respectively). Other fields in MatchName are ignored. When there is a match, to facilitate automatic subscription to guide listings in the EPG123 Client, the Call Sign and Number will be written to the MXF file. The Call Sign replaces the name given in the tuning parameters, and the TV Setup procedure will use the Number rather than assigning a channel number automatically.

config.xml contains the following configurable information. The name and position of the satellite, the ISO code of the region and the ID and language of the headend, which are written to the MXF file. These data are redundant and defunct, but, possibly arbitrary, values must be provided. The other configurable information is the name (without extension) of the tuning parameters file (the extension .csv is not configurable); the numbers (starting at 1) of its columns, as above; and the text string that signifies vertical polarization (the default is horizontal). The original config.xml is correct for a file exported from DVBViewer Pro.

Notes

By design the MXF file as written causes WMC to ignore the encrypted flag for all channels. Any channels incorrectly flagged as encrypted then do not acquire the ‘padlock’ symbol in WMC and do not need to be manually enabled.

When exporting from DVBViewer Pro, the extension .csv must be provided explicitly to obtain the correct tab-delimited format.

WMC assigns numbers 1000 and above to unmatched channels.

It helps to first to select source DVBS only and hide unused channels when saving to the clipboard from the EPG123 Client and when automatically matching channels after TV Signal Setup. Multiple channels can be selected in EPG123Client and hidden/unhidden as a block by pressing the Space Bar.
