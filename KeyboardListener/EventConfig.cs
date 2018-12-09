using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardListener
{
    public class EventAction
    {
        public Guid SwitchId { get; set; }

        public String Value { get; set; }

        public byte Brightness { get; set; }

        public String HexColor { get; set; }
    }

    public class Event
    {
        public List<EventAction> Actions { get; set; }
    }

    public class EventConfig
    {
        public Event Main { get; set; }

        public Event Tv { get; set; }

        public Event Sound { get; set; }

        public Event Light { get; set; }
    }
}
