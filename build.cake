#tool nuget:?package=GitVersion.CommandLine&version=5.3.7
#tool nuget:?package=NUnit.ConsoleRunner&version=3.10.0
#addin nuget:?package=Cake.FileHelpers&version=3.2.1
#addin nuget:?package=Cake.Git&version=0.22.0

var pluginName = "LeetSpeak";
var configuration = Argument ("configuration", "Release");
var version = GitVersion ().SemVer;

Task ("Clean")
    .Does (() => {
        CleanDirectory ("./src/" + pluginName + "/bin");
        Information("Clean complete.");
});

Task ("Restore")
    .IsDependentOn ("Clean")
    .Does (() => {
        NuGetRestore ("./src/" + pluginName + ".sln");
});

Task ("Update-Assembly-Info")
    .IsDependentOn ("Restore")
    .Does (() => {
        GitVersion (new GitVersionSettings {
            UpdateAssemblyInfo = true
        });
});

Task ("Update-Plugin-Json")
    .IsDependentOn ("Update-Assembly-Info")
    .Does (() => {
        string json = System.IO.File.ReadAllText("./src/" + pluginName + "/Properties/" + pluginName + ".json");
        json = TransformText(json).WithToken("name", pluginName).ToString();
        json = TransformText(json).WithToken("version", version).ToString();
        System.IO.File.WriteAllText("./src/" + pluginName + "/bin/" + pluginName + ".json", json);
});

Task("Build")
    .IsDependentOn ("Update-Plugin-Json")
    .Does(() => {
        MSBuild ("./src/" + pluginName + "/" + pluginName + ".csproj", settings => settings.SetConfiguration (configuration));
});

Task("Publish-Official-Repo")
    .IsDependentOn ("Build")
    .Does(() => {

        // package
        CreateDirectory("./src/" + pluginName + "/bin/latest");
        CopyFile("./src/" + pluginName + "/bin/" + pluginName + ".json", "./src/" + pluginName + "/bin/latest/" + pluginName + ".json");
        CopyFile("./src/" + pluginName + "/bin/" + pluginName + ".dll", "./src/" + pluginName + "/bin/latest/" + pluginName + ".dll");
        Zip("./src/" + pluginName + "/bin/latest", "./src/" + pluginName + "/bin/latest.zip");
        Information("Packaged plugin for publishing to official repo.");

        // copy to official repo workspace
        EnsureDirectoryExists("../DalamudPlugins/plugins/" + pluginName);
        CleanDirectory("../DalamudPlugins/plugins/" + pluginName);
        CopyFile("./src/" + pluginName + "/bin/" + pluginName + ".json", "../DalamudPlugins/plugins/" + pluginName + "/" + pluginName + ".json");
        CopyFile("./src/" + pluginName + "/bin/latest.zip", "../DalamudPlugins/plugins/" + pluginName + "/latest.zip");
        Information("Copied package into official plugin workspace.");
});

Task("Publish-Custom-Repo")
    .IsDependentOn ("Publish-Official-Repo")
    .Does(() => {

        // create new json for test version
        string json = System.IO.File.ReadAllText("./src/" + pluginName + "/Properties/" + pluginName + ".json");
        json = TransformText(json).WithToken("name", pluginName + " [Canary]").ToString();
        json = TransformText(json).WithToken("version", version).ToString();
        System.IO.File.WriteAllText("./src/" + pluginName + "/bin/" + pluginName + ".json", json);

        // package
        EnsureDirectoryExists("./src/" + pluginName + "/bin/latest");
        CleanDirectory("./src/" + pluginName + "/bin/latest");
        CopyFile("./src/" + pluginName + "/bin/" + pluginName + ".json", "./src/" + pluginName + "/bin/latest/" + pluginName + ".json");
        CopyFile("./src/" + pluginName + "/bin/" + pluginName + ".dll", "./src/" + pluginName + "/bin/latest/" + pluginName + ".dll");
        Zip("./src/" + pluginName + "/bin/latest", "./src/" + pluginName + "/bin/latest.zip");
        Information("Packaged plugin for publishing to custom repo.");

        // copy to custom repo workspace
        EnsureDirectoryExists("../DalamudPluginRepo/plugins/" + pluginName);
        CleanDirectory("../DalamudPluginRepo/plugins/" + pluginName);
        CopyFile("./src/" + pluginName + "/bin/" + pluginName + ".json", "../DalamudPluginRepo/plugins/" + pluginName + "/" + pluginName + ".json");
        CopyFile("./src/" + pluginName + "/bin/latest.zip", "../DalamudPluginRepo/plugins/" + pluginName + "/latest.zip");
        Information("Copied package into custom plugin workspace.");
});

Task("Cleanup")
    .IsDependentOn ("Publish-Custom-Repo")
    .Does(() => {
        GitCheckout("./", MakeAbsolute(File("./src/" + pluginName + "/Properties/AssemblyInfo.cs")));
        Information("Reverted assembly info.");
});

Task ("Default")
    .IsDependentOn ("Clean")
    .IsDependentOn ("Restore")
    .IsDependentOn ("Update-Assembly-Info")
    .IsDependentOn ("Update-Plugin-Json")
    .IsDependentOn ("Build")
    .IsDependentOn ("Publish-Official-Repo")
    .IsDependentOn ("Publish-Custom-Repo")
    .IsDependentOn ("Cleanup");

RunTarget ("Default");