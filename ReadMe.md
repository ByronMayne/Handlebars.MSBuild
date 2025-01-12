# Handlebars.MSBuild

When added to your project this exposes the ability to transform Handlebars (`.hbs`) templates as part of the build process. 

```xml
<ItemGroup>
    <HandlebarsProperty Include="Name" Value="Byron" />
    <HandlebarsProperty Include="Age" Value="34" />
    <!-- Use Templates from Files -->
    <HandlebarsTemplate Include="Passport.hbs" />
    <!-- Define Templates Inline -->
    <HandlebarsTemplate Include="DriversLicense.hbs">
        <Content>
        <![CDATA[
            <License>
                <Name>{{Name}}</Name>
                <Age>{{Age}}</Age>
            </License>
        ]]>
        </Content>
    </HandlebarsTemplate>
</ItemGroup>
```
The templates will not be rendered anywhere unless you define one of the two properties.

### OutputPath
```xml
<HandlebarsTemplate Include="Passport.hbs"  OutputPath="Passport.xml"/>
```
This will write the file relative to the Output directory where your project is building.

### AddToCompilation
```xml
<HandlebarsTemplate Include="Passport.hbs"  AddToCompilation="true" />
```
This will transform the file and write it out to the `IntermediateOutputPath` and add it to the compilation process


