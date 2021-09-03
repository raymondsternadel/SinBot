using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinBot.Model
{
    public class CommandRecord
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int CommandID { get; set; }
        public int ViewerID { get; set; }
        public DateTime DateTime { get; set; }
    }
}
