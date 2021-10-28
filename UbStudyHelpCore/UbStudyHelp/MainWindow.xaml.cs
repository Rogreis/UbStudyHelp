using ControlzEx.Theming;
using System;
using System.Windows;
using System.Windows.Controls;
using UbStudyHelp.Classes;

namespace UbStudyHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// For styling documentation <see href="https://mahapps.com/docs/guides/quick-start"/>
    /// </summary>
    public partial class MainWindow
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
                if (!dataFiles.CheckFiles(App.BaseTubFilesPath))
                {
                    return false;
                }
                return Book.Inicialize(App.BaseTubFilesPath);
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


        private void SetFontSize()
        {
            App.ParametersData.Appearance.SetFontSize(LabelTranslations);
            App.ParametersData.Appearance.SetFontSize(LabelTrack);
            App.ParametersData.Appearance.SetFontSize(ComboTrack);
            App.ParametersData.Appearance.SetFontSize(LabelThemes);
            App.ParametersData.Appearance.SetFontSize(ComboTheme);
            App.ParametersData.Appearance.SetFontSize(TOC_Left);
            App.ParametersData.Appearance.SetFontSize(TOC_Right);
            EventsControl.FireFontChanged(App.ParametersData.Appearance);
        }

        private void SetControlsStyles()
        {
            App.ParametersData.Appearance.SetAll(TOC_Left);
            App.ParametersData.Appearance.SetAll(TOC_Right);
            SetFontSize();
        }

        private void formText_Loaded(object sender, RoutedEventArgs e)
        {

            if (LoadData())
            {
                FillTreeView(TOC_Left, true);
                FillTreeView(TOC_Right, false);
                TOC_Left.SelectedItemChanged += TableOfContents_SelectedItemChanged;
                TOC_Right.SelectedItemChanged += TableOfContents_SelectedItemChanged;
                ComboTheme.Text= App.ParametersData.ThemeColor;
            }
            SetTheme();
            SetFontSize();
        }



        #region Tree Events
        private void TableOfContents_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = sender as TreeView;
            TreeViewItemUB item = tree.SelectedItem as TreeViewItemUB;
            EventsControl.FireTOCClicked(item.Entry);
        }

        #endregion

        private void LaunchUFSite(object sender, RoutedEventArgs e)
        {
            //Process.Start("http://www.urantia.org");
        }

        private void SetTheme()
        {
            string theme = App.ParametersData.ThemeName + "." + App.ParametersData.ThemeColor;
            ThemeManager.Current.ChangeTheme(Application.Current, theme);
        }

        private void ReverseTheme(object sender, RoutedEventArgs e)
        {
        }

        private void ComboTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = ComboTheme.SelectedItem as ComboBoxItem;
            App.ParametersData.ThemeColor = (string)item.Content; 
            SetTheme();
        }

        private void BtIncreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            App.ParametersData.Appearance.FontSizeInfo++;
            SetFontSize();
        }

        private void BtDecreseFontSize_Click(object sender, RoutedEventArgs e)
        {
            App.ParametersData.Appearance.FontSizeInfo--;
            SetFontSize();
        }

        private void BtToggleTheme_Toggled(object sender, RoutedEventArgs e)
        {
            // Set the application theme to Dark.Green
            Theme theme = ThemeManager.Current.DetectTheme();
            if (theme.Name.StartsWith("Dark"))
            {
                App.ParametersData.ThemeName = "Light";
            }
            else
            {
                App.ParametersData.ThemeName = "Dark";
            }
            SetTheme();
            SetFontSize();
        }
    }
}
