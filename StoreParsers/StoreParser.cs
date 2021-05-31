using OpenQA.Selenium;
using ProductSearch.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ProductSearch.StoreParsers
{
    abstract class StoreParser
    {
        public IWebDriver driver;
        public static void Scroll(IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight - 150)");
        }
        public abstract List<string> GetProductsFromWeb(IWebDriver driver);
        

        //main method to get rid of garbage for Zakaz.ua
        public virtual IEnumerable<Product> GetProductList(List<string> listOfProductsWithGarbage)
        {
            Console.WriteLine("Getting product list");
            List<MilkProduct> productList = new List<MilkProduct>();
            for (int i = 0; i < listOfProductsWithGarbage.Count; i++)
            {
                try
                {
                    // clearing from garbage chars
                    if (listOfProductsWithGarbage[i].StartsWith("-"))
                        listOfProductsWithGarbage[i] = listOfProductsWithGarbage[i].Substring(5);

                    //removing garbage from products with discount
                    string[] splitedInfo = listOfProductsWithGarbage[i].Split("\r\n");
                    if (splitedInfo.Length > 3)
                    {
                        splitedInfo = splitedInfo.Skip(2).ToArray();
                        splitedInfo[0] = splitedInfo[0].Substring(0, splitedInfo[0].IndexOf(" "));
                    }

                    //preparing name value
                    splitedInfo[0] = splitedInfo[0].TrimEnd(" грн".ToCharArray());
                    splitedInfo[1] = splitedInfo[1].TrimEnd(splitedInfo[2].ToCharArray());

                    splitedInfo[2] = splitedInfo[2].Replace(".", ",");
                    splitedInfo[0] = splitedInfo[0].Replace(".", ",");

                    ////preparing weight value
                    string measure = (splitedInfo[2].Split(" "))[1];
                    switch (measure)
                    {
                        case "мл":
                            splitedInfo[2] = splitedInfo[2].TrimEnd(measure.ToCharArray());
                            break;

                        case "л":
                            splitedInfo[2] = splitedInfo[2].TrimEnd(measure.ToCharArray());
                            splitedInfo[2] = Convert.ToString(Convert.ToDouble(splitedInfo[2]) * 1000);
                            break;

                        case "г":
                            splitedInfo[2] = splitedInfo[2].TrimEnd(measure.ToCharArray());
                            break;

                        case "кг":
                            splitedInfo[2] = splitedInfo[2].TrimEnd(measure.ToCharArray());
                            splitedInfo[2] = Convert.ToString(Convert.ToDouble(splitedInfo[2]) * 1000);
                            break;

                    }
                    splitedInfo[2] = splitedInfo[2].Replace(" ", "");


                    
                    // adding product to product list
                    productList.Add(new MilkProduct(splitedInfo[1], Convert.ToDouble(splitedInfo[0]), Convert.ToInt32(splitedInfo[2])));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }


            }
            //IEnumerable<Product> returnedList = productList;

            Console.WriteLine(((productList != null)?"Success":"Product list is empty!")+'\n');
            
            return productList;
        }
        
        public StoreParser(IWebDriver driver)
        {
            this.driver = driver;
        }

        public abstract ProductDataBase GetProductData();
        
        
       

         
        
    }       

        

}
