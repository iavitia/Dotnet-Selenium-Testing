using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.UI;
using System;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.Windows;
using System.IO;
using ApprovalTests;
using CreditCards.UITests.PageObjectModels;

namespace CreditCards.UITests
{
    public class CreditCardWebAppShould
    {
        private const string HomeUrl = "http://localhost:44108/";
        private const string AboutUrl = "http://localhost:44108/Home/About";
        private const string ApplyUrl = "http://localhost:44108/Apply";
        private const string HomeTitle = "Home Page - Credit Cards";

        [Fact]
        [Trait("Category", "Smoke")]
        public void LoadHomePage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                // Managing controlling window size and position
                driver.Manage().Window.Maximize();

                driver.Manage().Window.Minimize();

                driver.Manage().Window.Size = new System.Drawing.Size(300, 400);

                driver.Manage().Window.Position = new System.Drawing.Point(1, 1);

                driver.Manage().Window.Position = new System.Drawing.Point(50, 50);

                driver.Manage().Window.Position = new System.Drawing.Point(100, 100);

                driver.Manage().Window.FullScreen();
            }
        }

        [Fact]
        [Trait("Category", "Smoke")]
        public void ReloadHomePage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                driver.Navigate().Refresh();
                homePage.EnsurePageLoaded();
            }
        }
        [Fact]
        [Trait("Category", "Smoke")]
        public void ReloadHomePageOnBack()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                
                string initialToken = homePage.GenerationToken;

                driver.Navigate().GoToUrl(AboutUrl);
                driver.Navigate().Back();

                homePage.EnsurePageLoaded();

                string reloadedToken = homePage.GenerationToken;
                Assert.NotEqual(initialToken, reloadedToken);
            }
        }
        [Fact]
        [Trait("Category", "Smoke")]
        public void ReloadHomePageOnForward()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                driver.Navigate().Back();
                driver.Navigate().Forward();

                Assert.Equal(HomeTitle, driver.Title);
                Assert.Equal(HomeUrl, driver.Url);
            }
        }
        [Fact]
        [Trait("Category", "Smoke")]
        public void DisplayProductsAndRates()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                Assert.Equal("Easy Credit Card", homePage.Products[0].name);
                Assert.Equal("20% APR", homePage.Products[0].interestRate);

                Assert.Equal("Silver Credit Card", homePage.Products[1].name);
                Assert.Equal("18% APR", homePage.Products[1].interestRate);

                Assert.Equal("Gold Credit Card", homePage.Products[2].name);
                Assert.Equal("17% APR", homePage.Products[2].interestRate);
            }
        }
        [Fact]
        public void BeInitieatedFromHomePage_RandomGreeting()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                IWebElement randomGreetingApplyLink = driver.FindElement(By.PartialLinkText("- Apply Now!"));
                randomGreetingApplyLink.Click();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }
        [Fact]
        public void BeInitieatedFromHomePage_RandomGreeting_Using_XPATH()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                // Relative xpath for partial text
                IWebElement randomGreetingApplyLink = driver.FindElement(By.XPath("//a[text()[contains(.,'- Apply Now!')]]"));
                randomGreetingApplyLink.Click();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }
        [Fact]
        public void OpenContactFooterLinkInNewTab()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickContactFooterLink();

                // Contact opens in new tab. Switching tabs before making assertions
                ReadOnlyCollection<string> allTabs = driver.WindowHandles;
                string homePageTab = allTabs[0];
                string contactTab = allTabs[1];

                driver.SwitchTo().Window(contactTab);

                Assert.Equal("Contact - Credit Cards", driver.Title);
                Assert.EndsWith("/Home/Contact", driver.Url);
            }
        }
        [Fact]
        public void AlertIfLiveChatIsClosed()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickLiveChatFooterLink();

                // There is a 2 second settimeout in JS code.
                // using explicit wait so we don't run test before alert is loaded
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());

                Assert.EndsWith("Live chat is currently closed.", alert.Text);
                alert.Accept();
            }
        }
        [Fact]
        public void NavigateToAboutUsWhenAcceptClicked()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickAboutUs();

                // There is a 2 second settimeout in JS code.
                // using explicit wait so we don't run test before alert is loaded
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                IAlert alertBox = wait.Until(ExpectedConditions.AlertIsPresent());
                alertBox.Accept();

                Assert.EndsWith("/Home/About", driver.Url);
            }
        }
        [Fact]
        public void NotNavigateToAboutUsWhenCancelClick()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                homePage.ClickAboutUs();

                // There is a 2 second settimeout in JS code.
                // using explicit wait so we don't run test before alert is loaded
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                IAlert alertBox = wait.Until(ExpectedConditions.AlertIsPresent());
                alertBox.Dismiss();

                homePage.EnsurePageLoaded();
            }
        }
        [Fact]
        public void NotDisplayCookieUseMessage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                // using cookies
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                driver.Manage().Cookies.AddCookie(new Cookie("acceptedCookies", "true"));
                driver.Navigate().Refresh();

                Assert.False(homePage.IsCookieMessagePresent);

                driver.Manage().Cookies.DeleteCookieNamed("acceptedCookies");
                driver.Navigate().Refresh();

                Assert.True(homePage.IsCookieMessagePresent);
            }
        }
        
    }
}
