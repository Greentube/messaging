using ProtoBuf.Meta;

namespace Serialization.Protobuf
{
    public class ProtoBufOptions
    {
        public RuntimeTypeModel RuntimeTypeModel { get; set; } = RuntimeTypeModel.Default;
    }
}