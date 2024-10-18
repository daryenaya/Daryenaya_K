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
    public partial class АpplicationСhange : Form
    {
        static SqlConnection connection;
        static SqlCommand command;
        public  int ID { get; set; }
        public string TypeEquipment { get; set; }
        public string problem { get; set; }
        public АpplicationСhange()
        {
            InitializeComponent();
            Type();
        }
        public АpplicationСhange(int id, string type, string problem) : this()
        {
            this.ID = id;
            this.TypeEquipment = type;
            this.problem = problem;
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
        private void Type()
        {
            Connect();
            SqlCommand command = new SqlCommand("SELECT * FROM HomeTech", connection);
            HashSet<string> uniqueHomeTechTypes = new HashSet<string>();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    uniqueHomeTechTypes.Add(reader["homeTechType"].ToString());
                }
            }
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(uniqueHomeTechTypes.ToArray());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {

                comboBox2.DropDownStyle = ComboBoxStyle.DropDown;
                SqlCommand command1 = new SqlCommand($"SELECT * FROM HomeTech WHERE homeTechType = '{comboBox1.Text}'", connection);

                using (var reader = command1.ExecuteReader())
                {
                    comboBox2.Items.Clear();
                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader["homeTechModel"].ToString());
                    }
                }
            }
        }

        private void АpplicationСhange_Load(object sender, EventArgs e)
        {
            Connect();
            command = new SqlCommand($"SELECT homeTechModel FROM HomeTech WHERE homeTechType = '{TypeEquipment}'", connection);
            comboBox1.Text = TypeEquipment;
            comboBox2.Text = command.ExecuteScalar().ToString();
            richTextBox1.Text = problem;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connect();
            command = new SqlCommand($"SELECT typeEqipmentID FROM HomeTech WHERE homeTechModel = '{comboBox2.Text}'", connection);
            int type = Convert.ToInt32(command.ExecuteScalar());

            var result = MessageBox.Show("Вы хотите изменить заявку?", "Подтверждение изменения", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                command.CommandText = $"UPDATE Request SET [typeEqipmentID] = '{type}', [problemDescryption] = '{richTextBox1.Text}' WHERE [requestID] = {ID}";
                command.ExecuteNonQuery();
                MessageBox.Show("Заявка успешно обновлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Изменение заявки отменено.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            connection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
