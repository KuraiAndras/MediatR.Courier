using Nuke.Common.CI.GitHubActions;

[GitHubActions
(
    "Publish",
    GitHubActionsImage.WindowsLatest,
    OnPushTags = new[] { "*" },
    InvokedTargets = new[] { nameof(PushToNuGet) },
    ImportSecrets = new[] { nameof(NugetApiKey), "GITHUB_TOKEN" },
    FetchDepth = 0
)]
public partial class Build
{
}
