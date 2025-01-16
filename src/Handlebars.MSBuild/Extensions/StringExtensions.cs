using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    internal static class StringExtensions
    {
        public static int Hash(this string input)
        {
            unchecked
            {
                int hash1 = 5381;
                int hash2 = hash1;

                for (int i = 0; i < input.Length && input[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ input[i];
                    if (i == input.Length - 1 || input[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ input[i + 1];
                }

                return Math.Abs(hash1 + (hash2 * 1566083941));
            }
        }
    }
}
