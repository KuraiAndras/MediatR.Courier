using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

static class DotNetBuildSettingsExtensions
{
    public static DotNetBuildSettings SetContinuousIntegrationBuild(this DotNetBuildSettings settings, bool value) =>
        settings.SetProcessArgumentConfigurator(a => a.Add($"/p:ContinuousIntegrationBuild={value.ToString().ToLower()}"));
}