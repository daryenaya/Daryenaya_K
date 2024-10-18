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
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Appliances
{
    public partial class Authorization : Form
    {
        Passcs passcs = new Passcs();
        private string text = "";
        static SqlConnection connection;
        SqlCommand command;
        public Authorization()
        {
            InitializeComponent();
            textBox3.Visible = false;
            button2.Visible = false;
            //pictureBox1.Visible = false;

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

        int errors = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            Connect();
            command = new SqlCommand($"SELECT * FROM Customer WHERE login = '{textBox1.Text}'And password = '{textBox2.Text}'", connection);
            if (Convert.ToInt32(command.ExecuteScalar()) == 0 || text != textBox3.Text)
            {
                textBox3.Visible = true;
                button2.Visible = true;
                pictureBox1.Visible = true;
                command = new SqlCommand($"SELECT userID FROM Customer WHERE login = '{textBox1.Text}'", connection);
                int login = Convert.ToInt32(command.ExecuteScalar().ToString());
                command = new SqlCommand($"INSERT INTO LoginAttempts ([userID], [attemptTime], [successful])" +
                   $"VALUES('{login}',GETDATE(), 'Ошибочная попытка входа')", connection);
                command.ExecuteNonQuery();
                if (errors == 0)
                {
                    pictureBox1.Image = this.CreateImage(pictureBox1.Width, pictureBox1.Height);
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Text = "";
                    textBox2.Text = "";
                    errors++;
                }
                else if (errors == 1)
                {
                    if (textBox3.Text != text)
                    {
                        MessageBox.Show("Неверный ввод капчи", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    textBox3.Text = "";
                    pictureBox1.Image = this.CreateImage(pictureBox1.Width, pictureBox1.Height);
                    errors++;
                }
                else if (errors == 2)
                {
                    if (textBox3.Text != text)
                    {
                        MessageBox.Show("Неверный ввод капчи", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        errors++;
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox1.Text = "";
                        textBox1.Text = "";
                        errors++;
                    }
                    textBox3.Text = "";
                    pictureBox1.Image = this.CreateImage(pictureBox1.Width, pictureBox1.Height);
                    timer1.Start();
                    button1.Enabled = false;
                    MessageBox.Show("Было произведено много неудачных попыток входа. Следующая попытка будет предоставлена по истечении 3 минут", "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Перезапустите приложение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else
            {
                textBox3.Visible = false;
                button2.Visible = false;
                DataTable data = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command.CommandText, connection);
                adapter.Fill(data);
                foreach (DataRow row in data.Rows)
                {
                    int typeID = Convert.ToInt32(row.Field<int>("typeID"));
                    int userID = Convert.ToInt32(row.Field<int>("userID"));
                    command = new SqlCommand($"INSERT INTO LoginAttempts ([userID], [attemptTime], [successful])" +
                    $"VALUES('{userID}',GETDATE(), 'Успешно')", connection);
                    command.ExecuteNonQuery();
                    string fio = row.Field<string>("fio");
                    command = new SqlCommand($"SELECT type FROM Type WHERE typeID = {typeID}", connection);
                    string query = command.ExecuteScalar().ToString();
                    switch (typeID)
                    {
                        case 1:
                            Manager manager = new Manager(query, fio, userID);
                            manager.ShowDialog();
                            break;
                        case 2:
                            Operator operation = new Operator(query, fio, userID);
                            operation.ShowDialog();
                            break;
                        case 3:
                            Master master = new Master(query, fio, userID);
                            master.ShowDialog();
                            break;
                        case 4:
                            Customer customer = new Customer(query, fio, userID);
                            customer.ShowDialog();
                            break;
                    }
                }
            }
        }
        private Bitmap CreateImage(int Width, int Height)
        {
            Random rnd = new Random();

            //Создадим изображение
            Bitmap result = new Bitmap(Width, Height);

            //Вычислим позицию текста
            int Xpos = rnd.Next(0, Width - 50);
            int Ypos = rnd.Next(15, Height - 15);

            //Добавим различные цвета
            Brush[] colors = { Brushes.Black,
                     Brushes.Red,
                     Brushes.RoyalBlue,
                     Brushes.Green };

            //Укажем где рисовать
            Graphics g = Graphics.FromImage(result);

            //Пусть фон картинки будет серым
            g.Clear(Color.Gray);

            //Сгенерируем текст
            text = String.Empty;
            string ALF = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
            for (int i = 0; i < 5; ++i)
                text += ALF[rnd.Next(ALF.Length)];


            //Нарисуем сгенирируемый текст
            g.DrawString(text,
                         new Font("Arial", 15),
                         colors[rnd.Next(colors.Length)],
                         new PointF(Xpos, Ypos));

            //Добавим немного помех
            /////Линии из углов
            g.DrawLine(Pens.Black,
                       new Point(0, 0),
                       new Point(Width - 1, Height - 1));
            g.DrawLine(Pens.Black,
                       new Point(0, Height - 1),
                       new Point(Width - 1, 0));
            ////Белые точки
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    if (rnd.Next() % 20 == 0)
                        result.SetPixel(i, j, Color.White);

            return result;
        }
        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.PasswordChar = '\0'; 
            }
            else
            {
                textBox2.PasswordChar = '*'; 
            }
        }

        private void Authorization_Load(object sender, EventArgs e)
        {
            //pictureBox1.Image = this.CreateImage(pictureBox1.Width, pictureBox1.Height);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = this.CreateImage(pictureBox1.Width, pictureBox1.Height);
        }
        int sec = 10; //3 min

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (sec > 0)
            {
                sec--;
                button1.Enabled = false;
                textBox3.Visible = false;
                button2.Visible = false;
                pictureBox1.Visible = false;
            }
            else
            {
                timer1.Stop();
                sec = 180;
                button1.Enabled = true;
                textBox3.Visible = false;
                button2.Visible = false;
                pictureBox1.Visible = false;

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
