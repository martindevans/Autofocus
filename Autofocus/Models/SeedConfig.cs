namespace Autofocus.Models
{
    public record SeedConfig
    {
        public int? Seed { get; set; }
        public int? SubSeed { get; set; }
        public double? SubseedStrength { get; set; }
        public uint? SeedResizeFromWidth { get; set; }
        public uint? SeedResizeFromHeight { get; set; }
    }
}
