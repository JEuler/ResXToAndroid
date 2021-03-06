﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ResConverter
{
    static class Program
    {
        private struct XmlKeyValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public static void Main(string[] args)
        {
            // First parameter is the name of resx
            try
            {
#if DEBUG
				var fileName = "strings.xml";
				var mode = "atoi";
#else

                var fileName = args[ 0 ];
                var mode = args[1];
#endif

                var xDoc = XDocument.Load(fileName);
                IEnumerable<XmlKeyValue> result = null;

                if (mode.StartsWith("w"))
                {
                    result = from item in xDoc.Descendants("data")
                        select new XmlKeyValue
                        {
                            Name = item.Attribute("name")?.Value,
                            Value = item.Element("value")?.Value
                        };
                }
                else if (mode.StartsWith("a"))
                {
                    result = from item in xDoc.Descendants("string")
                        select new XmlKeyValue
                        {
                            Name = item.Attribute("name")?.Value,
                            Value = item.Value
                        };
                }

                switch (mode)
                {
                    case "wtoa":
                        var settings = new XmlWriterSettings {Indent = true};
                        var writer = XmlWriter.Create("strings.xml", settings);
                        writer.WriteStartElement("resources");
                        if (result != null)
                            foreach (var pair in result)
                            {
                                writer.WriteStartElement("string");
                                writer.WriteAttributeString("name", pair.Name);
                                var str = pair.Value;
                                // Apostrophe handle done with str.replace
                                writer.WriteValue(str.Replace("'", "\\'"));
                                writer.WriteEndElement();
                            }
                        writer.WriteEndElement();
                        writer.Close();
                        Console.WriteLine("All done, enjoy!");
                        break;
                    case "atoi":
                    {
                        var contents = new List<string>();
                        if (result != null)
                            contents.AddRange(
                                from pair in result 
                                let str = pair.Value 
                                let replaced = str.Replace("\"", "\\\"") 
                                select $"\"{pair.Name}\"=\"{replaced}\";");

                        File.WriteAllLines(string.Format(fileName + "_ios"), contents);
                        Console.WriteLine("All done, enjoy!");
                        break;
                    }
                    case "wtoi":
                    {
                        var contents = new List<string>();
                        if (result != null)
                            contents.AddRange(result.Select(pair => $"\"{pair.Name}\"=\"{pair.Value}\";"));

                        File.WriteAllLines(string.Format(fileName + "_ios"), contents);
                        Console.WriteLine("All done, enjoy!");
                        break;
                    }
                }
                // handle for other cases
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Give me normal filename, please! :-)");
                Console.WriteLine("Any key to exit:");
                Console.ReadLine();
            }

            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("You give me no file! Please, give me file! Or give the mode: atoi, wtoi, wtoa");
                Console.WriteLine("Any key to exit:");
                Console.ReadLine();
            }
        }
    }
}