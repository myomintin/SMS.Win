using CodeflexSystems.Library.BaseX;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Licensing
{
    internal static class LicenseManager
    {
        #region Fields

        private static IEnumerable<uint> VKText
        {
            get
            {
                yield return 4563;
                yield return 12411;
                yield return 10058;
                yield return 10115;
                yield return 10494;
                yield return 11722;
                yield return 10115;
                yield return 14362;
                yield return 6835;
                yield return 14699;
                yield return 13299;
                yield return 13514;
                yield return 10115;
                yield return 11827;
                yield return 13299;
                yield return 2078;
                yield return 6835;
                yield return 6003;
                yield return 6835;
                yield return 2078;
                yield return 6686;
                yield return 10115;
                yield return 13299;
                yield return 12411;
                yield return 13603;
                yield return 12958;
                yield return 9747;
                yield return 10115;
                yield return 13299;
                yield return 2078;
                yield return 10115;
                yield return 2394;
                yield return 2462;
                yield return 10494;
                yield return 10058;
                yield return 10115;
                yield return 2462;
                yield return 2394;
                yield return 1971;
                yield return 2462;
                yield return 2462;
                yield return 10115;
                yield return 2723;
                yield return 1971;
                yield return 2762;
                yield return 2462;
                yield return 2462;
                yield return 2462;
                yield return 1971;
                yield return 3098;
                yield return 10494;
                yield return 2363;
                yield return 2462;
                yield return 1971;
                yield return 10494;
                yield return 2394;
                yield return 10115;
                yield return 2363;
                yield return 10115;
                yield return 3307;
                yield return 10115;
                yield return 2363;
                yield return 10058;
                yield return 3307;
                yield return 9694;
                yield return 2762;
                yield break;
            }
        }

        private static IEnumerable<uint> VRText
        {
            get
            {
                yield return 4566;
                yield return 12414;
                yield return 10063;
                yield return 10118;
                yield return 10491;
                yield return 11727;
                yield return 10118;
                yield return 14367;
                yield return 6838;
                yield return 14702;
                yield return 13302;
                yield return 13519;
                yield return 10118;
                yield return 11830;
                yield return 13302;
                yield return 2075;
                yield return 6838;
                yield return 6006;
                yield return 6838;
                yield return 2075;
                yield return 6683;
                yield return 10118;
                yield return 13302;
                yield return 12414;
                yield return 13606;
                yield return 12955;
                yield return 9750;
                yield return 10118;
                yield return 13302;
                yield return 2075;
                yield return 2459;
                yield return 2726;
                yield return 3310;
                yield return 10491;
                yield return 2726;
                yield return 2366;
                yield return 10491;
                yield return 9750;
                yield return 1974;
                yield return 2958;
                yield return 2875;
                yield return 3103;
                yield return 3103;
                yield return 1974;
                yield return 2767;
                yield return 2958;
                yield return 2875;
                yield return 3310;
                yield return 1974;
                yield return 3103;
                yield return 10491;
                yield return 9374;
                yield return 2678;
                yield return 1974;
                yield return 2366;
                yield return 2678;
                yield return 2958;
                yield return 2726;
                yield return 2767;
                yield return 9691;
                yield return 2366;
                yield return 2726;
                yield return 2875;
                yield return 9374;
                yield return 10118;
                yield return 10063;
                yield break;
            }
        }

        private static IEnumerable<uint> AKText
        {
            get
            {
                yield return 4576;
                yield return 12360;
                yield return 10105;
                yield return 10160;
                yield return 10445;
                yield return 11769;
                yield return 10160;
                yield return 14377;
                yield return 6784;
                yield return 14680;
                yield return 13248;
                yield return 13561;
                yield return 10160;
                yield return 11776;
                yield return 13248;
                yield return 2093;
                yield return 6784;
                yield return 5952;
                yield return 6784;
                yield return 2093;
                yield return 6701;
                yield return 10160;
                yield return 13248;
                yield return 12360;
                yield return 13584;
                yield return 12973;
                yield return 9760;
                yield return 10160;
                yield return 13248;
                yield return 2093;
                yield return 3288;
                yield return 2809;
                yield return 2829;
                yield return 2409;
                yield return 9760;
                yield return 2409;
                yield return 10445;
                yield return 3288;
                yield return 1920;
                yield return 9384;
                yield return 10160;
                yield return 2312;
                yield return 2829;
                yield return 1920;
                yield return 2809;
                yield return 2829;
                yield return 3000;
                yield return 10105;
                yield return 1920;
                yield return 3288;
                yield return 2312;
                yield return 2409;
                yield return 2409;
                yield return 1920;
                yield return 2704;
                yield return 10160;
                yield return 9709;
                yield return 10445;
                yield return 9760;
                yield return 10105;
                yield return 10445;
                yield return 9760;
                yield return 2624;
                yield return 2312;
                yield return 3113;
                yield return 9384;
                yield break;
            }
        }

        private static IEnumerable<uint> ARText
        {
            get
            {
                yield return 4583;
                yield return 12367;
                yield return 10110;
                yield return 10167;
                yield return 10442;
                yield return 11774;
                yield return 10167;
                yield return 14382;
                yield return 6791;
                yield return 14687;
                yield return 13255;
                yield return 13566;
                yield return 10167;
                yield return 11783;
                yield return 13255;
                yield return 2090;
                yield return 6791;
                yield return 5959;
                yield return 6791;
                yield return 2090;
                yield return 6698;
                yield return 10167;
                yield return 13255;
                yield return 12367;
                yield return 13591;
                yield return 12970;
                yield return 9767;
                yield return 10167;
                yield return 13255;
                yield return 2090;
                yield return 10167;
                yield return 9767;
                yield return 2319;
                yield return 3007;
                yield return 10110;
                yield return 10167;
                yield return 9706;
                yield return 2814;
                yield return 1927;
                yield return 2814;
                yield return 2414;
                yield return 2319;
                yield return 9391;
                yield return 1927;
                yield return 2814;
                yield return 2711;
                yield return 3118;
                yield return 2319;
                yield return 1927;
                yield return 3295;
                yield return 9706;
                yield return 10442;
                yield return 3118;
                yield return 1927;
                yield return 2319;
                yield return 2414;
                yield return 2319;
                yield return 2631;
                yield return 9706;
                yield return 3295;
                yield return 2319;
                yield return 2631;
                yield return 2474;
                yield return 2814;
                yield return 3118;
                yield return 9767;
                yield break;
            }
        }

        private static readonly char[] xc = "0189".ToCharArray();

        #endregion

        #region Ctor, Dtor

        #endregion

        #region Methods

        private static char ConvVK(uint x, double factor, byte mask)
        {
            return (char)Math.Pow((x ^ mask), factor);
        }

        private static byte ConvVKM(uint x, double factor, byte mask)
        {
            return (byte)Math.Pow((x ^ mask), factor);
        }

        private static char ConvVR(uint x, double factor, byte mask)
        {
            return (char)Math.Pow((x ^ mask), factor);
        }

        // rsa_verify_key
        // Rijndael
        // PowerMask
        // XOR
        // Conv(m, 0.5, 85);
        private static string GetVRString()
        {
            string value = null;

            try
            {
                byte[] bs = ResourceHelper.GetKBytes(GetVK(), ConvVKM, 0.5, 85);
                return ResourceHelper.GetResourceString(GetVR(), bs, 48);
            }
            catch { }

            return value;
        }

        // Key file path to decrypt RSA for verification key
        // CodeflexSystems.SMS.Resources.e02fde20-22e5-4222-8f12-f0e1e9e1d9b4
        private static string GetVK()
        {
            return new string(VKText.Select(m => ConvVK(m, 0.5, 90)).ToArray());
        }

        // RSA file path to verify key
        // CodeflexSystems.SMS.Resources.259f51fc-7688-4769-8fa3-13754b156aed
        private static string GetVR()
        {
            return new string(VRText.Select(m => ConvVR(m, 0.5, 95)).ToArray());
        }

        private static char ConvAK(uint x, double factor, byte mask)
        {
            return (char)Math.Pow((x ^ mask), factor);
        }

        private static byte ConvAKM(uint x, double factor, byte mask)
        {
            return (byte)Math.Pow((x ^ mask), factor);
        }

        private static char ConvAR(uint x, double factor, byte mask)
        {
            return (char)Math.Pow((x ^ mask), factor);
        }

        // rsa_activate_key
        // Rijndael
        // PowerMask
        // XOR
        // Conv(m, 0.5, 100);
        private static string GetARString()
        {
            string value = null;

            try
            {
                byte[] bs = ResourceHelper.GetKBytes(GetAK(), ConvAKM, 0.5, 100);
                value = ResourceHelper.GetResourceString(GetAR(), bs, 48);
            }
            catch { }

            return value;
        }

        // Key file path to decrypt RSA for activation license
        // CodeflexSystems.SMS.Resources.9460c0f9-ae16-467d-9100-5ebfcdfc318a
        private static string GetAK()
        {
            return new string(AKText.Select(m => ConvAK(m, 0.5, 105)).ToArray());
        }

        // RSA file path to activate license
        // CodeflexSystems.SMS.Resources.ec17deb4-401a-4581-9bf8-1013b913248c
        private static string GetAR()
        {
            return new string(ARText.Select(m => ConvAR(m, 0.5, 110)).ToArray());
        }

        internal static SMSLicenseKeyInfo VerifySMS(string key)
        {
            if (string.IsNullOrEmpty(key))
                LicenseHelper.ThrowLicenseKeyIsNull();

            string pkey = Purify(key);
            if (pkey.Length != 55)
                LicenseHelper.ThrowLicenseKeyIsInvalid();

            byte[] kdata = Base32.Decode(pkey);
            int kl = kdata.Length; // 34
            int el = kl - 2; // 32
            int hl = el / 2; // 16
            if (kl != 34 || hl <= 0 || ((hl * 2) + 2) != kl)
                LicenseHelper.ThrowLicenseKeyIsInvalid();

            byte[] edata = new byte[el];
            byte[] id = new byte[2];
            Tweak(kdata);
            using (MemoryStream stream = new MemoryStream(kdata))
            {
                stream.Read(edata, 0, hl);
                stream.Read(id, 0, 2);
                stream.Read(edata, hl, el - hl);
            }

            string value = GetVRString();
            if (string.IsNullOrEmpty(value))
                LicenseHelper.ThrowRNF();

            byte[] sign = null;
            try
            {
                using (var csp = new RSACryptoServiceProvider())
                {
                    csp.FromXmlString(value);
                    sign = csp.SignData(id, new SHA256CryptoServiceProvider());
                }
            }
            catch { }
            if (sign == null)
                LicenseHelper.ThrowLicenseKeyIsInvalid();

            byte[] k = sign.Take(32).ToArray();
            byte[] l = sign.Skip(32).Take(16).ToArray();
            byte[] data = null;
            try
            {
                using (var alg = new RijndaelManaged())
                {
                    data = alg.CreateDecryptor(k, l).TransformFinalBlock(edata, 0, edata.Length);
                }
            }
            catch { }
            if (data == null || data.Length != 21)
                LicenseHelper.ThrowLicenseKeyIsInvalid();

            byte[] rd = new byte[1];
            byte[] ptypes = new byte[1];
            byte[] vers = new byte[2];
            byte[] ltypes = new byte[1];
            byte[] days = new byte[2];
            byte[] gdays = new byte[8];
            byte[] opts = new byte[2];
            byte[] cpl = new byte[1];
            byte[] spl = new byte[1];
            byte[] tpl = new byte[2];
            using (MemoryStream stream = new MemoryStream(data))
            {
                stream.Read(rd, 0, rd.Length);
                stream.Read(ptypes, 0, ptypes.Length);
                stream.Read(vers, 0, vers.Length);
                stream.Read(ltypes, 0, ltypes.Length);
                stream.Read(days, 0, days.Length);
                stream.Read(gdays, 0, gdays.Length);
                stream.Read(opts, 0, opts.Length);
                stream.Read(cpl, 0, cpl.Length);
                stream.Read(spl, 0, spl.Length);
                stream.Read(tpl, 0, tpl.Length);
            }

            SMSLicenseKeyInfo li = new SMSLicenseKeyInfo();
            try
            {
                bool[] bs = ToBoolArray(BitConverter.ToUInt16(opts, 0), 4);

                li.ProductType = (ProductType)ptypes[0];
                li.Version = new Version(vers[0], vers[1], 0, 0);
                li.LicenseType = (LicenseType)ltypes[0];
                li.TrialDays = BitConverter.ToUInt16(days, 0);
                li.GenerateDate = new DateTime(BitConverter.ToInt64(gdays, 0));
                li.AllowMultiLanguage = bs[0];
                li.AllowMultiCurrency = bs[1];
                li.AllowMultiSchool = bs[2];
                li.AllowMultiStudent = bs[3];
                li.CurrenciesPerLicense = cpl[0];
                li.SchoolsPerLicense = spl[0];
                li.StudentsPerLicense = BitConverter.ToUInt16(tpl, 0);
            }
            catch
            {
                LicenseHelper.ThrowLicenseKeyIsInvalid();
            }

            if (id == null || ptypes[0] != id[0] || ltypes[0] != id[1])
                LicenseHelper.ThrowLicenseKeyIsIncorrect();

            return li;
        }

        internal static LDI GetLDI(SMSLicenseInfo lic)
        {
            if (lic == null)
                LicenseHelper.ThrowLicenseIsNull();
            if (!lic.IsValid())
                LicenseHelper.ThrowLicense(lic);

            byte[] ptypes = new byte[] { (byte)lic.ProductType };
            byte[] vers = new byte[] { (byte)lic.Version.Major, (byte)lic.Version.Minor };
            byte[] ltypes = new byte[] { (byte)lic.LicenseType };
            byte[] gdate = BitConverter.GetBytes(lic.GenerateDate.Ticks);
            byte[] op1 = BitConverter.GetBytes(lic.AllowMultiLanguage);
            byte[] op2 = BitConverter.GetBytes(lic.AllowMultiCurrency);
            byte[] cpl = new byte[] { lic.CurrenciesPerLicense };
            byte[] op3 = BitConverter.GetBytes(lic.AllowMultiSchool);
            byte[] spl = new byte[] { lic.SchoolsPerLicense };
            byte[] op4 = BitConverter.GetBytes(lic.AllowMultiStudent);
            byte[] tpl = BitConverter.GetBytes(lic.StudentsPerLicense);
            byte[] tdays = BitConverter.GetBytes(lic.TrialDays);
            byte[] cadate = BitConverter.GetBytes(DateTime.Now.Ticks);
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (var hash in ToHash(true))
                    stream.Write(hash, 0, hash.Length);
                stream.Write(cadate, 0, cadate.Length);
                stream.Write(ptypes, 0, ptypes.Length);
                stream.Write(vers, 0, vers.Length);
                stream.Write(ltypes, 0, ltypes.Length);
                stream.Write(op1, 0, op1.Length);
                stream.Write(op2, 0, op2.Length);
                stream.Write(cpl, 0, cpl.Length);
                stream.Write(op3, 0, op3.Length);
                stream.Write(spl, 0, spl.Length);
                stream.Write(op4, 0, op4.Length);
                stream.Write(tpl, 0, tpl.Length);
                stream.Write(gdate, 0, gdate.Length);
                stream.Write(tdays, 0, tdays.Length);
                data = stream.ToArray();
            }

            string value = GetARString();
            if (string.IsNullOrEmpty(value))
                LicenseHelper.ThrowRNF();

            byte[] edata = null;
            using (var csp = new RSACryptoServiceProvider())
            {
                csp.FromXmlString(value);
                edata = csp.Encrypt(data, true);
            }

            return new LDI { A = Convert.ToBase64String(edata) };
        }

        internal static SMSLicenseInfo GetSMSLI(LDI ldi)
        {
            if (ldi == null)
                return new SMSLicenseInfo(LicenseStatus.Invalid, SR2.GetString("LINOE"));
            //License is null or empty.
            if (ldi.A == null || ldi.S == null)
                return new SMSLicenseInfo(LicenseStatus.Invalid, SR2.GetString("LIIV"));
            //License is invalid.

            try
            {
                byte[] data = Convert.FromBase64String(ldi.A);
                if (data.Length != 70)
                    return new SMSLicenseInfo(LicenseStatus.Invalid, SR2.GetString("LIIIV"));
                //License information is invalid.

                byte[] edata = Convert.FromBase64String(ldi.S);
                string value = GetARString();
                if (string.IsNullOrEmpty(value))
                    return new SMSLicenseInfo(LicenseStatus.InternalError, SR.GetString("RNF"));

                bool flag = false;
                using (var csp = new RSACryptoServiceProvider())
                {
                    csp.FromXmlString(value);
                    flag = csp.VerifyData(data, new SHA256CryptoServiceProvider(), edata);
                }
                if (!flag)
                    return new SMSLicenseInfo(LicenseStatus.Invalid, SR2.GetString("LSIIV"));
                //License signature is invalid.

                var list = ToHash(false);
                byte[] mhash = new byte[32];
                byte[] hash = new byte[32];
                byte[] cadate = new byte[8];
                byte[] ptypes = new byte[1];
                byte[] vers = new byte[2];
                byte[] ltypes = new byte[1];
                byte[] gdate = new byte[8];
                byte[] op1 = new byte[1];
                byte[] op2 = new byte[1];
                byte[] cpl = new byte[1];
                byte[] op3 = new byte[1];
                byte[] spl = new byte[1];
                byte[] op4 = new byte[1];
                byte[] tpl = new byte[2];
                byte[] tdays = new byte[2];
                byte[] sadate = new byte[8];
                using (MemoryStream stream = new MemoryStream(data))
                {
                    stream.Read(hash, 0, hash.Length);
                    stream.Read(cadate, 0, cadate.Length);
                    stream.Read(ptypes, 0, ptypes.Length);
                    stream.Read(vers, 0, vers.Length);
                    stream.Read(ltypes, 0, ltypes.Length);
                    stream.Read(op1, 0, op1.Length);
                    stream.Read(op2, 0, op2.Length);
                    stream.Read(cpl, 0, cpl.Length);
                    stream.Read(op3, 0, op3.Length);
                    stream.Read(spl, 0, spl.Length);
                    stream.Read(op4, 0, op4.Length);
                    stream.Read(tpl, 0, tpl.Length);
                    stream.Read(gdate, 0, gdate.Length);
                    stream.Read(tdays, 0, tdays.Length);
                    stream.Read(sadate, 0, sadate.Length);
                }
                mhash = list.SelectMany(l => l.Select(m => m)).ToArray();

                if (mhash.IsEqual(hash) == false)
                    return new SMSLicenseInfo(LicenseStatus.MachineHashMismatch, SR2.GetString("CALIAM"));
                //Computer and license information are mismatch.

                return new SMSLicenseInfo((ProductType)ptypes[0],
                    vers[0], vers[1],
                    (LicenseType)ltypes[0],
                    new DateTime(BitConverter.ToInt64(gdate, 0)),
                    new DateTime(BitConverter.ToInt64(cadate, 0)),
                    new DateTime(BitConverter.ToInt64(sadate, 0)),
                    BitConverter.ToUInt16(tdays, 0),
                    BitConverter.ToBoolean(op1, 0),
                    BitConverter.ToBoolean(op2, 0),
                    cpl[0],
                    BitConverter.ToBoolean(op3, 0),
                    spl[0],
                    BitConverter.ToBoolean(op4, 0),
                    BitConverter.ToUInt16(tpl, 0));
            }
            catch
            {
                return new SMSLicenseInfo(LicenseStatus.Invalid, SR2.GetString("LIIV"));
            }
        }

        private static List<byte[]> ToHash(bool flag)
        {
            var list = new List<object>();

            AddIdentifier(list, SR2.GetString("W32PID"), SR2.GetString("W32PV"), flag);
            AddIdentifier(list, SR2.GetString("W32DDID"), SR2.GetString("W32DDV"), flag);
            list.RemoveAll(m => m == null);

            var hash = new MD5CryptoServiceProvider();
            var hList = new List<byte[]>();

            if (list.Count < 2)
            {
                if (flag)
                    LicenseHelper.ThrowLicenseActivation();
                else
                    LicenseHelper.ThrowLicenseChecking();
            }
            foreach (var item in list.Take(2))
            {
                hList.Add(hash.ComputeHash(Encoding.ASCII.GetBytes(item.ToString())));
            }

            return hList;
        }

        private static void AddIdentifier(List<object> list, string path, string property, bool flag)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(property))
            {
                if (flag)
                    LicenseHelper.ThrowLicenseActivation();
                else
                    LicenseHelper.ThrowLicenseChecking();
            }
            using (ManagementClass management = new ManagementClass(path))
            {
                using (ManagementObjectCollection managementObjects = management.GetInstances())
                {
                    foreach (var item in managementObjects)
                    {
                        var obj = item[property];

                        if (obj != null)
                        {
                            list.Add(obj);
                            break;
                        }
                    }
                }
            }
        }

        private static string Purify(string key)
        {
            var karray = key.Where(m => m != '-').ToArray();

            for (int i = karray.Length - 1; i >= 0; i--)
                if (xc.Contains(karray[i]))
                    karray[i] = '=';

            return new string(karray);
        }

        private static bool[] ToBoolArray(ushort value, int length)
        {
            List<bool> list = new List<bool>();

            if (length < 0)
                length = 0;
            foreach (var c in Convert.ToString(value, 2).PadLeft(length, '0').ToArray().Reverse())
                list.Add(c == '1');

            return list.Take(length).ToArray();
        }

        private static void Tweak<T>(T[] source)
        {
            int len = source.Length;

            if (len % 2 != 0)
                len -= 1;
            for (int i = 0; i < len; i += 2)
            {
                T t = source[i];
                source[i] = source[i + 1];
                source[i + 1] = t;
            }
        }

        #endregion
    }
}
