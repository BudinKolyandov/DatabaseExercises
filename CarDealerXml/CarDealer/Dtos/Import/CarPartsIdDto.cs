using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlRoot(ElementName = "parts")]
    public class CarPartsIdDto
    {
        [XmlAttribute(AttributeName ="id")]
        public int PartId { get; set; }
    }
}