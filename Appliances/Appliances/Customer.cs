using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;

using QRCoder;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Drawing.Imaging;

namespace Appliances
{
    public partial class Customer : Form
    {

        static SqlConnection connection;
        SqlCommand command;
        public string Type { get; set; }
        public string Fio { get; set; }
        public int User { get; set; }
        public int ID {  get; set; }
        public string TypeEquipment { get; set; }
        public string problem { get; set; }
        public Customer()
        {
            InitializeComponent();
            Add();
            Add1();
        }
        public Customer(string type, string fio, int user) : this()
        {
            this.Type = type;
            this.Fio = fio;
            this.User = user;
            label1.Text = this.Type;
            label2.Text = this.Fio;

            GenerateQRCode(user);
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
                r.problemDescryption, 
                a.requestStatus, 
                r.completionDate, 
                b.repairParts, 
                r.masterID, 
                r.clientID
             FROM Request r
            INNER JOIN HomeTech h ON r.typeEqipmentID = h.typeEqipmentID
            LEFT JOIN [Status] a ON r.statusID = a.statusID
            LEFT JOIN Parts b ON r.partsID = b.partsID
            WHERE r.clientID = '{User}' AND r.statusID <> {2}";

            SqlCommand command = new SqlCommand(query, connection);

            SqlDataReader reader = command.ExecuteReader();

            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[7]);

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
            }
            reader.Close();
            dataGridView1.DataSource = null;
            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
            int totalRecords = data.Count;
            command = new SqlCommand($"SELECT Count(*) FROM Request WHERE clientID = '{User}'", connection);
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
                r.problemDescryption, 
                a.requestStatus, 
                r.completionDate, 
                b.repairParts, 
                r.masterID, 
                r.clientID
             FROM Request r
            INNER JOIN HomeTech h ON r.typeEqipmentID = h.typeEqipmentID
            LEFT JOIN [Status] a ON r.statusID = a.statusID
            LEFT JOIN Parts b ON r.partsID = b.partsID
            WHERE r.clientID = '{User}' AND r.statusID = {2}";

            SqlCommand command = new SqlCommand(query, connection);

            SqlDataReader reader = command.ExecuteReader();

            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[7]);

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
            }
            reader.Close();
            dataGridView2.DataSource = null;
            foreach (string[] s in data)
                dataGridView2.Rows.Add(s);

            int Records = data.Count;
            command = new SqlCommand($"SELECT Count(*) FROM Request WHERE clientID = '{User}'", connection);
            int Records1 = (int)command.ExecuteScalar();
            connection.Close();
            label4.Text = "Количество записей: " + Records + " из " + Records1;
        }

        private void Customer_Load(object sender, EventArgs e)
        {
            Add();
            Add1();
        }

        private void RefreshDataGridView()
        {
            Connect();
            command = new SqlCommand($"SELECT * FROM Request WHERE requestID = '{User}'", connection);
            command.ExecuteNonQuery();
           
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            АpplicationAdd аpplicationAdd = new АpplicationAdd(User);
            аpplicationAdd.ShowDialog();
            dataGridView1.Rows.Clear();
            Add();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            АpplicationСhange аpplicationChange = new АpplicationСhange(ID, TypeEquipment, problem);
            аpplicationChange.ShowDialog();
            dataGridView1.Rows.Clear();
            Add();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Authorization authorization = new Authorization();
            authorization.ShowDialog();
            this.Close();
        }
        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            TypeEquipment = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            problem = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
        }


        private void GenerateQRCode(int userID)
        {
            string feedbackFormUrl = GetFeedbackFormUrl(userID);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(feedbackFormUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20); 

            pictureBox2.Image = qrCodeImage;
        }

        private string GetFeedbackFormUrl(int userID)
        {
            string feedbackFormUrl = "https://rutube.ru/video/75e49fcac59674ba9ea4dc3653e668e5/?playlist=418518&playlistPage=1" + userID; 
            return feedbackFormUrl;
        }

    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
