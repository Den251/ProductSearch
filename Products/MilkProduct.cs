using System;
using System.Collections.Generic;
using System.Text;

namespace ProductSearch.Products
{
    [Serializable]
    class MilkProduct : Product
    {
        
        internal int Weight { get; set; }
        
        public MilkProduct(string Name, double Price, int Weight):base(Name, Price)
        {
            this.Weight = Weight;
        }
    }
}
