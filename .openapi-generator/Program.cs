using System.Diagnostics;


internal class Program
{
    #region Environment setup
    static bool isMac = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);

    static string generatorDirName = ".openapi-generator";

    static string workingDir = Environment.CurrentDirectory;
    static string docsDir => Path.Combine(workingDir, ".openapi-docs");
    static string generatorDir => Path.Combine(workingDir, generatorDirName);
    static string configFile => Path.Combine(workingDir, ".openapi-config.json");

    static string templateDir => Path.Combine(generatorDir, "templates", "csharp");
    //static string templateDir = Path.Combine(generatorDir, "templates/csharp");
    #endregion

    static void Main(string[] args)
    {
        workingDir = workingDir.Contains(generatorDirName) ? Directory.GetParent(workingDir).FullName : workingDir;
        Environment.CurrentDirectory = workingDir;


        Console.WriteLine($"Current working dir: {workingDir}");
        Console.WriteLine(string.Join(",", args));

        // Create OpenApi Config file 
        //var configFile = @"D:\Dev\ladybug_tools\openapi-to-dotnet\.openapi-generator\.openapi-config.json";
        //Config.ReplaceVersion(config, "0.0.2");
        var config = Config.ReadFromFile(configFile);
        config.PackageVersion = "0.0.2";
        config.Save(configFile);

        Console.WriteLine($"Working on {config.ProjectName} [{config.PackageVersion}]: {Path.Combine(workingDir, config.SourceFolder)}");

        //var projectName = config.ProjectName;

        // Clean 
        CleanUp(workingDir, config);

        // Generate dotnet schema
        Generate(config);


        //var done = ExeCommand("python", @"D:\Dev\ladybug_tools\openapi-to-dotnet\.openapi-generator\scripts\test.py", out var res);
        //Console.WriteLine(res);

    }


    static void CleanUp(string root, Config config)
    {
        var src = $"{root}/{config.SourceFolder}/{config.ProjectName}";

        var dirsTobeRemove = new List<string>() { 
            $"{root}/docs",
            $"{src}/Api", 
            $"{src}/Client", 
            $"{src}/Model",
            $"{src}/interface" };

        foreach (var dir in dirsTobeRemove)
        {
            var d = System.IO.Path.GetFullPath(dir);
            if (System.IO.Directory.Exists(d))
            {
                Console.WriteLine($"Cleaning: {d}");
                System.IO.Directory.Delete(d, true);
            }
               
        }

    }

    static void Generate(Config config)
    {
        var schemaJsons = System.IO.Directory.GetFiles(docsDir, "*_inheritance.json");
        //var src = Path.Combine(workingDir, config.SourceFolder);
        foreach (var schemaJson in schemaJsons)
        {
            Console.WriteLine($"Translating: {schemaJson}");
            var cmd = $"npx @openapitools/openapi-generator-cli generate -i {schemaJson} -t {templateDir} -g csharp -o . --skip-validate-spec -c {configFile}";
            Console.WriteLine(cmd);
            //Process.Start($"cmd {cmd}");
            ExeCommand("cmd", cmd, out var msg);
            //Console.WriteLine(msg);
        }
    }


    //#region Helpers
    ////prepare loader package
    //static void PrepareLoaderPackage(string version, string rhinoVersion)
    //{

    //    var v = version;
    //    if (string.IsNullOrEmpty(v))
    //        throw new ArgumentException($"Failed to get the version");

    //    var src = System.IO.Path.Combine(workingDir, "installer");
    //    var packageDir = System.IO.Path.Combine(src, "packages", "loader", rhinoVersion);

    //    var versionDir = System.IO.Path.Combine(packageDir, v);
    //    if (!Directory.Exists(versionDir)) 
    //        throw new ArgumentException($"Failed to find {versionDir}");

    //    var manifest = Path.Combine(packageDir, "manifest.txt");
    //    File.WriteAllText(manifest, v);

    //    Console.WriteLine($"Loader folder is created at: {packageDir}");

    //}


    //// Ladybug installer
    //static void GetLBTInstaller()
    //{
    //    var dir = System.IO.Path.Combine(workingDir, "installer", "packages", "lbt");
    //    if (Directory.Exists(dir))
    //        Directory.Delete(dir, true);

    //    Directory.CreateDirectory(dir);
    //    var apiUrl = @"https://api.github.com/repos/pollination/ladybug-tools-installer/releases/latest";
    //    HttpHelper.DownloadGithubAssets(dir, apiUrl);
    //    var file = Directory.GetFiles(dir).First();
    //    var newName = Path.Combine(dir, "install.exe");
    //    File.Move(file, newName);
    //}


    //// static void GetPollinationAppPanel()
    //// {
    ////     var rootDir = System.IO.Path.Combine(workingDir, "installer", "packages", "plugin");

    ////     var dir6 = System.IO.Path.Combine(rootDir, "6.0", "Pollination-panel");
    ////     var dir7 = System.IO.Path.Combine(rootDir, "7.0", "Pollination-panel");

    ////     Directory.CreateDirectory(dir6);
    ////     Directory.CreateDirectory(dir7);

    ////     var apiUrl = @"https://api.github.com/repos/pollination/rhino-panel/releases/latest";
    ////     var temp = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
    ////     var zip = HttpHelper.DownloadGithubAssets(temp, apiUrl);

    ////     System.IO.Compression.ZipFile.ExtractToDirectory(zip, temp, true);

    ////     var files = Directory.GetFiles(temp, "*", SearchOption.TopDirectoryOnly);

    ////     // copy Pollination.WebPanel.* and Ladybug.Geometry.*
    ////     var filtered = files.Where(_ => _.Contains("Pollination.WebPanel") || _.Contains("Ladybug.Geometry"));
    ////     foreach (var item in filtered)
    ////     {
    ////         if (item.EndsWith(".pdb"))
    ////             continue;
    ////         var newFile = Path.Combine(dir6, Path.GetFileName(item));
    ////         File.Copy(item, newFile);
    ////         Console.WriteLine($"Added {newFile}");
    ////     }

    ////     // copy JS folder 
    ////     var source = Path.Combine(temp, "JS");
    ////     var target = Path.Combine(dir6, "JS");

    ////     Directory.CreateDirectory(target);
    ////     var jsFiles = Directory.GetFiles(source, "*", SearchOption.TopDirectoryOnly);
    ////     foreach (var item in jsFiles)
    ////     {
    ////         var newFile = Path.Combine(target, Path.GetFileName(item));
    ////         File.Copy(item, newFile);
    ////         Console.WriteLine($"Added {newFile}");
    ////     }

    ////     // copy to 7.0 folder
    ////     CopyDirectory(dir6, dir7, true);


    //// }

    ////https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
    //static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    //{
    //    // Get information about the source directory
    //    var dir = new DirectoryInfo(sourceDir);

    //    // Check if the source directory exists
    //    if (!dir.Exists)
    //        throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

    //    // Cache directories before we start copying
    //    DirectoryInfo[] dirs = dir.GetDirectories();

    //    // Create the destination directory
    //    Directory.CreateDirectory(destinationDir);

    //    // Get the files in the source directory and copy to the destination directory
    //    foreach (FileInfo file in dir.GetFiles())
    //    {
    //        string targetFilePath = Path.Combine(destinationDir, file.Name);
    //        file.CopyTo(targetFilePath);
    //    }

    //    // If recursive and copying subdirectories, recursively call this method
    //    if (recursive)
    //    {
    //        foreach (DirectoryInfo subDir in dirs)
    //        {
    //            string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
    //            CopyDirectory(subDir.FullName, newDestinationDir, true);
    //        }
    //    }
    //}

    public static bool ExeCommand(string program, string argument, out string results)
    {
        results = string.Empty;

        var stdout = new List<string>();
        var stdErr = new List<string>();
        using (var p = new System.Diagnostics.Process())
        {
            p.StartInfo.FileName = program;
            //p.StartInfo.Arguments = argument;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine($"{argument} & exit");

            p.ErrorDataReceived += (s, m) => { if (m.Data != null) stdErr.Add(m.Data); };
            p.OutputDataReceived += (s, m) => { if (m.Data != null) Console.WriteLine(m.Data); };
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            p.WaitForExit();
            if (!p.HasExited)
            {
                p.Kill();
            }

        }

        stdout.AddRange(stdErr);
        var msg = string.Join(Environment.NewLine, stdout);
        var cmd = $"Command:\n{program} {argument}";

        if (stdErr.Count > 0)
        {
            msg = $"{cmd}\n{msg}";
            throw new ArgumentException($"{cmd}\n{string.Join(Environment.NewLine, stdErr)}");
        }

        results = msg;
        return stdErr.Count == 0;
    }


    //#endregion



}





