using System;
using System.Collections;
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
    public partial class АpplicationAdd : Form
    {
        static SqlConnection connection;
        static SqlCommand command;
        public int User { get; set; }
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
        public АpplicationAdd()
        {
            InitializeComponent();
            Type();
        }
        public АpplicationAdd(int user) : this()
        {
            this.User = user;
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


        private void АpplicationAdd_Load(object sender, EventArgs e)
        {
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

        private void button1_Click(object sender, EventArgs e)
        {
            Connect();
            command = new SqlCommand($"SELECT typeEqipmentID FROM HomeTech WHERE homeTechModel = '{comboBox2.Text}'", connection);
            int type = Convert.ToInt32(command.ExecuteScalar());
            command = new SqlCommand($"SELECT * FROM Status WHERE statusID = {3}", connection);
            int status = Convert.ToInt32(command.ExecuteScalar());
            // Получаем максимальный ID из базы данных
            int maxRequestId;
            using (SqlCommand command = new SqlCommand("SELECT ISNULL(MAX(requestID), 0) FROM Request", connection))
            {
                maxRequestId = (int)command.ExecuteScalar();
            }
            int request = maxRequestId + 1;
            command.CommandText = $"INSERT INTO Request ([startDate], [typeEqipmentID], [problemDescryption], [statusID], [completionDate], [partsID], [masterID], [clientID]) " +
    $"VALUES ('{DateTime.Now.ToString("yyyy-MM-dd")}', '{type}', '{richTextBox1.Text}', '{status}', NULL, NULL, NULL, '{User}')";

            command.ExecuteNonQuery();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
