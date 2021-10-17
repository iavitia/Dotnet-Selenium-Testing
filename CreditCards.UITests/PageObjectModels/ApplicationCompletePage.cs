using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditCards.UITests.PageObjectModels
{
    class ApplicationPageComplete : Page
    {
        public ApplicationPageComplete(IWebDriver driver)
        {
            Driver = driver;
        }
        protected override string PageUrl => "http://localhost:44108/Apply";
        protected override string PageTitle => "Application Complete - Credit Cards";

        public string Decision => Driver.FindElement(By.Id("Decision")).Text;
        public string ReferenceNumber => Driver.FindElement(By.Id("ReferenceNumber")).Text;
        public string FullName => Driver.FindElement(By.Id("FullName")).Text;
        public string Age => Driver.FindElement(By.Id("Age")).Text;
        public string Income => Driver.FindElement(By.Id("Income")).Text;
        public string RelationshipStatus => Driver.FindElement(By.Id("RelationshipStatus")).Text;
        public string ChooseBusinessSourceTV => Driver.FindElement(By.Id("BusinessSource")).Text;
    }
}
