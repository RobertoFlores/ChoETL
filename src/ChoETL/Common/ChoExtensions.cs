﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChoETL
{
    [Flags]
    public enum ChoStringSplitOptions
    {
        None = 0,
        RemoveEmptyEntries = 1,
        AllowQuotes = 2,
        All = RemoveEmptyEntries | AllowQuotes
    }

    public static class ChoExtensions
    {
        #region SplitNTrim Overloads (Public)

        /// <summary>
        /// Split the string into multiple strings by a ',', ';' Separators and trim them each.
        /// </summary>
        /// <param name="text">A string value to be splited and trim.</param>
        /// <returns>A string array contains splitted and trimmed string values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] SplitNTrim(this string text)
        {
            return SplitNTrim(text, new char[] { ',', ';' });
        }

        /// <summary>
        /// Split the string into multiple strings by the Separators and trim the each one.
        /// </summary>
        /// <param name="text">A string value to be splited and trim.</param>
        /// <param name="Separators">List of Separators used to split the string.</param>
        /// <returns>A string array contains splitted and trimmed string values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] SplitNTrim(this string text, char Separator)
        {
            return SplitNTrim(text, new char[] { Separator });
        }

        /// <summary>
        /// Split the string into multiple strings by the Separators and trim the each one.
        /// </summary>
        /// <param name="text">A string value to be splited and trim.</param>
        /// <param name="Separators">List of Separators used to split the string.</param>
        /// <returns>A string array contains splitted and trimmed string values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] SplitNTrim(this string text, char[] Separators)
        {
            return SplitNTrim(text, Separators, ChoStringSplitOptions.All);
        }

        public static string[] SplitNTrim(this string text, string value)
        {
            return SplitNTrim(text, value, ChoStringSplitOptions.All);
        }

        public static string[] SplitNTrim(this string text, string value, ChoStringSplitOptions stringSplitOptions, char quoteChar = '"')
        {
            if (text == null || text.Trim().Length == 0) return new string[] { };

            string word;
            List<string> tokenList = new List<string>();
            foreach (string token in Split(text, value, stringSplitOptions, quoteChar))
            {
                word = token != null ? token.Trim() : token;
                if (String.IsNullOrEmpty(word))
                {
                    if (stringSplitOptions != ChoStringSplitOptions.RemoveEmptyEntries)
                        tokenList.Add(word);
                }
                else
                    tokenList.Add(word);
            }

            return tokenList.ToArray();
        }

        public static string[] SplitNTrim(this string text, char[] Separators, ChoStringSplitOptions stringSplitOptions, char quoteChar = '"')
        {
            if (text == null || text.Trim().Length == 0) return new string[] { };

            string word;
            List<string> tokenList = new List<string>();
            foreach (string token in Split(text, Separators, stringSplitOptions, quoteChar))
            {
                word = token != null ? token.Trim() : token;
                if (String.IsNullOrEmpty(word))
                {
                    if (stringSplitOptions != ChoStringSplitOptions.RemoveEmptyEntries)
                        tokenList.Add(word);
                }
                else
                    tokenList.Add(word);
            }

            return tokenList.ToArray();
        }

        #endregion SplitNTrim Overloads (Public)

        #region Split Overloads (Public)

        /// <summary>
        /// Split the string into multiple strings by a ',', ';' Separators.
        /// </summary>
        /// <param name="text">A string value to be split.</param>
        /// <returns>A string array contains splitted values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] Split(this string text)
        {
            return SplitNTrim(text, new char[] { ',', ';' });
        }

        /// <summary>
        /// Split the string into multiple strings by a Separator.
        /// </summary>
        /// <param name="text">A string value to be split.</param>
        /// <param name="Separator">A Separator used to split the string.</param>
        /// <returns>A string array contains splitted values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] Split(this string text, char Separator)
        {
            return Split(text, new char[] { Separator });
        }

        /// <summary>
        /// Split the string into multiple strings by the Separators.
        /// </summary>
        /// <param name="text">A string value to be split.</param>
        /// <param name="Separators">List of Separators used to split the string.</param>
        /// <returns>A string array contains splitted values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] Split(this string text, char[] Separators)
        {
            return Split(text, Separators, ChoStringSplitOptions.All);
        }

        public static string[] Split(this string text, string value)
        {
            return Split(text, value, ChoStringSplitOptions.All);
        }

        /// <summary>
        /// Split the string into multiple strings by the Separators.
        /// </summary>
        /// <param name="text">A string value to be split.</param>
        /// <param name="Separators">List of Separators used to split the string.</param>
        /// <param name="ignoreEmptyWord">true, to ignore the empry words in the output list</param>
        /// <returns>A string array contains splitted values, if the input text is null/empty, an empty array will be returned.</returns>
        public static string[] Split(this string text, char[] Separators, ChoStringSplitOptions stringSplitOptions, char quoteChar = '"')
        {
            return Split(text, (object)Separators, stringSplitOptions, quoteChar);
        }

        public static string[] Split(this string text, string value, ChoStringSplitOptions stringSplitOptions, char quoteChar = '"')
        {
            return Split(text, (object)value, stringSplitOptions, quoteChar);
        }

        private static string[] Split(this string text, object Separators, ChoStringSplitOptions stringSplitOptions, char quoteChar = '"')
        {
            if (String.IsNullOrEmpty(text)) return new string[0];

            List<string> splitStrings = new List<string>();

            if (quoteChar == '\0')
                quoteChar = '"';

            if (Separators is char[] && Array.IndexOf(((char[])Separators), quoteChar) >= 0)
            {
                throw new ApplicationException("Invalid quote character passed.");
            }
            else if (Separators is string && ((string)Separators).Contains(quoteChar))
            {
                throw new ApplicationException("Invalid quote character passed.");
            }

            int len = Separators is char[] ? 0 : ((string)Separators).Length - 1;
            int i = 0;
            int quotes = 0;
            int singleQuotes = 0;
            int offset = 0;
            bool hasChar = false;
            string word = null;
            while (i < text.Length)
            {
                if ((stringSplitOptions & ChoStringSplitOptions.AllowQuotes) != ChoStringSplitOptions.AllowQuotes && text[i] == quoteChar) { quotes++; }
                //else if ((stringSplitOptions & ChoStringSplitOptions.AllowSingleQuoteEntry) != ChoStringSplitOptions.AllowSingleQuoteEntry && text[i] == '\'') { singleQuotes++; }
                else if (text[i] == '\\'
                    && i + 1 < text.Length && Contains(text, ++i, Separators))
                    hasChar = true;
                else if (Contains(text, i, Separators) &&
                    ((quotes > 0 && quotes % 2 == 0) || (singleQuotes > 0 && singleQuotes % 2 == 0))
                    || Contains(text, i, Separators) && quotes == 0 && singleQuotes == 0)
                {
                    if (hasChar)
                    {
                        word = NormalizeString(text.Substring(offset, i - len - offset).Replace("\\", String.Empty), quoteChar);
                        if (String.IsNullOrEmpty(word))
                        {
                            if (stringSplitOptions != ChoStringSplitOptions.RemoveEmptyEntries)
                                splitStrings.Add(word);
                        }
                        else
                            splitStrings.Add(word);

                        hasChar = false;
                    }
                    else
                    {
                        string subString = text.Substring(offset, i - len - offset);
                        if (subString.Length == 2)
                            splitStrings.Add(subString);
                        else
                        {
                            word = NormalizeString(subString, quoteChar);
                            if (String.IsNullOrEmpty(word))
                            {
                                if (stringSplitOptions != ChoStringSplitOptions.RemoveEmptyEntries)
                                    splitStrings.Add(word);
                            }
                            else
                                splitStrings.Add(word);
                        }
                    }

                    offset = i + 1;
                }
                i++;
            }

            splitStrings.Add(hasChar ? NormalizeString(text.Substring(offset).Replace("\\", String.Empty), quoteChar) : 
                NormalizeString(text.Substring(offset), quoteChar));

            return splitStrings.ToArray();
        }

        #endregion Split Overloads (Public)

        #region Contains Overloads (Public)

        public static bool Contains(char inChar, char[] findInChars)
        {
            foreach (char findInChar in findInChars)
            {
                if (findInChar == inChar) return true;
            }
            return false;
        }

        public static bool Contains(string text, int index, char[] findInChars)
        {
            char inChar = text[index];
            foreach (char findInChar in findInChars)
            {
                if (findInChar == inChar) return true;
            }
            return false;
        }

        public static bool Contains(string text, int index, string findInText)
        {
            index = index - (findInText.Length - 1);
            if (index < 0) return false;

            return text.IndexOf(findInText, index) == index;
        }

        #endregion Contains Overloads (Public)

        private static bool Contains(string text, int index, object findInChars)
        {
            if (findInChars is char[])
                return Contains(text, index, ((char[])findInChars));
            else if (findInChars is string)
                return Contains(text, index, ((string)findInChars));
            else
                return false;
        }

        private static string NormalizeString(string inString, char quoteChar)
        {
            if (inString == null || inString.Length < 2) return inString;
            if (inString[0] == quoteChar && inString[inString.Length - 1] == quoteChar)
                return inString.Substring(1, inString.Length - 2);
            //if (inString.Contains("\"\""))
            //    return inString.Replace("\"\"", "\"");
            //else if (inString.Contains("''"))
            //    return inString.Replace("''", "'");
            else
                return inString;
        }

        public static string Indent(this String text, int totalWidth = 1)
        {
            return Indent(text, totalWidth, ChoCharEx.HorizontalTab.ToString());
        }

        /// <summary>
        /// Left aligns the characters in this string on each line, padding on the right with
        /// the specified Unicode character, for a specified total length
        /// </summary>
        /// <param name="text">The string value which will be indented</param>
        /// <param name="totalWidth">The number of padding characters to be added at the beginning of each line in the resulting string. If the value is negative, it will be undented the specified character with number of specified width</param>
        /// <param name="paddingChar">A Unicode padding character.</param>
        /// <returns>A new System.String that is equivalent to this instance, but left-aligned and padded on the right with as many paddingChar characters as needed to each line of this string.</returns>
        public static string Indent(this String text, int totalWidth, string paddingChar)
        {
            if (text == null) return null;

            if (totalWidth == 0) return text;
            if (totalWidth < 0) return Unindent(text, totalWidth, paddingChar);

            string tabs = String.Empty;
            for (int index = 0; index < totalWidth; index++)
                tabs = tabs + paddingChar;

            string pattern = String.Format(@".*[{0}]*", Environment.NewLine);

            StringBuilder formattedtext = new StringBuilder();
            foreach (Match m in Regex.Matches(text, pattern))
            {
                if (m.ToString() == Environment.NewLine || String.IsNullOrEmpty(m.ToString().Trim()))
                    formattedtext.AppendFormat("{0}", m.ToString());
                else
                    formattedtext.AppendFormat("{0}{1}", tabs, m.ToString());
            }

            return formattedtext.ToString();
        }

        public static string Unindent(this String text)
        {
            return Unindent(text, 1, ChoCharEx.HorizontalTab.ToString());
        }

        public static string Unindent(this String text, int totalWidth)
        {
            return Unindent(text, totalWidth, ChoCharEx.HorizontalTab.ToString());
        }

        public static string Unindent(this String text, int totalWidth, string paddingChar)
        {
            if (text == null) return null;
            if (totalWidth == 0) return text;
            if (totalWidth < 0)
                return Indent(text, Math.Abs(totalWidth), paddingChar);

            string linePattern = String.Format(@".*[{0}]*", Environment.NewLine);
            string pattern = String.Format(@"{1}(?<text>.*[{0}]|.*)", Environment.NewLine, paddingChar);
            StringBuilder formattedMsg = new StringBuilder();

            for (int index = -1 * Math.Abs(totalWidth); index < 0; index++)
            {
                formattedMsg = new StringBuilder();
                foreach (Match m in Regex.Matches(text, linePattern))
                {
                    if (m.ToString() == Environment.NewLine || String.IsNullOrEmpty(m.ToString().Trim()))
                        formattedMsg.AppendFormat("{0}", m.ToString());
                    else
                    {
                        Match match = Regex.Match(m.ToString(), pattern);
                        if (!match.Success)
                            return text;
                        formattedMsg.AppendFormat("{0}", match.Groups["text"].ToString());
                    }
                }

                text = formattedMsg.ToString();
            }

            return formattedMsg.ToString();
        }
        public static bool ContainsMultiLines(this string inString)
        {
            if (inString.IsNullOrEmpty())
                return false;

            return inString.IndexOf(Environment.NewLine) != inString.LastIndexOf(Environment.NewLine);
        }

        public static void Reset(this object target)
        {
            Initialize(target);
        }

        public static void Initialize(this object target)
        {
            if (target == null)
                return;

            object defaultValue = null;
            foreach (PropertyDescriptor pd in ChoTypeDescriptor.GetProperties<DefaultValueAttribute>(target.GetType()))
            {
                try
                {
                    defaultValue = ChoTypeDescriptor.GetPropetyAttribute<DefaultValueAttribute>(pd).Value;
                    if (defaultValue != null)
                        ChoType.SetMemberValue(target, pd.Name, defaultValue);
                }
                catch (Exception ex)
                {
                    ChoETLFramework.WriteLog(ChoETLFramework.TraceSwitch.TraceError, "Error while assigning default value '{0}' to '{1}' member. {2}".FormatString(defaultValue, ChoType.GetMemberName(pd), ex.Message));
                }
            }

            ChoETLFramework.InitializeObject(target);

            if (target is IChoInitializable)
                ((IChoInitializable)target).Initialize();
        }

        public static void Raise(this EventHandler eventHandler, object sender, EventArgs e)
        {
            EventHandler lEventHandler = eventHandler;
            if (lEventHandler != null)
                lEventHandler(sender, e);
        }

        public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
            EventHandler<T> lEventHandler = eventHandler;
            if (lEventHandler != null)
                lEventHandler(sender, e);
        }

        public static string FormatString(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static IEnumerable<string> ReadLines(this StreamReader reader, char delimeter)
        {
            ChoGuard.ArgumentNotNull(reader, "TextReader");

            List<char> chars = new List<char>();
            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();

                if (c == delimeter)
                {
                    yield return new String(chars.ToArray());
                    chars.Clear();
                    continue;
                }

                chars.Add(c);
            }
        }

        public static IEnumerable<string> ReadLines(this TextReader reader, string EOLDelimiter = null, char quoteChar = ChoCharEx.NUL, int maxLineSize = 32768)
        {
            ChoGuard.ArgumentNotNull(reader, "TextReader");
            EOLDelimiter = EOLDelimiter ?? Environment.NewLine;

            bool inQuote = false;
            List<char> buffer = new List<char>();
            CircularBuffer<char> delim_buffer = new CircularBuffer<char>(EOLDelimiter.Length);
            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();
                delim_buffer.Enqueue(c);
                if (quoteChar != ChoCharEx.NUL && quoteChar == c)
                {
                    inQuote = !inQuote;
                }
                
                if (!inQuote)
                {
                    if (delim_buffer.ToString() == EOLDelimiter)
                    {
                        if (buffer.Count > 0)
                        {
                            string x = new String(buffer.ToArray());
                            yield return x.Substring(0, x.Length - (EOLDelimiter.Length - 1));
                            buffer.Clear();
                        }
                        continue;
                    }
                }
                buffer.Add(c);

                if (buffer.Count > maxLineSize)
                    throw new ApplicationException("Large line found. Check and correct the end of line delimiter.");
            }

            if (buffer.Count > 0)
                yield return new String(buffer.ToArray());
            else
                yield break;
        }

        private class CircularBuffer<T> : Queue<T>
        {
            private int _capacity;

            public CircularBuffer(int capacity)
                : base(capacity)
            {
                _capacity = capacity;
            }

            new public void Enqueue(T item)
            {
                if (_capacity > 0 && base.Count == _capacity)
                {
                    base.Dequeue();
                }
                base.Enqueue(item);
            }

            public override string ToString()
            {
                List<String> items = new List<string>();
                foreach (var x in this)
                {
                    items.Add(x.ToString());
                };
                return String.Join("", items);
            }
        }

        public static bool IsEmpty(this string text)
        {
            if (text != null)
                return text.Length == 0;
            return false;
        }

        public static bool IsNull(this string text)
        {
            return text == null;
        }

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static Type GetEnumerableType(this Type type)
        {
            foreach (Type type1 in type.GetInterfaces())
            {
                if (type1.IsGenericType && type1.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return type1.GetGenericArguments()[0];
            }
            return (Type)null;
        }

        public static bool IsGenericList(this Type type)
        {
            return Enumerable.Any<Type>((IEnumerable<Type>)type.GetInterfaces(), (Func<Type, bool>)(t =>
            {
                if (t.IsGenericType)
                    return t.GetGenericTypeDefinition() == typeof(IList<>);
                return false;
            }));
        }

        public static bool IsNullableType(this Type type)
        {
            return type != (Type)null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>).GetGenericTypeDefinition();
        }

        public static Type GetUnderlyingType(this Type type)
        {
            if (IsNullableType(type))
                return Nullable.GetUnderlyingType(type);
            return type;
        }

        public static object Default(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return (object)null;
        }

        public static Attribute GetCustomAttribute(this Type type, Type attributeType)
        {
            return GetCustomAttribute(type, attributeType, false);
        }

        public static Attribute GetCustomAttribute(this Type type, Type attributeType, bool inherit)
        {
            object[] customAttributes = type.GetCustomAttributes(attributeType, inherit);
            if (customAttributes != null && customAttributes.Length != 0)
                return customAttributes[0] as Attribute;
            return (Attribute)null;
        }

        //public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        //{
        //    return GetCustomAttribute<T>(type, false);
        //}

        //public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
        //{
        //    object[] customAttributes = type.GetCustomAttributes(typeof(T), inherit);
        //    if (customAttributes != null && customAttributes.Length != 0)
        //        return customAttributes[0] as T;
        //    return default(T);
        //}

        public static T GetCustomAttribute<T>(this Type type)
            where T : Attribute
        {
            Type attributeType = typeof(T);
            if (type == null)
                return default(T);

            if (type.GetCustomAttributes().Any(a => a.GetType() == attributeType))
                return (from x in type.GetCustomAttributes()
                        where x.GetType() == attributeType
                        select x).FirstOrDefault() as T;

            var interfaces = type.GetInterfaces();

            for (int i = 0; i < interfaces.Length; i++)
            {
                Attribute[] attr = GetCustomAttributes(interfaces[i], attributeType);
                if (attr != null && attr.Length > 0)
                    return (T)attr[0];
            }

            return default(T);
        }

        public static Attribute[] GetCustomAttributes(this Type type, Type attributeType)
        {
            if (type == null)
                return new Attribute[] { };

            if (type.GetCustomAttributes().Any(a => a.GetType() == attributeType))
                return (from x in type.GetCustomAttributes()
                        where x.GetType() == attributeType
                        select x).ToArray();

            var interfaces = type.GetInterfaces();

            for (int i = 0; i < interfaces.Length; i++)
            {
                Attribute[] attr = GetCustomAttributes(interfaces[i], attributeType);
                if (attr != null && attr.Length > 0)
                    return attr;
            }

            return new Attribute[] { };
        }

        public static bool IsSimple(this Type type)
        {
            CheckTypeIsNotNull(type);
            return type.IsPrimitive || typeof(string) == type || (type.IsEnum || typeof(DateTime) == type);
        }

        public static bool IsSubclassOfRawGeneric(this Type generic, Type toCheck)
        {
            for (; toCheck != (Type)null && toCheck != typeof(object); toCheck = toCheck.BaseType)
            {
                Type type = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == type)
                    return true;
            }
            return false;
        }

        private static bool IsInParametersMatch(ParameterInfo[] funcInParameterTypes, Type[] inParameterTypes)
        {
            if (inParameterTypes == null && funcInParameterTypes == null)
                return true;
            if (inParameterTypes == null && funcInParameterTypes != null || inParameterTypes != null && funcInParameterTypes == null || inParameterTypes.Length != funcInParameterTypes.Length)
                return false;
            for (int index = 0; index < funcInParameterTypes.Length; ++index)
            {
                if (funcInParameterTypes[index].ParameterType != inParameterTypes[index])
                    return false;
            }
            return true;
        }

        private static void CheckTypeIsNotNull(Type type)
        {
            if (type == (Type)null)
                throw new ArgumentNullException("Type");
        }
    }
}
