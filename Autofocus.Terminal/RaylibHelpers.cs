using Raylib_cs;

namespace Autofocus.Terminal;

public static class RaylibHelpers
{
    public static void ShowPng(string path)
    {
        var image = Raylib.LoadImage(path);
        try
        {
            Raylib.InitWindow(image.Width, image.Height, "PNG Viewer");
            Raylib.SetTargetFPS(60);

            var texture = Raylib.LoadTextureFromImage(image);
            try
            {
                Console.WriteLine($"{texture.Id} {texture.Width}x{texture.Height}");
                Console.WriteLine($"IsValid: {Raylib.IsTextureValid(texture)}");

                while (!Raylib.WindowShouldClose())
                {
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.White);
                    Raylib.DrawTexture(texture, 0, 0, Color.White);
                    Raylib.EndDrawing();
                }

                Raylib.CloseWindow();
            }
            finally
            {
                Raylib.UnloadTexture(texture);
            }
        }
        finally
        {
            Raylib.UnloadImage(image);
        }
    }
}