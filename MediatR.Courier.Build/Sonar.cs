using Nuke.Common;
using Nuke.Common.Tools.SonarScanner;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;

sealed partial class Build
{
    [Parameter] readonly string SonarProjectKey = string.Empty;
    [Parameter] readonly string SonarToken = string.Empty;
    [Parameter] readonly string SonarHostUrl = string.Empty;
    [Parameter] readonly string SonarOrganization = string.Empty;

    Target SonarBegin => _ => _
        .Before(Restore)
        .Requires(() => SonarProjectKey)
        .Requires(() => SonarToken)
        .Requires(() => SonarHostUrl)
        .Requires(() => SonarOrganization)
        .Executes(() => SonarScannerBegin(s => s
            .SetFramework("net5.0")
            .SetProjectKey(SonarProjectKey)
            .SetLogin(SonarToken)
            .SetServer(SonarHostUrl)
            .SetOrganization(SonarOrganization)
            .SetOpenCoverPaths("**/*.opencover.xml")
            .SetCoverageExclusions("**/*Example*/**")
            .SetVersion(GitVersion.NuGetVersionV2)));

    Target SonarEnd => _ => _
        .DependsOn(SonarBegin)
        .After(Test)
        .Executes(() => SonarScannerEnd(s => s
            .SetFramework("net5.0")
            .SetLogin(SonarToken)));
}
