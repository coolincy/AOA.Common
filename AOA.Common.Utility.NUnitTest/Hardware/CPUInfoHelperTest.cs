using System;

using AOA.Common.Utility.Hardware;

using NUnit.Framework;

namespace AOA.Common.Utility.NUnitTest.Hardware
{

    public class CPUInfoHelperTest
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetCPUIdTest()
        {
            var id = CPUInfoHelper.GetCPUId();
            Console.WriteLine(id);
            Assert.IsNotEmpty(id);
        }

        [Test]
        public void SystemUUIDTest()
        {
            var id = CPUInfoHelper.GetSystemUUID();
            Console.WriteLine(id);
            Assert.IsNotEmpty(id);
        }

    }

}