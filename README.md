# VeryJankDotNetAssemblyModifier
Uses mono cecil to randomize type names and the assembly name in an assembly to allow it to be loaded into a AppDomain if a similar assembly already exists in that AppDomain.

It will only change the type names of public types and won't affect nested types.

I made this specifically for making mods for a game where I wanted to be able to hot-reload my mod. However trying to load the same assembly into the AppDomain even if the code is different would not work. So this essentially makes it work by changing the signature of the assembly and avoids type collisions by adding random characters to the type names.
