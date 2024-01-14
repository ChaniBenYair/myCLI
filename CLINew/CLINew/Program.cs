using System.CommandLine;
using System.CommandLine.Help;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.SymbolStore;
using System.IO;

static string[] chooseLanguages(string[] languages, string[] extensions, string myLanguages)
{
    if(myLanguages.Equals("all"))
        return extensions; 

    string[] result = myLanguages.Split(' ');
    for (int i = 0; i < result.Length; i++)
    {
        for (int j = 0; j < languages.Length; j++)
        {
            if (result[i].Equals(languages[j]))
                result[i] = extensions[j];
            break;
        }
    }
    return result;
}

string[] arrLanguages = { "c#", "java", "html", "c++", "javaScript", "python", "c" ,"css"};
string[] extensions = { ".cs", ".java", ".html", ".cpp", ".js", ".py", ".c", ".css" };

var bundleCommand = new Command("bundle", "Bundle code files to a single file");

var outputOption = new Option<FileInfo>(new[] { "--output" ,"-o"}, "File path and name");
bundleCommand.AddOption(outputOption);

var languageOption = new Option<string>(new[] { "--language", "-l " },"List of programming languages");
bundleCommand.AddOption(languageOption);

var noteOption = new Option<bool>(new[] { "--note", "-n" }, () => false, "note the sorces code");
bundleCommand.AddOption(noteOption);

var authorOption = new Option<string>(new[] { "--author", "-a" }, "enter name");
bundleCommand.AddOption(authorOption);

var sortOption = new Option<string>(new[] { "--sort", "-s" }, () => "latter", "");
bundleCommand.AddOption(sortOption);

var removeEmptyLinesOption = new Option<bool>(new[] { "--remove-empty-lines", "-r" },()=>false,"remove empty lines");
bundleCommand.AddOption(removeEmptyLinesOption);



bundleCommand.SetHandler((output, languages, note, author, sort, remove) =>
{
    string[] myLanguages = chooseLanguages(arrLanguages, extensions, languages);
    List<string> folders = Directory.GetFiles(Directory.GetCurrentDirectory(), "", SearchOption.AllDirectories).Where(file=> ! file.Contains("bin")&&!file.Contains("Debug")).ToList();
    string[] files = folders.Where(file => myLanguages.Contains(Path.GetExtension(file))).ToArray();
    try
    {
        if (files.Length > 0)
        {
            using (var file = new StreamWriter(output.FullName, false))
            {
                if (!string.IsNullOrEmpty(author))
                    file.WriteLine($"#author:{author}\n");
                if (note)
                    file.WriteLine($"#this sorces:{Directory.GetCurrentDirectory()}\n");
                foreach (var f in files)
                {
                    if (note)
                        file.WriteLine($"#this sorces:{f}\n");
                    var lines = File.ReadAllLines(f);
                    if(remove)
                        foreach (var line in lines)
                        {
                            if (string.IsNullOrEmpty(line))
                                line.Remove(0);
                        }
                }
                Array.Sort(files);
            }
        }
    }
    catch (DirectoryNotFoundException ex)
    {
        Console.WriteLine("error path is invalid");
    }

}, outputOption, languageOption, noteOption, authorOption, sortOption, removeEmptyLinesOption);

var rootCommand = new RootCommand("Root command for file Bundler CLI");
rootCommand.AddCommand(bundleCommand);
rootCommand.InvokeAsync(args);

