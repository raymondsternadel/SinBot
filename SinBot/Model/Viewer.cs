using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinBot.Model
{
    public class Viewer
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
    }
}
