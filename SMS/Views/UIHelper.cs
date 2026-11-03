using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace CodeflexSystems.SMS.Views
{
    internal static class UIHelper
    {
        [Conditional("DEBUG")]
        public static void AddStateColumns(DataGrid dataGrid)
        {
            var col = dataGrid.Columns.LastOrDefault();

            if (col == null || col.Header.ToString() != "Overall State")
            {
                dataGrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Old State",
                    Binding = new Binding("OldObjectState")
                });
                dataGrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "State",
                    Binding = new Binding("ObjectState")
                });
                dataGrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Old OAS",
                    Binding = new Binding("OldOverallState")
                });
                dataGrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Overall State",
                    Binding = new Binding("OverallState")
                });
            }
            else
            {
                int index = dataGrid.Columns.Count - 1;

                if (index < 0)
                    index = 0;
                dataGrid.Columns.Insert(index, new DataGridTextColumn()
                {
                    Header = "Old State",
                    Binding = new Binding("OldObjectState")
                });
                ++index;
                dataGrid.Columns.Insert(index, new DataGridTextColumn()
                {
                    Header = "State",
                    Binding = new Binding("ObjectState")
                });
                ++index;
                dataGrid.Columns.Insert(index, new DataGridTextColumn()
                {
                    Header = "Old OAS",
                    Binding = new Binding("OldOverallState")
                });
            }
        }
    }
}
