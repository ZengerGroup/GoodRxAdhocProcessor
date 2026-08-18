using ClosedXML;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GoodRxAdhocProcessor
{
    public class WorkbookReader
    {
        XLWorkbook Workbook;
        IXLWorksheet Worksheet;
        private AppSettings _settings;
        int SelectedIndex;
        public Batch[] Batches;
        public WorkbookReader(IConfiguration configuration)
        {
            _settings = configuration.GetSection("Settings").Get<AppSettings>() ?? new AppSettings();
            Workbook = new XLWorkbook(_settings.AdhocPath);
            Worksheet = Workbook.Worksheet(_settings.SheetName);
            Batches = GetRanges();
        }
        private Batch[] GetRanges()
        {
            List<Batch> batches = new List<Batch>();
            try
            {
                int lastRow = Worksheet.LastRow().RowNumber();
                for(int i = 2; i < lastRow; i++)
                {
                    string jobNumber = Worksheet.Cell(i, "D").Value.ToString();
                    string descriptorJobCode = Worksheet.Cell(i, "G").Value.ToString();
                    bool exists = false;
                    if (jobNumber == String.Empty && (descriptorJobCode == String.Empty || descriptorJobCode.ToLower().Contains("base"))) continue;
                    if (Worksheet.Cell(i, "AJ").Value.ToString() == "XXX") continue;
                    for (int ii = 0; ii < batches.Count; ii++)
                    {
                        if (batches[ii].JobNumber == jobNumber && batches[ii].DescriptorJobCode == descriptorJobCode)
                        {
                            exists = true;
                            batches[ii].RowNumbers.Add(i);
                            break;
                        }
                    }
                    if(!exists) batches.Add(new Batch(jobNumber, descriptorJobCode, i));
                }
                return batches.ToArray();
            }
            catch { return new Batch[0]; }
        }
        public List<Row> GetBatchData()
        {
            List<Row> rows = new List<Row>();
            for (int i = 0; i < Batches.Length; i++) if (Batches[i].Selected) SelectedIndex = i;
            for (int i = 0; i < Batches[SelectedIndex].RowNumbers.Count; i++)
            {
                IXLRow row = Worksheet.Row(Batches[SelectedIndex].RowNumbers[i]);
                int[] packIdRange = new int[2]
                {
                    Int32.Parse(row.Cell("X").Value.ToString().Substring(1)),
                    Int32.Parse(row.Cell("Y").Value.ToString().Substring(1))
                };
                if (packIdRange[0] == 1) rows.Add(new Row(row, "", row.Cell("Z").Value.ToString(), row.Cell("AA").Value.ToString()));
                else if (packIdRange[0] == packIdRange[1]) rows.Add(new Row(row));
                else 
                {
                    int iteration = 0;
                    for (int packIdIndex = packIdRange[0]; packIdIndex < packIdRange[1]; packIdIndex++)
                    {
                        string[] memberIdRange = GetMemberIdRange(row.Cell("Z").Value.ToString(), iteration);
                        rows.Add(new Row(row, String.Format("T{0}", packIdIndex), memberIdRange[0], memberIdRange[1]));
                        iteration++;
                    }
                }
                    
            }
            return rows;
        }
        private string[] GetMemberIdRange(string startId, int iteration)
        {
            string prefix = Regex.Replace(startId, "[0-9]*", "");
            int startInt = Int32.Parse(Regex.Replace(startId, "[A-Za-z]*", ""));
            startInt = startInt + (100 * iteration);
            int endInt = startInt + 99;
            return [String.Format("{0}{1}", prefix, startInt), String.Format("{0}{1}", prefix, endInt)];
        }
        public void MarkExported()
        {
            for(int i = 0; i < Batches[SelectedIndex].RowNumbers.Count; i++)
            {
                Worksheet.Unprotect();
                Worksheet.Cell(Batches[SelectedIndex].RowNumbers[i], "AJ").SetValue("XXX");
            }
        }
        public void Close()
        {
            Workbook.Save();
            Workbook.Dispose();
        }
        public bool UpdatePricing()
        {
            try 
            {
                Worksheet.Unprotect();
                IXLWorksheet pricingSheet = Workbook.Worksheet(_settings.PricingSheet);
                for(int i = 0; i < Batches.Length; i++) if (Batches[i].Selected) UpdateBatchPricing(Batches[i], pricingSheet);
                Worksheet.Protect();
                return true; 
            }
            catch (Exception e){ Logger.WriteLog(e.Message, false); return false; }
        }
        public void UpdateBatchPricing(Batch batch, IXLWorksheet pricingSheet)
        {
            for(int i = 0; i < batch.RowNumbers.Count; i++)
            {
                string kitType = Worksheet.Cell(batch.RowNumbers[i], "AE").Value.ToString();
                string handlingCost = "", totalCost = "";
                foreach(IXLRow row in pricingSheet.Rows())
                {
                    if (row.Cell("A").Value.ToString() == kitType)
                    {
                        handlingCost = row.Cell("C").Value.ToString();
                        totalCost = row.Cell("B").Value.ToString();
                    }
                }
                Worksheet.Cell(batch.RowNumbers[i], "R").SetValue(Double.Parse(handlingCost));
                Worksheet.Cell(batch.RowNumbers[i], "S").SetValue(Double.Parse(totalCost));
                Worksheet.Cell(batch.RowNumbers[i], "T").SetValue(Double.Parse(handlingCost) + Double.Parse(totalCost) + Double.Parse(Worksheet.Cell(batch.RowNumbers[i], "Q").Value.ToString()));
            }
        }
    }
}
