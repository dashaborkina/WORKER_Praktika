using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using System.IO;


namespace WORKER
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<worker1> w;
        private worker_Entities _worker = new worker_Entities();
        worker_Entities db;
        public MainWindow()
        {
            InitializeComponent();
            db = new worker_Entities();
            db.worker1.Load();
            Grid.ItemsSource = db.worker1.Local.ToBindingList();
            w = db.worker1.Local.ToList<worker1>() as List<worker1>;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Неверный ввод данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Grid.SelectedItems.Count > 0)
            {
                for (int i = 0; i < Grid.SelectedItems.Count; i++)
                {
                    worker1 us = Grid.SelectedItems[i] as worker1;
                    if (us != null)
                    {
                        db.worker1.Remove(us);
                    }
                }
            }
            db.SaveChanges();
        }

        private void searchTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            db = new worker_Entities();

            db.worker1.Where(u => u.name.ToString().Contains(searchTBox.Text)).Load();
            db.worker1.Where(u => u.post.ToString().Contains(searchTBox.Text)).Load();
            db.worker1.Where(u => u.exp.ToString().Contains(searchTBox.Text)).Load();
            Grid.ItemsSource = db.worker1.Local.ToBindingList();

            if (searchTBox.Text.Length == 0)
                db.worker1.Load();
        }

        private void reportButton_Click(object sender, RoutedEventArgs e)
        {
            var allWorkers = _worker.worker1.OrderBy(p => p.name).ToList();
            var application = new Word.Application();

            Word.Document document = application.Documents.Add();
            Word.Paragraph paragraph = document.Paragraphs.Add();
            Word.Range userRange = paragraph.Range;

            userRange.Text = "Работники";
            userRange.InsertParagraphAfter();
            Word.Paragraph tableparagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableparagraph.Range;

            Word.Table infoTable = document.Tables.Add(tableRange, allWorkers.Count(), 3);

            infoTable.Borders.InsideLineStyle = infoTable.Borders.OutsideLineStyle
                    = Word.WdLineStyle.wdLineStyleSingle;
            infoTable.Range.Cells.VerticalAlignment
                    = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            Word.Range cellRange;
            cellRange = infoTable.Cell(1, 1).Range;
            cellRange.Text = "Фамилия";
            cellRange = infoTable.Cell(1, 2).Range;
            cellRange.Text = "Должность";
            cellRange = infoTable.Cell(1, 3).Range;
            cellRange.Text = "Стаж";

            infoTable.Rows[1].Range.Bold = 1;
            for (int i = 0; i < allWorkers.Count(); i++)
            {
                cellRange = infoTable.Cell(i + 2, 1).Range;
                cellRange.Text = allWorkers[i].name;
                cellRange = infoTable.Cell(i + 2, 2).Range;
                cellRange.Text = allWorkers[i].post;
                cellRange = infoTable.Cell(i + 2, 3).Range;
                cellRange.Text = allWorkers[i].exp.ToString();
            }

            application.Visible = true;
            document.SaveAs2(@"C:\Users\Dasha Borkina\Desktop.docx");
        }

        private void jsonButton_Click(object sender, RoutedEventArgs e)
        {
            //Making a json file
            File.WriteAllText("input.json", string.Empty);
            foreach (worker1 worker1 in w)
            {
                Worker wd = new Worker()
                {
                    name = worker1.name,
                    post = worker1.post,
                    exp = Convert.ToInt32(worker1.exp)
                };
                File.AppendAllText("input.json", JsonConvert.SerializeObject(wd));
            }

            //Reading a json file
            List<Worker> ww = new List<Worker>();
            JsonTextReader reader = new JsonTextReader(new StreamReader("input.json"));
            reader.SupportMultipleContent = true;
            while (reader.Read())
            {
                JsonSerializer serializer = new JsonSerializer();
                Worker point = serializer.Deserialize<Worker>(reader);
                ww.Add(point);
            }
            string d = "";
            foreach (Worker www in ww)
            {
                d += www.name + " " + www.post + " " + www.exp + Environment.NewLine;
            }
            MessageBox.Show(d);
        }

        class Worker
        {
            public string name { get; set; }
            public string post { get; set; }
            public int exp { get; set; }

        }
    }

}
