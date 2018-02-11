using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace netcore.Core.Utilities
{
    public class Helpers 
    {
        /// <summary>
        /// Create a md5 hash from input
        /// </summary>
        /// <param name="input">Input data that pass through hash function</param>
        /// <returns>Hash string</returns>
        public static string MD5Hash(string input)
        {
            var hash = new StringBuilder();
            var md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        /// <summary>
        /// Compress to the Base64 string from ASCII encoding input
        /// </summary>
        /// <param name="base64Input">ASCII String to be compressed</param>
        /// <returns>Base64 string</returns>
        public static string CompressToBase64(string asciiString)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                byte[] raw = System.Text.Encoding.ASCII.GetBytes(asciiString);       
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                byte[] bytes =  memory.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Decompress from the Base64 string into ASCII encoding output
        /// </summary>
        /// <param name="base64Input">Base64 String to be decompressed</param>
        /// <returns>ASCII encoding string</returns>
        public static string DecompressFromBase64(string base64Input)
        {
            byte[] cbytes = Convert.FromBase64String(base64Input);
            using (GZipStream stream = new GZipStream(new MemoryStream(cbytes), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return System.Text.Encoding.ASCII.GetString(memory.ToArray());
                }
            }
        }

        public static string EncodeToHashId(int id, int domain) 
        {
            // create a short hash id for user
            var hashIds = new Hashids("I am a hero, yes we can", 5);
            return hashIds.Encode(id, domain);
        }

        public static int[] DecodeFromHashId(string hashId) 
        {
            // decode the hash id and get the domain value
            var hashIds = new Hashids("I am a hero, yes we can", 5);
            return hashIds.Decode(hashId);
        }

        public static int GetIdFromHashId(string hashId) 
        {
            // decode the hash id and get the domain value
            var nums = DecodeFromHashId(hashId);

            return (nums.Length > 0) ? DecodeFromHashId(hashId)[0] : -1;
        }

        public static int GetDomainFromHashId(string hashId) 
        {
            // decode the hash id and get the domain value
            var nums = DecodeFromHashId(hashId);

            return (nums.Length > 1) ? DecodeFromHashId(hashId)[1] : -1;
        }
    }
}