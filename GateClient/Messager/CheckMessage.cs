using CommunityToolkit.Mvvm.Messaging.Messages;
using GateClient.Dto;

namespace GateClient.Messager
{
    public class CheckMessage : ValueChangedMessage<GateInDto>
    {
        public CheckMessage(GateInDto value) : base(value)
        {
        }
    }
}
