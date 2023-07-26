using Autofocus.Config;
using Autofocus.Models;

namespace Autofocus
{
    public interface IStableDiffusion
    {
        public Task<IProgress> Progress(bool skipCurrentImage = false);

        public Task Ping();

        public Task<IQueueStatus> QueueStatus();

        public Task<IScriptsResponse> Scripts();

        public Task<IEnumerable<ISampler>> Samplers();

        public Task<ISampler> Sampler(string name);

        public Task<IEnumerable<IUpscaler>> Upscalers();

        public Task<IUpscaler> Upscaler(string name);

        public Task<IEnumerable<IPromptStyle>> Styles();

        public Task<IPromptStyle> Style(string name);

        public Task<IEnumerable<IStableDiffusionModel>> StableDiffusionModels();

        public Task<IStableDiffusionModel> StableDiffusionModel(string name);

        public Task<IEmbeddings> Embeddings();

        public Task<IMemory> Memory();

        public Task<IPngInfo> PngInfo(Base64EncodedImage image);

        public Task<ITextToImageResult> TextToImage(TextToImageConfig config);

        public Task<IImageToImageResult> Image2Image(ImageToImageConfig config);

        public Task<IInterrogateResult> Interrogate(Base64EncodedImage image, InterrogateModel model = InterrogateModel.CLIP);

        public Task<IInterrogateResult> Interrogate(InterrogateConfig config);

        public Task<ControlNet?> TryGetControlNet();
    }
}
