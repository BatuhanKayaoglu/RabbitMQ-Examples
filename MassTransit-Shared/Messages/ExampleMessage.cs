using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit_Shared.Messages
{
    public class ExampleMessage : IMessage
    {
        public string Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
