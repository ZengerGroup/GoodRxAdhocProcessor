using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoodRxAdhocProcessor
{
    public class ProcessHandler
    {
        AppSettings _settings;
        List<Row> BatchData;
        public ProcessHandler(IConfiguration configuration)
        {
            _settings = configuration.GetSection("Settings").Get<AppSettings>() ?? new AppSettings();
        }
        public bool BeginProcessing(string workingPath, WorkbookReader reader)
        {
            try
            {
                Logger.WriteLog("Beginning processing.", true);
                BatchData = reader.GetBatchData();
                Logger.WriteLog("Writing CSV file.", false);
                CsvBuilder csvBuilder = new CsvBuilder(BatchData, workingPath);
                Logger.WriteLog("Writing JSON file.", false);
                JsonBuilder jsonBuilder = new JsonBuilder(BatchData, workingPath, _settings.FtpPath);
                Logger.WriteLog("Updating and saving Adhoc workbook.", false);
                reader.MarkExported();
                reader.Close();
                return true;
            }
            catch { return false; }
            
        }
    }
}
