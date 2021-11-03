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
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            TOC_Left.SelectedItemChanged += TableOfContents_SelectedItemChanged;
            TOC_Right.SelectedItemChanged += TableOfContents_SelectedItemChanged;
        }

        public void Initialize()
        {
            SetFontSize();
            SetControlsStyles();
            FillTreeView(TOC_Left, true);
            FillTreeView(TOC_Right, false);
        }


        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetControlsStyles();
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(TOC_Left);
            App.Appearance.SetFontSize(TOC_Right);
            LeftMenuTop.SetFontSize();
        }


        private void SetControlsStyles()
        {
            App.Appearance.SetAll(TOC_Left);
            App.Appearance.SetAll(TOC_Right);
            SetFontSize();
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

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
