using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDtos
{
    [XmlType("Purchase")]
    public class PurchaseDto
    {
        public string Card { get; set; }

        public string Cvc { get; set; }

        public string Date { get; set; }

        public PurchasedGameDto Game { get; set; }
    }
}