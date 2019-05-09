using System;
using AOA.Common.Utility.Crypto;
using NUnit.Framework;

namespace Tests
{

    public class Tests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetHardwareIdTest()
        {
            var hwId = HardwareIdHelper.GetHardwareId();
            Console.WriteLine(hwId);
            Assert.IsNotEmpty(hwId);
            var hwInfo = HardwareIdHelper.GetInfoFromHardwareId(hwId);
            Assert.IsNotEmpty(hwInfo);
            Assert.GreaterOrEqual(hwInfo.Length, 2);
        }

        [Test]
        public void GetHardwareIdBase64Test()
        {
            var hwId = HardwareIdHelper.GetHardwareIdBase64();
            Console.WriteLine(hwId);
            Assert.IsNotEmpty(hwId);
            var hwInfo = HardwareIdHelper.GetInfoFromHardwareIdBase64(hwId);
            Assert.IsNotEmpty(hwInfo);
            Assert.GreaterOrEqual(hwInfo.Length, 2);
        }

    }

}