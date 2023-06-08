using System.Text.Json.Serialization;

namespace Autofocus.CtrlNet;

internal class ControlNetModuleListResponse
{
    [JsonPropertyName("module_list")] public string[] ModuleList { get; set; } = null!;
}