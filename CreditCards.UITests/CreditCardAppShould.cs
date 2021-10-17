using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using Xunit.Abstractions;
using CreditCards.UITests.PageObjectModels;

namespace CreditCards.UITests
{
    [Trait("Category", "Applications")]
    public class CreditCardAppShould : IClassFixture<ChromeDriverFixture>
    {
        private const string Homeurl = "http://localhost:44108";
        private const string ApplyUrl = "http://localhost:44108/Apply";
        private readonly ChromeDriverFixture ChromeDriverFixture;

        public CreditCardAppShould(ChromeDriverFixture chromeDriverFixture)
        {
            ChromeDriverFixture = chromeDriverFixture;
            ChromeDriverFixture.Driver.Manage().Cookies.DeleteAllCookies();
            ChromeDriverFixture.Driver.Navigate().GoToUrl("about:blank");
        }

        [Fact]
        public void BeInitiatedFromHomePage_NewLowRate()
        {
                var homePage = new HomePage(ChromeDriverFixture.Driver);
                homePage.NavigateTo();

                ApplicationPage applicationPage = homePage.ClickApplyLowRateLink();

                applicationPage.EnsurePageLoaded();
        }
        [Fact]
        public void BeInitiatedFromHomePafe_EasyApplication()
        {
                var homePage = new HomePage(ChromeDriverFixture.Driver);
                homePage.NavigateTo();

                homePage.WaitForEasyApplicationCarouselPage();

                ApplicationPage applicationPage = homePage.ClickApplyEasyApplicationLink();

                applicationPage.EnsurePageLoaded();
        }
        [Fact]
        public void BeInitiatedFromHomePage_CustomerService()
        {
                var homePage = new HomePage(ChromeDriverFixture.Driver);
                homePage.NavigateTo();

                WebDriverWait wait = new WebDriverWait(ChromeDriverFixture.Driver, TimeSpan.FromSeconds(35));

                

                IWebElement applyLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("customer-service-apply-now")));

                applyLink.Click();

                Assert.Equal("Credit Card Application - Credit Cards", ChromeDriverFixture.Driver.Title);
                Assert.Equal(ApplyUrl, ChromeDriverFixture.Driver.Url);
            
        }
        [Fact]
        public void BeSubmittedWhenValid()
        {
            const string FirstName = "Sam";
            const string LastName = "Smith";
            const string number = "1234-ds";
            const string Age = "35";
            const string Income = "50000";

            
                var applicationPage = new ApplicationPage(ChromeDriverFixture.Driver);
                applicationPage.NavigateTo();

                applicationPage.EnterFirstName(FirstName);
                applicationPage.EnterLastName(LastName);
                applicationPage.EnterFrequentFlyerNumber(number);
                applicationPage.EnterAge(Age);
                applicationPage.EnterAnnualIncome(Income);
                applicationPage.ChooseSingleStatus();
                applicationPage.ChooseBusinessSourceTV();
                applicationPage.AcceptTerms();
                ApplicationPageComplete applicationPageComplete = applicationPage.SubmitApplication();

                applicationPageComplete.EnsurePageLoaded();

                Assert.Equal("ReferredToHuman", applicationPageComplete.Decision);
                Assert.NotEmpty(applicationPageComplete.ReferenceNumber);
                Assert.Equal($"{FirstName} {LastName}", applicationPageComplete.FullName);
                Assert.Equal(Age, applicationPageComplete.Age);
                Assert.Equal(Income, applicationPageComplete.Income);
                Assert.Equal("Single", applicationPageComplete.RelationshipStatus);
                Assert.Equal("TV", applicationPageComplete.ChooseBusinessSourceTV);
            
        }
        [Fact]
        public void InvalidSubmission()
        {
            const string FirstName = "Sam";
            var invalidAge = "17";
            var validAge = "18";
            
                var applicationPage = new ApplicationPage(ChromeDriverFixture.Driver);
                applicationPage.NavigateTo();

                applicationPage.EnterFirstName(FirstName);
                applicationPage.EnterFrequentFlyerNumber("123456-A");
                applicationPage.EnterAge(invalidAge);
                applicationPage.EnterAnnualIncome("55000");
                applicationPage.ChooseSingleStatus();
                applicationPage.ChooseBusinessSourceTV();
                applicationPage.AcceptTerms();
                applicationPage.SubmitApplication();

                // validation failed and correct prompts
                Assert.Equal(2, applicationPage.ValidationErrorMessages.Count);
                Assert.Contains("Please provide a last name", applicationPage.ValidationErrorMessages);
                Assert.Contains("You must be at least 18 years old", applicationPage.ValidationErrorMessages);

                // Fixing errors and resubmitting
                applicationPage.EnterLastName("Smith");
                applicationPage.ClearAge();
                applicationPage.EnterAge(validAge);

                ApplicationPageComplete applicationPageComplete = applicationPage.SubmitApplication();

                applicationPageComplete.EnsurePageLoaded();

        }

    }
}
