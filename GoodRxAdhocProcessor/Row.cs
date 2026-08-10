using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;
using ClosedXML.Excel;

namespace GoodRxAdhocProcessor
{
    public class Row
    {
        int PackIdCount;
        public OrderedDictionary<string, string> RowData;

        public Row(IXLRow row)
        {
            RowData = new OrderedDictionary<string, string>();
            PackIdCount = GetPackIdCount(row);
            ConsumeRowData(row);
        }
        public Row(IXLRow row, string packId, string memberIdStart, string memberIdEnd)
        {
            RowData = new OrderedDictionary<string, string>();
            PackIdCount = GetPackIdCount(row);
            ConsumeRowData(row);
            RowData["packid1"] = packId;
            RowData["memberidstart1"] = memberIdStart;
            RowData["memberidend1"] = memberIdEnd;
        }
        private void ConsumeRowData(IXLRow row)
        {
            RowData.Add("filename", "Adhocs");
            RowData.Add("name_prefix","");
            RowData.Add("first_name", "");
            RowData.Add("middle_name", "");
            RowData.Add("last_name", "");
            RowData.Add("suffix", "");
            RowData.Add("full_name", row.Cell("H").Value.ToString());
            RowData.Add("title", "");
            RowData.Add("company", row.Cell("I").Value.ToString());
            RowData.Add("alternate_1_address", row.Cell("K").Value.ToString());
            RowData.Add("delivery_address", row.Cell("J").Value.ToString());
            RowData.Add("city", row.Cell("L").Value.ToString());
            RowData.Add("state", row.Cell("M").Value.ToString());
            RowData.Add("zip_4", row.Cell("N").Value.ToString());
            RowData.Add("country", "");
            RowData.Add("original_alternate_1_address", "");
            RowData.Add("original_delivery_address", "");
            RowData.Add("original_city", "");
            RowData.Add("original_state", "");
            RowData.Add("original_zip", "");
            RowData.Add("inputfile", "");
            RowData.Add("return_code", "");
            RowData.Add("coa_move_type", "");
            RowData.Add("coa_move_date", "");
            RowData.Add("npis", row.Cell("BF").Value.ToString());
            RowData.Add("claims", "");
            RowData.Add("utilizers", "");
            RowData.Add("savings", "");
            RowData.Add("boxstyle", "");
            RowData.Add("phone", "");
            RowData.Add("emailable", row.Cell("P").Value.ToString());
            RowData.Add("presort_sequence", "");
            RowData.Add("jobid", row.Cell("BG").Value.ToString());
            RowData.Add("basenetwork", "");
            RowData.Add("basebrand", "");
            RowData.Add("pcn", row.Cell("AD").Value.ToString());
            RowData.Add("bin", row.Cell("AC").Value.ToString());
            RowData.Add("group_id", row.Cell("V").Value.ToString());
            RowData.Add("packid1", row.Cell("X").Value.ToString());
            RowData.Add("memberidstart1", row.Cell("Z").Value.ToString());
            RowData.Add("memberidend1", row.Cell("AA").Value.ToString());
            RowData.Add("packid2", "");
            RowData.Add("memberidstart2", "");
            RowData.Add("memberidend2", "");
            RowData.Add("mail_status", "");
            RowData.Add("group2", "");
            RowData.Add("npi2", "");
            RowData.Add("card", "");
            RowData.Add("jobdescription", "");
            RowData.Add("delivery_date", row.Cell("U").Value.ToString());
            RowData.Add("impb_digits", "");
            RowData.Add("savings_line", "");
            RowData.Add("initial_kit_type", GetInitialKitType(row.Cell("AE").Value.ToString(), row.Cell("AF").Value.ToString()));
            RowData.Add("misc2", "");
            RowData.Add("drop_number", "");
            RowData.Add("impb_human_readable", "");
            RowData.Add("misc1", "");
            RowData.Add("matched_type", "");
            RowData.Add("matched_date", "");
            RowData.Add("all_in_cpp", GetAllInCpp(row.Cell("T").Value.ToString()));
            RowData.Add("job_code", row.Cell("G").Value.ToString());
            RowData.Add("mail_date", row.Cell("U").Value.ToString());
            RowData.Add("bin_2", "");
            RowData.Add("pcn_2", "");
            RowData.Add("pack_id_3", "");
            RowData.Add("bin_3", "");
            RowData.Add("pcn_3", "");
            RowData.Add("group_3", "");
            RowData.Add("member_id_start_3", "");
            RowData.Add("member_id_end_3", "");
            RowData.Add("pack_id_4", "");
            RowData.Add("bin_4", "");
            RowData.Add("pcn_4", "");
            RowData.Add("group_4", "");
            RowData.Add("member_id_start_4", "");
            RowData.Add("member_id_end_4", "");
            RowData.Add("stream_type", "");
            RowData.Add("pull_criteria", "");
            RowData.Add("test_pair", "");
            RowData.Add("test_vs_control_kit_type", "");
            RowData.Add("test_vs_control_list_type", "");
            RowData.Add("original_source_job_code", "");
        }
        private string GetInitialKitType(string kitType, string tab)
        {
            try
            {
                if (char.IsDigit(kitType[0])) return tab;
                else return kitType;
            }
            catch { return kitType; }
        }
        private string GetAllInCpp(string allInCpp)
        {
            double cost;
            if (double.TryParse(allInCpp, out cost))
            {
                cost = cost / (PackIdCount + 1);
                if (cost == 0) return allInCpp;
                else return cost.ToString();
            }
            else return allInCpp;
        }
        private int GetPackIdCount(IXLRow row)
        {
            int start = Int32.Parse(row.Cell("X").Value.ToString().Substring(1));
            int end = Int32.Parse(row.Cell("Y").Value.ToString().Substring(1));
            return end - start;
        }
    }
}
