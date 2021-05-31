using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Threading;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.PageObjects;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using ProductSearch.StoreParsers;
using ProductSearch.Products;
using System.Data.SqlClient;
using System.Data;

namespace ProductSearch
{

    class Program
    {
        
        static void Main(string[] args)
        {
            IWebDriver driver = new FirefoxDriver();
            StoreParser storeParser = null;
            ProductDataBase productdata = null;

            //Getting data from Silpo
            storeParser = new SilpoStoreParser(driver);
            productdata = storeParser.GetProductData();
            productdata.Serialization("Silpo.dat");
            List<MilkProduct> milkList = (List<MilkProduct>)productdata.Deserialization("Silpo.dat");


            //Getting data from Auchan
            //storeParser = new AuchanStoreParser(driver);
            //productdata = storeParser.GetProductData();
            //productdata.Serialization("Auchan.dat");
            //List<MilkProduct> milkList = (List<MilkProduct>)productdata.Deserialization("Auchan.dat");

            //Getting data from Auchan
            //storeParser = new EkoMarketStoreParser(driver);
            //productdata = storeParser.GetProductData();
            //productdata.Serialization("EkoMarket.dat");
            //List<MilkProduct> milkList = (List<MilkProduct>)productdata.Deserialization("EkoMarket.dat");

            
            try
            {
                SqlConnection connection;
                string connetionString = @"Data Source=DESKTOP-LMP94US;Initial Catalog=milkDataBase;Integrated Security=True";
                connection = new SqlConnection(connetionString);
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                string sql = "INSERT INTO silpoMilk(milk_name, milk_weight, milk_price) VALUES(@milk_name, @milk_weight, @milk_price);";
                SqlCommand cmd = new SqlCommand(sql, connection, trans);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add("@milk_name", System.Data.SqlDbType.NVarChar);
                cmd.Parameters.Add("@milk_weight", System.Data.SqlDbType.Int);
                cmd.Parameters.Add("@milk_price", System.Data.SqlDbType.Float);

                foreach (var item in milkList)
                {
                    cmd.Parameters[0].Value = item.Name;
                    cmd.Parameters[1].Value = item.Weight;
                    cmd.Parameters[2].Value = item.Price;

                    cmd.ExecuteNonQuery();
                }

                
                trans.Commit();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                        
            Console.WriteLine("Done");
            Console.ReadKey();

            // Typical query to compare prices from different stores

            //SELECT * FROM(SELECT* FROM silpoMilk WHERE milk_name LIKE '%Яготинське%' AND milk_name LIKE '%2,6%') AS silpo
            //JOIN(SELECT * FROM auchanMilk WHERE milk_name LIKE '%Яготинське%' AND milk_name LIKE '%2,6%') AS auchan
            // ON auchan.milk_weight = silpo.milk_weight;

        }
    }
}
