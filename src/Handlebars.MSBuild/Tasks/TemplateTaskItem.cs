using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;

namespace Handlebars.MSBuild.Tasks
{
    public enum StreamType
    {
        File,
        Memory
    }

    internal class TemplateTaskItem : AbstractTaskItem
    {
        public StreamType StreamType { get; }
        public string? FilePath { get; }

        public string? ItemSpec { get; }
        public string? Content
        {
            get => GetMetadata<string>(nameof(Content), null);
        }

        public bool AddToCompilation
        {
            get => GetMetadata<bool>(nameof(AddToCompilation), false);
        }

        /// <summary>
        /// Gets the path the file should be written to
        /// </summary>
        public string? OutputPath
        {
            get => GetMetadata<string>(nameof(OutputPath), null);
        }

        /// <summary>
        /// Gets the content from the template that was generated
        /// </summary>
        public string? TransformedTemplate
        {
            get => GetMetadata<string>(nameof(TransformedTemplate), null);
            set => SetMetadata<string>(nameof(TransformedTemplate), value);
        }

        public bool Pack
        {
            get => GetMetadata<bool>(nameof(Pack), false);
            set => SetMetadata<bool>(nameof(Pack), value);
        }

        public string? IntermediateNugetPackPath
        {
            get => GetMetadata<string>(nameof(IntermediateNugetPackPath), null);
            set => SetMetadata<string>(nameof(IntermediateNugetPackPath), value);
        }

        /// <summary>
        /// Gets the path in the intermediate output directory where the template
        /// will be written to disk then added to the compiler. 
        /// </summary>
        public string? IntermediateCompilationPath
        {
            get => GetMetadata<string>(nameof(IntermediateCompilationPath), null);
            set => SetMetadata<string>(nameof(IntermediateCompilationPath), value);
        }


        private TemplateTaskItem(ITaskItem taskItem) : base(taskItem)
        {
            FilePath = null;
            ItemSpec = taskItem.ItemSpec;

            if (string.IsNullOrWhiteSpace(Content))
            {
                FilePath = GetMetadata<string>("FullPath", null);
                StreamType = StreamType.File;
            }
            else
            {
                StreamType = StreamType.Memory;
            }
        }

        public string GetContent()
        {
            return StreamType switch
            {
                StreamType.File => File.ReadAllText(FilePath!),
                StreamType.Memory => Content!,
                _ => string.Empty,
            };
        }


        /// <inheritdoc cref="AbstractTaskItem.Validate(ref List{string})"/>
        public override bool Validate(TaskLoggingHelper logger)
        {
            switch (StreamType)
            {
                case StreamType.File:
                    if (string.IsNullOrWhiteSpace(FilePath))
                    {
                        logger.LogError("The HandlebarsTemplate is defined with no value for the path. The `Include` property must have a file path or explicitly set the 'Content' property");
                        return false;
                    }
                    else if (!File.Exists(FilePath))
                    {
                        logger.LogError($"The HandlebarsTemplate 'Include' value is set to '{FilePath}' which does not exist on disk.");
                        return false;
                    }
                    break;
                case StreamType.Memory:
                    if (string.IsNullOrWhiteSpace(Content))
                    {
                        logger.LogError($"The HandlebarsTemplate '{FilePath}' has 'Content' defined by it's empty");
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TemplateTaskItem"/> class
        /// </summary>
        public static TemplateTaskItem Create(ITaskItem taskItem)
        {
            TemplateTaskItem template = new(taskItem);
            return template;
        }
    }
}