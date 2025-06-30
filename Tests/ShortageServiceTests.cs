using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VismaShortageManagement.Models;
using VismaShortageManagement.Services;

namespace VismaShortageManagement.Tests
{
    [TestClass]
    public class ShortageServiceTests
    {
        private IShortageService _shortageService;
        private TestDataService _testDataService;

        [TestInitialize]
        public void Setup()
        {
            _testDataService = new TestDataService();
            _shortageService = new ShortageService(_testDataService);
        }

        [TestMethod]
        public void RegisterShortage_NewShortage_ShouldReturnTrue()
        {
            var shortage = new Shortage("Laptop", "John", Room.MeetingRoom, Category.Electronics, 5);
            
            var result = _shortageService.RegisterShortage(shortage);
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RegisterShortage_DuplicateWithLowerPriority_ShouldReturnFalse()
        {
            var shortage1 = new Shortage("Laptop", "John", Room.MeetingRoom, Category.Electronics, 8);
            var shortage2 = new Shortage("Laptop", "Jane", Room.MeetingRoom, Category.Electronics, 5);
            
            _shortageService.RegisterShortage(shortage1);
            var result = _shortageService.RegisterShortage(shortage2);
            
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RegisterShortage_DuplicateWithHigherPriority_ShouldReturnTrue()
        {
            var shortage1 = new Shortage("Laptop", "John", Room.MeetingRoom, Category.Electronics, 5);
            var shortage2 = new Shortage("Laptop", "Jane", Room.MeetingRoom, Category.Electronics, 8);
            
            _shortageService.RegisterShortage(shortage1);
            var result = _shortageService.RegisterShortage(shortage2);
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DeleteShortage_UserCanDeleteOwnShortage_ShouldReturnTrue()
        {
            var shortage = new Shortage("Laptop", "John", Room.MeetingRoom, Category.Electronics, 5);
            _shortageService.RegisterShortage(shortage);
            
            var result = _shortageService.DeleteShortage("Laptop", Room.MeetingRoom, "John", false);
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DeleteShortage_UserCannotDeleteOthersShortage_ShouldReturnFalse()
        {
            var shortage = new Shortage("Laptop", "John", Room.MeetingRoom, Category.Electronics, 5);
            _shortageService.RegisterShortage(shortage);
            
            var result = _shortageService.DeleteShortage("Laptop", Room.MeetingRoom, "Jane", false);
            
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DeleteShortage_AdminCanDeleteAnyShortage_ShouldReturnTrue()
        {
            var shortage = new Shortage("Laptop", "John", Room.MeetingRoom, Category.Electronics, 5);
            _shortageService.RegisterShortage(shortage);
            
            var result = _shortageService.DeleteShortage("Laptop", Room.MeetingRoom, "Admin", true);
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetShortages_UserSeesOnlyOwnShortages()
        {
            var shortage1 = new Shortage("Laptop", "John", Room.MeetingRoom, Category.Electronics, 5);
            var shortage2 = new Shortage("Coffee", "Jane", Room.Kitchen, Category.Food, 3);
            
            _shortageService.RegisterShortage(shortage1);
            _shortageService.RegisterShortage(shortage2);
            
            var result = _shortageService.GetShortages("John", false);
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Laptop", result[0].Title);
        }

        [TestMethod]
        public void GetShortages_AdminSeesAllShortages()
        {
            var shortage1 = new Shortage("Laptop", "John", Room.MeetingRoom, Category.Electronics, 5);
            var shortage2 = new Shortage("Coffee", "Jane", Room.Kitchen, Category.Food, 3);
            
            _shortageService.RegisterShortage(shortage1);
            _shortageService.RegisterShortage(shortage2);
            
            var result = _shortageService.GetShortages("Admin", true);
            
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetShortages_FilterByTitle_ShouldReturnMatchingResults()
        {
            var shortage1 = new Shortage("Wireless Speaker", "John", Room.MeetingRoom, Category.Electronics, 5);
            var shortage2 = new Shortage("Coffee", "John", Room.Kitchen, Category.Food, 3);
            
            _shortageService.RegisterShortage(shortage1);
            _shortageService.RegisterShortage(shortage2);
            
            var result = _shortageService.GetShortages("John", false, "Speaker");
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Wireless Speaker", result[0].Title);
        }

        [TestMethod]
        public void GetShortages_ShouldReturnSortedByPriority()
        {
            var shortage1 = new Shortage("Low Priority", "John", Room.MeetingRoom, Category.Electronics, 3);
            var shortage2 = new Shortage("High Priority", "John", Room.Kitchen, Category.Food, 9);
            var shortage3 = new Shortage("Medium Priority", "John", Room.Bathroom, Category.Other, 6);
            
            _shortageService.RegisterShortage(shortage1);
            _shortageService.RegisterShortage(shortage2);
            _shortageService.RegisterShortage(shortage3);
            
            var result = _shortageService.GetShortages("John", false);
            
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("High Priority", result[0].Title);
            Assert.AreEqual("Medium Priority", result[1].Title);
            Assert.AreEqual("Low Priority", result[2].Title);
        }
    }

    // Test helper class
    public class TestDataService : IDataService
    {
        private List<Shortage> _shortages = new List<Shortage>();

        public List<Shortage> LoadShortages()
        {
            return new List<Shortage>(_shortages);
        }

        public void SaveShortages(List<Shortage> shortages)
        {
            _shortages = new List<Shortage>(shortages);
        }
    }
}
