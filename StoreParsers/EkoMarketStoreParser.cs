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

namespace ProductSearch.StoreParsers
{
    class EkoMarketStoreParser : StoreParser
    {
        public EkoMarketStoreParser(IWebDriver driver) : base(driver)
        {

        }
        public override List<string> GetProductsFromWeb(IWebDriver driver)
        {
            Console.WriteLine("Getting info from the EkoMarket");
            List<string> productList = new List<string>();

            try
            {
                
                driver.Url = "https://eko.zakaz.ua/uk/";
                Thread.Sleep(3000);

                Actions action = new Actions(driver);
                IWebElement element = driver.FindElement(By.XPath("//*[text()='Молочне і яйця']"));
                action.MoveToElement(element).Perform();
                Thread.Sleep(1000);

                driver.FindElement(By.XPath("//*[text()='Молоко']")).Click();
                Thread.Sleep(5000);

                
                IList<IWebElement> pageElements = driver.FindElements(By.XPath("//div[@class='jsx-33926795 ProductsBox__listItem']"));
                for (int i = 1; i <= 3; i++)
                {

                    if (i == 1)
                    {

                        productList.AddRange(pageElements.Select(x => x.Text));
                        Thread.Sleep(5000);
                        continue;

                    }

                    driver.FindElement(By.CssSelector("[href*='/uk/categories/milk-ekomarket/?page=" + i + "']")).Click();
                    Thread.Sleep(6000);
                    pageElements = driver.FindElements(By.XPath("//div[@class='jsx-33926795 ProductsBox__listItem']"));
                    productList.AddRange(pageElements.Select(x => x.Text));

                }

                

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            driver.Close();
            Console.WriteLine(((productList != null) ? "Success" : "List is empty!") + '\n');
            return productList;
            
            
        }

        public override ProductDataBase GetProductData()
        {
            ProductDataBase productDataBase = new EkoMarketProductDataBase();
            productDataBase.data = GetProductList(GetProductsFromWeb(driver));

            return productDataBase;
        }
    }
}
