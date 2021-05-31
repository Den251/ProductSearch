using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ProductSearch
{
    abstract class ProductDataBase
    {
        public IEnumerable<Product> data;
        

        public void Serialization(string fileToWriteIn)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileToWriteIn, FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(fs, data);
            }
            Console.WriteLine("Serialization");
        }
        public IEnumerable<Product> Deserialization(string fileToReadFrom)
        {
            IEnumerable<Product> productList = new List<Product>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileToReadFrom, FileMode.Open))
            {
                productList = (IEnumerable<Product>)binaryFormatter.Deserialize(fs);
            }
            Console.WriteLine(((productList != null) ? "Deserialization was successful" : "Product list is empty!") + '\n');
            return productList;
        }

       
    }
}
