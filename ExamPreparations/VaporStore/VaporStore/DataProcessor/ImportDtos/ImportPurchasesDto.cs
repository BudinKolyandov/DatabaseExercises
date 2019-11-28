namespace VaporStore.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using VaporStore.Data.Models;

    [XmlType("Purchase")]
    public class ImportPurchasesDto
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        public PurchaseType Type { get; set; }

        [RegularExpression(@"^[\dA-Z]{4}-[\dA-Z]{4}-[\dA-Z]{4}$")]
        public string Key { get; set; }
        public string Card { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
