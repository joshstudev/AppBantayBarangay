using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace BantayBarangayApp
{
    public partial class ResolvedReportsPage : ContentPage
    {
        private ObservableCollection<Report> ResolvedReports { get; set; }

        public ResolvedReportsPage()
        {
            InitializeComponent();
            ResolvedReports = new ObservableCollection<Report>(App.Reports.Where(r => r.Status == "Resolved"));
            ResolvedReportsListView.ItemsSource = ResolvedReports;
        }

        private async void OnViewDetailsClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var report = button?.CommandParameter as Report;
            if (report != null)
            {
                await DisplayAlert("Report Details", 
                    $"Title: {report.Title}\n" +
                    $"Description: {report.Description}\n" +
                    $"Location: {report.Location}\n" +
                    $"Category: {report.Category}\n" +
                    $"Date: {report.DateReported}\n" +
                    $"Reported By: {report.ReportedBy}", 
                    "OK");
            }
        }
    }
}