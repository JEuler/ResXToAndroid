using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ResXToAndroid {
	class Program {
		static void Main( string[] args )
		{
			// First parameter is the name of resx
			try
			{
				var fileName = args[0];
				string resxFile = fileName;
				XDocument xDoc = XDocument.Load(resxFile);

				var result = from item in xDoc.Descendants("data")
					select new
					{
						Name = item.Attribute("name").Value,
						Value = item.Element("value").Value
					};

				XmlWriterSettings settings = new XmlWriterSettings {Indent = true};
				XmlWriter writer = XmlWriter.Create("strings.xml", settings);
				writer.WriteStartElement("resources");
				foreach (var pair in result)
				{
					writer.WriteStartElement("string");
					writer.WriteAttributeString("name", pair.Name);
					writer.WriteValue(pair.Value);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
				writer.Close();
				Console.WriteLine( "All done, enjoy!" );
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Give me normal filename, please! :-)");
				Console.WriteLine( "Any key to exit:" );
				Console.ReadLine();
			}

			catch (IndexOutOfRangeException)
			{
				Console.WriteLine( "You give me no file! Please, give me file!" );
				Console.WriteLine("Any key to exit:");
				Console.ReadLine();
			}
		}
	}
}
