using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Converter.XmlElements
{
    [XmlRoot("DailyExRates")]
    public class DailyExRates
    {
        [XmlAttribute("Date")]
        public string Date { get; set; }
        [XmlElement("Currency")]
        public List<Currency> Currencies { get; set; }
    }
}
