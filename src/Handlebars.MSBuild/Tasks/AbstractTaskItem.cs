using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Handlebars.MSBuild.Tasks
{
    internal abstract class AbstractTaskItem
    {
        private readonly ITaskItem m_taskItem;

        protected AbstractTaskItem(ITaskItem taskItem)
        {
            m_taskItem = taskItem;
        }

        /// <summary>
        /// Runs validations and returns back if they passed or not
        /// </summary>
        /// <param name="logger">The logger used to output errors for validations</param>
        /// <returns>True if there are no errors otherwise false</returns>
        public abstract bool Validate(TaskLoggingHelper logger);

        protected void SetMetadata<T>(string key, T? value)
        {
            if (value != null)
            {
                m_taskItem.SetMetadata(key, value.ToString());
            }
        }

        [return: NotNullIfNotNull(nameof(defaultValue))]
        protected T? GetMetadata<T>(string name, T? defaultValue)
        {
            string? content = m_taskItem.GetMetadata(name);
            if (content == null)
            {
                return defaultValue;
            }
            Type type = typeof(T);

            object? result = default(T);
            if (type == typeof(string))
            {
                result = content;
            }
            else if (type == typeof(bool))
            {
                if (bool.TryParse(content, out bool parsedBool))
                {
                    result = parsedBool;
                }
            }
            else if (type == typeof(int))
            {
                if (int.TryParse(content, out int parsedInt))
                {
                    result = parsedInt;
                }
            }

            return (T?)result!;
        }
    }
}