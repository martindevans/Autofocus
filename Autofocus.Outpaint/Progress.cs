namespace Autofocus.Outpaint
{
    internal class Progress
    {
        public event Action<float>? ProgressEvent;

        private readonly IStableDiffusion _api;
        private float _progress;

        public Progress(IStableDiffusion api)
        {
            _api = api;
        }

        public void Report(float value)
        {
            _progress = Math.Max(value, _progress);
            ProgressEvent?.Invoke(_progress);
        }

        public async Task<T> Report<T>(float start, float end, Task<T> task)
        {
            start = Math.Max(start, _progress);
            end = Math.Max(start, end);

            Report(start);

            while (!task.IsCompleted)
            {
                try
                {
                    var value = await _api.Progress(true);
                    Report(start + (end - start) * (float)value.Progress);
                }
                catch (TimeoutException)
                {

                }
                finally
                {
                    await Task.Delay(350);
                }
            }

            Report(end);

            return await task;
        }
    }
}
