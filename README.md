# DotNetAssemblyNameChanger
Uses mono cecil to add random letters to the internal assembly name (not the file name) in a C# .NET Framework assembly to allow it to be loaded into a AppDomain if an assembly with the same internal assembly name already exists in that AppDomain.

I made this specifically for making mods for a game where I wanted to be able to hot-reload my mod after making updates without needing to restart the whole application. However trying to load the assembly with new code into the AppDomain would not work. So this essentially makes it work by changing the internal name of the assembly.

Potential downsides of this could be that every assembly will technically stay loaded in memory which could cause excess memory usage. It could also cause problems if your assemblies don't know to 'unload' themselves i.e. stop running code in the old assemblies. However the convenience is worth it for me as a developer. 

## Usage

`AssemblyPostProcess.exe [Optional: Path to input assembly] [Optional: Path to output directory] [Optional: -i (Interactive mode)]`

If no arguments are provided, the tool will open and ask the user to input the paths.

If no output directory path is provided, the directory of AssemblyPostProcess.exe will be used.

The interactive mode argument is just to make the tool pause at the end and wait for user input before closing.
