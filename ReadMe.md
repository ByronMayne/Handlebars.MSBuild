# Handlebars.MSBuild

Adds a custom MSBuild Task for transforming [Handlebars Templates](https://handlebarsjs.com/) as part of the build process. Define your templates that you want to transform define `HandlebarsTemplate` items. 

```xml
<Project>
    <ItemGroup>
        <HandlebarsTemplate Include="/AssemblyMetadata.hbs"/>
    </ItemGroup>
</Project>
```
You can define your templates as files on disk

```hbs
{{! AssemblyMetadata.hbs}}
public static class AssemblyMetadata
{
    public static readonly string Name = {{MSBuildProjectName}};
    public static readonly Version Version = new Version({{Version}});
}
```

## Properties 
There is a wide range of properties that are [already exposed](https://github.com/ByronMayne/Handlebars.MSBuild/blob/main/src/Handlebars.MSBuild/Handlebars.MSBuild.props) but you can also define your own.

```xml
<ItemGroup>
    <HandlebarsProperty Include="Name" Value="John Smith"/>
</ItemGroup>
```
Then you can use this value in your template.
```hbs
Hello {{Name}}, Nice to meet you!
```


## Export

### Output Directory

At this point no files will be generated as defining the templates only makes the custom task populated the metadata `TransformedTemplate`. You can either do what you want this this or use one of the following export features. 

```xml
<Project>
    <ItemGroup>
        <HandlebarsTemplate 
            Include="/AssemblyMetadata.hbs" 
            OutputPath="AssemblyMetadata.cs"/>
    </ItemGroup>
</Project>
```
Writes the generated template to the `bin` directory using the given name.

### NuGet Package
```xml
<Project>
    <ItemGroup>
        <HandlebarsTemplate 
            Include="/AssemblyMetadata.hbs" 
            Pack="True"
            PackagePath="Build/AssemblyMetadata.cs"/>
    </ItemGroup>
</Project>
```
Adds the generated output to the NuGet package that will be generated.

### Compilation
```xml
<Project>
    <ItemGroup>
        <HandlebarsTemplate 
            Include="/AssemblyMetadata.hbs" 
            AddToCompilation="True" />
    </ItemGroup>
</Project>
```
This will add the generated file to the compilation process. 