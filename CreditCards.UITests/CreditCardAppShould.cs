﻿using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using Xunit.Abstractions;

namespace CreditCards.UITests
{
    [Trait("Category", "Applications")]
    public class CreditCardAppShould
    {
        private const string Homeurl = "http://localhost:44108";
        private const string ApplyUrl = "http://localhost:44108/Apply";

        // Using Xunit.Abstractions to create test output in tests
        private readonly ITestOutputHelper output;
        public CreditCardAppShould(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void BeInitiatedFromHomePage_NewLowRate()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(Homeurl);
                DemoHelper.Pause();

                IWebElement applyLink = driver.FindElement(By.Name("ApplyLowRate"));
                applyLink.Click();
                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }

        [Fact]
        public void BeInitiatedFromHomePage_EasyApplication()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(Homeurl);
                DemoHelper.Pause();

                IWebElement carouselNext = driver.FindElement(By.CssSelector("[data-slide='next']"));
                carouselNext.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
                IWebElement applyLink = wait.Until((d) => d.FindElement(By.LinkText("Easy: Apply Now!")));
                applyLink.Click();
                // IWebElement applyLink = driver.FindElement(By.LinkText("Easy: Apply Now!"));
                // applyLink.Click();
                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }
        [Fact]
        public void BeInitiatedFromHomePafe_EasyApplication_Prebuilt_Conditions()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(Homeurl);
                driver.Manage().Window.Minimize();
                DemoHelper.Pause();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(11));
                IWebElement applyLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Easy: Apply Now!")));
                applyLink.Click();

                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }
        [Fact]
        public void BeInitiatedFromHomePage_CustomerService()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                output.WriteLine($"{DateTime.Now.ToLongTimeString()} Navigating to {Homeurl}");
                driver.Navigate().GoToUrl(Homeurl);

                output.WriteLine($"{DateTime.Now.ToLongTimeString()} Finding element using explicit wait");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(35));

                

                IWebElement applyLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("customer-service-apply-now")));

                output.WriteLine($"{DateTime.Now.ToLongTimeString()} Found element Displayed={applyLink.Displayed} Enabled={applyLink.Enabled}");
                output.WriteLine($"{DateTime.Now.ToLongTimeString()} Clicking element");
                applyLink.Click();
                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }
        [Fact]
        public void BeSubmittedWhenValid()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(ApplyUrl);

                driver.FindElement(By.Id("FirstName")).SendKeys("Sam");
                driver.FindElement(By.Id("LastName")).SendKeys("Smith");
                driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys("123456-A");
                driver.FindElement(By.Id("Age")).SendKeys("35");
                driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys("55000");
                driver.FindElement(By.Id("Single")).Click();
                IWebElement businessSourceElement = driver.FindElement(By.Id("BusinessSource"));
                SelectElement businessSource = new SelectElement(businessSourceElement);
                
                // Check default selected option is correct
                Assert.Equal("I'd Rather Not Say", businessSource.SelectedOption.Text);
                
                // Get all the available options
                foreach(IWebElement option in businessSource.Options)
                {
                    output.WriteLine($"Value: {option.GetAttribute("value")} Text: {option.Text}");
                }
                // Make sure we have the correct number of options
                Assert.Equal(5, businessSource.Options.Count);

                // Ways to select an element
                businessSource.SelectByValue("Email");
                businessSource.SelectByText("Internet Search");
                businessSource.SelectByIndex(4);

                driver.FindElement(By.Id("TermsAccepted")).Click();

                driver.FindElement(By.Id("SubmitApplication")).Click();
                // driver.FindElement(By.Id("Single")).Submit();

                Assert.StartsWith("Application Complete", driver.Title);
                Assert.Equal("ReferredToHuman", driver.FindElement(By.Id("Decision")).Text);
                Assert.NotEmpty(driver.FindElement(By.Id("ReferenceNumber")).Text);
                Assert.Equal("Sam Smith", driver.FindElement(By.Id("FullName")).Text);
                Assert.Equal("35", driver.FindElement(By.Id("Age")).Text);
                Assert.Equal("55000", driver.FindElement(By.Id("Income")).Text);
                Assert.Equal("Single", driver.FindElement(By.Id("RelationshipStatus")).Text);
                Assert.Equal("TV", driver.FindElement(By.Id("BusinessSource")).Text);

                DemoHelper.Pause();
            }
        }
        [Fact]
        public void InvalidSubmission()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var invalidAge = 17;
                var validAge = 18;
                driver.Navigate().GoToUrl(ApplyUrl);

                driver.FindElement(By.Id("FirstName")).SendKeys("Sam");
                driver.FindElement(By.Id("LastName")).SendKeys("");
                driver.FindElement(By.Id("FrequentFlyerNumber")).SendKeys("123456-A");
                driver.FindElement(By.Id("Age")).SendKeys($"{invalidAge}");
                driver.FindElement(By.Id("GrossAnnualIncome")).SendKeys("55000");
                driver.FindElement(By.Id("Single")).Click();
                IWebElement businessSourceElement = driver.FindElement(By.Id("BusinessSource"));
                SelectElement businessSource = new SelectElement(businessSourceElement);
                businessSource.SelectByIndex(4);
                driver.FindElement(By.Id("TermsAccepted")).Click();
                driver.FindElement(By.Id("SubmitApplication")).Click();

                // validation failed and correct prompts
                var validationErrors = driver.FindElements(By.CssSelector(".validation-summary-errors > ul > li"));
                Assert.Equal(2, validationErrors.Count);
                Assert.Equal("Please provide a last name", validationErrors[0].Text);
                Assert.Equal("You must be at least 18 years old", validationErrors[1].Text);

                // Fixing errors and resubmitting
                driver.FindElement(By.Id("LastName")).SendKeys("Smith");
                driver.FindElement(By.Id("Age")).Clear();
                driver.FindElement(By.Id("Age")).SendKeys($"{validAge}");
                driver.FindElement(By.Id("SubmitApplication")).Click();

                Assert.StartsWith("Application Complete", driver.Title);
                Assert.Equal("ReferredToHuman", driver.FindElement(By.Id("Decision")).Text);
                Assert.NotEmpty(driver.FindElement(By.Id("ReferenceNumber")).Text);
                Assert.Equal("Sam Smith", driver.FindElement(By.Id("FullName")).Text);
                Assert.Equal($"{validAge}", driver.FindElement(By.Id("Age")).Text);
                Assert.Equal("55000", driver.FindElement(By.Id("Income")).Text);
                Assert.Equal("Single", driver.FindElement(By.Id("RelationshipStatus")).Text);
                Assert.Equal("TV", driver.FindElement(By.Id("BusinessSource")).Text);

                DemoHelper.Pause();
            }
        }

    }
}
