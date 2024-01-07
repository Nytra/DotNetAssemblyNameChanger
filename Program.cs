using Mono.Cecil;
using System.Reflection;

Console.WriteLine("AssemblyPostProcess\n");

string executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
string? executingAssemblyDirectory = Path.GetDirectoryName(executingAssemblyLocation);
Console.WriteLine("Executing assembly directory: " + executingAssemblyDirectory);

Random rand = new Random();
string? assemblyPath = null;
string? writeDirectory = null;
bool interactive = false;
bool invalidArgs = false;

if (args.Length == 0)
{
    interactive = true;
    TakeInput();
}
else if (args.Length >= 1)
{
    if (args[0] == "-i")
    {
        interactive = true;
        TakeInput();
    }
    else
    {
        assemblyPath = args[0];
        if (args.Length >= 2)
        {
            if (args[1] == "-i")
            {
                interactive = true;
            }
            else
            {
                writeDirectory = args[1];
                if (args.Length >= 3)
                {
                    if (args[2] == "-i")
                    {
                        interactive = true;
                    }
                    else
                    {
                        invalidArgs = true;
                    }
                }
            }
        }
    }
}
else
{
    invalidArgs = true;
}

if (invalidArgs)
{
    throw new Exception("Invalid arguments! Expected: InputAssemblyPath and OutputDirectoryPath. Optional: -i (Interactive mode)");
}

if (string.IsNullOrWhiteSpace(assemblyPath))
{
    throw new Exception("Assembly path cannot be null or whitespace!");
}

assemblyPath = RemoveQuotesFromString(assemblyPath);

if (string.IsNullOrWhiteSpace(writeDirectory))
{
    writeDirectory = null;
}
else
{
    writeDirectory = RemoveQuotesFromString(writeDirectory);
}

if (!Path.Exists(assemblyPath))
{
    throw new Exception("Input assembly file does not exist!");
}

if (!Path.Exists(writeDirectory))
{
    Console.WriteLine();
    Console.WriteLine("Output directory does not exist! Output file will now be written to the location of the executing assembly.");
    writeDirectory = executingAssemblyDirectory;
}

assemblyPath = Path.GetFullPath(assemblyPath);

Console.WriteLine();

Console.WriteLine("Actual Assembly path: " + assemblyPath);
Console.WriteLine("Actual Output directory path: " + writeDirectory);

Console.WriteLine();

PostProcessAssembly(assemblyPath);

string? RemoveQuotesFromString(string str)
{
    return str?.Replace("\"", "").Replace("'", "");
}

void TakeInput()
{
    Console.WriteLine();
    Console.Write("Input assembly path: ");
    assemblyPath = Console.ReadLine();
    Console.Write("Output directory path: ");
    writeDirectory = Console.ReadLine();
}

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

    ModuleDefinition module = ModuleDefinition.ReadModule(assemblyPath);

    Console.WriteLine("Loaded module: " + module.Name);

    Console.WriteLine("Assembly name definition: " + module.Assembly.Name.ToString());

    Console.WriteLine("\nProcessing public types...");

    int changedTypesCount = 0;

    foreach(TypeDefinition typeDef in module.Types)
    {
        if (typeDef.IsNotPublic) continue;
        Console.WriteLine();
        Console.WriteLine("Found type: " + typeDef.FullName);
        Console.WriteLine("Changing type name...");
        typeDef.Name = typeDef.Name + GetRandomString(32);
        Console.WriteLine("New type name: " + typeDef.FullName);
        changedTypesCount += 1;
    }

    if (changedTypesCount == 0)
    {
        Console.WriteLine("No public types found.\n");
    }
    else
    {
        Console.WriteLine();
    }

    module.Assembly.Name.Name += GetRandomString(32);

    Console.WriteLine("Changed assembly name to: " + module.Assembly.Name.Name);

    Console.WriteLine();

    Console.WriteLine("New assembly name definition: " + module.Assembly.Name.ToString());

    Console.WriteLine();

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

if (interactive)
{
    Console.Write("\nPress enter to quit.");
    Console.ReadLine();
}