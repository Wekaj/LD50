using System.Text.Json.Serialization;

namespace LD50.Entities {
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Formation {
        FrontArc,
        Group,
    }
}
