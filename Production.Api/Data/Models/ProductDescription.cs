﻿using System;
using System.Collections.Generic;

namespace Production.Api.Models
{
    public partial class ProductDescription
    {
        public ProductDescription()
        {
            ProductModelProductDescriptionCulture = new HashSet<ProductModelProductDescriptionCulture>();
        }

        public int ProductDescriptionId { get; set; }
        public string Description { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCulture { get; set; }
    }
}
