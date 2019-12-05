using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Song")]
    public class ImportSongsToPerformetDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}