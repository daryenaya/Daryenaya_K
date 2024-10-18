using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appliances
{
    internal class Passcs
    {
        public SqlConnection connection;
        public SqlCommand command;
        public Passcs()
        {
            connection = new SqlConnection("Data Source=LAPTOP-GGKOPBEE\\SQLEXPRESS;Initial Catalog=daryenaya_d;Integrated Security=True;");
            command = new SqlCommand();
        }
        
    }
}
