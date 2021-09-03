using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinBot.Model
{
    public class CommandMessage
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int CommandID { get; set; }
        public string Message { get; set; }
    }
}
