namespace Autofocus.Config.Scripts
{
    public class ScriptNotFoundException
        : Exception
    {
        public string ScriptName { get; }

        public ScriptNotFoundException(string scriptName)
        {
            ScriptName = scriptName;
        }
    }
}
