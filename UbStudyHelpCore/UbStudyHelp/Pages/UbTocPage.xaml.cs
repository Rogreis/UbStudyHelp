using System;
using System.Collections.Generic;
using System.Text;
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

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbTocPage.xaml
    /// </summary>
    public partial class UbTocPage : Page
    {
        public UbTocPage()
        {
            InitializeComponent();
            this.Loaded += UbTocPage_Loaded;
        }

        private void UbTocPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoadData())
            {
                FillTreeView(TOC_Left, true);
                FillTreeView(TOC_Right, false);
                TOC_Left.SelectedItemChanged += TableOfContents_SelectedItemChanged;
                TOC_Right.SelectedItemChanged += TableOfContents_SelectedItemChanged;
            }
        }

        private bool LoadData()
        {
            GetDataFiles dataFiles = new GetDataFiles();
            try
            {
                if (!dataFiles.CheckFiles(App.BaseTubFilesPath))
                {
                    return false;
                }
                return Book.Inicialize(App.BaseTubFilesPath);
            }
            catch (Exception ex)
            {
                EventsControl.FireSendMessage("Loading TOC data", ex);
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


        private void TableOfContents_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = sender as TreeView;
            TreeViewItemUB item = tree.SelectedItem as TreeViewItemUB;
            EventsControl.FireTOCClicked(item.Entry);
        }



    }
}
