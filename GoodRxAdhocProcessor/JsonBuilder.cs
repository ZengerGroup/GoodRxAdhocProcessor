using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GoodRxAdhocProcessor
{
    public class JsonBuilder
    {
        string OutPath;
        List<Row> Rows;
        public JsonBuilder(List<Row> rows, string workingPath, string copyPath)
        {
            Rows = rows;
            OutPath = GetOutPath(workingPath);
            FillFile();
            Logger.WriteLog("Copying JSON file.", false);
            CopyFile(copyPath);
        }
        private string GetOutPath(string workingPath)
        {
            return Path.Combine(workingPath, String.Format("doctor-mail-backfeed-adhoc_{0}_{1}_v1-0-1.json",
                Rows[0].RowData["job_code"], DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")));
        }
        private void FillFile()
        {
            StreamWriter sWriter = new StreamWriter(OutPath);
            sWriter.WriteLine("[");
            for (int i = 0; i < Rows.Count; i++) 
            {
                if (i == Rows.Count - 1) sWriter.WriteLine(JsonSerializer.Serialize(Rows[i].RowData));
                else sWriter.WriteLine(String.Format("{0},", JsonSerializer.Serialize(Rows[i].RowData)));
            }
            sWriter.WriteLine("]");
            sWriter.Close();
        }
        private void CopyFile(string copyPath)
        {
            File.Copy(OutPath, Path.Combine(copyPath, Path.GetFileName(OutPath)));
        }
    }
}
