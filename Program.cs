using Mono.Cecil;
using System.Reflection;

Console.WriteLine("AssemblyPostProcess\n");

string executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
string? executingAssemblyDirectory = Path.GetDirectoryName(executingAssemblyLocation);
Console.WriteLine("Executing assembly directory: " + executingAssemblyDirectory);

Random rand = new Random();
string? inputAssemblyPath = null;
string? outputDirectoryPath = null;
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
        inputAssemblyPath = args[0];
		if (args.Length >= 2)
		{
			if (args[1] == "-i")
			{
				interactive = true;
			}
			else
			{
                outputDirectoryPath = args[1];
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

if (string.IsNullOrWhiteSpace(inputAssemblyPath))
{
	throw new Exception("Assembly path cannot be null or whitespace!");
}

inputAssemblyPath = RemoveQuotesFromString(inputAssemblyPath);

if (!string.IsNullOrWhiteSpace(outputDirectoryPath))
{
    outputDirectoryPath = RemoveQuotesFromString(outputDirectoryPath);
}

if (!Path.Exists(inputAssemblyPath))
{
	throw new Exception("Input assembly file does not exist!");
}

if (!Path.Exists(outputDirectoryPath))
{
	Console.WriteLine();
	Console.WriteLine("Output directory does not exist! Output file will now be written to the location of the executing assembly.");
    outputDirectoryPath = executingAssemblyDirectory;
}
else if (!Directory.Exists(outputDirectoryPath))
{
    outputDirectoryPath = Path.GetDirectoryName(outputDirectoryPath);
}

inputAssemblyPath = Path.GetFullPath(inputAssemblyPath);

Console.WriteLine();

Console.WriteLine("Actual Assembly path: " + inputAssemblyPath);
Console.WriteLine("Actual Output directory path: " + outputDirectoryPath);

Console.WriteLine();

PostProcessAssembly(inputAssemblyPath, outputDirectoryPath);

string? RemoveQuotesFromString(string str)
{
	return str?.Replace("\"", "").Replace("'", "");
}

void TakeInput()
{
	Console.WriteLine();
	Console.Write("Input assembly path: ");
    inputAssemblyPath = Console.ReadLine();
	Console.Write("Output directory path: ");
    outputDirectoryPath = Console.ReadLine();
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

void PostProcessAssembly(string assemblyPath, string? outputDirectoryPath)
{
	Console.WriteLine($"Loading assembly from path: \"{assemblyPath}\"");

	ModuleDefinition module = ModuleDefinition.ReadModule(assemblyPath);

	Console.WriteLine("Loaded module: " + module.Name);

	Console.WriteLine("Assembly name definition: " + module.Assembly.Name.ToString());

	module.Assembly.Name.Name += GetRandomString(32);

	Console.WriteLine("\nChanged assembly name to: " + module.Assembly.Name.Name);

	Console.WriteLine("\nNew assembly name definition: " + module.Assembly.Name.ToString());

	module.Name = "Processed_" + module.Name;

	string writePath;
	if (!Path.Exists(outputDirectoryPath)) // this handles null writeDirectory
	{
        // fallback to executingAssemblyDirectory
        string executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
        string? executingAssemblyDirectory = Path.GetDirectoryName(executingAssemblyLocation);
		if (executingAssemblyDirectory != null)
		{
            writePath = executingAssemblyDirectory + "\\" + module.Name;
        }
		else
		{
			throw new Exception("Somehow the executing assembly directory is null???");
		}
	}
	else
	{
		writePath = outputDirectoryPath + "\\" + module.Name;
	}

	Console.WriteLine("\nWriting module to path: " + writePath);

	module.Write(writePath);
}

if (interactive)
{
	Console.Write("\nPress enter to quit.");
	Console.ReadLine();
}