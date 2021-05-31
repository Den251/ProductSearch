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
using System.Text.RegularExpressions;

namespace ProductSearch.StoreParsers
{
    class SilpoStoreParser : StoreParser
    {
        public SilpoStoreParser(IWebDriver driver) : base(driver)
        {

        }
        public override List<string> GetProductsFromWeb(IWebDriver driver)
        {
            Console.WriteLine("Getting info from the Silpo");
            List<string> SilpoList = new List<string>();

            try
            {
                

                driver.Url = "https://shop.silpo.ua/collect";
                Thread.Sleep(3000);

                driver.FindElement(By.XPath("//div[@class='button-switch-item']")).Click();
                Thread.Sleep(10000);

                driver.FindElement(By.XPath("//input[@class='store-select__store ']")).SendKeys("вул. Берковецька, 6д");
                Thread.Sleep(5000);

                driver.FindElement(By.XPath("//button[@class='btn btn-primary confirm-btn']")).Click();
                Thread.Sleep(5000);

                driver.FindElement(By.XPath("//div[@class='all-product_btn']")).Click();
                Thread.Sleep(3000);

                Actions action = new Actions(driver);
                IWebElement element = driver.FindElement(By.XPath("//*[text()='Молочні продукти та яйця']"));
                action.MoveToElement(element).Perform();
                Thread.Sleep(1000);

                driver.FindElement(By.XPath("//*[text()='Молоко, вершки']")).Click();
                Thread.Sleep(5000);

                Scroll(driver);
                Thread.Sleep(10000);

                driver.FindElement(By.XPath("//*[text()='Показати ще']")).Click();
                Thread.Sleep(10000);

                Scroll(driver);
                Thread.Sleep(10000);

                Scroll(driver);
                Thread.Sleep(10000);


                IList<IWebElement> pageElements = driver.FindElements(By.XPath("//div[@class='product-list-item']"));
                SilpoList.AddRange(pageElements.Select(x => x.Text));

                
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            driver.Close();
            Console.WriteLine(((SilpoList != null) ? "Success" : "List is empty!") + '\n');
            return SilpoList;

        }
        public override IEnumerable<Product> GetProductList(List<string> listOfProductsWithGarbage)
        {
            Console.WriteLine("Getting product list");
            List<MilkProduct> productList = new List<MilkProduct>();
            
            for (int i = 0; i < listOfProductsWithGarbage.Count; i++)
                {
                    try
                    {
                        // clearing from garbage chars
                        if (listOfProductsWithGarbage[i].StartsWith(" \r"))
                            listOfProductsWithGarbage[i] = listOfProductsWithGarbage[i].Substring(5);


                        else if (listOfProductsWithGarbage[i].StartsWith(" "))
                            listOfProductsWithGarbage[i] = listOfProductsWithGarbage[i].Substring(1);

                        //prepearing weight value
                        var splitedInfo = listOfProductsWithGarbage[i].Split("\r\n");


                        splitedInfo[1] = splitedInfo[1].Replace(".", ",");
                        string measure = Regex.Replace(splitedInfo[1], @"[^а-я]+", String.Empty);
                        switch (measure)
                        {
                            case "мл":
                                splitedInfo[1] = splitedInfo[1].TrimEnd(measure.ToCharArray());
                                break;

                            case "л":
                                splitedInfo[1] = splitedInfo[1].TrimEnd(measure.ToCharArray());
                                splitedInfo[1] = Convert.ToString(Convert.ToDouble(splitedInfo[1]) * 1000);
                                break;

                            case "г":
                                splitedInfo[1] = splitedInfo[1].TrimEnd(measure.ToCharArray());
                                break;

                            case "кг":
                                splitedInfo[1] = splitedInfo[1].TrimEnd(measure.ToCharArray());
                                splitedInfo[1] = Convert.ToString(Convert.ToDouble(splitedInfo[1]) * 1000);
                                break;

                        }

                        // adding product to product list

                        productList.Add(new MilkProduct(splitedInfo[0], Convert.ToDouble(splitedInfo[2] + "," + splitedInfo[3]), Convert.ToInt32(splitedInfo[1])));

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

            Console.WriteLine(((productList != null) ? "Success" : "Product list is empty!") + '\n');
            
            return productList;
        }

        public override ProductDataBase GetProductData()
        {
            ProductDataBase productDataBase = new SilpoProductDataBase();
            productDataBase.data = GetProductList(GetProductsFromWeb(driver));

            return productDataBase;
        }

         
    }
}