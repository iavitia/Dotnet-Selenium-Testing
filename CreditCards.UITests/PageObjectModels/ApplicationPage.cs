using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditCards.UITests.PageObjectModels
{
    class ApplicationPage : Page
    {
        public ApplicationPage(IWebDriver driver)
        {
            Driver = driver;
        }

        public ReadOnlyCollection<string> ValidationErrorMessages
        {
            get
            {
                return Driver.FindElements(
                    By.CssSelector(".validation-summary-errors > ul > li"))
                    .Select(x => x.Text)
                    .ToList()
                    .AsReadOnly();
            }
        }

        protected override string PageUrl => "http://localhost:44108/Apply";
        protected override string PageTitle => "Credit Card Application - Credit Cards";
        public void ClearAge() => Driver.FindElement(By.Id("Age")).Clear();
        public void EnterFirstName(string firstName) => Driver.FindElement(By.Id("FirstName")).SendKeys(firstName);
        public void EnterLastName(string lastName) => Driver.FindElement(By.Id("LastName")).SendKeys(lastName);
        public void EnterFrequentFlyerNumber(string frequentFlyerNumber) => Driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys(frequentFlyerNumber);
        public void EnterAge(string age) => Driver.FindElement(By.Id("Age")).SendKeys(age);
        public void EnterAnnualIncome(string annualIncome) => Driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys(annualIncome);
        public void ChooseSingleStatus() => Driver.FindElement(By.Id("Single")).Click();
        public void ChooseBusinessSourceTV()
        {
            IWebElement businessSourceElement = Driver.FindElement(By.Id("BusinessSource"));
            SelectElement businessSource = new SelectElement(businessSourceElement);
            businessSource.SelectByValue("TV");
        }
        public void AcceptTerms() => Driver.FindElement(By.Id("TermsAccepted")).Click();
        public ApplicationPageComplete SubmitApplication()
        {
            Driver.FindElement(By.Id("SubmitApplication")).Click();
            return new ApplicationPageComplete(Driver);
        }
    }
}
