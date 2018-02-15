using System;
using System.Collections.Generic;

namespace netcore.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Return a copy of string
        /// </summary>
        /// <param name="text">The text will be copied</param>
        /// <returns>A copied string</returns>
        public static string Copy(this string text)
        {
            return $"{text}";
        }

        /// <summary>
        /// Parse digit string into integer data type
        /// </summary>
        /// <param name="text">digit string</param>
        /// <returns>Integer value</returns>
        public static int ToInt(this string text)
        {
            return int.Parse(text);
        }

        /// <summary>
        /// Parse digit string into double data type
        /// </summary>
        /// <param name="text">digit string</param>
        /// <returns>Double value</returns>
        public static double ToDouble(this string text)
        {
            return double.Parse(text);
        }
        
        /// <summary>
        /// Eclipsed the string within specified length and suffix with '...' 
        /// </summary>
        /// <param name="text">string that will be eclipsed</param>
        /// <param name="length">the position that start to replace with '...'</param>
        /// <returns>Eclipsed string</returns>
        public static string EclipsedString(this string text, int length) 
        {
            if (text.Length < length)
                throw new ArgumentOutOfRangeException(nameof(text), "The length of text should longer than that of substring.");

            return String.Concat(text.Substring(0, length), "...");            
        }
    }
}