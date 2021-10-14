using CreditCards.UITests;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestSite
{
    public class LoginTest
    {
        private const string HomeUrl = "https://www.target.com";

        // [Fact]
        //public void TestLogin()
        //{
        //    using (IWebDriver driver = new ChromeDriver())
        //    {
        //        driver.Navigate().GoToUrl(HomeUrl);
        //        driver.FindElement(By.Id("controls-account-links")).Click();
        //        DemoHelper.Pause();
        //    }
        //}
        [Fact]
        public void ValidateLoginErros()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                // driver.FindElement(By.Id("cart")).Click();
                driver.FindElement(By.CssSelector("[data-menu='account']")).Click();
                //driver.FindElement(By.LinkText("Sign in")).Click();
                DemoHelper.Pause();
            }
        }
    }
    
}
