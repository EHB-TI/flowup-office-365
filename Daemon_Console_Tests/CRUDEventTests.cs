using Microsoft.VisualStudio.TestTools.UnitTesting;
using UUIDproducer;
using daemon_console;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daemon_Console_Tests
{
    [TestClass]
    class CRUDEventTests
    {
        [TestMethod]
        public void Create_Event_Returns_XML_Validation()
        {
            // Arrange
            Producer producer = new Producer();
            string empty = "";

            // Act
            string r = producer.getMessage();


            // Assert
            Assert.AreEqual(r, empty, "Account not debited correctly");

        }

        //[TestMethod]
        //public void Debit_WithValidAmount_UpdatesBalance()
        //{
        //    // Arrange
        //    double beginningBalance = 11.99;
        //    double debitAmount = 4.55;
        //    double expected = 7.44;
        //    BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

        //    // Act
        //    account.Debit(debitAmount);

        //    // Assert
        //    double actual = account.Balance;
        //    Assert.AreEqual(expected, actual, 0.001, "Account not debited correctly");
        //}
    }
}
