using System.Text.Json;

public class PostProcessor
{

    static string workingDir = Environment.CurrentDirectory;

    public static void CleanUp(Config config)
    {
        var dirs = new List<string>();
        var files = new List<string>();

        // remove unused test folder
        var dir = System.IO.Path.Combine(workingDir, config.SourceFolder, $"{config.ProjectName}.Test");
        dirs.Add(dir);
    

        // remove unused Client folder
        dir = System.IO.Path.Combine(workingDir, config.SourceFolder, config.ProjectName, "Client");
        dirs.Add(dir);


        // remove all *AllOf.cs files
        var modelDir = System.IO.Path.Combine(workingDir, config.SourceFolder, config.ProjectName, "Model");
        var csFiles = System.IO.Directory.GetFiles(modelDir, "*AllOf.cs");
        files.AddRange(csFiles);

        // remove all *AllOf.md files
        var docDir = System.IO.Path.Combine(workingDir, "docs");
        var mdFiles = System.IO.Directory.GetFiles(docDir, "*AllOf.md");
        files.AddRange(mdFiles);


        foreach (var d in dirs)
        {
            if (!Directory.Exists(d))
                continue;

            Console.WriteLine($"Cleaning: {d}");
            Directory.Delete(d, true);
        }

        foreach (var d in files)
        {
            if (!File.Exists(d))
                continue;

            Console.WriteLine($"Cleaning: {d}");
            File.Delete(d);
        }

    }


    //def get_allof_types_from_json(source_json_url):
    //#load schema json, and get all union types
    //unitItem = []

    //with open(source_json_url, "rb") as jsonFile:
    //    data = json.load(jsonFile)

    //for sn, sp in data['components']['schemas'].items() :
    //    props = []
    //    if 'properties' in sp:
    //        props = sp['properties']
    //    elif 'allOf' in sp:
    //        all_objs = sp['allOf']
    //        for obj in all_objs:
    //            if 'properties' in obj:
    //                props = obj['properties']
    //    else:
    //        # skip enum type
    //        continue
    //    if props == []:
    //        continue
    //    get_allof_types(props, unitItem)
    //return unitItem

    public static void GetAllofTypes(string sourceJsonPath)
    {
        var json = File.ReadAllText(sourceJsonPath);
        var obj = JsonSerializer.Deserialize<JsonElement>(json);

        var objs = obj.GetProperty("components").GetProperty("schemas");
        //var aaa = aa as System.Text.Json.Nodes.JsonArray;

    }

    public static void CheckType()
    {
        //    all_types = get_allof_types_from_json(source_json_url)

        //    root = os.path.dirname(os.path.dirname(__file__))
        //    source_folder = os.path.join(root, 'src', name_space, 'Model')
        //    check_csfiles(source_folder, all_types)

        //    source_folder = os.path.join(root, 'src', name_space, 'Api')
        //    if os.path.exists(source_folder):
        //        check_csfiles(source_folder, all_types)


        //// get inheritance json
        //var schemaJsons = System.IO.Directory.GetFiles(docsDir, "*_inheritance.json");
        //var script = $"{scriptsDir}/post_gen_script.py";
        //foreach (var schemaJson in schemaJsons)
        //{
        //    Console.WriteLine($"Translating: {schemaJson}");
        //    var cmd = $"python {script} {schemaJson}";
        //    Console.WriteLine(cmd);
        //    //Process.Start($"cmd {cmd}");
        //    ExeCommand("cmd", cmd, out var msg);
        //    //Console.WriteLine(msg);
        //}


    }

}





