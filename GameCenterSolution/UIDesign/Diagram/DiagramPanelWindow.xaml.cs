using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace UIDesign.Diagram
{
    /// <summary>
    /// Interaction logic for DiagramPanelWindow.xaml
    /// </summary>
    public partial class DiagramPanelWindow : Window
    {
        public DiagramPanelWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "jpg | *.jpg";
            if (dialog.ShowDialog() != true) return;

            string filePath = dialog.FileName;
            Size imgSize = DiagramBorder.RenderSize;
            RenderTargetBitmap bitmap = new RenderTargetBitmap((Int32)imgSize.Width, (Int32)imgSize.Height, 96.0, 96.0, PixelFormats.Pbgra32);
            bitmap.Render(DiagramBorder);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                encoder.Save(fs);
            }
        }
    }
}
