# GrideMgfExtractor
Extracts and repacks mgf files from the game Frame Gride.  
I am not aware of other games using the format but I hear its a format used in Dreamcast games.  
If you find any .mgf files with the file magic of MGF or MGFL, let me know and send me a sample.  
I might be able to add support for it.  

The format does not include names, but the Frame Gride way of solving this seems to be with txt files.  
So I have builtin functionality to use these txt files, and repacking results in generating one with local paths.  

# Using the Extractor
Drag and drop the mgf archive onto the program to unpack.  
Drag and drop the entire folder you wish to pack onto the program to repack.  

If a txt file is found in the same folder as the archive when unpacking,  
and has the same number of filled lines as the number of files in the archive,  

It will be used to get names.  
If a txt file is not found, the files with simply be named with an index number.

# Path Technical Notes
Here is how I deal with paths provided by Frame Gride currently,  

Comments are often seen in them so I strip those first, starting with //  
Next I try to get the path between double quotes "path"  
Ideally they exist, if not it still should be able to get the path  
Next if the path starts with a drive like V:, I remove it  
Next if the path still contains any double quotes " I remove them  
Next if the path starts with \ or / I remove it so that Path.Combine works  
Finally I make the entire path lowercase.

I see some paths seem to be relative, while others may be a full system path in these txt files.  
I decided to not write full system paths while repacking as that seems pointless for a user's system.  
The relative paths seen in the txt files appear to be relative to the full system path ones.  
It feels hacky to attempt to find out if they belong to another path for now.  

I have only been testing all of this on one sample sent by someone.  
I am not aware if the game has anymore of these archives or txt files with them at the current time.

# Building
You must clone this project and the dependency:  
```
git clone https://github.com/WarpZephyr/GrideMgfExtractor.git  
git clone https://github.com/WarpZephyr/SoulsFormats.git  
```

Then link them in Visual Studio 2022 by adding SoulsFormats as a dependency to GrideMgfExtractor.  

# Credits
ZAC for sending me the sample. (.littlezac on discord, Snes Nerd on Youtube)  
SoulsFormats for the easy format drop in library  