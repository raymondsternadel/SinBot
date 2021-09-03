using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SinBot.Views;

namespace SinBot.Misc
{
    public class NewChatMessageEventArgs
    {
        public NewChatMessage TheNewChatMessage { get; set; }
        public Timer TheTimer { get; set; }
    }
}
