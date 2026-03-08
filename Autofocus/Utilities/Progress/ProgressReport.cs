namespace Autofocus.Utilities.Progress;

public record struct ProgressReport(float Progress, Base64EncodedImage? Intermediate);