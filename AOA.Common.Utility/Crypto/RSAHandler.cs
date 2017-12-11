using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace AOA.Common.Utility.Crypto
{

	public class RSAHandler
	{
		public RSACryptoServiceProvider CreateRSAProvider(String privateKeyFile)
		{
			RSAParameters parameters1;
			parameters1 = new RSAParameters();
			StreamReader reader1 = new StreamReader(privateKeyFile);
			XmlDocument document1 = new XmlDocument();
			document1.LoadXml(reader1.ReadToEnd());
			XmlElement element1 = (XmlElement)document1.SelectSingleNode("root");
			parameters1.Modulus = ReadChild(element1, "Modulus");
			parameters1.Exponent = ReadChild(element1, "Exponent");
			parameters1.D = ReadChild(element1, "D");
			parameters1.DP = ReadChild(element1, "DP");
			parameters1.DQ = ReadChild(element1, "DQ");
			parameters1.P = ReadChild(element1, "P");
			parameters1.Q = ReadChild(element1, "Q");
			parameters1.InverseQ = ReadChild(element1, "InverseQ");
			CspParameters parameters2 = new CspParameters();
			parameters2.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider(parameters2);
			provider1.ImportParameters(parameters1);
			return provider1;
		}

		public RSACryptoServiceProvider CreateRSAEncryptProvider(String publicKeyFile)
		{
			RSAParameters parameters1;
			parameters1 = new RSAParameters();
			StreamReader reader1 = new StreamReader(publicKeyFile);
			XmlDocument document1 = new XmlDocument();
			document1.LoadXml(reader1.ReadToEnd());
			XmlElement element1 = (XmlElement)document1.SelectSingleNode("root");
			parameters1.Modulus = ReadChild(element1, "Modulus");
			parameters1.Exponent = ReadChild(element1, "Exponent");
			CspParameters parameters2 = new CspParameters();
			parameters2.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider(parameters2);
			provider1.ImportParameters(parameters1);
			return provider1;
		}

		public RSAPKCS1SignatureDeformatter CreateRSADeformatter(String publicKeyFile)
		{
			RSAParameters parameters1;
			parameters1 = new RSAParameters();
			StreamReader reader1 = new StreamReader(publicKeyFile);
			XmlDocument document1 = new XmlDocument();
			document1.LoadXml(reader1.ReadToEnd());
			XmlElement element1 = (XmlElement)document1.SelectSingleNode("root");
			parameters1.Modulus = ReadChild(element1, "Modulus");
			parameters1.Exponent = ReadChild(element1, "Exponent");
			CspParameters parameters2 = new CspParameters();
			parameters2.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider(parameters2);
			provider1.ImportParameters(parameters1);
			RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(provider1);
			deformatter.SetHashAlgorithm("SHA1");
			return deformatter;
		}

		public RSAPKCS1SignatureFormatter CreateRSAFormatter(String privateKeyFile)
		{
			RSAParameters parameters1;
			parameters1 = new RSAParameters();
			StreamReader reader1 = new StreamReader(privateKeyFile);
			XmlDocument document1 = new XmlDocument();
			document1.LoadXml(reader1.ReadToEnd());
			XmlElement element1 = (XmlElement)document1.SelectSingleNode("root");
			parameters1.Modulus = ReadChild(element1, "Modulus");
			parameters1.Exponent = ReadChild(element1, "Exponent");
			parameters1.D = ReadChild(element1, "D");
			parameters1.DP = ReadChild(element1, "DP");
			parameters1.DQ = ReadChild(element1, "DQ");
			parameters1.P = ReadChild(element1, "P");
			parameters1.Q = ReadChild(element1, "Q");
			parameters1.InverseQ = ReadChild(element1, "InverseQ");
			CspParameters parameters2 = new CspParameters();
			parameters2.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider(parameters2);
			provider1.ImportParameters(parameters1);
			RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(provider1);
			formatter.SetHashAlgorithm("SHA1");
			return formatter;
		}

		public byte[] GetHashData(byte[] data)
		{
			HashAlgorithm algorithm1 = HashAlgorithm.Create("SHA1");
			return algorithm1.ComputeHash(data);
		}

		public byte[] GetHashData(Stream data)
		{
			HashAlgorithm algorithm1 = HashAlgorithm.Create("SHA1");
			return algorithm1.ComputeHash(data);
		}

		private byte[] ReadChild(XmlElement parent, string name)
		{
			XmlElement element1 = (XmlElement)parent.SelectSingleNode(name);
			return Convert.FromBase64String(element1.InnerText);
		}
	}
}