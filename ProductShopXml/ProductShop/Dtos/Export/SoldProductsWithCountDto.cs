using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("SoldProducts")]
    public class SoldProductsWithCountDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray(ElementName = "products")]
        public SoldProductsDto[] Products { get; set; }
    }
}