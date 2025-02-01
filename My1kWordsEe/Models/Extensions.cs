using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace My1kWordsEe.Models
{
    public static class Extensions
    {
        // The longest word in Estonian is 43 characters long
        public const int MaxWordLength = 43;

        private static readonly Regex UnicodeWordRegex = new(@"(\p{L}|[0-9])+", RegexOptions.Compiled);

        public static bool ValidateWord(this string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return false;
            }

            if (word.Length > MaxWordLength)
            {
                return false;
            }

            Match m = UnicodeWordRegex.Match(word);

            // We are looking for an exact match, not just a search hit. This matches what
            // the RegularExpressionValidator control does
            return m.Success && m.Index == 0 && m.Length == word.Length;
        }

        // todo: return Result with the error message explaining the error
        public static bool ValidateSentence(this string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return false;
            }

            if (sentence.Length > 1024)
            {
                return false;
            }

            var areWordsValid = sentence
                .Split((char[])[' ', ':', ';', ',', '.', '?', '!', '-', '\''], StringSplitOptions.RemoveEmptyEntries)
                .All(w => w.ValidateWord());

            return areWordsValid;
        }

        public static bool IsRegistrationEnabled(this IConfiguration configuration)
        {
            var isRegistrationEnabled = configuration["IsRegistrationEnabled"];
            if (string.IsNullOrWhiteSpace(isRegistrationEnabled))
            {
                return false;
            }
            return bool.Parse(isRegistrationEnabled);
        }

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

        public static string GetJsonSchema(Type type)
        {
            JsonSerializerOptions options = new(JsonSerializerOptions.Default)
            {
                MaxDepth = 12,
                WriteIndented = false,
                //RespectNullableAnnotations = true
            };
            JsonNode schema = options.GetJsonSchemaAsNode(type, ExporterOptions);


            var etEncoderSettings = new TextEncoderSettings();
            etEncoderSettings.AllowCharacters(
                '\u00C4', // Ä
                '\u00E4', // ä
                '\u00D5', // Õ
                '\u00F5', // õ
                '\u00D6', // Ö
                '\u00F6', // ö
                '\u00DC', // Ü
                '\u00FC', // ü
                '\u010C', // Č
                '\u010D', // č
                '\u0160', // Š
                '\u0161', // š
                '\u017D', // Ž
                '\u017E',  // ž
                '\''
              );
            etEncoderSettings.AllowRange(UnicodeRanges.BasicLatin);
            return schema.ToJsonString(new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(etEncoderSettings),
                WriteIndented = false
            });
        }
    }
}