
//{
//    "packageName": "HoneybeeSchema",
//    "projectName": "HoneybeeSchema",
//    "packageUrl": "https://github.com/ladybug-tools/honeybee-schema-dotnet",
//    "packageVersion": "1.50.3",
//    "targetFramework": "v4.5",
//    "sourceFolder": "src",
//    "optionalAssemblyInfo": false,
//    "optionalProjectFile": false
//}

using System.Text.Json;
using System.Text.Json.Serialization;

//public static class
public class Config
{
    [JsonPropertyName("packageName")]
    public string PackageName { get; set; }

    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; }

    [JsonPropertyName("packageUrl")]
    public string PackageUrl { get; set; }

    [JsonPropertyName("packageVersion")]
    public string PackageVersion { get; set; }

    [JsonPropertyName("targetFramework")] 
    public string TargetFramework { get; set; }

    [JsonPropertyName("sourceFolder")] 
    public string SourceFolder { get; set; }

    [JsonPropertyName("optionalAssemblyInfo")] 
    public bool OptionalAssemblyInfo { get; set; }

    [JsonPropertyName("optionalProjectFile")] 
    public bool OptionalProjectFile { get; set; }

    public static Config ReadFromFile(string configFile)
    {
        var jsonString = System.IO.File.ReadAllText(configFile);
        var obj = JsonSerializer.Deserialize<Config>(jsonString, _options);
        return obj;
    }

    public void Save(string configFile)
    {
        var jsonString = JsonSerializer.Serialize(this, _options);
        System.IO.File.WriteAllText(configFile, jsonString);
    }

    public static void ReplaceVersion(string configFile, string newVersion)
    {
        var c = Config.ReadFromFile(configFile);
        c.PackageVersion = newVersion;
        c.Save(configFile);
    }

    private static JsonSerializerOptions _options = new JsonSerializerOptions { WriteIndented = true };


}





