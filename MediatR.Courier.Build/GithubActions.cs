using Nuke.Common.CI.GitHubActions;

[GitHubActions
(
    "Publish",
    GitHubActionsImage.WindowsLatest,
    OnPushTags = new[] { "*" },
    InvokedTargets = new[] { nameof(PushToNuGet) },
    ImportSecrets = new[] { nameof(NugetApiKey), "GITHUB_TOKEN" },
    EnableGitHubContext = true
)]
public partial class Build
{
}
