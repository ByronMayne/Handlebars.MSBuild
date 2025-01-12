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
        public string? Content { get; }
        public bool AddToCompilation { get; }

        /// <summary>
        /// Gets the path the file should be written to
        /// </summary>
        public string? OutputPath { get; }

        /// <summary>
        /// Gets the content from the template that was generated
        /// </summary>
        public string? TransformedTemplate
        {
            get => GetMetadata<string>(nameof(TransformedTemplate), null);
            set => SetMetadata<string>(nameof(TransformedTemplate), value);
        }

        public string? GeneratedSourcePath
        {
            get => GetMetadata<string>(nameof(GeneratedSourcePath), null);
            set => SetMetadata<string>(nameof(GeneratedSourcePath), value);
        }


        private TemplateTaskItem(ITaskItem taskItem) : base(taskItem)
        {
            FilePath = null;
            Content = GetMetadata<string>("Content", null);
            OutputPath = GetMetadata<string>("OutputPath", null);
            AddToCompilation = GetMetadata<bool>("AddToCompilation", false);

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