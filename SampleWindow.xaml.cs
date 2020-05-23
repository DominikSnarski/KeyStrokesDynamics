using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SampleWindow : Window
    {
        private Sample sample;
        private SampleTime temporary;
        private Repository repository;
        public SampleWindow()
        {
            InitializeComponent();
            repository = new Repository();
            sample = new Sample();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            temporary = new SampleTime();

            if(Name.Text != null)
            {
                sample.Name = Name.Text;
            }
            else
            {
                sample.Name = "Anonymous";
            }

            temporary.Time = DateTime.Now.Ticks;
            temporary.Code = (int)e.Key;
            Debug.WriteLine("Code {0}, Time {1}", temporary.Code, temporary.Time);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (temporary == null) return;
            temporary.Time = DateTime.Now.Ticks - temporary.Time;
            Debug.WriteLine("Code {0}, Time {1}", temporary.Code, temporary.Time);
            SaveSample();
        }

        private void SaveSample()
        {
            SampleTime sam = sample.SampleTimes.Where(t => t.Code.Equals(temporary.Code)).FirstOrDefault();

            if(sam != null)
            {
                temporary.Time = (sam.Time + temporary.Time) / 2;
                sample.SampleTimes.Remove(sam);
                sample.SampleTimes.Add(temporary);
            }
            else
            {
                sample.SampleTimes.Add(temporary);
            }

            temporary = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sample.SampleTimes.Count > 0)
            repository.Add(sample);

            this.Close();
        }
    }
}
