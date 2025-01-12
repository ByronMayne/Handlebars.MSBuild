using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Handlebars.MSBuild.Tasks
{
    internal class PropertyTaskItem : AbstractTaskItem
    {
        public string Name { get; }
        public string Value { get; }

        public PropertyTaskItem(ITaskItem taskItem) : base(taskItem)
        {
            Name = taskItem.ItemSpec;
            Value = taskItem.GetMetadata("Value");
        }


        public override bool Validate(TaskLoggingHelper logger)
        {
            if (string.IsNullOrEmpty(Name))
            {
                logger.LogError("The Property is defined with no value for the name. The `Include` property must have a name.");
                return false;
            }

            if (string.IsNullOrEmpty(Value))
            {
                logger.LogError($"The Property '{Name}' is defined with no value. The `Value` property must have a value.");
                return false;
            }
            return true;
        }

        public static PropertyTaskItem Create(ITaskItem taskItem)
        {
            return new PropertyTaskItem(taskItem);
        }
    }
}
