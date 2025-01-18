using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        [Required]
        public string TargetDir { get; set; }

        public TransformHandlebarsTemplatesTask()
        {
            BuildEngine = null!;
            TargetDir = null!;
            HostObject = null!;
            IntermediateOutputPath = null!;
            Templates = Array.Empty<ITaskItem>();
            Properties = Array.Empty<ITaskItem>();
        }



        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "TransformHandlebarsTemplatesTask.Execute()");

            if (!Directory.Exists(IntermediateOutputPath))
            {
                Directory.CreateDirectory(IntermediateOutputPath);
            }

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
            try
            {
                foreach (TemplateTaskItem template in templates)
                {
                    string content = template.GetContent();
                    string transformed = handlebars.Compile(content)(properties);
                    template.TransformedTemplate = transformed;

                    if (template.AddToCompilation)
                    {
                        template.IntermediateCompilationPath = WriteIntermediate(template);
                    }

                    if (template.Pack)
                    {
                        template.IntermediateNugetPackPath = WriteIntermediate(template);
                    }

                    else if (template.OutputPath is string outputPath)
                    {
                        if (!Path.IsPathRooted(outputPath))
                        {
                            outputPath = Path.Combine(TargetDir, outputPath);
                        }

                        string directory = Path.GetDirectoryName(outputPath);
                        if (!string.IsNullOrWhiteSpace(directory) &&
                            !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        File.WriteAllText(outputPath, transformed);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError($"Unhandled exception when running Handlebars.MSBuild: \n{ex}");
            }

            return true;
        }

        private string WriteIntermediate(TemplateTaskItem template)
        {
            string fileName = Path.GetFileName(template.FilePath);
            string directory = Path.GetDirectoryName(template.FilePath);
            string filePath = Path.Combine(IntermediateOutputPath, fileName);

            if (!File.Exists(filePath) || File.GetLastWriteTime(filePath) - DateTime.Now > TimeSpan.FromSeconds(2))
            {
                File.WriteAllText(filePath, template.TransformedTemplate);
            }



            return filePath;
        }
    }
}
