#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Orleans.Runtime;

namespace Orleans.Streaming.JsonConverters
{
    internal sealed class QualifiedStreamIdJsonConverter : JsonConverter<QualifiedStreamId>
    {
        private readonly string? _qualifiedStreamIdType = typeof(QualifiedStreamId).AssemblyQualifiedName;

        public override QualifiedStreamId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                return default;
            }

            string? providerName = null;
            StreamId streamId = default;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();

                    reader.Read();

                    switch (propertyName)
                    {
                        case "pvn":
                            providerName = reader.GetString();
                            break;
                        case "sid":
                            streamId = JsonSerializer.Deserialize<StreamId>(ref reader, options);                            
                            break;
                    }
                }
            }

            if (providerName is null || streamId == default)
            {
                return default;
            }

            return new QualifiedStreamId(providerName, streamId);
        }

        public override void Write(Utf8JsonWriter writer, QualifiedStreamId value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("$type", _qualifiedStreamIdType);
            writer.WriteString("pvn", value.ProviderName);
            writer.WritePropertyName("sid");
            JsonSerializer.Serialize(writer, value.StreamId, options);            
            writer.WriteEndObject();
        }
    }
}
