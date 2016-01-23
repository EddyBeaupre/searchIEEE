using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace searchIEEE
{
    public partial class ieeeResult : Form
    {
        private DataTable dataTable;
        private DataRow[] dataRows;

        public delegate void CallbackEventHandler(Boolean? Status);
        CallbackEventHandler callbackEventHandler = null;

        public ieeeResult(DataTable dataTable)
        {
            try
            {
                InitializeComponent();
                this.dataTable = dataTable;
                this.dataRows = null;
            }
            catch
            {
                if (callbackEventHandler != null)
                    callbackEventHandler(null);
            }
        }

        public ieeeResult(DataTable dataTable, CallbackEventHandler callbackEventHandler)
        {
            try
            {
                InitializeComponent();
                this.dataTable = dataTable;
                this.dataRows = null;
                this.callbackEventHandler = callbackEventHandler;
            }
            catch
            {
                if (callbackEventHandler != null)
                    callbackEventHandler(null);
            }
        }

        public ieeeResult(DataRow[] dataRows)
        {
            try
            {
                InitializeComponent();
                this.dataRows = dataRows;
                this.dataTable = null;
            }
            catch
            {
                if (callbackEventHandler != null)
                    callbackEventHandler(null);
            }

        }

        public ieeeResult(DataRow[] dataRows, CallbackEventHandler callbackEventHandler)
        {
            try
            {
                InitializeComponent();
                this.dataRows = dataRows;
                this.dataTable = null;
                this.callbackEventHandler = callbackEventHandler;
            }
            catch
            {
                if (callbackEventHandler != null)
                    callbackEventHandler(null);
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
                if (callbackEventHandler != null)
                    callbackEventHandler(null);
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
                if (this.dataTable != null)
                {
                    ouiDatabaseView.DataSource = dataTable;
                    setupDatabaseView();
                }

                else if (this.dataRows != null)

                {
                    ouiDatabaseView.ColumnCount = 5;
                    setupDatabaseView();

                    foreach (DataRow dataRow in dataRows)
                    {
                        ouiDatabaseView.Rows.Add(dataRow[0], dataRow[1], dataRow[2], dataRow[3], dataRow[4]);
                    }
                }
                else
                {
                    if (callbackEventHandler != null)
                        callbackEventHandler(false);
                }

                ouiDatabaseView.Sort(ouiDatabaseView.Columns[1], ListSortDirection.Ascending);
                ouiDatabaseView.Visible = true;

                if (callbackEventHandler != null)
                    callbackEventHandler(true);
            }
            catch
            {
                if (callbackEventHandler != null)
                    callbackEventHandler(null);
            }
        }
    }
}
