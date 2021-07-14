namespace Tests
{
    public interface IBaseModule
    {
        bool BaseFlag { get; set; }
    }

    public record BaseModule : IBaseModule
    {
        public bool BaseFlag { get; set; }
    }
}
