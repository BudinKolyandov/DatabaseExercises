using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{

    public class UsersWithProductsDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray(ElementName = "users")]
        public UsersWithProductsSoldDto[] Users { get; set; }
    }
}
