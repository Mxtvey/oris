using GameAndDot.Shared.Enums;

namespace GameAndDot.Shared.Models
{
    public class EventMessage
    {
        public EventType Type { get; set; }

        public string Id { get; set; }

        public string Username { get; set; }

        public List<string> Players { get; set; } = new();
        public string? Color { get; set; }

    }
}
