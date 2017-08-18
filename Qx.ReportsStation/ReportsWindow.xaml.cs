using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Qx.Common;
using System.Collections;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace Qx.ReportsStation
{
    /// <summary>
    /// Interaction logic for ReportsWindow.xaml
    /// </summary>
    public partial class ReportsWindow : System.Windows.Window
    {
        private class UserUsage
        {
            public string FirstName { set; get; }
            public string LastName { set; get; }
            public string UserName { set; get; }
            public string License { set; get; }
            public string PID { set; get; }
            public string EmployeeId { set; get; }
            public string Profession { set; get; }
            public string UsageType { set; get; }
            public long Sum { set; get; }
            public string IsDeleted { set; get; }
        }

        //private DBConnect db;

        public ReportsWindow()
        {
            InitializeComponent();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            //var allHistory = RemoteObjectProvider.GetHistoryAccess().


            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("יש לבחור תאריך תחילה ותאריך סיום וללחוץ על הצג");
                return;
            }

            if (EndDatePicker.SelectedDate.Value < StartDatePicker.SelectedDate.Value)
            {
                MessageBox.Show("תאריך הסיום חייב להיות גדול או שווה מתאריך ההתחלה");
                return;
            }

            var result = RemoteObjectProvider.GetHistoryAccess().UsageReport(StartDatePicker.SelectedDate.Value, EndDatePicker.SelectedDate.Value);

            var list = new List<UserUsage>();
            foreach (var item in (IEnumerable)result)
            {
                var row = item as object[];
                list.Add(new UserUsage { 
                    FirstName = (string)row[0], 
                    LastName = (string)row[1], 
                    UserName = (string)row[2], 
                    License = (string)row[3], 
                    PID = (string)row[4], 
                    EmployeeId = (string)row[5], 
                    Profession = (string)row[6], 
                    IsDeleted = (string)row[7],
                    UsageType = (string)row[8], 
                    Sum = (long)row[9] });
            }

            ResultDataGrid.ItemsSource = list;
            ResultDataGrid.Items.Refresh();
        }

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var items = ResultDataGrid.ItemsSource as List<UserUsage>;
            if (items == null)
            {
                MessageBox.Show("אין מה לייצא");
                return;
            }

            var saveFileDialog = new System.Windows.Forms.SaveFileDialog() {Filter="Excel File(.xlsx)|*.xlsx"};
            if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return; 

            var xlApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook xlWorkBook = xlApp.Workbooks.Add();
            Worksheet xlWorkSheet = xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Cells[1, 1] = "שם משתמש";
            xlWorkSheet.Cells[1, 2] = "שם פרטי";
            xlWorkSheet.Cells[1, 3] = "שם משפחה";
            xlWorkSheet.Cells[1, 4] = "תעודת זהות";
            xlWorkSheet.Cells[1, 5] = "מספר רשיון";
            xlWorkSheet.Cells[1, 6] = "מספר עובד";
            xlWorkSheet.Cells[1, 7] = "מקצוע";
            xlWorkSheet.Cells[1, 8] = "סוג שימוש";
            xlWorkSheet.Cells[1, 9] = "כמות שימוש";

            for (int i = 2; i < items.Count; i++)
            {
                xlWorkSheet.Cells[i, 1] = items[i-2].UserName;
                xlWorkSheet.Cells[i, 2] = items[i-2].FirstName;
                xlWorkSheet.Cells[i, 3] = items[i-2].LastName;
                xlWorkSheet.Cells[i, 4] = items[i-2].PID;
                xlWorkSheet.Cells[i, 5] = items[i-2].License;
                xlWorkSheet.Cells[i, 6] = items[i-2].EmployeeId;
                xlWorkSheet.Cells[i, 7] = items[i-2].Profession;
                xlWorkSheet.Cells[i, 8] = items[i-2].UsageType;
                xlWorkSheet.Cells[i, 9] = items[i-2].Sum;
            }

            xlWorkBook.Close(true, saveFileDialog.FileName, null);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            

            //File.WriteAllLines("Test.xlsx", items.Select(item => item.ToExcelFormat()));
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
            }
            catch
            {

            }
            finally
            {
                obj = null;
                GC.Collect();
            }
        }
    }
}
