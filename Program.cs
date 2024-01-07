using Mono.Cecil;
using System.Reflection;

Console.WriteLine("AssemblyPostProcess\n");

string executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
string executingAssemblyDirectory = Path.GetDirectoryName(executingAssemblyLocation);
Console.WriteLine("Executing assembly directory: " + executingAssemblyDirectory);

Random rand = new Random();

string assemblyPath, writeDirectory;

if (args.Length == 0)
{
    Console.Write("Input assembly path: ");
    assemblyPath = Console.ReadLine();
    Console.Write("Input directory to write the processed assembly to: ");
    writeDirectory = Console.ReadLine();
}
else if (args.Length == 2)
{
    assemblyPath = args[0].Replace("\"", "");
    writeDirectory = args[1].Replace("\"", "");
}
else
{
    throw new Exception("Expected 2 arguments! InputAssemblyPath and OutputDirectory path");
}

if (string.IsNullOrWhiteSpace(assemblyPath))
{
    throw new Exception("Assembly path cannot be null!");
}

if (string.IsNullOrWhiteSpace(writeDirectory))
{
    writeDirectory = null;
}

Console.WriteLine("Assembly path: " + assemblyPath);
Console.WriteLine("Output directory path: " + writeDirectory ?? executingAssemblyDirectory);

PostProcessAssembly(assemblyPath);

string GetRandomString(int charCount)
{
    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    var stringChars = new char[charCount];

    for (int i = 0; i < stringChars.Length; i++)
    {
        stringChars[i] = chars[rand.Next(chars.Length)];
    }

    var finalString = new String(stringChars);

    return finalString;
}

void PostProcessAssembly(string assemblyPath)
{
    Console.WriteLine($"Loading assembly from path: \"{assemblyPath}\"");

    //Assembly assembly = Assembly.LoadFrom(assemblyPath);

    ModuleDefinition module = ModuleDefinition.ReadModule(assemblyPath);

    Console.WriteLine("Loaded module: " + module.Name);

    Console.WriteLine("Assembly name: " + module.Assembly.Name.ToString());

    Console.WriteLine("\nProcessing public types...\n");

    foreach(TypeDefinition typeDef in module.Types)
    {
        if (typeDef.IsNotPublic) continue;
        Console.WriteLine("Found type: " + typeDef.FullName);
        Console.WriteLine("Changing type name...");
        typeDef.Name = typeDef.Name + GetRandomString(32);
        Console.WriteLine("New type name: " + typeDef.FullName);
    }

    Console.WriteLine();

    module.Assembly.Name.Name += GetRandomString(32);

    Console.WriteLine("Changed assembly name to: " + module.Assembly.Name.Name);

    module.Name = "Processed_" + module.Name;

    string writePath;
    if (writeDirectory == null)
    {
        writePath = Path.GetDirectoryName(executingAssemblyLocation) + "\\" + module.Name;
    }
    else
    {
        writePath = writeDirectory + "\\" + module.Name;
    }

    Console.WriteLine("Writing module to path: " + writePath);

    module.Write(writePath);
}