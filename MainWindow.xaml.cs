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
    public partial class MainWindow : Window
    {
        private Sample sample;
        private SampleTime temporary;
        private Repository repository;

        public MainWindow()
        {
            InitializeComponent();
            repository = new Repository();
            sample = new Sample();
        }

        private void TakeSample(object sender, RoutedEventArgs e)
        {
            SampleWindow sampleWindow = new SampleWindow();
            sampleWindow.Show();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            temporary = new SampleTime();

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

            if (sam != null)
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

        private void Reset(object sender, RoutedEventArgs e)
        {
            NameLabelE.Content = "";
            NameLabelM.Content = "";
            NameLabelC.Content = "";
            TB1.Text = "";
            TB2.Text = "";
            TBK.Text = "";
            sample = new Sample();
            EffectiveLabel.Content = "";
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void KNN(object sender, RoutedEventArgs e)
        {
            int k = 2;
            if (sample.SampleTimes.Count == 0) return;
            if (TBK.Text != null)
            {
                try
                {
                    k = Int32.Parse(TBK.Text);
                }
                catch
                {
                    k = 2;
                }
            }

            var list = repository.GetQueryable().ToList();

            var euklidList = Euklid(list, sample).OrderBy(s => s.Lenght).Take(k);
            var manhattanList = Manhattan(list, sample).OrderBy(s => s.Lenght).Take(k);
            var czebyszewList = Czebyszew(list, sample).OrderBy(s => s.Lenght).Take(k);

            NameLabelE.Content = FindName(euklidList.ToList());
            NameLabelM.Content = FindName(manhattanList.ToList());
            NameLabelC.Content = FindName(czebyszewList.ToList());
        }

        private string FindName(List<ComparisonResult> list)
        {
            string name = "";
            List<ComparisonResult> classList = new List<ComparisonResult>();
            foreach(ComparisonResult c in list)
            {
                if (classList.Where(cl => cl.Class.Equals(c.Class)).FirstOrDefault() == null)
                    classList.Add(new ComparisonResult() { Class = c.Class, Lenght = 1 });
                else
                    classList.Find(cl => cl.Class.Equals(c.Class)).Lenght++;
            }

            var max = classList.Max(c => c.Lenght);
            var al = classList.FindAll(c => c.Lenght.Equals(max));
            if (al.Count > 1)
            {
                long min = -1;
                foreach(ComparisonResult comparison in al)
                {
                    var num = list.FindAll(c => c.Class.Equals(comparison.Class)).Sum(c => c.Lenght);

                    if(min == -1)
                    {
                        name = comparison.Class;
                        min = num;
                    }
                    else if(num < min)
                    {
                        name = comparison.Class;
                        min = num;
                    }

                }
            }
            else
            {
                name = al.FirstOrDefault().Class;
            }

            return name;
        }

        private List<ComparisonResult> Euklid(List<Sample> samples, Sample sample)
        {
            List<ComparisonResult> list = new List<ComparisonResult>();

            int ns = sample.SampleTimes.Count;

            foreach(Sample s in samples)
            {
                double result = 0;
                for(int i=0; i<Math.Min(s.SampleTimes.Count,ns);i++)
                {
                    var x = sample.SampleTimes[i].Time - s.SampleTimes[i].Time;
                    result += Math.Pow(x,2);
                }
                list.Add(new ComparisonResult() { Class = s.Name, Lenght = (long)Math.Sqrt(result) });
            }

            return list;

        }

        private List<ComparisonResult> Manhattan(List<Sample> samples, Sample sample)
        {
            List<ComparisonResult> list = new List<ComparisonResult>();

            int ns = sample.SampleTimes.Count;

            foreach (Sample s in samples)
            {
                double result = 0;
                for (int i = 0; i < Math.Min(s.SampleTimes.Count, ns); i++)
                {
                    var x = sample.SampleTimes[i].Time - s.SampleTimes[i].Time;
                    result += Math.Abs(x);
                }
                list.Add(new ComparisonResult() { Class = s.Name, Lenght = (long)result });
            }

            return list;

        }

        private List<ComparisonResult> Czebyszew(List<Sample> samples, Sample sample)
        {
            List<ComparisonResult> list = new List<ComparisonResult>();

            int ns = sample.SampleTimes.Count;

            foreach (Sample s in samples)
            {
                List<long> listResults = new List<long>();
                for (int i = 0; i < Math.Min(s.SampleTimes.Count, ns); i++)
                {
                    var x = sample.SampleTimes[i].Time - s.SampleTimes[i].Time;
                    listResults.Add(Math.Abs(x));
                }
                list.Add(new ComparisonResult() { Class = s.Name, Lenght = listResults.Max() });
            }

            return list;

        }

        private void Effectiveness(object sender, RoutedEventArgs e)
        {
            var list = repository.GetQueryable().ToList();

            if (list.Count < 2) return;

            int samples = list.Count();
            int correctSamples = 0;

            foreach(Sample sam in list)
            {
                var lis = list.ToList();
                lis.Remove(sam);
                bool cr = KNN_Ef(sam, lis);
                if (cr) correctSamples++;
            }

            double result = ((double)correctSamples) / ((double)samples);
            EffectiveLabel.Content = result.ToString();

        }

        private bool KNN_Ef(Sample sample,List<Sample> list)
        {
            int k = 2;
            if (sample.SampleTimes.Count == 0) return false;
            if (TBK.Text != null)
            {
                try
                {
                    k = Int32.Parse(TBK.Text);
                }
                catch
                {
                    k = 2;
                }
            }

            //var list = repository.GetQueryable().ToList();

            var euklidList = Euklid(list, sample).OrderBy(s => s.Lenght).Take(k);
            var manhattanList = Manhattan(list, sample).OrderBy(s => s.Lenght).Take(k);
            var czebyszewList = Czebyszew(list, sample).OrderBy(s => s.Lenght).Take(k);

            var NameE = FindName(euklidList.ToList());
            var NameM = FindName(manhattanList.ToList());
            var NameC = FindName(czebyszewList.ToList());

            int correct = 0;

            if (NameE.Equals(sample.Name)) correct++;
            if (NameM.Equals(sample.Name)) correct++;
            if (NameC.Equals(sample.Name)) correct++;

            if (correct >= 2) return true;
            else return false;
        }
    }
}
