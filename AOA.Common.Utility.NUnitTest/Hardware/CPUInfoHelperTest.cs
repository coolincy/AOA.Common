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
            var cpuId = CPUInfoHelper.GetCPUId();
            Console.WriteLine(cpuId);
            Assert.IsNotEmpty(cpuId);
        }

    }

}