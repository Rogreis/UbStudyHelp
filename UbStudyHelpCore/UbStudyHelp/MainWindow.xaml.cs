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
using UbStudyHelp.Classes;
using UrantiaBook.Classes;

namespace UbStudyHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowMessage(string message, bool fatalError= false)
        {
            MessageBox.Show(message);
            if (fatalError)
            {
                System.Windows.Application.Current.Shutdown();
                // Environment.Exit(0)
            }
        }

        private bool LoadData()
        {
            GetDataFiles dataFiles = new GetDataFiles();
            try
            {
                string exePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                exePath = System.IO.Path.Combine(exePath, "TUB_Files");
                if (!dataFiles.CheckFiles(exePath))
                {
                    return false;
                }
                return Book.Inicialize(exePath);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, true);
                return false;
            }
        }

        private void FillTreeView(TreeView tree, bool useLeftTranslation)
        {
            Translation translation = useLeftTranslation ? Book.LeftTranslation : Book.RightTranslation;

            TreeViewItem nodePaper = null;
            foreach (TOC_Entry entry in translation.TableOfContents)
            {
                if (entry.Section == 0)
                {
                    nodePaper = new TreeViewItemUB(entry);
                    //if (entry.IsExpanded) nodePaper.Expand();
                    tree.Items.Add(nodePaper);
                }
                else if (entry.ParagraphNo == 0)
                {
                    TreeViewItem nodeSection = new TreeViewItemUB(entry);
                    nodeSection.Tag = entry;
                    nodePaper.Items.Add(nodeSection);
                }
            }
        }

        private void formText_Loaded(object sender, RoutedEventArgs e)
        {

            if (LoadData())
            {
                FillTreeView(TOC_Left, true);
                FillTreeView(TOC_Right, false);
            }
        }
    }
}
