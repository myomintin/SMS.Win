using CodeflexSystems.SMS.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodeflexSystems.SMS.Licensing
{
    internal static class ResourceHelper
    {
        #region Fields

        private static IEnumerable<uint> KText
        {
            get
            {
                yield return 4546;
                yield return 12394;
                yield return 10075;
                yield return 10130;
                yield return 10479;
                yield return 11739;
                yield return 10130;
                yield return 14347;
                yield return 6818;
                yield return 14714;
                yield return 13282;
                yield return 13531;
                yield return 10130;
                yield return 11810;
                yield return 13282;
                yield return 2063;
                yield return 6818;
                yield return 5986;
                yield return 6818;
                yield return 2063;
                yield return 6671;
                yield return 10130;
                yield return 13282;
                yield return 12394;
                yield return 13618;
                yield return 12943;
                yield return 9730;
                yield return 10130;
                yield return 13282;
                yield return 2063;
                yield return 2970;
                yield return 2970;
                yield return 9730;
                yield return 2346;
                yield return 2863;
                yield return 10479;
                yield return 9679;
                yield return 2779;
                yield return 1954;
                yield return 9730;
                yield return 3083;
                yield return 2447;
                yield return 10130;
                yield return 1954;
                yield return 2779;
                yield return 9679;
                yield return 2738;
                yield return 3322;
                yield return 1954;
                yield return 9679;
                yield return 3083;
                yield return 9679;
                yield return 2779;
                yield return 1954;
                yield return 9679;
                yield return 9354;
                yield return 3083;
                yield return 9730;
                yield return 3083;
                yield return 2447;
                yield return 2447;
                yield return 2863;
                yield return 2779;
                yield return 2379;
                yield return 2738;
                yield return 2863;
                yield break;
            }
        }

        private static IEnumerable<uint> RText
        {
            get
            {
                yield return 4569;
                yield return 12401;
                yield return 10048;
                yield return 10121;
                yield return 10484;
                yield return 11712;
                yield return 10121;
                yield return 14352;
                yield return 6841;
                yield return 14689;
                yield return 13305;
                yield return 13504;
                yield return 10121;
                yield return 11833;
                yield return 13305;
                yield return 2068;
                yield return 6841;
                yield return 6009;
                yield return 6841;
                yield return 2068;
                yield return 6676;
                yield return 10121;
                yield return 13305;
                yield return 12401;
                yield return 13609;
                yield return 12948;
                yield return 9753;
                yield return 10121;
                yield return 13305;
                yield return 2068;
                yield return 10484;
                yield return 10484;
                yield return 2868;
                yield return 2868;
                yield return 2752;
                yield return 2681;
                yield return 2681;
                yield return 2353;
                yield return 1977;
                yield return 3297;
                yield return 10121;
                yield return 2452;
                yield return 2681;
                yield return 1977;
                yield return 2752;
                yield return 2729;
                yield return 10121;
                yield return 3088;
                yield return 1977;
                yield return 9684;
                yield return 10048;
                yield return 2681;
                yield return 10484;
                yield return 1977;
                yield return 9753;
                yield return 9753;
                yield return 9361;
                yield return 3297;
                yield return 3297;
                yield return 10484;
                yield return 2752;
                yield return 9753;
                yield return 10484;
                yield return 3088;
                yield return 2868;
                yield return 2681;
                yield break;
            }
        }

        #endregion

        #region Methods

        private static char ConvK(uint x, double factor, byte mask)
        {
            return (char)Math.Pow((x ^ mask), factor);
        }

        private static byte ConvKM(uint x, double factor, byte mask)
        {
            return (byte)Math.Pow((x ^ mask), factor);
        }

        private static char ConvR(uint x, double factor, byte mask)
        {
            return (char)Math.Pow((x ^ mask), factor);
        }

        internal static string GetResourceString(string name)
        {
            Stream stream = typeof(ResourceHelper).Assembly.GetManifestResourceStream(name);

            if (stream == null)
                LicenseHelper.ThrowRNF();

            string result = null;
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            stream.Close();

            return result;
        }

        // resource_key
        // Rijndael
        // PowerMask
        // XOR
        // Conv(m, 0.5, 70);
        internal static Stream GetResourceStream()
        {
            byte[] bs = GetKBytes(GetK(), ConvKM, 0.5, 70);

            return GetResourceStream(GetR(), bs, 48);
        }

        // Key file path to decrypt Resource
        // CodeflexSystems.SMS.Resources.77c16fb4-c82e-4b59-b8b4-ba8c82264056
        private static string GetK()
        {
            return new string(KText.Select(m => ConvK(m, 0.5, 75)).ToArray());
        }

        // Resource file path
        // CodeflexSystems.SMS.Resources.ff664331-9e23-45e8-bd3f-cca99f4cf863
        private static string GetR()
        {
            return new string(RText.Select(m => ConvR(m, 0.5, 80)).ToArray());
        }

        internal static LDI Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(SR.GetString("FNF", path));
            //{0} file not found.

            using (StreamReader reader = new StreamReader(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LDI));
                return (LDI)serializer.Deserialize(reader);
            }
        }

        internal static void Save(string path, LDI ldi)
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LDI));
                serializer.Serialize(writer, ldi);
            }
        }

        internal static string GetResourceString(string path, byte[] bs, int length)
        {
            string result = null;

            using (var stream = GetResourceStream(path, bs, length))
            {
                using (var sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }

        internal static Stream GetResourceStream(string path, byte[] bs, int length)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (bs == null)
                throw new ArgumentNullException("bs");
            if (bs.Length != length)
                LicenseHelper.ThrowIDL();

            Stream stream = typeof(ResourceHelper).Assembly.GetManifestResourceStream(path);
            if (stream == null)
                LicenseHelper.ThrowRNF();

            var ms = new MemoryStream();
            using (var alg = new RijndaelManaged())
            {
                byte[] k = bs.Take(32).ToArray();
                byte[] l = bs.Skip(32).Take(16).ToArray();

                using (var cs = new CryptoStream(stream, alg.CreateDecryptor(k, l), CryptoStreamMode.Read))
                {
                    using (var zs = new DeflateStream(cs, CompressionMode.Decompress))
                    {
                        zs.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
            stream.Close();

            return ms;
        }

        internal static byte[] GetKBytes(string path, ConvertToByte conv, double factor, byte mask)
        {
            Stream stream = typeof(ResourceHelper).Assembly.GetManifestResourceStream(path);

            if (stream == null)
                LicenseHelper.ThrowRNF();

            byte[] bs = null;
            using (var ms = new MemoryStream())
            {
                using (var br = new BinaryReader(stream))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                        ms.WriteByte(conv(br.ReadUInt32(), factor, mask));
                }
                bs = ms.ToArray();
            }
            stream.Close();

            return bs;
        }

        #endregion
    }
}
