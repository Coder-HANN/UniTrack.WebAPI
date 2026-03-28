using System.Text.Json;
using System.Text.Json.Serialization;

namespace UniTrack.Application.Abstraction.Services.JsonConverter
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString()!;
            // dd.MM.yyyy ve yyyy-MM-dd her ikisini de destekle
            if (DateOnly.TryParseExact(str, "dd.MM.yyyy", out var date1)) return date1;
            if (DateOnly.TryParseExact(str, "yyyy-MM-dd", out var date2)) return date2;
            return DateOnly.Parse(str);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString("dd.MM.yyyy"));
    }
}
