using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppBantayBarangay.Models
{
    /// <summary>
    /// Custom JSON converter for ReportStatus that handles null values
    /// by defaulting to Pending status
    /// </summary>
    public class ReportStatusConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ReportStatus) || objectType == typeof(ReportStatus?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                // Handle null token
                if (reader.TokenType == JsonToken.Null)
                {
                    System.Diagnostics.Debug.WriteLine("[ReportStatus] Null token detected, defaulting to Pending");
                    return ReportStatus.Pending;
                }

                // Handle undefined/missing
                if (reader.TokenType == JsonToken.Undefined)
                {
                    System.Diagnostics.Debug.WriteLine("[ReportStatus] Undefined token detected, defaulting to Pending");
                    return ReportStatus.Pending;
                }

                // Get the value
                var value = reader.Value?.ToString();

                // If null or empty string, return Pending
                if (string.IsNullOrWhiteSpace(value))
                {
                    System.Diagnostics.Debug.WriteLine("[ReportStatus] Empty/null value detected, defaulting to Pending");
                    return ReportStatus.Pending;
                }

                // Try to parse the enum value
                if (Enum.TryParse<ReportStatus>(value, true, out var status))
                {
                    System.Diagnostics.Debug.WriteLine($"[ReportStatus] Parsed '{value}' to {status}");
                    return status;
                }

                // If parsing fails, default to Pending
                System.Diagnostics.Debug.WriteLine($"[ReportStatus] Failed to parse '{value}', defaulting to Pending");
                return ReportStatus.Pending;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ReportStatus] Exception: {ex.Message}, defaulting to Pending");
                return ReportStatus.Pending;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("Pending");
            }
            else
            {
                writer.WriteValue(value.ToString());
            }
        }
    }
}
