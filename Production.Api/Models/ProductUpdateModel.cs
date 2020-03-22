using System.Runtime.Serialization;

namespace Production.Api.Models
{
    [DataContract]
    public class ProductUpdateModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool? MakeFlag { get; set; }

        [DataMember]
        public bool? FinishedGoodsFlag { get; set; }

        [DataMember]
        public string Color { get; set; }
    }
}
