using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace AOA.Common.Utility.Crypto
{
	public class RSAKeyGenerator
	{
		public static void Main(String[] argv)
		{
			Console.WriteLine("用法：RSAKeyGenerator [SaveToFile].");
			Console.WriteLine("保存到文件或者显示到屏幕.\n");
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			RSAParameters keys = rsa.ExportParameters(true);
			String pkxml = "<root>\n<Modulus>" + Convert.ToBase64String(keys.Modulus) + "</Modulus>";
			pkxml += "\n<Exponent>" + Convert.ToBase64String(keys.Exponent) + "</Exponent>\n</root>";

			String psxml = "<root>\n<Modulus>" + Convert.ToBase64String(keys.Modulus) + "</Modulus>";
			psxml += "\n<Exponent>" + Convert.ToBase64String(keys.Exponent) + "</Exponent>";
			psxml += "\n<D>" + Convert.ToBase64String(keys.D) + "</D>";
			psxml += "\n<DP>" + Convert.ToBase64String(keys.DP) + "</DP>";
			psxml += "\n<P>" + Convert.ToBase64String(keys.P) + "</P>";
			psxml += "\n<Q>" + Convert.ToBase64String(keys.Q) + "</Q>";
			psxml += "\n<DQ>" + Convert.ToBase64String(keys.DQ) + "</DQ>";
			psxml += "\n<InverseQ>" + Convert.ToBase64String(keys.InverseQ) + "</InverseQ>\n</root>";

			Console.WriteLine(rsa.ToXmlString(true));
			if (argv.Length > 0 && argv[0] == "SaveToFile")
			{
				SaveToFile("publickey.xml", pkxml);
				SaveToFile("privatekey.xml", psxml);
				Console.WriteLine("RSA Key保存成功！");
			}
			else
			{
				Console.WriteLine("PublicKey:\n" + pkxml);
				Console.WriteLine("\nPrivateKey:\n" + psxml);
			}
			CheckSign();
			CheckEncrypt();
		}

		//保存文件
		static void SaveToFile(String filename, String data)
		{
			System.IO.StreamWriter sw = System.IO.File.CreateText(filename);
			sw.WriteLine(data);
			sw.Close();
		}

		//演示数字签名算法
		static void CheckSign()
		{
			Console.WriteLine("\nVerify Signature TEST:\n===========================");
			RSAHandler rsahd = new RSAHandler();
			RSAPKCS1SignatureDeformatter deformat = rsahd.CreateRSADeformatter("publickey.xml");
			RSAPKCS1SignatureFormatter format = rsahd.CreateRSAFormatter("privatekey.xml");
			//读入一个文件
			String filename = "samplefile.jpg";
			Stream sm = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			byte[] hashsm = rsahd.GetHashData(sm);
			Console.WriteLine("1.HashData:\n" + Convert.ToBase64String(hashsm));
			byte[] signdata = format.CreateSignature(hashsm);
			Console.WriteLine("\n2.Signature:\n" + Convert.ToBase64String(signdata));
			if (deformat.VerifySignature(hashsm, signdata))
			{
				Console.WriteLine("\n3.Signature is OK!");
			}
		}

		//演示加密解密处理
		static void CheckEncrypt()
		{
			Console.WriteLine("\n加密解密测试:\n===========================");
			RSAHandler rsahd = new RSAHandler();
			RSACryptoServiceProvider rsaprd = rsahd.CreateRSAProvider("privatekey.xml");
			RSACryptoServiceProvider rsaprd1 = rsahd.CreateRSAEncryptProvider("publickey.xml");
			String text = "Hello World!";
			Console.WriteLine("1.明文:\n" + text);
			byte[] data = new UnicodeEncoding().GetBytes(text);
			byte[] endata = rsaprd1.Encrypt(data, true);
			Console.WriteLine("2.publicKey加密后的数据:\n" + Convert.ToBase64String(endata));
			byte[] dedata = rsaprd.Decrypt(endata, true);
			Console.WriteLine("3.privateKey解密后的数据:\n" + (new UnicodeEncoding()).GetString(dedata));

		}

	}
}