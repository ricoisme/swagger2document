using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace swagger2word
{
    internal class BaseModel
    {
        public string Tag { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string MethodName { get; set; }
        public string RequestType { get; set; }
        public string ParaName { get; set; } = string.Empty;
        public string ParaType { get; set; }
        public bool? ParaRequired { get; set; }
        public string ParaDescription { get; set; }
    }

    internal sealed class ReportModel : BaseModel
    {
        public string Parameters { get; set; } = string.Empty;
        public List<ParametersItem> ParameterItems { get; set; } = new List<ParametersItem>();
    }

    internal sealed class ParametersItem
    {
        [JsonPropertyName("名稱")]
        public string ParaName { get; set; } = string.Empty;
        [JsonPropertyName("類型")]
        public string ParaType { get; set; }
        [JsonPropertyName("必填")]
        public bool? ParaRequired { get; set; }
        [JsonPropertyName("描述")]
        public string ParaDescription { get; set; }
    }
}
