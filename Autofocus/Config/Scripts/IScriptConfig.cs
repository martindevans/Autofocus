namespace Autofocus.Config.Scripts;

public interface IScriptConfig
{
    string Key { get; }

    object?[] ToJsonArgs();
}