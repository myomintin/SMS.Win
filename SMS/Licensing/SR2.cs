using System.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CodeflexSystems.SMS.Licensing
{
    internal static class SR2
    {
        #region Fields

        private const string unavailable = "Unavailable";
        private static readonly ResourceSet rs;

        #endregion

        #region Ctor, Dtor

        static SR2()
        {
            rs = new ResourceSet(ResourceHelper.GetResourceStream());
        }

        #endregion

        #region Methods

        internal static object GetObject(string id)
        {
            return rs.GetObject(id);
        }

        internal static string GetString(string id)
        {
            string text = rs.GetString(id);

            if (text == null)
            {
                text = unavailable;
                System.Diagnostics.Debug.WriteLine(unavailable);
            }

            return text;
        }

        internal static string GetString(string id, params object[] args)
        {
            string text = rs.GetString(id);

            if (text == null)
            {
                text = unavailable;
                System.Diagnostics.Debug.WriteLine(unavailable);
            }
            else if (args != null && args.Length > 0)
                text = string.Format(CultureInfo.CurrentCulture, text, args);

            return text;
        }

        #endregion
    }
}
