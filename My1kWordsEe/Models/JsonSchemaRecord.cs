using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Unicode;

namespace My1kWordsEe.Models
{
    public record JsonSchemaRecord
    {
        public JsonNode JsonNode { get; init; }

        public string String { get; init; }

        public BinaryData BinaryData { get; init; }

        private JsonSchemaRecord(JsonNode jsonNode)
        {
            this.JsonNode = jsonNode;
            this.String = jsonNode.ToJsonString(new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = false
            });
            this.BinaryData = BinaryData.FromString(this.String);
        }

        public override string ToString()
        {
            return this.String;
        }

        private static readonly ConcurrentDictionary<Type, JsonSchemaRecord> Cache = new();

        /// <summary>
        /// see: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/extract-schema#transform-the-generated-schema
        /// </summary>
        private static JsonSchemaExporterOptions ExporterOptions = new()
        {
            TransformSchemaNode = (context, schema) =>
            {
                // Determine if a type or property and extract the relevant attribute provider.
                ICustomAttributeProvider? attributeProvider = context.PropertyInfo is not null
                    ? context.PropertyInfo.AttributeProvider
                    : context.TypeInfo.Type;

                // Look up any description attributes.
                DescriptionAttribute? descriptionAttr = attributeProvider?
                    .GetCustomAttributes(inherit: true)
                    .Select(attr => attr as DescriptionAttribute)
                    .FirstOrDefault(attr => attr is not null);

                // Apply description attribute to the generated schema.
                if (descriptionAttr != null)
                {
                    if (schema is not JsonObject jObj)
                    {
                        // Handle the case where the schema is a Boolean.
                        JsonValueKind valueKind = schema.GetValueKind();
                        Debug.Assert(valueKind is JsonValueKind.True or JsonValueKind.False);
                        schema = jObj = new JsonObject();
                        if (valueKind is JsonValueKind.False)
                        {
                            jObj.Add("not", true);
                        }
                    }

                    jObj.Insert(0, "description", descriptionAttr.Description);
                }

                return schema;
            }
        };

        public static JsonSchemaRecord For(Type type)
        {
            return Cache.GetOrAdd(type, (type) =>
            {
                JsonSerializerOptions options = new(JsonSerializerOptions.Default)
                {
                    MaxDepth = 12,
                    WriteIndented = false,
                    //RespectNullableAnnotations = true
                };
                JsonNode schema = options.GetJsonSchemaAsNode(type, ExporterOptions);

                return new JsonSchemaRecord(schema);
            });
        }
    }
}
