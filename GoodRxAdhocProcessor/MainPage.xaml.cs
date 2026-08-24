using Microsoft.Extensions.Configuration;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;


namespace GoodRxAdhocProcessor
{
    public partial class MainPage : ContentPage
    {
        WorkbookReader Reader;
        ProcessHandler Handler;
        private readonly AppSettings _settings;
        public MainPage(IConfiguration configuration)
        {
            InitializeComponent();
            _settings = configuration.GetSection("Settings").Get<AppSettings>() ?? new AppSettings();
            Logger.InitializeLogger(configuration);
            try
            {
                Reader = new WorkbookReader(configuration);
                Handler = new ProcessHandler(configuration);
                UpdateDisplay();
            }
            catch { DisplayWorkbookError(File.Exists(configuration.GetSection("settings").Get<AppSettings>().AdhocPath)); }
            
        }
        private void UpdateDisplay()
        {
            if (Reader.Batches.Length == 0) BatchViews.Children.Add(new BatchView(new Batch(false), 0));
            for (int i = 0; i < Reader.Batches.Length; i++) BatchViews.Children.Add(new BatchView(Reader.Batches[i], i));
        }

        private async void FolderPicker_Clicked(object sender, EventArgs e)
        {
            string defaultPath = Path.Combine(_settings.DefaultPath, DateTime.Now.ToString("yyyy"));
            var result = await CommunityToolkit.Maui.Storage.FolderPicker.PickAsync(defaultPath, new CancellationToken());
            if(result.IsSuccessful) SelectedFolderData.Text = result.Folder.Path;
        }

        private async void ProcessButton_Clicked(object sender, EventArgs e)
        {
            if (SelectedFolderData.Text != null)
            {
                List<int> selectedIndices = new List<int>();
                for (int i = 0; i < Reader.Batches.Length; i++) if (Reader.Batches[i].Selected) selectedIndices.Add(i);
                if (selectedIndices.Count == 0) await DisplayAlertAsync("No batch selected.", "Please select an available batch and try again.", "Okay");
                else if (selectedIndices.Count > 1) await DisplayAlertAsync("Too many batches selected.", "Please select only one batch, then try again.", "Okay");
                else
                {
                    if (Reader.UpdatePricing())
                    {
                        if (Handler.BeginProcessing(SelectedFolderData.Text, Reader)) await DisplayAlertAsync("Processing complete", "You may exit the program.", "Okay");
                    }
                    else await DisplayAlertAsync("Workbook Error", "Failed to update pricing, pleace check adhoc spreadsheet.", "Okay");   
                }
            }
            else await DisplayAlertAsync("No folder selected.", "Please select a working folder and try again.", "Okay");
        }
        private async void DisplayWorkbookError(bool adhocSheetExists)
        {
            if (!adhocSheetExists) await DisplayAlertAsync("Workbook Error", "Unable to find Excel workbook. Check the path and try again.", "Okay");
            else await DisplayAlertAsync("Workbook Error", "Unable to open Excel workbook. Ensure it is not open, then try again.", "Okay");
            Application.Current?.Quit();
        }
    }
}
