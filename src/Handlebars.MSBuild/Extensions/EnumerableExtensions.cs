using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlebars.MSBuild.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Invokes an action on each element in the source collection
        /// </summary>
        /// <typeparam name="T">The type of the collection</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="action">the action to preform</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T>? action)
        {
            if (action == null)
            {
                return;
            }

            foreach (T element in source)
            {
                action(element);
            }
        }

        /// <summary>
        /// Invokes an action on each element in the source collection
        /// </summary>
        /// <typeparam name="T">The type of the collection</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="action">the action to preform with the index of the item provided </param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int>? action)
        {
            if (action == null)
            {
                return;
            }

            int index = 0;
            foreach (T element in source)
            {
                action(element, index);
                index++;
            }
        }

        public static IEnumerable<T> While<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
                yield return element;
            }
        }

        public static IEnumerable<T> While<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int index = 0;
            foreach (T element in source)
            {
                action(element, index);
                yield return element;
                index++;
            }
        }

    }
}
