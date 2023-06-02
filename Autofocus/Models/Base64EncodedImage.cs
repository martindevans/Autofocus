using System.Text.Json;
using System.Text.Json.Serialization;

namespace Autofocus.Models;

public class Base64EncodedImage
{
    internal readonly byte[] Data;

    internal Base64EncodedImage(byte[] data)
    {
        Data = data;
    }

    public Image ToImage()
    {
        return Image.Load(new MemoryStream(Data));
    }

    public static Base64EncodedImage FromImage(Image image)
    {
        var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return new Base64EncodedImage(stream.ToArray());
    }
}

internal class Base64EncodedImageConverter
    : JsonConverter<Base64EncodedImage>
{
    public override Base64EncodedImage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Base64EncodedImage(reader.GetBytesFromBase64());
    }

    public override void Write(Utf8JsonWriter writer, Base64EncodedImage value, JsonSerializerOptions options)
    {
        writer.WriteBase64StringValue(value.Data);
    }
}