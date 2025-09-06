// Get the value of the PATH environment variable
string path = Environment.GetEnvironmentVariable("PATH") ?? "";
// Split the PATH value by semicolon and store in an array
string[] paths = path.Split(';', StringSplitOptions.RemoveEmptyEntries);
// Use HashSet for duplicate checking
HashSet<string> uniquePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

// For each path
foreach (string p in paths)
{
    // Check if the path exists
    bool exists = Directory.Exists(p);
    // Check if the path is duplicated
    bool isDuplicate = !uniquePaths.Add(p);

    // Display the result
    string result = "";
    if (!exists)
    {
        result += "X";
    }
    if (isDuplicate)
    {
        result += "D";
    }
    Console.WriteLine($"{result}\t{p}");
}