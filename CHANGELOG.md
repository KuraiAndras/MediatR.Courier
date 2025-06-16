# 7.0.1
- Remove unnecessary check for _options.UseTaskWhenAll
- README: Add 'Parallel notification handling' section

# 7.0.0
- Removed Nuke
- Updated to .NET 8
- Fixed sonarcloud. Now only run on master pushes, on PRs we only run dotnet test and format
- Removed old azure devops pipelines
- Pipelines now fully written in github actions
- Updated dependencies
- Added documentation comments to `ICourier`, `CourierInjector` and `CourierOptions`
- Use Invoke iso DynamicInvoke for better performance
- Fix bug where RemoveWeak would sometimes remove too many handlers
- Add `UseTaskWhenAll` option for parallel notification handling

# 6.0.0
- Update MediatR to version 12
- Deprecate DI package
- Move test and sample code into one project
- Removed support for convention and interface clients
- Multi target .NET standard 2.0 and .NET 6
- Use reproducible builds
- Include Readme and Changelog in package

# 5.0.0
- Update MediatR to version 10
- Update target framework to netstandard2.1
