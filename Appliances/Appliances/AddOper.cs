using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Appliances
{
    public partial class AddOper : Form
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public string type1 { get; set; }
        public string problem { get; set; }
        public string status { get; set; }
        public string client { get; set; }
        static SqlConnection connection;
        SqlCommand command;
        public AddOper()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
        }
        public AddOper(int id, DateTime start, string type1, string problem, string status, string client) : this()
        {
            this.id = id;
            this.start = start;
            this.type1 = type1;
            this.problem = problem;
            this.status = status;
            this.client = client;
        }
        static public void Connect()
        {
            try
            {
                connection = new SqlConnection("Data Source=LAPTOP-GGKOPBEE\\SQLEXPRESS;Initial Catalog=daryenaya_d;Integrated Security=True;");
                connection.Open();
            }
            catch (SqlException ex)
            { Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {ex.Message}"); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connect();

            command = new SqlCommand($"SELECT userID FROM Customer WHERE fio = '{comboBox1.Text}'", connection);
            int master = Convert.ToInt32(command.ExecuteScalar());

            var result = MessageBox.Show("Вы хотите принять заявку?", "Подтверждение завки", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                command.CommandText = $"UPDATE Request SET [statusID] = {1}, [problemDescryption] = '{richTextBox1.Text}', [masterID] = {master} WHERE [requestID] = {id}";
                command.ExecuteNonQuery();
                MessageBox.Show("Заявка успешно принята!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Прием заявки отменен.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            connection.Close();
        }

        private void AddOper_Load(object sender, EventArgs e)
        {
            Connect();
            command = new SqlCommand($"SELECT fio FROM Customer WHERE typeID = {3}", connection);
            using (var reader = command.ExecuteReader())
            {
                comboBox1.Items.Clear();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["fio"].ToString());
                }
            }
            dateTimePicker1.Value = start;
            command = new SqlCommand($"SELECT homeTechModel FROM HomeTech WHERE homeTechType = '{type1}'", connection);
            textBox1.Text = type1;
            textBox2.Text = command.ExecuteScalar().ToString();
            richTextBox1.Text = problem;
            textBox3.Text = status;
            textBox4.Text = client;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
