using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using static Nuke.Common.Tools.Chocolatey.ChocolateyTasks;
[GitHubActions
(
    "Publish",
    GitHubActionsImage.WindowsLatest,
    OnPushTags = new[] { "*" },
    InvokedTargets = new[] { nameof(PushToNuGet) },
    ImportSecrets = new[] { nameof(NugetApiKey) },
    EnableGitHubToken = true
)]
[GitHubActions
(
    "run-ci",
    GitHubActionsImage.WindowsLatest,
    OnPullRequestBranches = new[] { "master", "develop" },
    OnPushBranches = new[] { "master", "develop" },
    InvokedTargets = new[] { nameof(RunCi), nameof(InstallJava) },
    ImportSecrets = new[]
    {
        nameof(SonarHostUrl),
        nameof(SonarProjectKey),
        nameof(SonarToken),
        nameof(SonarOrganization),
    },
    EnableGitHubToken = true,
    FetchDepth = 0
)]
public partial class Build
{
    Target InstallJava => _ => _
        .Before(SonarBegin)
        .Executes(() => Chocolatey("install openjdk --version=17.0.2 -y"));
}
