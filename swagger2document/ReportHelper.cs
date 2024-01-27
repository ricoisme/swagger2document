using Microsoft.OpenApi.Models;
using Microsoft.Reporting.NETCore;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;


namespace swagger2word;

//ref: https://github.com/lkosson/reportviewercore
internal class ReportHelper
{

    readonly Dictionary<string, (string, string)> _map = new Dictionary<string, (string, string)>
    {
        { "1",("PDF",".pdf")},
        { "2",("EXCELOPENXML", ".xlsx")},
        { "3",("WORDOPENXML", ".docx")},
        { "4",("HTML4.0", ".htm")},
        { "5",("MHTML", ".mhtml")},
        { "a",("template", ".rdlc")},
        { "b",("template2", ".rdlc")}
    };

    internal async Task<string> GenerateAsync(OpenApiDocument openApiDocument, string docType, string templateType)
    {
        var report = new LocalReport();
        var parameters = new[] {
            new ReportParameter("Title", openApiDocument.Info.Title),
            new ReportParameter("Version", openApiDocument.Info.Version),
            new ReportParameter("Description", openApiDocument.Info.Description)
        };

        var currentTemplate = _map
            .Where(kv => kv.Key.Equals(templateType)).FirstOrDefault().Value;
        var currentFormatter = _map.FirstOrDefault(kv => kv.Key.Equals(docType));

        using var fileStream = new FileStream($"{currentTemplate.Item1}{currentTemplate.Item2}", FileMode.Open);
        report.LoadReportDefinition(fileStream);
        var items = new List<ReportModel>();
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        foreach (var item in openApiDocument.Paths)
        {
            var reportModel = new ReportModel();
            reportModel.Url = item.Key;
            if (item.Value.Operations != null)
            {
                foreach (var operation in item.Value.Operations)
                {
                    reportModel.MethodName = operation.Key.ToString();
                    foreach (var tag in operation.Value.Tags)
                    {
                        reportModel.Tag += tag.Name + Environment.NewLine;
                    }
                    reportModel.Summary = operation.Value.Summary;
                    reportModel.Description = operation.Value.Description;
                    if (operation.Value.RequestBody != null)
                    {
                        reportModel.RequestType = string.Join(',', operation.Value.RequestBody.Content.Keys);
                    }
                    if (operation.Value.Parameters?.Count > 0)
                    {
                        foreach (var parameter in operation.Value.Parameters)
                        {
                            var paraitme = new ParametersItem
                            {
                                ParaName = parameter.Name,
                                ParaType = parameter.In?.ToString() ?? "",
                                ParaRequired = parameter.Required,
                                ParaDescription = parameter.Description
                            };
                            reportModel.ParameterItems.Add(paraitme);
                        }
                        reportModel.Parameters = JsonSerializer.Serialize(reportModel.ParameterItems, jsonOptions);
                    }
                }
            }
            items.Add(reportModel);
        }

        var finallyItems = items.Select(model => new ReportModel
        {
            Tag = model.Tag,
            Summary = model.Summary,
            Description = model.Description,
            Url = model.Url,
            Parameters = model.Parameters,
            MethodName = model.MethodName,
            RequestType = model.RequestType,
        });

        if (currentTemplate.Item1.Equals("template2"))
        {
            var flattenedItems = items
              .SelectMany(model => model.ParameterItems
                  .Select(paramItem =>
                      new ReportModel
                      {
                          Tag = model.Tag,
                          Summary = model.Summary,
                          Description = model.Description,
                          Url = model.Url,
                          MethodName = model.MethodName,
                          RequestType = model.RequestType,
                          Parameters = model.Parameters,
                          ParaName = paramItem.ParaName,
                          ParaType = paramItem.ParaType,
                          ParaRequired = paramItem.ParaRequired,
                          ParaDescription = paramItem.ParaDescription,
                      }));
            report.DataSources.Add(new ReportDataSource("ReportItem", finallyItems.Concat(flattenedItems)));
        }
        else
        {
            report.DataSources.Add(new ReportDataSource("ReportItem", finallyItems));
        }
        report.SetParameters(parameters);
        var result = report.Render(currentFormatter.Value.Item1);
        var outfile = Path.Combine($"Export{currentFormatter.Value.Item2}");
        await File.WriteAllBytesAsync(outfile, result);
        return outfile;
    }
}
