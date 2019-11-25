using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlRoot(ElementName = "parts")]
    public class CarPartsDto
    {
        [XmlElement("partId")]
        public CarPartsIdDto[] PartsId { get; set; }
    }
}