﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Controls
{
    public delegate void dlShowIndexDetails(string indexEntry);

    /// <summary>
    /// Interaction logic for IndexBrowserDataEntry.xaml
    /// </summary>
    public partial class IndexBrowserDataEntry : UserControl
    {

        public event dlShowIndexDetails ShowIndexDetails = null;

        private ObservableCollection<string> LocalIndexLettersEntries = new ObservableCollection<string>();

        // Object to manipulate the index
        public UbStudyHelp.Classes.Index Index { get; set; }


        public IndexBrowserDataEntry()
        {
            InitializeComponent();

            // Events
            this.Loaded += IndexBrowserDataEntry_Loaded;
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            ComboWhatToSearchInIndex.KeyDown += ComboWhatToSearchInIndex_KeyDown;
            ComboWhatToSearchInIndex.DropDownClosed += ComboWhatToSearchInIndex_DropDownClosed;
            ComboBoxIndexSearch.DropDownClosed += ComboBoxIndexSearch_DropDownClosed;

            foreach(string entry in App.ParametersData.IndexLetters)
            {
                LocalIndexLettersEntries.Add(entry);
            }
            ComboWhatToSearchInIndex.ItemsSource = LocalIndexLettersEntries;
            if (ComboWhatToSearchInIndex.Items.Count > 0)
            {
                ComboWhatToSearchInIndex.SelectedIndex = 0;
            }
        }

        private void AddEntry(string indexEntry)
        {
            // Just avoid duplicates
            if (App.ParametersData.IndexLetters.Contains(indexEntry, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }
            if (App.ParametersData.IndexLetters.Count == App.ParametersData.MaxExpressionsStored)
            {
                LocalIndexLettersEntries.RemoveAt(LocalIndexLettersEntries.Count - 1);
                App.ParametersData.IndexLetters.RemoveAt(App.ParametersData.IndexLetters.Count - 1);
            }
            LocalIndexLettersEntries.Insert(0, indexEntry);
            App.ParametersData.IndexLetters.Insert(0, indexEntry);
        }


        private void FillComboBoxIndexEntry(string indexEntry)
        {
            if (!Index.Load() || indexEntry.Length < 2)
                return;

            List<string> list = Index.Search(indexEntry);
            if (list == null || list.Count == 0)
            {
                ComboBoxIndexSearch.IsEnabled = false;
                return;
            }
            int maxItems = Math.Min(list.Count, App.ParametersData.MaxExpressionsStored);
            ComboBoxIndexSearch.Items.Clear();
            for (int i = 0; i < maxItems; i++)
            {
                ComboBoxIndexSearch.Items.Add(list[i]);
            }

            AddEntry(indexEntry);
            if (ComboBoxIndexSearch.Items.Count > 0)
            {
                ComboBoxIndexSearch.SelectedIndex = 0;
                ComboBoxIndexSearch.IsEnabled = true;
            }

        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(TextBlockLabelIndex);
            App.Appearance.SetFontSize(ComboWhatToSearchInIndex);
            App.Appearance.SetFontSize(ComboBoxIndexSearch);
            App.Appearance.SetFontSize(TextBlockLabelIndex);
            App.Appearance.SetHeight(ComboWhatToSearchInIndex);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(TextBlockLabelIndex);
            App.Appearance.SetThemeInfo(ComboWhatToSearchInIndex);
            App.Appearance.SetThemeInfo(ComboBoxIndexSearch);
            App.Appearance.SetThemeInfo(TextBlockLabelIndex);
            App.Appearance.SetThemeInfo(ComboWhatToSearchInIndex);
        }


        #region events


        private void IndexBrowserDataEntry_Loaded(object sender, RoutedEventArgs e)
        {
            string text = ComboWhatToSearchInIndex.Text.Trim();
            if (text.Length > 1)
            {
                FillComboBoxIndexEntry(text);
            }
        }

        private void ComboWhatToSearchInIndex_DropDownClosed(object sender, EventArgs e)
        {
            string text = ComboWhatToSearchInIndex.Text.Trim();
            if (text.Length > 1)
            {
                FillComboBoxIndexEntry(text);
            }
        }

        private void ComboBoxIndexSearch_DropDownClosed(object sender, EventArgs e)
        {
            string text = ComboBoxIndexSearch.Text.Trim();
            ShowIndexDetails?.Invoke(text);
        }

        private void ComboWhatToSearchInIndex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = ComboWhatToSearchInIndex.Text.Trim();
                FillComboBoxIndexEntry(text);
            }
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        #endregion

    }
}