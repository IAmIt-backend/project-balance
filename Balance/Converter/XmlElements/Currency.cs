using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Converter.XmlElements
{
    public class Currency
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }
        [XmlElement("NumCode")]
        public int NumCode { get; set; }
        [XmlElement("CharCode")]
        public string CharCode { get; set; }
        [XmlElement("Scale")]
        public int Scale { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Rate")]
        public decimal Rate { get; set; }
    }
}
