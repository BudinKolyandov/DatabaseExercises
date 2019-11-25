using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlType("car")]
    public class CarsWithTheirListOfPartsDto
    {
        [XmlAttribute("make")]
        public string Make { get; set; }
        
        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlArray(ElementName ="parts")]
        public ListOfPartsDto[] Parts { get; set; }

    }
}
