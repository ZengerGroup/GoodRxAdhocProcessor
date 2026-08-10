using System;
using System.Collections.Generic;
using System.Text;

namespace GoodRxAdhocProcessor
{
    public class CsvBuilder
    {
        string Header;
        string OutPath;
        List<Row> Rows;
        public CsvBuilder(List<Row> rows, string workingPath)
        {
            Rows = rows;
            Header = BuildHeader();
            OutPath = GetOutPath(workingPath);
            FillSheet();
        }
        private string BuildHeader()
        {
            return String.Format("\"{0}\"", string.Join("\",\"", Rows[0].RowData.Keys));
        }
        private string GetOutPath(string workingPath)
        {
            return Path.Combine(workingPath, String.Format("doctor-mail-backfeed-adhoc_{0}_{1}_v1-0-1.csv", 
                Rows[0].RowData["job_code"], DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")));
        }
        private void FillSheet()
        {
            StreamWriter sWriter = new StreamWriter(OutPath);
            sWriter.WriteLine(Header);
            for(int i = 0; i < Rows.Count; i++)
            {
                sWriter.WriteLine(String.Format("\"{0}\"", string.Join("\",\"", Rows[i].RowData.Values)));
            }
            sWriter.Close();
        }
    }
}
