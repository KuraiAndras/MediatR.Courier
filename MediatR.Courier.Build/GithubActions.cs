using Nuke.Common.CI.GitHubActions;

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
    InvokedTargets = new[] { nameof(RunCi) },
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
}
