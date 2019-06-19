using System;
using System.Security.Cryptography;

using AOA.Common.Utility.Crypto;

using NUnit.Framework;

namespace AOA.Common.Utility.NUnitTest.Crypto
{

    public class RSAHelperTest
    {


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RsaSignTest()
        {
            (string pubKey, string priKey) = RSAHelper.GenRSAKeyPair();
            Console.WriteLine(pubKey);
            Console.WriteLine(priKey);

            string sign = RSAHelper.Sign(priKey, "testsetsetset", HashAlgorithmName.SHA256);
            Console.WriteLine(sign);

            bool signOK = RSAHelper.Verify(pubKey, "testsetsetset", sign, HashAlgorithmName.SHA256);

            Assert.IsTrue(signOK);
        }

        [Test]
        public void RsaCryptTest()
        {
            (string pubKey, string priKey) = RSAHelper.GenRSAKeyPair();
            Console.WriteLine(pubKey);
            Console.WriteLine(priKey);

            string encyypted = RSAHelper.EncryptString(pubKey, "testsetsetset");
            Console.WriteLine(encyypted);

            var decrypt = RSAHelper.DecryptString(priKey, encyypted);

            Assert.IsTrue(decrypt == "testsetsetset");
        }

    }

}