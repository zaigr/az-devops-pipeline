using System;
using System.Runtime.Serialization;

namespace Production.Api.Models
{
    [DataContract]
    public class ProductCreateModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ProductNumber { get; set; }

        [DataMember]
        public bool? MakeFlag { get; set; }

        [DataMember]
        public bool? FinishedGoodsFlag { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public short SafetyStockLevel { get; set; }

        [DataMember]
        public short ReorderPoint { get; set; }

        [DataMember]
        public decimal StandardCost { get; set; }

        [DataMember]
        public decimal ListPrice { get; set; }

        [DataMember]
        public string Size { get; set; }

        [DataMember]
        public string SizeUnitMeasureCode { get; set; }

        [DataMember]
        public string WeightUnitMeasureCode { get; set; }

        [DataMember]
        public decimal? Weight { get; set; }

        [DataMember]
        public int DaysToManufacture { get; set; }

        [DataMember]
        public string ProductLine { get; set; }

        [DataMember]
        public string Class { get; set; }

        [DataMember]
        public string Style { get; set; }

        [DataMember]
        public int? ProductSubcategoryId { get; set; }

        [DataMember]
        public int? ProductModelId { get; set; }

        [DataMember]
        public DateTime SellStartDate { get; set; }

        [DataMember]
        public DateTime? SellEndDate { get; set; }

        [DataMember]
        public DateTime? DiscontinuedDate { get; set; }
    }
}
