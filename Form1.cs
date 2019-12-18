using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Phonebook
{
    public partial class Form1 : Form
    {
        //public DataTable Table;
        public string Path = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=SPRAV.mdb";

        public Form1()
        {

            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Программа для работы с базой телефонных номеров Администрации города Челябинска v 2.0";
            dataGridView1.DataSource = GetTable();
            dataGridView1.DefaultCellStyle.Font = new Font("Times New Roman", 9F);
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightCyan;
            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Times New Roman", 9F, FontStyle.Bold);
            dataGridView1.Columns[2].DefaultCellStyle.BackColor = Color.LightCyan;
            dataGridView1.Columns[4].DefaultCellStyle.BackColor = Color.LightCyan;
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(System.Globalization.CultureInfo.CreateSpecificCulture("Ru"));
            dataGridView1.Columns[0].Width = 220;
            dataGridView1.Columns[1].Width = 120;
            dataGridView1.Columns[2].Width = 60;

            textBox_FIO.Select(); //установка курсора в строку поиска

        }
        private async void TextBox_FIOTextChanged(object sender, EventArgs e) //поиск по фамилии
        {
            textBox_Podrazdelenie.Clear(); maskedTextBox_Telefon.Clear();
            if (textBox_FIO.Text.Length < 2) return;  //не ищем если знаков меньше 2
            string request = "SELECT Люди.Фамилия, Люди.Имя, Люди.Отчество, IIf([НетТелефон],\"\",[Телефон]) AS Тел1, IIf([НетКабинет],\"\",[Кабинет]) AS Каб, Должности.Должность, Подразделения.Подразделения1, Структура.Адрес " +
    "FROM (Стили RIGHT JOIN (Подразделения INNER JOIN Структура ON Подразделения.Код_подразделения = Структура.КодПодразделения) ON Стили.Стиль = Структура.Стиль) INNER JOIN ((Люди LEFT JOIN Должности ON Люди.КодДолжности = Должности.Код_должности) INNER JOIN Справочник ON Люди.код_Сотрудника = Справочник.Код_сотрудника) ON Структура.КодСтруктуры = Справочник.КодСтруктуры " +
    "WHERE [Люди.Фамилия] Like '" + textBox_FIO.Text + "%' order by [Люди.Имя]";
            DataTable tab = new DataTable();
            await Task.Run(() => tab = GetTable(request));
            dataGridView1.DataSource = tab;
            dataGridView1[0, 0].Selected = false;
        }
        private async void TextBox_PodrazdelenieTextChanged(object sender, EventArgs e) //поиск по организации
        {

            textBox_FIO.Clear(); maskedTextBox_Telefon.Clear();
            if (textBox_Podrazdelenie.Text.Length < 4) return;   //не ищем если знаков меньше 2
            string request = "SELECT Люди.Фамилия, Люди.Имя, Люди.Отчество, IIf([НетТелефон],\"\",[Телефон]) AS Тел1, IIf([НетКабинет],\"\",[Кабинет]) AS Каб, " +
    "Должности.Должность, Подразделения.Подразделения1, Структура.Адрес FROM (Стили RIGHT JOIN (Подразделения INNER JOIN Структура ON Подразделения.Код_подразделения = Структура.КодПодразделения) ON Стили.Стиль = Структура.Стиль) INNER JOIN ((Люди LEFT JOIN Должности ON Люди.КодДолжности = Должности.Код_должности) INNER JOIN Справочник ON Люди.код_Сотрудника = Справочник.Код_сотрудника) ON Структура.КодСтруктуры = Справочник.КодСтруктуры " +
    "WHERE [Подразделения.Подразделения1] Like '%" + textBox_Podrazdelenie.Text + "%' order by [Фамилия]";
            DataTable tab = new DataTable();
            await Task.Run(() => tab = GetTable(request));
            dataGridView1.DataSource = tab;
            dataGridView1[0, 0].Selected = false;

        }
        private async void MaskedTextBox_Telefon_TextChanged(object sender, EventArgs e) //поиск по номеру телефона
        {
            textBox_FIO.Clear(); textBox_Podrazdelenie.Clear();
            if (maskedTextBox_Telefon.Text.Length < 3) return;  //не ищем если знаков меньше 2
            string Request = "SELECT Люди.Фамилия, Люди.Имя, Люди.Отчество, IIf([НетТелефон],\"\",[Телефон]) AS Тел1, IIf([НетКабинет],\"\",[Кабинет]) AS Каб, Должности.Должность, Подразделения.Подразделения1, Структура.Адрес " +
                    "FROM (Стили RIGHT JOIN (Подразделения INNER JOIN Структура ON Подразделения.Код_подразделения = Структура.КодПодразделения) ON Стили.Стиль = Структура.Стиль) INNER JOIN ((Люди LEFT JOIN Должности ON Люди.КодДолжности = Должности.Код_должности) INNER JOIN Справочник ON Люди.код_Сотрудника = Справочник.Код_сотрудника) ON Структура.КодСтруктуры = Справочник.КодСтруктуры " +
                    "WHERE [Телефон] Like '" + maskedTextBox_Telefon.Text + "%' order by [Фамилия]";
            DataTable tab = new DataTable();
            await Task.Run(() => tab = GetTable(Request));
            dataGridView1.DataSource = tab;
            dataGridView1[0, 0].Selected = false;
        }


        private DataTable GetTable(string request = "")  //выдаем таблицу с результатами поиска
        {
            DataTable table = new DataTable();
            table.Columns.Add("ФИО");       // 0
            table.Columns.Add("Телефон"); // 1
            table.Columns.Add("Кабинет");     // 2
            table.Columns.Add("Должность");       // 3
            table.Columns.Add("Подразделение");       // 4
            table.Columns.Add("Адрес");      // 5
            if (request == "") return table;
            OleDbConnection Connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=SPRAV.mdb");
            Connection.Open();
            OleDbCommand Command = new OleDbCommand(request, Connection);
            OleDbDataReader Reader = Command.ExecuteReader(); ;

            while (Reader.Read() == true)
            { // Заполнение Клеток таблицы

                table.Rows.Add(
                    Reader.GetValue(0).ToString().ToUpper() + "\n" + Reader.GetValue(1).ToString() + " " + Reader.GetValue(2).ToString(),  //Фамилия Имя Отчество
                    Reader.GetValue(3),
                    Reader.GetValue(4),
                    Reader.GetValue(5),
                    Reader.GetValue(6),
                    Reader.GetValue(7)
                               );
            }
            Command.Dispose(); Connection.Close();
            return table;
        }

        private void DataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string selectedCell = dataGridView1.CurrentCell.Value.ToString().Trim();
            //string selectedColl = dataGridView1.CurrentCellAddress.ToString();
            string selectedColl = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].HeaderText.ToString(); //получение заголовка столбца
            if (selectedColl == "Кабинет" || selectedColl == "Телефон" || selectedColl == "Подразделение")
            {
                if (selectedColl == "Подразделение") selectedColl = "Подразделения1";
                string Request = "SELECT Люди.Фамилия, Люди.Имя, Люди.Отчество, IIf([НетТелефон],\"\",[Телефон]) AS Тел1, IIf([НетКабинет],\"\",[Кабинет]) AS Каб, Должности.Должность, Подразделения.Подразделения1, Структура.Адрес " +
        "FROM (Стили RIGHT JOIN (Подразделения INNER JOIN Структура ON Подразделения.Код_подразделения = Структура.КодПодразделения) ON Стили.Стиль = Структура.Стиль) INNER JOIN ((Люди LEFT JOIN Должности ON Люди.КодДолжности = Должности.Код_должности) INNER JOIN Справочник ON Люди.код_Сотрудника = Справочник.Код_сотрудника) ON Структура.КодСтруктуры = Справочник.КодСтруктуры " +
        "WHERE [" + selectedColl + "] Like '" + selectedCell + "' order by [Фамилия]";

                dataGridView1.DataSource = GetTable(Request);
            }
        }
        private void Button1_Click(object sender, EventArgs e) //отчистка
        {
            textBox_FIO.Clear(); textBox_Podrazdelenie.Clear(); maskedTextBox_Telefon.Clear();
            dataGridView1.DataSource = GetTable();
            label3.Visible = false;
        }
        private void ОПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа для работы с базой телефонных номеров\nАдминистрации города Челябинска" + "\n© Юрасов Вячеслав Викторович\n+7 (919) 113-24-13\n \n2665603@gmail.com");
        }
    }
}
