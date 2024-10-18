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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Appliances
{
    public partial class UpdateOper : Form
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public string type1 { get; set; }
        public string problem { get; set; }
        public string status { get; set; }
        public string master { get; set; }
        public DateTime finish { get; set; }
        public string parts { get; set; }

        public string client { get; set; }
        public string massage { get; set; }
        static SqlConnection connection;
        SqlCommand command;
        public UpdateOper()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
        }
        public UpdateOper(int id, DateTime start, string type1, string problem, string status, DateTime finish, string parts, string master, string client, string massage) : this()
        {
            this.id = id;
            this.start = start;
            this.type1 = type1;
            this.problem = problem;
            this.status = status;
            this.finish = finish;
            this.parts = parts;
            this.master = master;
            this.client = client;
            this.massage = massage;

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
            try
            {
                // Замените 2 на корректное значение statusID
                command = new SqlCommand("UPDATE Request SET [statusID] = @statusID, completionDate = @completionDate WHERE [requestID] = @requestID", connection);
                command.Parameters.AddWithValue("@statusID", 2); // Убедитесь, что 2 существует в таблице Status
                command.Parameters.AddWithValue("@completionDate", dateTimePicker2.Value); // Используйте значение из dateTimePicker
                command.Parameters.AddWithValue("@requestID", id);

                int affectedRows = command.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    MessageBox.Show("Запись обновлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Запись не найдена.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void UpdateOper_Load(object sender, EventArgs e)
        {
            Connect();
            dateTimePicker1.Value = start;
            if (finish != DateTime.MinValue)
            {
                dateTimePicker2.Value = finish;
            }
            command = new SqlCommand($"SELECT homeTechModel FROM HomeTech WHERE homeTechType = '{type1}'", connection);
            textBox1.Text = type1;
            textBox2.Text = command.ExecuteScalar().ToString();
            richTextBox1.Text = problem;
            textBox3.Text = status;
            textBox4.Text = parts;
            textBox5.Text = master;
            textBox6.Text = client;
            textBox7.Text = massage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
