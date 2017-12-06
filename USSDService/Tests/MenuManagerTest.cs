using exactmobile.ussdservice.common.menu;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace TestProject1
{
    /// <summary>
    ///This is a test class for MenuManagerTest and is intended
    ///to contain all MenuManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MenuManagerTest
    {
        private TestContext testContextInstance;
        private int menuId = 1;
        private int ussdNumber= 33333;
        private object expectedCount =9;
        private object actualCount;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod()]
        public void LoadCampaignMenuTest()
        {
            int campaignID = 1;
            MenuManager menuManager = new MenuManager(campaignID, ussdNumber, menuId);
            Assert.IsTrue(menuManager.Menu.Count == 0);
            ShowMenu(menuManager.Menu, String.Empty);
        }

        [TestMethod()]
        public void BuildMenuTest()
        {
            int campaignID = 2;
            MenuManager menuManager = new MenuManager(campaignID, 37492, menuId);
            ShowMenu(menuManager.Menu, String.Empty);
            int menuID = 4;
            string output = menuManager.BuildMenu(menuID);
            Console.WriteLine(output);
            Assert.AreEqual(expectedCount, actualCount);
            ShowMenu(menuManager.Menu, String.Empty); 
        }

        protected void ShowMenu(List<UssdMenu> menuList, String spacer)
        {
            if (menuList.Count == 0) return;
            foreach (UssdMenu menu in menuList)
            {
                foreach (UssdMenuItem menuItem in menu.MenuItems)
                {
                    TestContext.WriteLine(spacer + menu.Name + "->" + menuItem.Name);
                }
                ShowMenu(menu.SubMenus, (spacer + " "));
            }
        }
    }
}
