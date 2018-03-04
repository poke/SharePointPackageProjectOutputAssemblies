# SharePoint: Package project output assemblies

[![See this package on NuGet](https://img.shields.io/nuget/v/SharePointPackageProjectOutputAssemblies.svg?style=flat-square)](https://www.nuget.org/packages/SharePointPackageProjectOutputAssemblies)

This package contains MSBuild targets to support packaging other assemblies from the project output directory into the SharePoint solution package. It enables the use of special tokens in the source path for custom assemblies within the solution’s `Package.package` file. The supported tokens are `$OutputPath$` and `$Configuration$`, which will be replaced respectively by the build output path or the current build configuration.

## Background

SharePoint solution packages have the ability to deploy assemblies when a solution is installed. This is a very useful feature since the SharePoint deployment mechanism will automatically make sure to deploy the assemblies on all machines of the farm. In addition to the project’s own assembly, other assemblies can be specified in the `Package.package` file which will then be included in the published `WSP` solution file. However, this mechanism is limited to assemblies from other projects (referencing a `.csproj` using the `projectOutputAssembly` tag) or custom assemblies with a fixed path (using the `customAssembly` tag). A fixed relative path works well when referencing assemblies that exist at a static location within the project directory; but when the assembly comes from a NuGet dependency, then there is no static path for it. Instead, the assembly will be copied over to the output directory when building the project. This means that with normal project setups, there are multiple locations where the assembly could be placed, depending on the current build configuration (`Debug` or `Release`), making the use of a static path in the `customAssembly` tag difficult.

This package solves this problem by making it possible to specify a source path for custom assemblies that contains replacement tokens to reference the output directory or other configuration-based paths. When publishing the SharePoint package, the included build task will replace these tokens in the source path configuration just before the actual package contents are collected.

## How to use

After adding the package to your project, it will automatically run when packaging the SharePoint solution, e.g. when publishing the project. To add custom assemblies to the package, open the `Package.package` file and add a `customAssembly` tag for each assembly, using the replacement tokens within the `sourcePath`:

    <customAssembly location="assemblyName.dll" deploymentTarget="GlobalAssemblyCache" sourcePath="$OutputPath$\assemblyName.dll" />
    <customAssembly location="assemblyName.dll" deploymentTarget="GlobalAssemblyCache" sourcePath="bin\$Configuration$\assemblyName.dll" />

The following tokens are available:

* `$OutputPath$` – The build output path.
* `$Configuration$` – The build configuration, e.g. `Debug` or `Release`.

### Example

The following is an example of how a `Package.package` file could look like that adds `netstandard.dll` and `System.ValueTuple.dll` from the build output directory into the SharePoint solution package. When installing the solution in a farm, these assemblies will be installed into the global assembly cache for all machines.

    <?xml version="1.0" encoding="utf-8"?>
    <package xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="04d04f77-2757-4ac4-94b1-9d407987152b" solutionId="04d04f77-2757-4ac4-94b1-9d407987152b" resetWebServer="true" sharePointProductVersion="16.0" name="ExampleSharePointProject" xmlns="http://schemas.microsoft.com/VisualStudio/2008/SharePointTools/PackageModel">
      <assemblies>
        <customAssembly location="netstandard.dll" deploymentTarget="GlobalAssemblyCache" sourcePath="$OutputPath$\netstandard.dll" />
        <customAssembly location="System.ValueTuple.dll" deploymentTarget="GlobalAssemblyCache" sourcePath="$OutputPath$\System.ValueTuple.dll" />
      </assemblies>
      <features>
        <featureReference itemId="88261b0a-7b10-4004-bd07-eac36647d129" />
      </features>
    </package>
