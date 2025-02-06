using System.Text.Json.Nodes;

namespace PROD_PdfJsonViewer_POC.TestConsole
{
    internal class Program
    {
        static char Spacer = '*';

        static void Main(string[] args)
        {
            Console.WriteLine("JSON Editor Test Console");

            var filePath = @"E:\Test Data\PDF_JSON_Viewer\33GRAF26A_IFB.json";

            var json = File.ReadAllText(filePath);

            var parsedJson = JsonNode.Parse(json);

            Console.WriteLine(parsedJson.ToString());

            char spacer = ' ';

            if (parsedJson is JsonObject jsonObject)
            {
                IterateJsonObject(jsonObject);
            }
            else if (parsedJson is JsonArray jsonArray)
            {
                IterateJsonArray(jsonArray);
            }
        }

        static void IterateJsonObject(JsonObject jsonObject, int indent = 0)
        {
            foreach (var kvp in jsonObject)
            {

                if (kvp.Value is JsonObject nestedObject)
                {
                    Console.WriteLine($"{new string(Spacer, indent)}{kvp.Key}: ");
                    IterateJsonObject(nestedObject, indent + 4);
                }
                else if (kvp.Value is JsonArray nestedArray)
                {
                    IterateJsonArray(nestedArray, indent + 4);
                }
                else if (kvp.Value is JsonValue nestedValue)
                {
                    PrintJsonValue(kvp.Key, nestedValue, indent);
                }
            }
        }

        static void IterateJsonArray(JsonArray jsonArray, int indent = 0)
        {
            foreach (var item in jsonArray)
            {
                //Console.WriteLine(item);
                if (item is JsonObject nestedObject)
                {
                    IterateJsonObject(nestedObject);
                }
                else if (item is JsonArray nestedArray)
                {
                    IterateJsonArray(nestedArray);
                }
                else if (item is JsonValue nestedValue)
                {
                    PrintJsonValue(nestedValue);
                }
            }
        }

        static void PrintJsonKey(string key, int indent = 0)
        {
            Console.WriteLine($"{new string(Spacer, indent)}{key}");
        }

        static void PrintJsonValue(JsonValue jsonValue, int indent = 0)
        {
            Console.WriteLine(jsonValue.ToString());
        }

        static void PrintJsonValue(string key, JsonValue jsonValue, int indent = 0)
        {
            Console.WriteLine($"{new string(Spacer, indent)}{key}: {jsonValue}");
        }
    }
}
