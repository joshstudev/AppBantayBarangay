using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppBantayBarangay.Models
{
    public class User
    {
        public string UserId { get; set; }  // Firebase UID
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }  // Don't store in database, only for registration
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        
        // Handle both number (0,1) and string ("Official","Resident") formats
        [JsonConverter(typeof(FlexibleUserTypeConverter))]
        public UserType UserType { get; set; }
        
        public string CreatedAt { get; set; }
        
        // Helper property for full name
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    }
    
    // Custom converter to handle both number and string UserType formats
    public class FlexibleUserTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UserType) || objectType == typeof(UserType?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.Null)
                {
                    return UserType.Resident; // Default
                }
                
                if (reader.TokenType == JsonToken.Integer)
                {
                    // Handle number format (0 = Official, 1 = Resident)
                    var value = Convert.ToInt32(reader.Value);
                    System.Diagnostics.Debug.WriteLine($"[UserType] Converting number {value} to enum");
                    return (UserType)value;
                }
                else if (reader.TokenType == JsonToken.String)
                {
                    // Handle string format ("Official" or "Resident")
                    var value = reader.Value.ToString();
                    System.Diagnostics.Debug.WriteLine($"[UserType] Converting string '{value}' to enum");
                    
                    if (Enum.TryParse<UserType>(value, true, out var result))
                    {
                        return result;
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"[UserType] Unknown format, using default Resident");
                return UserType.Resident; // Default
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UserType] Error: {ex.Message}, using default Resident");
                return UserType.Resident;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Always write as string for new/updated records
            writer.WriteValue(value.ToString());
        }
    }
}
