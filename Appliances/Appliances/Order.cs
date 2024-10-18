using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appliances
{
    public partial class Order : Form
    {
        static SqlConnection connection;
        SqlCommand command;
        public int id { get; set; }
        public int user {  get; set; }
        public Order()
        {
            InitializeComponent();
            Connect();
            SqlCommand command = new SqlCommand("SELECT * FROM Parts", connection);
            HashSet<string> uniqueHomeTechTypes = new HashSet<string>();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    uniqueHomeTechTypes.Add(reader["repairParts"].ToString());
                }
            }
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(uniqueHomeTechTypes.ToArray());
        }
        public Order(int id, int user) : this(){
            this.id = id;
            this.user = user;
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

        private void button1_Click(object sender, EventArgs e)
        {
            Connect();
            int maxRequestId;
            using (SqlCommand command = new SqlCommand("SELECT ISNULL(MAX(partsID), 0) FROM Parts", connection))
            {
                maxRequestId = (int)command.ExecuteScalar();
            }
            int request = maxRequestId + 1;
            command = new SqlCommand($"UPDATE Request SET [partsID] = {comboBox1.Text} WHERE [requestID] = {id})", connection);
            command = new SqlCommand($"INSERT INTO Parts ([repairParts], [count], [masterID])" +
                $"VALUES ({request}, '{comboBox1.Text}', {textBox1.Text}, {user})", connection);
            int maxtId;
            using (SqlCommand command = new SqlCommand("SELECT ISNULL(MAX(commentID), 0) FROM Comment", connection))
            {
                maxtId = (int)command.ExecuteScalar();
            }
            int request1 = maxtId + 1;
            // Не используйте commentID, если это IDENTITY
            command = new SqlCommand($"INSERT INTO Comment (message, masterID, requestID) VALUES" +
                $"('{textBox2.Text}', {user}, {id})", connection);

            command.ExecuteNonQuery();

            MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Order_Load(object sender, EventArgs e)
        {

        }
    }
}
