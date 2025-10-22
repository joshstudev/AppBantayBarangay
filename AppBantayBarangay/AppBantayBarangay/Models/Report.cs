using System;
using Newtonsoft.Json;

namespace AppBantayBarangay.Models
{
    public class Report
    {
        public string ReportId { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }  // Base64 image data
        
        // Make nullable to handle old reports without coordinates
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        public string LocationAddress { get; set; }
        public string DateReported { get; set; }
        
        // Use custom converter that handles null values
        [JsonConverter(typeof(ReportStatusConverter))]
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        
        public string ReportedBy { get; set; }  // User ID
        public string ReporterName { get; set; }  // User's full name
        public string ReporterEmail { get; set; }
        public string ResolvedBy { get; set; }
        public string DateResolved { get; set; }
        public string ResolutionNotes { get; set; }
        
        // Helper property for display
        public string StatusText => Status.ToString();
        public string ShortDescription => Description?.Length > 100 ? Description.Substring(0, 100) + "..." : Description;
    }

    public enum ReportStatus
    {
        Pending,
        InProgress,
        Resolved,
        Rejected
    }
}
