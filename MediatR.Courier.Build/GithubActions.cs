﻿using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Serilog;
using static Nuke.Common.Tools.Chocolatey.ChocolateyTasks;
[GitHubActions
(
    "Publish",
    GitHubActionsImage.WindowsLatest,
    OnPushTags = new[] { "*" },
    InvokedTargets = new[] { nameof(PushToNuGet) },
    ImportSecrets = new[] { nameof(NugetApiKey), nameof(NugetApiUrl) },
    EnableGitHubToken = true,
    FetchDepth = 0,
    CacheIncludePatterns = new string[0],
    CacheKeyFiles = new string[0]
)]
[GitHubActions
(
    "run-ci",
    GitHubActionsImage.WindowsLatest,
    OnPullRequestBranches = new[] { "master", "develop" },
    OnPushBranches = new[] { "master", "develop" },
    InvokedTargets = new[] { nameof(Test), nameof(SonarEnd), nameof(InstallJava) },
    ImportSecrets = new[]
    {
        nameof(SonarHostUrl),
        nameof(SonarProjectKey),
        nameof(SonarToken),
        nameof(SonarOrganization),
    },
    EnableGitHubToken = true,
    FetchDepth = 0,
    CacheIncludePatterns = new string[0],
    CacheKeyFiles = new string[0]
)]
public partial class Build
{
    string JavaVersion { get; } = "17.0.2";

    Target InstallJava => _ => _
        .Before(SonarBegin)
        .Executes(() =>
        {
            Chocolatey($"install openjdk --version={JavaVersion} --no-progress -y");

            var javaHome = @$"C:\Program Files\OpenJDK\jdk-{JavaVersion}";
            Log.Information("Setting java home to {javaHome}", javaHome);
            Environment.SetEnvironmentVariable("JAVA_HOME", javaHome);
        });
}
