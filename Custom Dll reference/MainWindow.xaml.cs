using Alternet.Editor.Wpf;
using Alternet.Scripter;
using Alternet.Syntax.Parsers.Roslyn;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Custom_Dll_reference
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private ScriptRun scriptRun = new ScriptRun();
        private TextSource csharpSource = new TextSource();
        private CsParser csParser1 = new CsParser();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            csharpSource.Text = ReadTxt();
            scriptRun.ScriptSource.WithDefaultReferences();
            scriptRun.AssemblyKind = ScriptAssemblyKind.DynamicLibrary;
            csharpSource.Lexer = csParser1;
            this.edit.Source = csharpSource;
        }
        private  string ReadTxt()
        {
            var txt = "";
            try
            {
                var path = @"Template\Template.txt";
                StreamReader sr = new StreamReader(path, Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    txt = txt + line.ToString() + "\r\n";
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
            return txt;
        }

        private void btn_adddll_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "file (.dll)|*.dll",
                Title = "Add  a dll File..."
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                var fileinfo = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                var filepath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                scriptRun.ScriptSource.SearchPaths.Add(filepath);
                csParser1.Repository.AddFileReference(openFileDialog.FileName);
                csParser1.Repository.RegisterAssembly(fileinfo);
                scriptRun.ScriptSource.References.Add(fileinfo);

            }
        }

        private void btn_Compile_Click(object sender, RoutedEventArgs e)
        {
            scriptRun.ScriptSource.FromScriptCode(edit.Text);
            scriptRun.ScriptSource.WithDefaultReferences();
            scriptRun.ScriptSource.References.Add("TestDll");
            scriptRun.AssemblyKind = ScriptAssemblyKind.DynamicLibrary;
            bool generateModulesOnDisk = scriptRun.ScriptHost.GenerateModulesOnDisk;
            scriptRun.ScriptHost.GenerateModulesOnDisk = true;
            bool result = false;
            try
            {
                result = scriptRun.Compile();
            }
            finally
            {
                scriptRun.ScriptHost.GenerateModulesOnDisk = generateModulesOnDisk;
            }
            if (scriptRun.ScriptHost.CompileFailed)
            {
                if (scriptRun.ScriptHost.CompilerErrors.Length > 0)
                {
                    var info = "";
                    foreach (var item in scriptRun.ScriptHost.CompilerErrors)
                    {
                        info += item.Message+"\r\n";
                    }
                    MessageBox.Show(info);
                }
            }
            else
            {
                MessageBox.Show("Compiled successfully...");
            }
            scriptRun.Dispose();
        }

        private bool Build()
        {
            scriptRun.ScriptSource.FromScriptCode(edit.Text);
            scriptRun.ScriptSource.WithDefaultReferences();
            scriptRun.ScriptSource.References.Add("TestDll");
            scriptRun.AssemblyKind = ScriptAssemblyKind.DynamicLibrary;
            bool generateModulesOnDisk = scriptRun.ScriptHost.GenerateModulesOnDisk;
            scriptRun.ScriptHost.GenerateModulesOnDisk = true;
            bool result = false;
            try
            {
                result = scriptRun.Compile();

            }
            finally
            {
                scriptRun.ScriptHost.GenerateModulesOnDisk = generateModulesOnDisk;
            }
 
            if (scriptRun.ScriptHost.CompileFailed)
            {
                if (scriptRun.ScriptHost.CompilerErrors.Length > 0)
                {
                    var info = "";
                    foreach (var item in scriptRun.ScriptHost.CompilerErrors)
                    {
                        info += item.Message + "\r\n";
                    }
                    MessageBox.Show(info);
                }
            }
            else
            {
               //  MessageBox.Show("Compiled successfully...");
            }
            scriptRun.Dispose();
            return result;
        }
        private void btn_Run_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() => {
                if (Build())
                {
                    // scriptRun.Run();
                    try
                    {
                        var c = scriptRun.RunMethod("Run", null, new object[] { 10 });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
  
                }
            }));
         

        }
    }
}
