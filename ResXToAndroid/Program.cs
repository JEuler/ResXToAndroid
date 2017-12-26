using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ResXToAndroid
{
	class Program {
		struct XmlKeyValue
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}

		static void Main( string[] args ) {
			// First parameter is the name of resx
			try {
#if DEBUG
				var fileName = "strings.xml";
				var mode = "atoi";
#else

				var fileName = args[ 0 ];
				var mode = args[1];
#endif

				XDocument xDoc = XDocument.Load( fileName );
				IEnumerable<XmlKeyValue> result = null;

				if (mode.StartsWith("w"))
				{

					result = from item in xDoc.Descendants("data")
						select new XmlKeyValue()
						{
							Name = item.Attribute("name")?.Value,
							Value = item.Element("value")?.Value
						};
				} else if (mode.StartsWith("a"))
				{
					result = from item in xDoc.Descendants("string")
						select new XmlKeyValue()
						{
							Name = item.Attribute("name")?.Value,
							Value = item.Value
						};
				}
				// Future for ios convertion
				else
				{
					
				}

				if ( mode == "wtoa" ) {
					XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
					// ToDo change name stuff
					XmlWriter writer = XmlWriter.Create( "strings.xml", settings );
					writer.WriteStartElement( "resources" );
					foreach ( var pair in result ) {
						writer.WriteStartElement( "string" );
						writer.WriteAttributeString( "name", pair.Name );
						var str = pair.Value;
						// Apostrophe handle done with str.replace
						writer.WriteValue( str.Replace( "'", "\\'" ) );
						writer.WriteEndElement();
					}
					writer.WriteEndElement();
					writer.Close();
					Console.WriteLine( "All done, enjoy!" );
				}
				else if ( mode == "atoi" ) {
					List<string> strs = new List<string>();
					foreach ( var pair in result )
					{
						var str = pair.Value;
						var replaced = str.Replace("\"", "\\\"");
						strs.Add( string.Format( "\"{0}\"=\"{1}\";", pair.Name, replaced ) );
					}

					File.WriteAllLines( string.Format( fileName + "_ios" ), strs );
					Console.WriteLine( "All done, enjoy!" );
				} else if (mode == "wtoi")
				{
					List<string> strs = new List<string>();
					foreach ( var pair in result ) {
						strs.Add( string.Format( "\"{0}\"=\"{1}\";", pair.Name, pair.Value ) );
					}

					File.WriteAllLines( string.Format(fileName + "_ios"), strs );
					Console.WriteLine( "All done, enjoy!" );
				}
				// handle for other cases
			}
			catch ( FileNotFoundException ) {
				Console.WriteLine( "Give me normal filename, please! :-)" );
				Console.WriteLine( "Any key to exit:" );
				Console.ReadLine();
			}

			catch ( IndexOutOfRangeException ) {
				Console.WriteLine( "You give me no file! Please, give me file! Or give me mode: atoi, wtoi, wtoa" );
				Console.WriteLine( "Any key to exit:" );
				Console.ReadLine();
			}
		}
	}
}
