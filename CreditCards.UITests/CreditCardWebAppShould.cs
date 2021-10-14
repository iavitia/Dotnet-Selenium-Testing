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
                driver.Navigate().GoToUrl(HomeUrl);

                // Managing controlling window size and position
                driver.Manage().Window.Maximize();
                DemoHelper.Pause();

                driver.Manage().Window.Minimize();
                DemoHelper.Pause();

                driver.Manage().Window.Size = new System.Drawing.Size(300, 400);
                DemoHelper.Pause();

                driver.Manage().Window.Position = new System.Drawing.Point(1, 1);
                DemoHelper.Pause();

                driver.Manage().Window.Position = new System.Drawing.Point(50, 50);
                DemoHelper.Pause();

                driver.Manage().Window.Position = new System.Drawing.Point(100, 100);
                DemoHelper.Pause();

                driver.Manage().Window.FullScreen();
                DemoHelper.Pause();

                Assert.Equal(HomeTitle, driver.Title);
                Assert.Equal(HomeUrl, driver.Url);
            }
        }

        [Fact]
        [Trait("Category", "Smoke")]
        public void ReloadHomePage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {

                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                driver.Navigate().Refresh();
                Assert.Equal(HomeTitle, driver.Title);
                Assert.Equal(HomeUrl, driver.Url);
            }
        }
        [Fact]
        [Trait("Category", "Smoke")]
        public void ReloadHomePageOnBack()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                IWebElement generationTokenElement =
                    driver.FindElement(By.Id("GenerationToken"));
                string initialToken = generationTokenElement.Text;
                driver.Navigate().GoToUrl(AboutUrl);
                driver.Navigate().Back();
                DemoHelper.Pause();

                Assert.Equal(HomeTitle, driver.Title);
                Assert.Equal(HomeUrl, driver.Url);

                string reloadedToken = driver.FindElement(By.Id("GenerationToken")).Text;
                Assert.NotEqual(initialToken, reloadedToken);
            }
        }
        [Fact]
        [Trait("Category", "Smoke")]
        public void ReloadHomePageOnForward()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(AboutUrl);
                DemoHelper.Pause();

                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                driver.Navigate().Back();
                DemoHelper.Pause();

                driver.Navigate().Forward();
                DemoHelper.Pause();

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
                driver.Navigate().GoToUrl(HomeUrl);
                var homePage = new HomePage(driver);

                DemoHelper.Pause();


                Assert.Equal("Easy Credit Card", homePage.Products[0].name);
                Assert.Equal("20% APR", homePage.Products[0].interestRate);

                Assert.Equal("Silver Credit Card", homePage.Products[1].name);
                Assert.Equal("18% APR", homePage.Products[1].interestRate);

                Assert.Equal("Gold Credit Card", homePage.Products[2].name);
                Assert.Equal("17% APR", homePage.Products[2].interestRate);

                DemoHelper.Pause();
            }
        }
        [Fact]
        public void BeInitieatedFromHomePage_RandomGreeting()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                IWebElement randomGreetingApplyLink = driver.FindElement(By.PartialLinkText("- Apply Now!"));
                randomGreetingApplyLink.Click();
                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }
        [Fact]
        public void BeInitieatedFromHomePage_RandomGreeting_Using_XPATH()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                // Relative xpath for partial text
                IWebElement randomGreetingApplyLink = driver.FindElement(By.XPath("//a[text()[contains(.,'- Apply Now!')]]"));
                randomGreetingApplyLink.Click();
                DemoHelper.Pause();

                Assert.Equal("Credit Card Application - Credit Cards", driver.Title);
                Assert.Equal(ApplyUrl, driver.Url);
            }
        }
        [Fact]
        public void OpenContactFooterLinkInNewTab()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);
                DemoHelper.Pause();

                driver.FindElement(By.Id("ContactFooter")).Click();

                // Contact opens in new tab. Switching tabs before making assertions
                ReadOnlyCollection<string> allTabs = driver.WindowHandles;
                string homePageTab = allTabs[0];
                string contactTab = allTabs[1];

                driver.SwitchTo().Window(contactTab);
                DemoHelper.Pause();

                Assert.Equal("Contact - Credit Cards", driver.Title);
                Assert.EndsWith("/Home/Contact", driver.Url);
            }
        }
        [Fact]
        public void AlertIfLiveChatIsClosed()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);

                driver.FindElement(By.Id("LiveChat")).Click();

                // There is a 2 second settimeout in JS code.
                // using explicit wait so we don't run test before alert is loaded
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());

                Assert.EndsWith("Live chat is currently closed.", alert.Text);
                DemoHelper.Pause();
                alert.Accept();
                DemoHelper.Pause();
            }
        }
        [Fact]
        public void NotNavigateToAboutUsWhenCancelClick()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(HomeUrl);

                driver.FindElement(By.Id("LearnAboutUs")).Click();

                // There is a 2 second settimeout in JS code.
                // using explicit wait so we don't run test before alert is loaded
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                IAlert alertBox = wait.Until(ExpectedConditions.AlertIsPresent());
                alertBox.Dismiss();

                Assert.Equal(HomeUrl, driver.Url);
            }
        }
        [Fact]
        public void NotDisplayCookieUseMessage()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                // using cookies
                driver.Navigate().GoToUrl(HomeUrl);
                driver.Manage().Cookies.AddCookie(new Cookie("acceptedCookies", "true"));

                driver.Navigate().Refresh();

                ReadOnlyCollection<IWebElement> message = driver.FindElements(By.Id("CookiesBeingUsed"));

                Assert.Empty(message);

                Cookie cookieValue = driver.Manage().Cookies.GetCookieNamed("acceptedCookies");
                Assert.Equal("true", cookieValue.Value);

                driver.Manage().Cookies.DeleteCookieNamed("acceptedCookies");
                driver.Navigate().Refresh();
                Assert.NotNull(driver.FindElement(By.Id("CookiesBeingUsed")));
            }
        }
        
    }
}
