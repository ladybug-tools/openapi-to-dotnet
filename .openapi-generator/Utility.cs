public static class Utility
{
    #region Environment setup
    internal static bool isMac = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);

    internal static string generatorDirName = ".openapi-generator";

    internal static string workingDir = Environment.CurrentDirectory;
    internal static string docsDir => Path.Combine(workingDir, ".openapi-docs");
    internal static string generatorDir => Path.Combine(workingDir, generatorDirName);
    internal static string configFile => Path.Combine(workingDir, ".openapi-config.json");

    internal static string templateDir => Path.Combine(generatorDir, "templates", "csharp");
    internal static string scriptsDir => Path.Combine(generatorDir, "scripts");

    #endregion
}





