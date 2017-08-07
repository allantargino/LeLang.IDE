using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
            GetExample("HelloWorld.le");

            MainGrid.RowDefinitions[2].Height = new GridLength(0);
            CodeGrid.ColumnDefinitions[1].Width = new GridLength(0);
        }

        private void LeCodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LeCodeBox.Document == null)
                return;

            var lines = LeCodeBox.Document.Blocks.Count;

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
                CleanStyle(LeCodeBox);
                foreach (var paragraph in LeCodeBox.Document.Blocks)
                {
                    i++;

                    if (i == value.Line)
                    {
                        LeCodeBox.TextChanged -= this.LeCodeBox_TextChanged;
                        paragraph.FontStyle = FontStyles.Italic;
                        paragraph.Foreground = Brushes.Red;
                        LeCodeBox.TextChanged += this.LeCodeBox_TextChanged;
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        #region RichTextMethods

        private string GetRichText(RichTextBox txt)
        {
            return new TextRange(txt.Document.ContentStart, txt.Document.ContentEnd).Text;
        }

        private void SetRichText(RichTextBox txt, IEnumerable<string> content)
        {
            txt.Document.Blocks.Clear();
            foreach (string line in content)
                txt.Document.Blocks.Add(new Paragraph(new Run(line)));
        }

        private void CleanStyle(RichTextBox txt)
        {
            foreach (var paragraph in txt.Document.Blocks)
            {
                paragraph.FontStyle = FontStyles.Normal;
                paragraph.Foreground = Brushes.White;
            }
        }

        #endregion


        private void FillErrorPanel(IEnumerable<Error> errorList)
        {
            ErrorListView.ItemsSource = errorList;
            MainGrid.RowDefinitions[2].Height = new GridLength(100);
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
            MainGrid.RowDefinitions[2].Height = new GridLength(0);
        }

        #region Menu

        #region MenuFile

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                SetRichText(LeCodeBox, File.ReadAllLines(openFileDialog.FileName));
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".le",
                InitialDirectory = Settings.ExamplesDirectory
            };
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, GetRichText(LeCodeBox));
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        #endregion

        #region Build

        private void MenuItem_CompileinC_Click(object sender, RoutedEventArgs e)
        {
            var code = GetRichText(LeCodeBox);
            LeCompiler.Compile(code);

            Thread.Sleep(1500);

            var result = LeCompiler.GetCompilationResults();

            switch (result)
            {
                case CompilationResult.Success:
                    SetCompiledCode();
                    CleanStyle(LeCodeBox);
                    ShowCompiledPanel();
                    break;
                case CompilationResult.Error:
                    var handler = new ErrorParser(Settings.ErrorFile);
                    var errors = handler.GetErrors();
                    FillErrorPanel(errors);
                    CompiledBox.Document.Blocks.Clear();
                    HideCompiledPanel();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region CodeSnippets

        private void MenuItem_HelloWorld_Click(object sender, RoutedEventArgs e)
        {
            GetExample("HelloWorld.le");
        }

        private void MenuItem_ReadWrite_Click(object sender, RoutedEventArgs e)
        {
            GetExample("ReadWrite.le");
        }

        private void MenuItem_Loops_Click(object sender, RoutedEventArgs e)
        {
            GetExample("Loops.le");
        }

        private void MenuItem_Conditions_Click(object sender, RoutedEventArgs e)
        {
            GetExample("Conditions.le");
        }

        private void MenuItem_ErrorDemo_Click(object sender, RoutedEventArgs e)
        {
            GetExample("ErrorDemo.le");
        }

        private void GetExample(string name)
        {
            var example = Examples.GetExample(name);
            SetRichText(LeCodeBox, example);
            HideCompiledPanel();
        }

        private void HideCompiledPanel()
        {
            CodeGrid.ColumnDefinitions[1].Width = new GridLength(0);
        }

        private void ShowCompiledPanel()
        {
            CodeGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
        }

        #endregion

        #endregion


    }
}
