﻿using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlType("Car")]
    public class ImportCarsDto
    {
        [XmlElement("make")]
        public string Make { get; set; }
        
        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TravelledDistance { get; set; }

        [XmlElement("parts")]
        public CarPartsDto Parts { get; set; }
    }
}
