using Autofocus.Models;

namespace Autofocus.Config;

public record PromptConfig
{
    public required string Positive { get; init; }
    public required string Negative { get; init; }
    public List<IPromptStyle> Styles { get; init; } = new();
}