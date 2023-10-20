using CommandLine;
using FolkLibrary;

//var countryInfo = JsonSerializer.Deserialize<List<CountryInfo>>(File.ReadAllText(@"D:\Development\folk-library\Application\Properties\Countries.json"))!
//    .ToDictionary(x => x.alpha2, x => x.en, StringComparer.InvariantCultureIgnoreCase);

//foreach (var artist in Directory.EnumerateDirectories(@"D:\Music\Folk").SkipLast(1))
//{
//    var file = Path.Combine(artist, "info.json");

//    var json = File.ReadAllText(file);

//    var node = JsonSerializer.Deserialize<JsonNode>(json)!.AsObject();

//    var location = JsonNode.Parse($$"""
//    {
//        "countryCode": "{{(string?)node["country"]!}}",
//        "countryName": "{{countryInfo[(string)node["country"]!]}}",
//        "district": "{{(string?)node["district"]!}}",
//        "municipality": "{{(string?)node["municipality"]!}}",
//        "parish": "{{(string?)node["parish"]!}}"
//    }
//    """)!.AsObject();

//    var keysToRemove = location.Where(k => String.IsNullOrEmpty((string?)k.Value)).Select(k => k.Key).ToList();
//    foreach (var key in keysToRemove)
//        location.Remove(key);

//    node.Remove("country");
//    node.Remove("district");
//    node.Remove("municipality");
//    node.Remove("parish");
//    node.Remove("isAbroad");

//    node["location"] = location;

//    json = JsonSerializer.Serialize(node, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

//    File.WriteAllText(file, json);

//    Console.WriteLine(Path.GetFileName(artist));
//}

//return;

Parser.Default.ParseArguments<CopyArguments, object>(args)
.WithParsed<CopyArguments>(CopyHandler.Handle);

sealed record class CountryInfo(string alpha2, string en);