using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Appliances
{
    public partial class Manager : Form
    {
        static SqlConnection connection;
        SqlCommand command;
        public string Type { get; set; }
        public string Fio { get; set; }
        public int User { get; set; }

        public Manager()
        {
            InitializeComponent();
            Pass();
        }

        public Manager(string type, string fio, int user) : this()
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
            {
                Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {ex.Message}");
            }
        }
        public void Pass()
        {
            Connect();
            string quer = $@"
            SELECT   
                r.attemptTime, 
                h.login, 
                r.successful
            FROM LoginAttempts r
            INNER JOIN Customer h ON r.userID  = h.userID;";
            SqlCommand command = new SqlCommand(quer, connection);

            SqlDataReader reader = command.ExecuteReader();

            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[3]);

                data[data.Count - 1][0] = Convert.ToDateTime(reader[0]).ToString();
                data[data.Count - 1][1] = reader[1].ToString();
                data[data.Count - 1][2] = reader[2].ToString();

            }
            reader.Close();
        }
        public void Add()
        {
            Connect();
            string query = $@"
             SELECT 
                r.requestID, 
                r.startDate, 
                h.homeTechType,
                r.problemDescryption, 
                a.requestStatus, 
                r.completionDate, 
                b.repairParts, 
                с.fio, 
                d.fio,
	            f.message
            FROM Request r
            INNER JOIN HomeTech h ON r.typeEqipmentID = h.typeEqipmentID
            LEFT JOIN [Status] a ON r.statusID = a.statusID
            LEFT JOIN Parts b ON r.partsID = b.partsID
            LEFT JOIN Customer с ON r.masterID = с.userID
            LEFT JOIN Customer d ON r.clientID = d.userID
            LEFT JOIN Comment f ON r.requestID = f.requestID
            WHERE r.statusID = {0}";

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
                if (reader[5] != DBNull.Value)
                {
                    data[data.Count - 1][5] = Convert.ToDateTime(reader[5]).ToString("yyyy-MM-dd");
                }
                data[data.Count - 1][6] = reader[6].ToString();
                data[data.Count - 1][7] = reader[7].ToString();
                data[data.Count - 1][8] = reader[8].ToString();
                data[data.Count - 1][9] = reader[9].ToString();
            }
            reader.Close();
            dataGridView1.DataSource = null;
            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
            int totalRecords = data.Count;
            command = new SqlCommand($"SELECT Count(*) FROM Request", connection);
            int totalRecords1 = (int)command.ExecuteScalar();
            connection.Close();
            label4.Text = "Количество записей: " + totalRecords + " из " + totalRecords1;
        }
        public void Add1()
        {
            Connect();
            string query = $@"
             SELECT 
                r.requestID, 
                r.startDate, 
                h.homeTechType,
                r.problemDescryption, 
                a.requestStatus, 
                r.completionDate, 
                b.repairParts, 
                с.fio, 
                d.fio,
	            f.message
            FROM Request r
            INNER JOIN HomeTech h ON r.typeEqipmentID = h.typeEqipmentID
            LEFT JOIN [Status] a ON r.statusID = a.statusID
            LEFT JOIN Parts b ON r.partsID = b.partsID
            LEFT JOIN Customer с ON r.masterID = с.userID
            LEFT JOIN Customer d ON r.clientID = d.userID
            LEFT JOIN Comment f ON r.requestID = f.requestID
            WHERE r.statusID <> {0}";

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
                if (reader[5] != DBNull.Value)
                {
                    data[data.Count - 1][5] = Convert.ToDateTime(reader[5]).ToString("yyyy-MM-dd");
                }
                data[data.Count - 1][6] = reader[6].ToString();
                data[data.Count - 1][7] = reader[7].ToString();
                data[data.Count - 1][8] = reader[8].ToString();
                data[data.Count - 1][9] = reader[9].ToString();
            }
            reader.Close();
            dataGridView2.DataSource = null;
            foreach (string[] s in data)
                dataGridView2.Rows.Add(s);
            int Records = data.Count;
            command = new SqlCommand($"SELECT Count(*) FROM Request", connection);
            int Records1 = (int)command.ExecuteScalar();
            connection.Close();
        }

        private void Manager_Load(object sender, EventArgs e)
        {
            Add();
            Add1();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            AddOper addOper = new AddOper(id, start, type1, problem, status, client);
            addOper.ShowDialog();
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            Add();
            Add1();
        }
        public int id { get; set; }
        public int id1 { get; set; }
        public DateTime start { get; set; }
        public string type1 { get; set; }
        public string problem { get; set; }
        public string status { get; set; }
        public string master { get; set; }
        public DateTime finish { get; set; }
        public string parts { get; set; }

        public string client { get; set; }
        public string massage { get; set; }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.SelectedRows[0].Cells.Count >= 9)
                {
                    id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());

                    if (DateTime.TryParse(dataGridView1.SelectedRows[0].Cells[1].Value.ToString(), out DateTime parsedStart))
                    {
                        start = parsedStart;
                    }

                    type1 = dataGridView1.SelectedRows[0].Cells[2].Value?.ToString() ?? string.Empty;
                    problem = dataGridView1.SelectedRows[0].Cells[3].Value?.ToString() ?? string.Empty;
                    status = dataGridView1.SelectedRows[0].Cells[4].Value?.ToString() ?? string.Empty;
                    client = dataGridView1.SelectedRows[0].Cells[8].Value?.ToString() ?? string.Empty;
                }
                else
                {
                    MessageBox.Show("Недостаточно ячеек в выделенной строке.");
                }
            }
        }

        public void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            // Проверяем, что имя столбца начинается с "Column" и имеет номер от 1 до 10
            if (e.Column.Name.StartsWith("Column") &&
                int.TryParse(e.Column.Name.Substring(6), out int columnIndex) &&
                columnIndex >= 1 &&
                columnIndex <= 10)
            {
                // Для строкового сравнения
                // Сначала проверяем, являются ли значения числовыми
                if (decimal.TryParse(e.CellValue1?.ToString(), out decimal val1) &&
                    decimal.TryParse(e.CellValue2?.ToString(), out decimal val2))
                {
                    e.SortResult = val1.CompareTo(val2); // Сравнение как чисел
                }
                else
                {
                    // Если не удалось распарсить, сравниваем как строки
                    e.SortResult = string.Compare(
                        e.CellValue1?.ToString() ?? string.Empty,
                        e.CellValue2?.ToString() ?? string.Empty);
                }

                // Отменяем стандартное поведение
                e.Handled = true;
            }
        }


        private void button3_Click_1(object sender, EventArgs e)
        {
            Connect();
            command = new SqlCommand($"SELECT COUNT(*) FROM Comment WHERE [requestID] = {id}", connection);
            int count = (int)command.ExecuteScalar();

            if (count >= 0)
            {
                DialogResult result = MessageBox.Show("Вы точно хотите удалить этот запрос?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    command = new SqlCommand($"DELETE FROM Request WHERE [requestID] = {id}", connection);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Запрос успешно удален.", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Удаление отменено.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Запрос с указанным ID не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dataGridView1.Rows.Clear();
            Add();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Authorization authorization = new Authorization();
            authorization.ShowDialog();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UpdateOper updateOper = new UpdateOper(id1, start, type1, problem, status, finish, parts, master, client, massage);
            updateOper.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Connect();
            command = new SqlCommand($"SELECT COUNT(*) FROM Comment WHERE [requestID] = {id1}", connection);
            int count = (int)command.ExecuteScalar();

            if (count >= 0)
            {
                DialogResult result = MessageBox.Show("Вы точно хотите удалить этот запрос?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    command = new SqlCommand($"DELETE FROM Request WHERE [requestID] = {id1} and [statusID] = 1", connection);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Запрос успешно удален.", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Удаление отменено.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Запрос с указанным ID не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dataGridView2.Rows.Clear();
            Add1();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
    
}
