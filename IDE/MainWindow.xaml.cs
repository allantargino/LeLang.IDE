using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace IDE
{
    public partial class MainWindow : Window
    {
        private int _lastLines = 0;
        public LineCount LineCount { get; set; }

        public Examples Examples { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            LineCount = new LineCount();

            Examples = new Examples();
            var example = Examples.GetExample("HelloWorld.le");

            TxtBox.Document.Blocks.Clear();
            foreach (var line in example)
            {
                TxtBox.Document.Blocks.Add(new Paragraph(new Run(line)));
            }
        }

        private void TxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtBox.Document == null)
                return;

            var lines = TxtBox.Document.Blocks.Count;

            if (_lastLines != lines)
            {
                _lastLines = lines;

                var count = string.Empty;
                for (int i = 1; i <= lines; i++)
                {
                    count += i.ToString() + "\n";
                }

                LineCount.Lines = count;
            }
        }

        private void ErrorListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = (Error)ErrorListView.SelectedValue;

            try
            {
                int i = 0;
                foreach (var paragraph in TxtBox.Document.Blocks)
                {
                    i++;
                    var text = new TextRange(paragraph.ContentStart,
                                   paragraph.ContentEnd).Text;

                    if (i == value.Line)
                    {
                        TxtBox.TextChanged -= this.TxtBox_TextChanged;
                        paragraph.FontStyle = FontStyles.Italic;
                        paragraph.Foreground = Brushes.Red;
                        TxtBox.TextChanged += this.TxtBox_TextChanged;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private string GetRichText(RichTextBox txt)
        {
            return new TextRange(txt.Document.ContentStart, txt.Document.ContentEnd).Text;
        }

        private void CompilerC_Click(object sender, RoutedEventArgs e)
        {
            var code = GetRichText(TxtBox);
            LeCompiler.Compile(code);

            Thread.Sleep(2500);


            var result = LeCompiler.GetCompilationResults();

            switch (result)
            {
                case CompilationResult.Success:
                    SetCompiledCode();
                    break;
                case CompilationResult.Error:
                    var handler = new ErrorParser(Settings.ErrorFile);
                    var errors = handler.GetErrors();
                    FillErrorPanel(errors);
                    break;
                default:
                    break;
            }
        }

        private void FillErrorPanel(IEnumerable<Error> errorList)
        {
            ErrorListView.ItemsSource = errorList;
        }

        private void SetCompiledCode()
        {
            var compiledFile = Examples.GetExample("temp.c");
            CompiledBox.Document.Blocks.Clear();
            foreach (var line in compiledFile)
            {
                CompiledBox.Document.Blocks.Add(new Paragraph(new Run(line)));
            }
            ErrorListView.ItemsSource = null;
        }

    }
}
