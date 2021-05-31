using System;
using System.Collections.Generic;
using System.Text;

namespace ProductSearch
{
    [Serializable]
    abstract class Product
    {
        internal string Name { get; set; }
        internal double Price { get; set; }
        
        public Product(string Name, double Price)
        {
            this.Name = Name;
            this.Price = Price;
            
        }

    }
}
