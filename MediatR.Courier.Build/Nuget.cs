using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using System.IO;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static System.IO.Directory;

sealed partial class Build
{
    [Parameter] readonly string NugetApiUrl = default!;
    [Parameter] readonly string NugetApiKey = default!;

    Target PushToNuGet => _ => _
        .DependsOn(Pack)
        .Requires(() => NugetApiUrl)
        .Requires(() => NugetApiKey)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
            EnumerateFiles(ArtifactsDirectory, "*.nupkg", SearchOption.AllDirectories)
                .SelectMany(x =>
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(NugetApiUrl)
                        .SetApiKey(NugetApiKey))));
}