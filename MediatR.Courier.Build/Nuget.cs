﻿using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
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
        .Executes(() =>
            EnumerateFiles(ArtifactsDirectory, "*.nupkg", SearchOption.AllDirectories)
                .ForEach(x =>
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(NugetApiUrl)
                        .SetApiKey(NugetApiKey))));
}