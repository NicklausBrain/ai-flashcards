using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;

using My1kWordsEe.Models.Grammar;

namespace My1kWordsEe.Models.Semantics
{
    /// <summary>
    /// A word in the Estonian language with respective senses and forms.
    /// </summary>
    public record EtWord
    {
        public required string Value { get; init; }

        public LanguageCode Language => LanguageCode.Et;

        public WordSense[] Senses { get; init; } = Array.Empty<WordSense>();

        /// <summary>
        /// Sample pronunciation of the word.
        /// </summary>
        public Uri? AudioUrl { get; init; } // todo: use relative url

        [JsonIgnore]
        public WordSense DefaultSense => Senses[0];

        public static Lazy<string> SensesJsonSchema = new Lazy<string>(() =>
        {
            JsonSerializerOptions options = new(JsonSerializerOptions.Default)
            {
                MaxDepth = 12,
            };
            JsonNode schema = options.GetJsonSchemaAsNode(typeof(WordSense[]), exporterOptions);
            return schema.ToString();
        });

        private static JsonSchemaExporterOptions exporterOptions = new()
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
    }
}