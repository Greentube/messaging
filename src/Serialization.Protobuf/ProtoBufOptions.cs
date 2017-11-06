using ProtoBuf.Meta;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ProtoBufOptions
    {
        public RuntimeTypeModel RuntimeTypeModel { get; set; } = RuntimeTypeModel.Default;
    }
}