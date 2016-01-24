using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace searchIEEE
{
    public partial class ieeeResult : Form
    {
        private List<IeeeRecord> dataResults = null;

        public ieeeResult(List<IeeeRecord> dataResults)
        {
            try
            {
                InitializeComponent();
                this.dataResults = dataResults;
            }
            catch
            {
                this.Close();
            }
        }

        private void setupDatabaseView()
        {
            try
            {
                ouiDatabaseView.ColumnHeadersDefaultCellStyle.Font = new Font(ouiDatabaseView.Font, FontStyle.Bold);
                ouiDatabaseView.Columns[0].HeaderText = "ID";
                ouiDatabaseView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                ouiDatabaseView.Columns[1].HeaderText = "Assignment";
                ouiDatabaseView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                ouiDatabaseView.Columns[1].DefaultCellStyle.Format = "MAC";
                ouiDatabaseView.Columns[1].DefaultCellStyle.FormatProvider = new formatOID();
                ouiDatabaseView.Columns[2].HeaderText = "Registry";
                ouiDatabaseView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                ouiDatabaseView.Columns[3].HeaderText = "Organization Name";
                ouiDatabaseView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                ouiDatabaseView.Columns[4].HeaderText = "Organization Address";
                ouiDatabaseView.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                ouiDatabaseView.MultiSelect = false;
                ouiDatabaseView.ReadOnly = true;
                ouiDatabaseView.CellFormatting += ouiDatabaseView_CellFormatting;
            }
            catch
            {
                this.Close();
            }
        }

        private void ouiDatabaseView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.CellStyle.FormatProvider is ICustomFormatter)
                {
                    e.Value = (e.CellStyle.FormatProvider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter).Format(e.CellStyle.Format, e.Value, e.CellStyle.FormatProvider);
                    e.FormattingApplied = true;
                }
            }
            catch
            {
                e.FormattingApplied = false;
            }
        }

        private void ieeeResult_Shown(object sender, EventArgs e)
        {
            try
            {
                ouiDatabaseView.Visible = false;
                ouiDatabaseView.ColumnCount = 5;
                setupDatabaseView();

                if (this.dataResults != null)
                {
                    Int64 Count = 0;

                    

                    foreach (IeeeRecord dataRow in this.dataResults)
                    {
                        ouiDatabaseView.Rows.Add(Count++, dataRow.Oid64, dataRow.Registry, dataRow.OrganizationName, dataRow.OrganizationAddress);
                    }
                }
                ouiDatabaseView.Visible = true;
            }
            catch
            {
                this.Close();
            }
        }

        private void ieeeResult_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.dataResults != null)
            {
                ouiDatabaseView.DataSource = null;
                ouiDatabaseView.Dispose();
                
                this.dataResults.Clear();
                this.dataResults = null;
            }
        }
    }

    public class formatOID : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            try
            {
                if (formatType == typeof(ICustomFormatter))
                    return (this);
                else
                    return (null);
            }
            catch
            {
                return (null);
            }
        }

        public static string toString(UInt64 OID)
        {
            try
            {
                int Count = 0;
                string output = string.Empty;

                foreach (Char c in OID.ToString("X12").PadRight(12, '0'))
                {
                    if (((Count % 2) == 0) && (Count > 0))
                    {
                        output += ":";
                    }
                    output += c;
                    Count++;
                }
                return (output);
            }
            catch
            {
                return (null);
            }
        }

        public static String cleanOID(String OID)
        {
            return (new Regex("[^a-fA-F0-9]").Replace(OID, "").ToUpper().PadRight(12, '0'));
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            try
            {
                if (!formatProvider.Equals(this)) return (null);

                if (!format.StartsWith("MAC")) return (null);

                if (arg is UInt64)
                {
                    return (toString((UInt64)arg));
                }

                else
                {
                    return (null);
                }

            }
            catch
            {
                return (null);
            }
        }
    }
}
