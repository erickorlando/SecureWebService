
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Xml;

namespace ConsoleApplication384.encoder
{
	public class CustomTextMessageEncoder : MessageEncoder
	{
		private CustomTextMessageEncoderFactory factory;
		private XmlWriterSettings writerSettings;
		private string contentType;
		private byte[] key;
   
		public CustomTextMessageEncoder(CustomTextMessageEncoderFactory factory)
		{
			this.factory = factory;
			
			this.writerSettings = new XmlWriterSettings();            
		}

		public override string ContentType
		{
			get
			{
				return "text/xml";
			}
		}

		public override string MediaType
		{
			get 
			{
				return "text/xml";
			}
		}

		public override MessageVersion MessageVersion
		{
			get 
			{
				return this.factory.MessageVersion;
			}
		}

		public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
		{   
			byte[] msgContents = new byte[buffer.Count];
			Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
			bufferManager.ReturnBuffer(buffer.Array);

			MemoryStream stream = new MemoryStream(msgContents);
			return ReadMessage(stream, int.MaxValue);
		}

		public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
		{
			
			var sr = new StreamReader(stream);
			var wireResponse = sr.ReadToEnd();

			var logicalResponse = GetDecryptedResponse(wireResponse);
			logicalResponse = String.Format(
					@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
							<s:Body>
								{0}
							</s:Body>
						</s:Envelope>",
					logicalResponse);

			XmlReader reader = XmlReader.Create(new StringReader(logicalResponse));
			return Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion.Soap11);
		}

		private string GetDecryptedResponse(string encryptedResponse)
		{
			var doc = new XmlDocument();
			doc.LoadXml(encryptedResponse);
			var cipherNode = doc.SelectSingleNode("//*[local-name(.)='Body']//*[local-name(.)='CipherValue']");
			
			byte[] cypher = Convert.FromBase64String(cipherNode.InnerText);

			byte[] key = GetEncryptingKey();
			byte[] iv = GetIV(cypher);
			

			var AesManagedAlg = new AesCryptoServiceProvider();
			AesManagedAlg.KeySize = 128;
			AesManagedAlg.Key = key;
			AesManagedAlg.IV = iv;

			var body = ExtractIVAndDecrypt(AesManagedAlg, cypher, 0, cypher.Length);

			return UTF8Encoding.UTF8.GetString(body);
		}

		internal static byte[] ExtractIVAndDecrypt(SymmetricAlgorithm algorithm, byte[] cipherText, int offset, int count)
		{
			byte[] buffer2;
			if (cipherText == null)
			{
				throw new Exception();
			}
			if ((count < 0) || (count > cipherText.Length))
			{
				throw new Exception();
			}
			if ((offset < 0) || (offset > (cipherText.Length - count)))
			{
				throw new Exception();
			}
			int num = algorithm.BlockSize / 8;
			byte[] dst = new byte[num];
			Buffer.BlockCopy(cipherText, offset, dst, 0, dst.Length);
			algorithm.Padding = PaddingMode.ISO10126;
			algorithm.Mode = CipherMode.CBC;
			try
			{
				using (ICryptoTransform transform = algorithm.CreateDecryptor(algorithm.Key, dst))
				{
					buffer2 = transform.TransformFinalBlock(cipherText, offset + dst.Length, count - dst.Length);
				}
			}
			catch (CryptographicException exception)
			{
				throw new Exception();
			}
			return buffer2;
		}

		private byte[] GetIV( byte[] cypher)
		{            
			byte[] IV = new byte[16];
			Array.Copy(cypher, IV, 16);
			return IV;
		}      

		private byte[] GetEncryptingKey()
		{
			return this.key;

			/*
			string wrappingKey = "";
			X509Certificate2 serverCert = new X509Certificate2(File.ReadAllBytes(@"c:\server_cert.p12"));
			RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)serverCert.PrivateKey;
			var enckey = rsa.Decrypt(Convert.FromBase64String(wrappingKey), true);
			return enckey;
			 * */
		}

		public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
		{
			MemoryStream stream = new MemoryStream();
			XmlWriter writer = XmlWriter.Create(stream, this.writerSettings);
			message.WriteMessage(writer);
			writer.Close();

			SaveEncryptionKey(message);

			byte[] messageBytes = stream.GetBuffer();
			int messageLength = (int)stream.Position;
			stream.Close();

			int totalLength = messageLength + messageOffset;
			byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
			Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

			ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
			return byteArray;
		}

		private void SaveEncryptionKey(Message message)
		{
			var secHeaderType = message.GetType().GetField("securityHeader", 
											  BindingFlags.NonPublic | BindingFlags.Instance);
			var secHeader = secHeaderType.GetValue(message);

			var encTokenType = secHeader.GetType().BaseType.BaseType.GetField("encryptingToken", 
											  BindingFlags.NonPublic | BindingFlags.Instance);

			var token = (WrappedKeySecurityToken)encTokenType.GetValue(secHeader);
			var securityKey = (InMemorySymmetricSecurityKey)token.SecurityKeys[0];

			var symmetricKey = securityKey.GetType().GetField("symmetricKey", 
											   BindingFlags.NonPublic | BindingFlags.Instance);
			
			this.key = (byte[])symmetricKey.GetValue(securityKey);
		}

		public override void WriteMessage(Message message, Stream stream)
		{
			XmlWriter writer = XmlWriter.Create(stream, this.writerSettings);
			message.WriteMessage(writer);
			writer.Close();
		}
	}
}
