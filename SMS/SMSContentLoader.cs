using CodeflexSystems.SmartUI.Windows;
using CodeflexSystems.SmartUI.Windows.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeflexSystems.SMS
{
    internal sealed class SMSContentLoader : DefaultContentLoader
    {
        public List<Uri> MenuUris { get; set; }

        protected override object LoadContent(Uri uri)
        {
            // don't do anything in design mode
            if (ModernUIHelper.IsInDesignMode || App.Current == null)
                return null;

            if (this.MenuUris == null)
            {
                var sv = App.Current.MainWindow as Views.ShellView;

                if (sv != null)
                {
                    this.MenuUris = sv.MenuLinkGroups.SelectMany(gm => gm.Links.Select(lm => lm.Source)).ToList();
                    this.MenuUris.AddRange(sv.TitleLinks.Select(m => m.Source));
                }
            }

            if (this.MenuUris != null && this.MenuUris.Contains(uri))
                return Application.LoadComponent(uri);
            
            return null;
        }

        public void Reset()
        {
            if (this.MenuUris != null)
                this.MenuUris.Clear();
            this.MenuUris = null;
        }
    }
}
