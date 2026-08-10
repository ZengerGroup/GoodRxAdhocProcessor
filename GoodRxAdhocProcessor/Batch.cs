using System;
using System.Collections.Generic;
using System.Text;

namespace GoodRxAdhocProcessor
{
    public class Batch
    {
        public string JobNumber;
        public string DescriptorJobCode;
        public List<int> RowNumbers;
        public bool Selected;
        public Batch(string jobNumber, string descriptorJobCode, int rowNumber)
        {
            Selected = false;
            JobNumber = jobNumber;
            DescriptorJobCode = descriptorJobCode;
            RowNumbers = new List<int>();
            RowNumbers.Add(rowNumber);
        }
        public Batch(bool empty)
        {
            Selected = false;
            JobNumber = "No Batches Found";
            DescriptorJobCode = "_";
            RowNumbers = new List<int>();
        }
    }
}
