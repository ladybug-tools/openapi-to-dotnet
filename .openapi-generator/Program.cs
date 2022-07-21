using System.Diagnostics;

internal class Program
{

    static void Main(string[] args)
    {
        Utility.workingDir = Utility.workingDir.Contains(Utility.generatorDirName) ? Directory.GetParent(Utility.workingDir).FullName : Utility.workingDir;
        Environment.CurrentDirectory = Utility.workingDir;


        Console.WriteLine($"Current working dir: {Utility.workingDir}");
        Console.WriteLine(string.Join(",", args));

        // Create OpenApi Config file 
        var config = Config.ReadFromFile(Utility.configFile);
        config.PackageVersion = "0.0.2.0";
        config.Save(Utility.configFile);

        Console.WriteLine($"Working on {config.ProjectName} [{config.PackageVersion}]: {Path.Combine(Utility.workingDir, config.SourceFolder)}");

        //var projectName = config.ProjectName;

        // Clean 
        CleanUp(Utility.workingDir, config);

        // Generate dotnet schema
        Generate(config);

        //post process
        PostProcess();

        // this is only for HoneybeeSchema
        if (config.PackageName.ToLower() == "honeybeeschema")
        {
            UpdateGlobalDefault();
        }

        UpdateAssemblyVersion();
        CreateInterfaces();


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
        var schemaJsons = System.IO.Directory.GetFiles(Utility.docsDir, "*_inheritance.json");
        //var src = Path.Combine(Utility.workingDir, config.SourceFolder);
        foreach (var schemaJson in schemaJsons)
        {
            Console.WriteLine($"Translating: {schemaJson}");
            var cmd = $"npx @openapitools/openapi-generator-cli generate -i {schemaJson} -t {Utility.templateDir} -g csharp -o . --skip-validate-spec -c {Utility.configFile}";
            Console.WriteLine(cmd);
            //Process.Start($"cmd {cmd}");
            ExeCommand("cmd", cmd, out var msg);
            //Console.WriteLine(msg);
        }
    }

    static void PostProcess()
    {
        var schemaJsons = System.IO.Directory.GetFiles(Utility.docsDir, "*_inheritance.json");
        var script = $"{Utility.scriptsDir}/post_gen_script.py";
        foreach (var schemaJson in schemaJsons)
        {
            Console.WriteLine($"Translating: {schemaJson}");
            var cmd = $"python {script} {schemaJson}";
            Console.WriteLine(cmd);
            //Process.Start($"cmd {cmd}");
            ExeCommand("cmd", cmd, out var msg);
        }

    }

    static void UpdateGlobalDefault()
    {
        var script = $"{Utility.scriptsDir}/create_global_default.py";
        var cmd = $"python {script}";
        Console.WriteLine(cmd);
        //Process.Start($"cmd {cmd}");
        ExeCommand("cmd", cmd, out var msg);

    }

    static void UpdateAssemblyVersion()
    {
        var script = $"{Utility.scriptsDir}/update_assembly_version.py";
        var cmd = $"python {script}";
        Console.WriteLine(cmd);
        //Process.Start($"cmd {cmd}");
        ExeCommand("cmd", cmd, out var msg);

    }

    static void CreateInterfaces()
    {
        var schemaJsons = System.IO.Directory.GetFiles(Utility.docsDir, "*_mapper.json");
        var script = $"{Utility.scriptsDir}/create_interface.py";
        foreach (var schemaJson in schemaJsons)
        {
            Console.WriteLine($"Translating: {schemaJson}");
            var cmd = $"python {script} {schemaJson}";
            Console.WriteLine(cmd);
            //Process.Start($"cmd {cmd}");
            ExeCommand("cmd", cmd, out var msg);
        }

    }

    //#region Helpers
    ////prepare loader package
    //static void PrepareLoaderPackage(string version, string rhinoVersion)
    //{

    //    var v = version;
    //    if (string.IsNullOrEmpty(v))
    //        throw new ArgumentException($"Failed to get the version");

    //    var src = System.IO.Path.Combine(Utility.workingDir, "installer");
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
    //    var dir = System.IO.Path.Combine(Utility.workingDir, "installer", "packages", "lbt");
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
    ////     var rootDir = System.IO.Path.Combine(Utility.workingDir, "installer", "packages", "plugin");

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





