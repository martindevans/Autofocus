using Autofocus;
using Autofocus.Outpaint.Extensions;
using Autofocus.Terminal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

var api = new StableDiffusion("http://martin-strix:1234")
{
    PingEndpoint = "/",
    EnableProgress = false,
};

//await new SimpleEndToEnd().Run(api);
//await new StableDiffusionCpp().Run(api);
//await new VideoPrototyping().Run(api);
//await new TiledUpscalerPrototype().Run(api);
//await new AfterDetailerDemo().Run(api);
//await new Outpaint2Demo().Run(api);
//await new Queueing().Run(api);
//await new PixelArt().Run(api);
//await new Repainter().Run(api);