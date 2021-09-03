using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinBot.Model
{
    public class Command
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string TriggerPhrase { get; set; }
        public string MediaType { get; set; }
        public string Path { get; set; }
        public string TriggerKey { get; set; }
    }
}
