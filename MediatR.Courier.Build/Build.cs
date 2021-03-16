using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using System;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly bool CiBuild;

    [Solution] readonly Solution Solution;
    [GitVersion(Framework = "netcoreapp3.1")] readonly GitVersion? GitVersion;
    [PathExecutable] readonly Tool Git;

    string TagVersion => Git.Invoke("describe --tags").First().Text ?? throw new InvalidOperationException("Cloud not get version from git");

    string NugetVersion => GitVersion?.NuGetVersionV2 ?? TagVersion;
    string AssemblyVersion => GitVersion?.AssemblySemVer ?? TagVersion;
    string AssemblyFileVersion => GitVersion?.AssemblySemFileVer ?? TagVersion;
    string InformationalVersion => GitVersion?.InformationalVersion ?? TagVersion;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() => DotNetRestore(s => s
            .SetProjectFile(Solution)));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => DotNetBuild(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .SetContinuousIntegrationBuild(CiBuild)
            .SetAssemblyVersion(AssemblyVersion)
            .SetFileVersion(AssemblyFileVersion)
            .SetInformationalVersion(InformationalVersion)
            .EnableNoRestore()));

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() => DotNetTest(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .SetCollectCoverage(true)
            .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
            .EnableNoRestore()
            .EnableNoBuild()));
}
