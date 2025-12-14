using GameAndDot.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAndDot.Shared
{
    public class DotMessage
    {
        public EventType Type { get; set; }   // "place_dot" или "dot_added"
        public int X { get; set; }
        public int Y { get; set; }
        public string Player { get; set; }
    }

}
