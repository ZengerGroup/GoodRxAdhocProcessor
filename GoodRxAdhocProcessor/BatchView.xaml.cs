namespace GoodRxAdhocProcessor;

public partial class BatchView : ContentView
{
	int Index;
	Batch TargetBatch;
	public BatchView(Batch batch, int index)
	{
		InitializeComponent();
		TargetBatch = batch;
		Index = index;
		JobNumberData.Text = TargetBatch.JobNumber;
		JobCodeData.Text = TargetBatch.DescriptorJobCode;
		RowCountData.Text = TargetBatch.RowNumbers.Count.ToString();
	}

    private void Selected_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
		TargetBatch.Selected = e.Value;
    }
}