using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbTocPage.xaml
    /// </summary>
    public partial class UbTocPage : Page
    {

        private bool InternalChange = false;  // Used to avoid sending again a worked event

        public UbTocPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            EventsControl.BilingualChanged += EventsControl_BilingualChanged;
            EventsControl.TranslationsChanged += EventsControl_TranslationsChanged;
            TOC_Left.SelectedItemChanged += TOC_Left_SelectedItemChanged;
            TOC_Right.SelectedItemChanged += TOC_Right_SelectedItemChanged;
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(TOC_Left);
            App.Appearance.SetFontSize(TOC_Right);
        }


        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(TOC_Left);
            App.Appearance.SetThemeInfo(TOC_Right);
        }

        private void SetItemEvents(TreeViewItemUB item)
        {
            item.Expanded += Item_Expanded;
            item.Collapsed += Item_Collapsed;
        }


        private void FillTreeView(TreeView tree, bool useLeftTranslation)
        {
            Translation translation = useLeftTranslation ? Book.LeftTranslation : Book.RightTranslation;
            tree.Tag = translation;
            TreeViewItemUB itemPaper = null;

            tree.Items.Clear();
            foreach (TOC_Entry entry in translation.TableOfContents)
            {
                if (entry.Section == 0)
                {
                    itemPaper = new TreeViewItemUB(entry);
                    SetItemEvents(itemPaper);
                    tree.Items.Add(itemPaper);
                }
                else if (entry.ParagraphNo == 0)
                {
                    TreeViewItemUB itemSection = new TreeViewItemUB(entry);
                    SetItemEvents(itemSection);
                    itemPaper.Items.Add(itemSection);
                }
            }

        }

        /// <summary>
        /// Search recursively for an item 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        private TreeViewItemUB FindItem(ItemCollection collection, TOC_Entry entry)
        {
            if (collection == null)
            {
                return null;
            }
            foreach (TreeViewItemUB item in collection)
            {
                if (entry == item.Entry)
                {
                    return item;
                }
                TreeViewItemUB itemNew = FindItem(item.Items, entry);
                if (itemNew != null)
                {
                    return itemNew;
                }
            }
            return null;
        }

        private static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate { }));
        }

        private void SelectItem(TreeView tree, TOC_Entry entry)
        {
            TreeViewItemUB item = FindItem(tree.Items, entry);
            InternalChange = true;
            item.IsSelected = true;
            DoEvents();
            InternalChange = false;
        }


        #region events
        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
        }

        private void TOC_Right_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (InternalChange)
            {
                return;
            }
            try
            {
                TreeViewItemUB item = TOC_Right.SelectedItem as TreeViewItemUB;
                SelectItem(TOC_Left, item.Entry);
                EventsControl.FireTOCClicked(item.Entry);
            }
            catch { }  // Errors are ignored
        }

        private void TOC_Left_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (InternalChange)
            {
                return;
            }
            try
            {
                TreeViewItemUB item = TOC_Left.SelectedItem as TreeViewItemUB;
                SelectItem(TOC_Right, item.Entry);
                EventsControl.FireTOCClicked(item.Entry);
            }
            catch { }  // Errors are ignored
        }

        private void Item_Collapsed(object sender, RoutedEventArgs e)
        {
            if (InternalChange)
            {
                return;
            }
            try
            {
                TreeViewItemUB item = e.Source as TreeViewItemUB;
                TreeView tree = item.Parent == TOC_Left ? TOC_Right : TOC_Left;
                TreeViewItemUB itemNew = FindItem(tree.Items, item.Entry);
                InternalChange = true;
                itemNew.IsExpanded = false;
                DoEvents();
                InternalChange = false;
            }
            catch  { } // Errors are ignored
        }

        private void Item_Expanded(object sender, RoutedEventArgs e)
        {
            if (InternalChange)
            {
                return;
            }
            try
            {
                TreeViewItemUB item = e.Source as TreeViewItemUB;
                TreeView tree = item.Parent == TOC_Left ? TOC_Right : TOC_Left;
                TreeViewItemUB itemNew = FindItem(tree.Items, item.Entry);
                InternalChange = true;
                itemNew.IsExpanded = true;
                DoEvents();
                InternalChange = false;
            }
            catch { } // Errors are ignored
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void EventsControl_BilingualChanged(bool ShowBilingual)
        {
            TOC_Right.Visibility = ShowBilingual ? Visibility.Visible : Visibility.Hidden;
            TabItemRight.Visibility = ShowBilingual ? Visibility.Visible : Visibility.Hidden;
        }

        private void EventsControl_TranslationsChanged()
        {
            FillTreeView(TOC_Left, true);
            FillTreeView(TOC_Right, false);
        }


        #endregion


        public void Initialize()
        {
            SetFontSize();
            SetAppearence();
            TOC_Right.Visibility = App.ParametersData.ShowBilingual ? Visibility.Visible : Visibility.Hidden;
            TabItemRight.Visibility = App.ParametersData.ShowBilingual ? Visibility.Visible : Visibility.Hidden;
            FillTreeView(TOC_Left, true);
            FillTreeView(TOC_Right, false);
        }


    }
}
