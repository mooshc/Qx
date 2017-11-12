using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Qx.EntitySerialization
{
    public class EntitySerializer
    {
        private const string ENCRYPTION_PASSPHRASE = "FDSF$GC=FG%";
        private static readonly byte[] ENCRYPTION_KEY = Encoding.ASCII.GetBytes("Va1Bdfkfjlk93hnsf08gsf8ojhsdgf08");
        private static readonly byte[] ENCRYPTION_IV = Encoding.ASCII.GetBytes("3nvf9jmfnskfncks");
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };

        public static void SerializeToFile(object obj, string type, string decription, string location)
        {
            if (!Directory.Exists(location))
            {
                throw new EntitySerializerException("Could not access location: " + location);
            }
            var fileMetadata = new EntityFileMetaData(type, decription);
            var fileName = fileMetadata.GenerateFileName();
            var fullFilePath = Path.Combine(location, fileName);
            string json = JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            CompressAndEncrypt(json, fullFilePath);
        }

        public static T DeserializeFromFile<T>(string type, string location, out string fileName)
        {
            if (!Directory.Exists(location))
            {
                throw new EntitySerializerException("Could not access location: " + location);
            }
            var filePaths = Directory.GetFiles(location, $"*{type}*{EntityFileMetaData.FILE_EXT_NAME}", SearchOption.TopDirectoryOnly);
            if (filePaths.Length == 0)
            {
                throw new EntitySerializerException($"Could not find any DB files of type '{type}'");
            }
            var selectedFilePath = GetLatestVersionFile(filePaths);
            fileName = Path.GetFileName(selectedFilePath);
            string json = DecryptAndDecompress(selectedFilePath);
            return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
        }

        private static string GetLatestVersionFile(IEnumerable<string> filePaths)
        {
            EntityFileMetaData latestVersionFileMetadata = null;
            string latestVersionFilePath = null;
            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);
                EntityFileMetaData entityFileMetaData = EntityFileMetaData.FromFileName(fileName);
                if (latestVersionFileMetadata == null || latestVersionFileMetadata.Version < entityFileMetaData.Version)
                {
                    latestVersionFileMetadata = entityFileMetaData;
                    latestVersionFilePath = filePath;
                }
            }
            return latestVersionFilePath;
        }
        
        private static void CompressAndEncrypt(string plainText, string filePath)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = ENCRYPTION_KEY;
                rijAlg.IV = ENCRYPTION_IV;
                rijAlg.Padding = PaddingMode.Zeros;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate))
                using (var csEncrypt = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write))
                using (var gs = new GZipStream(csEncrypt, CompressionMode.Compress))
                using (var swCompressAndEncrypt = new StreamWriter(gs))
                {
                    //Write all data to the stream.
                    swCompressAndEncrypt.Write(plainText);
                }
            }
        }

        public static string DecryptAndDecompress(string filePath)
        {
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = ENCRYPTION_KEY;
                rijAlg.IV = ENCRYPTION_IV;
                rijAlg.Padding = PaddingMode.Zeros;
                
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (var fs = File.OpenRead(filePath))
                using (CryptoStream csDecrypt = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
                using (GZipStream gz = new GZipStream(csDecrypt, CompressionMode.Decompress))
                using (StreamReader srDecryptAndDecompress = new StreamReader(gz))
                {
                    // Read the decrypted bytes from the decrypting stream
                    // and place them in a string.
                    return srDecryptAndDecompress.ReadToEnd();
                }
            }
        }
    }
}
