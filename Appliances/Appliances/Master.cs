using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appliances
{
    public partial class Master : Form
    {
        Passcs passcs = new Passcs();
        static SqlConnection connection;
        SqlCommand command;
        public string Type { get; set; }
        public string Fio { get; set; }
        public int User { get; set; }
        public Master()
        {
            InitializeComponent();
        }
        public Master(string type, string fio, int user) : this()
        {
            this.Type = type;
            this.Fio = fio;
            this.User = user;
            label1.Text = this.Type;
            label2.Text = this.Fio;
        }


    static public void Connect()
        {
            try
            {
                connection = new SqlConnection("Data Source=LAPTOP-GGKOPBEE\\SQLEXPRESS;Initial Catalog=daryenaya_d;Integrated Security=True;");
                connection.Open();
            }
            catch (SqlException ex)
            { Console.WriteLine($"Ощибка доступа к базе данных. Исключение: {ex.Message}"); }
        }
        private void Add()
        {
            Connect();
            string query = $@"
             SELECT 
                r.requestID, 
                r.startDate, 
                h.homeTechType,
	            h.homeTechModel,
                r.problemDescryption, 
                a.requestStatus, 
                r.completionDate, 
                b.repairParts, 
                b.count, 
	            f.message
            FROM Request r
            INNER JOIN HomeTech h ON r.typeEqipmentID = h.typeEqipmentID
            LEFT JOIN [Status] a ON r.statusID = a.statusID
            LEFT JOIN Parts b ON r.partsID = b.partsID
            LEFT JOIN Comment f ON r.requestID = f.requestID
            WHERE r.masterID = '{User}' and r.statusID = {1}";

            SqlCommand command = new SqlCommand(query, connection);

            SqlDataReader reader = command.ExecuteReader();

            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[10]);

                data[data.Count - 1][0] = reader[0].ToString();
                data[data.Count - 1][1] = Convert.ToDateTime(reader[1]).ToString("yyyy-MM-dd");
                data[data.Count - 1][2] = reader[2].ToString();
                data[data.Count - 1][3] = reader[3].ToString();
                data[data.Count - 1][4] = reader[4].ToString();
                data[data.Count - 1][4] = reader[5].ToString();
                if (reader[6] != DBNull.Value)
                {
                    data[data.Count - 1][6] = Convert.ToDateTime(reader[6]).ToString("yyyy-MM-dd");
                }
                data[data.Count - 1][7] = reader[7].ToString();
                data[data.Count - 1][8] = reader[8].ToString();
                data[data.Count - 1][9] = reader[9].ToString();
            }
            reader.Close();
            dataGridView1.DataSource = null;
            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
            int totalRecords = data.Count;
            command = new SqlCommand($"SELECT Count(*) FROM Request WHERE masterID = '{User}'", connection);
            int totalRecords1 = (int)command.ExecuteScalar();
            connection.Close();
            label3.Text = "Количество записей: " + totalRecords + " из " + totalRecords1;
        }
        private void Add1()
        {
            Connect();
            string query = $@"
             SELECT 
            r.requestID, 
            r.startDate, 
            h.homeTechType,
	        h.homeTechModel,
            r.problemDescryption, 
            a.requestStatus, 
            r.completionDate, 
            b.repairParts, 
            b.count, 
	        f.message
        FROM Request r
        INNER JOIN HomeTech h ON r.typeEqipmentID = h.typeEqipmentID
        LEFT JOIN [Status] a ON r.statusID = a.statusID
        LEFT JOIN Parts b ON r.partsID = b.partsID
        LEFT JOIN Comment f ON r.requestID = f.requestID
        WHERE r.masterID = '{User}' and r.statusID <> {1}";

            SqlCommand command = new SqlCommand(query, connection);

            SqlDataReader reader = command.ExecuteReader();

            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[10]);

                data[data.Count - 1][0] = reader[0].ToString();
                data[data.Count - 1][1] = Convert.ToDateTime(reader[1]).ToString("yyyy-MM-dd");
                data[data.Count - 1][2] = reader[2].ToString();
                data[data.Count - 1][3] = reader[3].ToString();
                data[data.Count - 1][4] = reader[4].ToString();
                data[data.Count - 1][5] = reader[5].ToString();
                if (reader[6] != DBNull.Value)
                {
                    data[data.Count - 1][6] = Convert.ToDateTime(reader[6]).ToString("yyyy-MM-dd");
                }
                data[data.Count - 1][7] = reader[7].ToString();
                data[data.Count - 1][8] = reader[8].ToString();
                data[data.Count - 1][9] = reader[9].ToString();
            }
            reader.Close();
            dataGridView2.DataSource = null;
            foreach (string[] s in data)
                dataGridView2.Rows.Add(s);
            int Records = data.Count;
            command = new SqlCommand($"SELECT Count(*) FROM Request WHERE masterID = '{User}'", connection);
            int Records1 = (int)command.ExecuteScalar();
            connection.Close();
            label4.Text = "Количество записей: " + Records + " из " + Records1;
        }

        private void Master_Load(object sender, EventArgs e)
        {
            Add();
            Add1();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Order order = new Order(id, User);
            order.ShowDialog();
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            Add();
            Add1();

        }
        public int id {  get; set; }
        public int count { get; set; }
        public string name { get; set; }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
