using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinBot.Model
{
    public class InformationalMessage
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Message { get; set; }
    }
}
