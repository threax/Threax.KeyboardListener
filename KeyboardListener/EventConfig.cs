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

        public byte? Brightness { get; set; }

        public String HexColor { get; set; }
    }

    public class HotkeyEvent
    {
        public List<EventAction> Actions { get; set; } = new List<EventAction>();
    }

    public class EventConfig
    {
        public HotkeyEvent Main { get; set; }

        public HotkeyEvent Tv { get; set; }

        public HotkeyEvent Sound { get; set; }

        public HotkeyEvent Light { get; set; }

        public HotkeyEvent SecondTvEvent { get; set; }
    }
}
