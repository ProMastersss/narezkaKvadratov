using System;
using System.Collections.Generic;
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
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;


namespace Нарезка_квадратов
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int wPole = 20, hPole = 16; //[высота, ширина]
        private int[,] pole = new int[hPole, wPole];
        private static int WidthCanvas = 1280;
        private static int HeightCanvas = 1024;
        private Rectangle prjamougolnic;
        private int widthKvadrat = WidthCanvas/wPole , heightKvadrat = HeightCanvas/hPole;
        private Random r = new Random((int)DateTime.Now.Ticks);
        private System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
        private OpenFileDialog opendialog = new OpenFileDialog();
        private System.Windows.Forms.FolderBrowserDialog folderDialod = new System.Windows.Forms.FolderBrowserDialog(); 
        private String putSave;
        private String putFolder;
        private String[] listFiles;
        private StreamWriter file;
        private System.Drawing.Bitmap table;
        private System.Drawing.Graphics gfxTable;
        public MainWindow()
        {
            InitializeComponent();
            //AllocConsole();

            risCanvas();

            for(int i = 0; i<hPole; i++)
            {
                for (int j = 0; j < wPole; j++)
                    pole[i, j] = 0;
            }
        }

        private void risCanvas()
        {
            canvas.Width = WidthCanvas;
            canvas.Height = HeightCanvas;
            int y = 100, x = 0;

            for (int i = 0; i < hPole; i++)
            {
                for (int j = 0; j < wPole; j++)
                {
                    prjamougolnic = new Rectangle();
                    prjamougolnic.Width = 30;
                    prjamougolnic.Height = 30;
                    prjamougolnic.Stroke = Brushes.LightGray;
                    Canvas.SetLeft(prjamougolnic, y);
                    Canvas.SetTop(prjamougolnic, x);
                    canvas.Children.Add(prjamougolnic);
                    y += 30;
                }
                x += 30;
                y = 100;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            for (int im = 0; im < listFiles.Length; im++)
            {
                table = new System.Drawing.Bitmap(1281, 1025);
                gfxTable = System.Drawing.Graphics.FromImage(table);
                canvas.Children.Clear();
                risCanvas();
                Directory.CreateDirectory(putSave + @"\Level" + int.Parse(textBoxLevel.Text));
                file = new StreamWriter(putSave + @"\Level" + int.Parse(textBoxLevel.Text) + @"\Level" + int.Parse(textBoxLevel.Text) + ".txt");

                int narisKvadratov = 0; //Колличество нарисованных квадратов
                for (int i = 0; i < hPole; i++)
                    for (int j = 0; j < wPole; j++)
                    {
                        if (pole[i, j] == 0)
                        {
                            narisKvadratov++;
                            risuem(i, j, narisKvadratov, listFiles[im]);
                        }
                    }
                gfxTable.Dispose();
                file.Close();

                //Сохраняем разметку
                table.Save(putSave + @"\Level" + textBoxLevel.Text + @"\razmetka.png", System.Drawing.Imaging.ImageFormat.Png);
                table.Dispose();
                table = new System.Drawing.Bitmap(listFiles[im]);
                table.Save(putSave + @"\Level" + textBoxLevel.Text + @"\success.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                table.Dispose();
                textBoxLevel.Text = (Int32.Parse(textBoxLevel.Text) + 1).ToString();

                // Вывести массив
                if (true)
                {
                    for (int i = 0; i < hPole; i++)
                    {
                        for (int j = 0; j < wPole; j++)
                        {
                            pole[i, j] = 0;
                        }
                    }
                }
                
            }
        }

        private int wKvadrat(int a, int h)
        {
            int i, svobod = 0;
            do
            {
                svobod = 0;
                do
                {
                    i = r.Next(int.Parse(textBoxMin.Text), int.Parse(textBoxMax.Text) + 1);
                } while (!(wPole - a >= i && wPole - (a + i) >= int.Parse(textBoxMin.Text) || wPole - (a + i) == 0));

                for (int n = a + i; n < wPole; n++)
                {
                    if (pole[h, n] == 0) svobod++;
                    else break;
                }

            } while (!(svobod >= int.Parse(textBoxMin.Text) || svobod == 0));
            return i;
        }

        private int hKvadrat(int a)
        {
            int i;
            do
            {
                i = r.Next(int.Parse(textBoxMin.Text), int.Parse(textBoxMax.Text) + 1);
            } while (!(hPole - a >= i && hPole - (a + i) >= int.Parse(textBoxMin.Text) || hPole - (a + i) == 0));

            return i;
        }

        private void risuem(int i, int j, int kol, String pathImage)
        {
            bool b = false;
            int w=0, h=0;
            do
            {
                b = false;
                w = wKvadrat(j, i);
                h = hKvadrat(i);
                for (int n = j; n < j + w; n++)
                {
                    if (pole[i, n] != 0)
                        b = true;
                }
                for (int n = i; n < i + h; n++)
                {
                    if (pole[n, j] != 0)
                        b = true;
                }
            } while (b);

            for (int n = i; n < i + h; n++)
                for(int p = j; p < j + w; p++)
                {
                    pole[n, p] = kol;
                }

            //Рисуем на канве
            prjamougolnic = new Rectangle();
            prjamougolnic.Width = 30 * w;
            prjamougolnic.Height = 30 * h;
            prjamougolnic.Stroke = Brushes.Black;
            Canvas.SetLeft(prjamougolnic, 100 + (j * 30));
            Canvas.SetTop(prjamougolnic, 0 + (i * 30));

            //Пишем текст

            VisualBrush myBrush = new VisualBrush();
            StackPanel aPanel = new StackPanel();
            TextBlock someText = new TextBlock();
            someText.Text = kol.ToString();
            FontSizeConverter fSizeConverter = new FontSizeConverter();
            someText.FontSize = (double)fSizeConverter.ConvertFromString("16pt");
            someText.Margin = new Thickness(10);
            aPanel.Children.Add(someText);
            myBrush.Visual = aPanel;
            prjamougolnic.Fill = myBrush;
            canvas.Children.Add(prjamougolnic);

            //Записываем в файл
            file.Write((j * widthKvadrat + 2) + "," + (i * heightKvadrat + 2)+ ",");
            //Обрезаем изображение
            System.Drawing.Bitmap bmp = Crop(pathImage, w * widthKvadrat - 4, h * heightKvadrat - 4, j * widthKvadrat + 4, i * heightKvadrat + 4);
            bmp.Save(putSave + @"\Level" + textBoxLevel.Text + @"\image" + kol + ".png", System.Drawing.Imaging.ImageFormat.Png);
            bmp.Dispose();
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, 4);
            gfxTable.DrawRectangle(pen, new System.Drawing.Rectangle(j * widthKvadrat, i * heightKvadrat, w * widthKvadrat, h * heightKvadrat));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            dialog.ShowDialog();
            putSave = dialog.SelectedPath;
        }

        private System.Drawing.Bitmap Crop(string img, int width, int height, int x, int y)
        {
            try
            {
                    System.Drawing.Image image = System.Drawing.Image.FromFile(img);
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    bmp.SetResolution(80, 60);

                    System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(bmp);
                    gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    gfx.DrawImage(image, new System.Drawing.Rectangle(0, 0, width, height), x, y, width, height, System.Drawing.GraphicsUnit.Pixel);
                gfx.Dispose();
                image.Dispose();
                    return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            folderDialod.ShowDialog();
            putFolder = folderDialod.SelectedPath;
            listFiles = Directory.GetFiles(putFolder, "*.jpg");
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();
    }
}
