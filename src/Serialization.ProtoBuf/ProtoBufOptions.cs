using ProtoBuf.Meta;

namespace Serialization.ProtoBuf
{
    public class ProtoBufOptions
    {
        public RuntimeTypeModel RuntimeTypeModel { get; set; } = RuntimeTypeModel.Default;
    }
}