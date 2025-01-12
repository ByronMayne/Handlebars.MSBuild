using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Handlebars.MSBuild.Extensions;
using Handlebars.MSBuild.Tasks;
using HandlebarsDotNet;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Handlebars.MSBuild
{
    public class TransformHandlebarsTemplatesTask : Task
    {
        /// <summary>
        /// Gets the templates that will be transformed
        /// </summary>
        [Output]
        public ITaskItem[] Templates { get; set; }

        /// <summary>
        /// Gets the data that will be used to transform the templates
        /// </summary>
        public ITaskItem[] Properties { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }


        public TransformHandlebarsTemplatesTask()
        {
            BuildEngine = null!;
            HostObject = null!;
            IntermediateOutputPath = null!;
            Templates = Array.Empty<ITaskItem>();
            Properties = Array.Empty<ITaskItem>();
        }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "TransformHandlebarsTemplatesTask.Execute()");

            TemplateTaskItem[] templates = Templates
                .Select(TemplateTaskItem.Create)
                .Where(t => t.Validate(Log))
                .ToArray();

            Dictionary<string, string> properties = Properties
                .Select(PropertyTaskItem.Create)
                .Where(p => p.Validate(Log))
                .ToDictionary(p => p.Name, p => p.Value);


            HandlebarsConfiguration configuration = new HandlebarsConfiguration();
            IHandlebars handlebars = HandlebarsDotNet.Handlebars.Create(configuration);

            foreach(TemplateTaskItem template in templates)
            {
                string content = template.GetContent();
                string transformed = handlebars.Compile(content)(properties);
                template.TransformedTemplate = transformed;
                Log.LogMessage(MessageImportance.High, "Setting metadata");

                if (template.AddToCompilation)
                {
                    string fileName = Path.GetFileName(template.FilePath);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileName = Path.GetRandomFileName();
                    }
                    string filePath = Path.Combine(IntermediateOutputPath, $"{fileName}.cs");
                    template.GeneratedSourcePath = filePath;

                    if (!Directory.Exists(IntermediateOutputPath))
                    {
                        Directory.CreateDirectory(IntermediateOutputPath);
                    }
                    File.WriteAllText(filePath, transformed);
                }
                else if(!string.IsNullOrEmpty(template.OutputPath))
                {
                    string directory = Path.GetDirectoryName(template.OutputPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.WriteAllText(transformed, template.OutputPath);
                }
            }

            return true;
        }

    }
}
