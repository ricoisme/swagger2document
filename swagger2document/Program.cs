using swagger2word;





var josnSource = Environment.GetCommandLineArgs().Length > 3 ? Environment.GetCommandLineArgs()[1] : "";
var docType = Environment.GetCommandLineArgs().Length > 3 ? Environment.GetCommandLineArgs()[2] : "3";
var templateType = Environment.GetCommandLineArgs().Length > 3 ? Environment.GetCommandLineArgs()[3] : "b";

if (string.IsNullOrEmpty(josnSource))
{
    throw new ArgumentException("json source was empty");
}

var json = string.Empty;
if (josnSource.StartsWith("http", StringComparison.OrdinalIgnoreCase))
{
    json = await GetJsonFromRemoteAsync();
}
else
{
    json = File.ReadAllText(josnSource);
}

var reader = new Microsoft.OpenApi.Readers.OpenApiStringReader();
var openApiDocument = reader.Read(json, out var diagnostic);

if (diagnostic != null && diagnostic.Errors.Count > 0)
{
    Console.WriteLine("Error reading OpenAPI document:");
    foreach (var error in diagnostic.Errors)
    {
        Console.WriteLine($"- {error.Message}");
    }
}
else
{
    var reportHelper = new ReportHelper();
    var outfile = await reportHelper.GenerateAsync(openApiDocument, docType, templateType);
    Console.WriteLine(Path.Combine(AppContext.BaseDirectory, outfile));
}



async Task<string> GetJsonFromRemoteAsync()
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
        {
            Console.WriteLine("SSL error skipped");
            return true;
        }
    };

    var client = new HttpClient(handler);
    var response = await client.GetAsync(josnSource);
    response.EnsureSuccessStatusCode();
    var responseBody = await response.Content.ReadAsStringAsync();
    return responseBody;
}


public partial class Program;
