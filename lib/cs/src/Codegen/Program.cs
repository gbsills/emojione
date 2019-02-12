using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Codegen {

    public class Program {

        // xml writer settings
        private XmlWriterSettings _settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = false, NamespaceHandling = NamespaceHandling.OmitDuplicates, OmitXmlDeclaration = true };

        /// <summary>
        /// Path to the emoji.json file.
        /// </summary>
        public string EmojiFile { get; set; } = "../../../../../../emoji.json";

        /// <summary>
        /// Path where generated source file will be created.
        /// </summary>
        public string SourceDir { get; set; } = "../../../EmojiOne";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) {
            var program = new Program();
            program.Execute();
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns></returns>
        public bool Execute() {
            try {
                // load and parse emoji.json
                var file = new FileInfo(EmojiFile);
                Console.WriteLine("Loading " + file.FullName);

                string json = File.ReadAllText(EmojiFile);
                var emojis = JsonConvert.DeserializeObject<Dictionary<string, Emoji>>(json);

                // remove ascii symbols and digits
                string chars = @"0123456789#*";
                foreach (char c in chars) {
                    var codepoint = ToCodePoint(c.ToString());
                    emojis.Remove(codepoint);
                }

                // write regex patterns and dictionaries to partial class
                Directory.CreateDirectory(SourceDir);
                file = new FileInfo(Path.Combine(SourceDir, "EmojiOne.generated.cs"));
                Console.WriteLine("Writing code to " + file.FullName);
                using (StreamWriter sw = new StreamWriter(Path.Combine(SourceDir, "EmojiOne.generated.cs"), false, Encoding.UTF8)) {
                    sw.WriteLine(@"using System.Collections.Generic;");
                    sw.WriteLine();
                    sw.WriteLine(@"namespace EmojiOne {");
                    sw.WriteLine();
                    sw.WriteLine(@"    public static partial class EmojiOne {");
                    sw.WriteLine();
                    sw.WriteLine(@"        private const int SHORTNAME_INDEX = 0;");
                    sw.WriteLine();
                    sw.WriteLine(@"        private const int CATEGORY_INDEX = 1;");
                    sw.WriteLine();
                    sw.WriteLine(@"        private const int ASCII_INDEX = 2;");
                    sw.WriteLine();
                    var asciis = emojis.Values.Where(x => x.Ascii.Any());
                    sw.Write(@"        private const string ASCII_PATTERN = @""(?<=\s|^)(");
                    for (int i = 0; i < asciis.Count(); i++) {
                        var emoji = asciis.ElementAt(i);
                        for (int j = 0; j < emoji.Ascii.Length; j++) {
                            sw.Write(Regex.Escape(emoji.Ascii[j]));
                            if (j < emoji.Ascii.Length - 1) {
                                sw.Write("|");
                            }
                        }
                        if (i < asciis.Count() - 1) {
                            sw.Write("|");
                        }
                    }
                    sw.WriteLine(@")(?=\s|$|[!,\.])"";");
                    sw.WriteLine();
                    sw.WriteLine(@"        private const string IGNORE_PATTERN = @""<object[^>]*>.*?</object>|<span[^>]*>.*?</span>|<i[^>]*>.*?</i>|<(?:object|embed|svg|img|div|span|p|a)[^>]*>"";");
                    sw.WriteLine();
                    sw.Write(@"        private const string SHORTNAME_PATTERN = @""(");
                    for (int i = 0; i < emojis.Count; i++) {
                        var emoji = emojis.ElementAt(i).Value;
                        if (i > 0) {
                            sw.Write("|");
                        }
                        sw.Write(Regex.Escape(emoji.Shortname));
                        for (int j = 0; j < emoji.ShortnameAlternates.Length; j++) {
                            sw.Write("|");
                            sw.Write(Regex.Escape(emoji.ShortnameAlternates[j]));
                        }
                    }
                    sw.WriteLine(@")"";");
                    sw.WriteLine();
                    sw.Write(@"        private const string UNICODE_PATTERN = @""(");
                    // NOTE: these must be ordered by length of the unicode code point
                    var codepoints = emojis.Values.SelectMany(e => e.CodePoints.BaseAndDefaultMatches).OrderByDescending(cp => cp.Length).ToList();
                    for (int i = 0; i < codepoints.Count; i++) {
                        var cp = codepoints.ElementAt(i);
                        sw.Write(ToSurrogateString(cp));
                        if (i < codepoints.Count - 1) {
                            sw.Write("|");
                        }
                    }
                    sw.WriteLine(@")"";");
                    sw.WriteLine();
                    sw.WriteLine(@"        private static readonly Dictionary<string, string> ASCII = new Dictionary<string, string> {");
                    for (int i = 0; i < asciis.Count(); i++) {
                        var emoji = asciis.ElementAt(i);
                        for (int j = 0; j < emoji.Ascii.Length; j++) {
                            sw.Write(@"            [""{0}""] = ""{1}""", emoji.Ascii[j].Replace("\\", "\\\\"), emoji.CodePoints.Base);
                            if (j < emoji.Ascii.Length - 1) {
                                sw.WriteLine(",");
                            }
                        }
                        if (i < asciis.Count() - 1) {
                            sw.WriteLine(",");
                        }
                    }
                    sw.WriteLine();
                    sw.WriteLine(@"        };");
                    sw.WriteLine();
                    sw.WriteLine(@"        private static readonly Dictionary<string, string> SHORTNAMES = new Dictionary<string, string> {");
                    for (int i = 0; i < emojis.Count; i++) {
                        var emoji = emojis.ElementAt(i).Value;
                        sw.Write(@"            [""{0}""] = ""{1}""", emoji.Shortname, emoji.CodePoints.Base);
                        for (int j = 0; j < emoji.ShortnameAlternates.Length; j++) {
                            sw.WriteLine(",");
                            sw.Write(@"            [""{0}""] = ""{1}""", emoji.ShortnameAlternates[j], emoji.CodePoints.Base);
                        }
                        if (i < emojis.Count - 1) {
                            sw.WriteLine(",");
                        }
                    }
                    sw.WriteLine();
                    sw.WriteLine(@"        };");
                    sw.WriteLine();
                    sw.WriteLine(@"        private static readonly Dictionary<string, string> ALTERNATES = new Dictionary<string, string> {");
                    var alternates = emojis.Values.Where(x => x.CodePoints.AlternateMatches.Any());
                    for (int i = 0; i < alternates.Count(); i++) {
                        var emoji = alternates.ElementAt(i);
                        for (int j = 0; j < emoji.CodePoints.AlternateMatches.Length; j++) {
                            sw.Write(@"            [""{0}""] = ""{1}""", emoji.CodePoints.AlternateMatches[j], emoji.CodePoints.Base);
                            if (j < emoji.CodePoints.AlternateMatches.Length - 1) {
                                sw.WriteLine(",");
                            }
                        }
                        if (i < alternates.Count() - 1) {
                            sw.WriteLine(",");
                        }
                    }
                    sw.WriteLine();
                    sw.WriteLine(@"        };");
                    sw.WriteLine();
                    sw.WriteLine(@"        private static readonly Dictionary<string, string[]> EMOJI = new Dictionary<string, string[]> {");

                    for (int i = 0; i < emojis.Count; i++) {
                        var emoji = emojis.ElementAt(i).Value;
                        sw.Write($@"            [""{emoji.CodePoints.Base}""] = new string[] {{""{emoji.Shortname}"", ""{emoji.Category}"", {(emoji.Ascii.Any() ? $@"""{emoji.Ascii.First().Replace("\\", "\\\\")}""" : "null")}}}");
                        if (i < emojis.Count - 1) {
                            sw.WriteLine(",");
                        }
                    }
                    sw.WriteLine();
                    sw.WriteLine(@"        };");
                    sw.WriteLine();

                    sw.WriteLine(@"    }");
                    sw.WriteLine(@"}");
                }
                Console.WriteLine("Done!");
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Convert a unicode character to its code point/code pair(s)
        /// </summary>
        /// <param name="unicode"></param>
        /// <returns></returns>
        private string ToCodePoint(string unicode) {
            string codepoint = "";
            for (var i = 0; i < unicode.Length; i += char.IsSurrogatePair(unicode, i) ? 2 : 1) {
                if (i > 0) {
                    codepoint += "-";
                }
                codepoint += string.Format("{0:X4}", char.ConvertToUtf32(unicode, i));
            }
            return codepoint.ToLower();
        }


        /// <summary>
        /// Converts a codepoint to unicode surrogate pairs
        /// </summary>
        /// <param name="unicode"></param>
        /// <returns></returns>
        private string ToSurrogateString(string codepoint) {
            var unicode = ToUnicode(codepoint);
            string s2 = "";
            for (int x = 0; x < unicode.Length; x++) {
                s2 += string.Format("\\u{0:X4}", (int)unicode[x]);
            }
            return s2;
        }

        /// <summary>
        /// Converts a unicode code point/code pair to a unicode character
        /// </summary>
        /// <param name="codepoints"></param>
        /// <returns></returns>
        private string ToUnicode(string codepoint) {
            if (codepoint.Contains('-')) {
                var pair = codepoint.Split('-');
                string[] hilos = new string[pair.Length];
                char[] chars = new char[pair.Length];
                for (int i = 0; i < pair.Length; i++) {
                    var part = Convert.ToInt32(pair[i], 16);
                    if (part >= 0x10000 && part <= 0x10FFFF) {
                        var hi = Math.Floor((decimal)(part - 0x10000) / 0x400) + 0xD800;
                        var lo = ((part - 0x10000) % 0x400) + 0xDC00;
                        hilos[i] = new String(new char[] { (char)hi, (char)lo });
                    } else {
                        chars[i] = (char)part;
                    }
                }
                if (hilos.Any(x => x != null)) {
                    return string.Concat(hilos);
                } else {
                    return new String(chars);
                }

            } else {
                var i = Convert.ToInt32(codepoint, 16);
                return char.ConvertFromUtf32(i);
            }
        }

        /// <summary>
        /// Removes all attributes except those specified in <paramref name="attributes"/>.
        /// </summary>
        /// <param name="svg"></param>
        /// <param name="attributes">
        /// Name of attributes to keep, or <c>null</c> to keep all attributes. A good default is "width", "height" and "viewBox"
        /// (according to https://stackoverflow.com/a/34249810/891843 it is safe to remove "xmlns", "xmlns:xlink" and "version" for inline svgs).
        /// </param>
        /// <returns></returns>
        private string RemoveAttributes(string svg, params string[] attributes) {

            var doc = new XmlDocument();
            doc.LoadXml(svg);

            doc.PreserveWhitespace = false;
            if (doc.DocumentType != null) {
                doc.RemoveChild(doc.DocumentType);
            }

            if (attributes != null && attributes.Length > 0) {
                // only keep specified attributes
                var remove = new List<XmlAttribute>();
                for (int i = 0; i < doc.DocumentElement.Attributes.Count; i++) {
                    var attr = doc.DocumentElement.Attributes[i];
                    if (!attributes.Any(x => attr.Name.Equals(x, System.StringComparison.OrdinalIgnoreCase))) {
                        remove.Add(attr);
                    }
                }
                foreach (var a in remove) {
                    doc.DocumentElement.Attributes.Remove(a);
                }
            }

            StringBuilder sb = new StringBuilder();
            using (XmlWriter xw = XmlWriter.Create(sb, _settings)) {
                doc.Save(xw);
            }

            svg = sb.ToString();
            if (!attributes.Contains("xmlns")) {
                // since we can't seem to remove the "root" namespace (xmlns="http://www.w3.org/2000/svg") from the XmlDocument we take care of it with a simple string replacement instead
                svg = svg.Replace(@" xmlns=""http://www.w3.org/2000/svg""", "");
            }
            return svg;
        }
    }
}


