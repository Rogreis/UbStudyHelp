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
using System.Windows.Shapes;
using UbStudyHelp.Classes;

namespace UbStudyHelp
{
    /// <summary>
    /// Interaction logic for frmNewVersion.xaml
    /// </summary>
    public partial class frmNewVersion
    {
        public frmNewVersion()
        {
            InitializeComponent();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonInstallAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonTranslations_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonApplication_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReStartApp()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            System.Windows.Application.Current.Shutdown();
        }



        private class UpdateElement
        {
            public Translation Translation { get; set; }

            public string Name
            {
                get
                {
                    if (Translation != null)
                    {
                        return Translation.Description;
                    }
                    return "";
                }
            }

        }

        private List<UpdateElement> UpdateElements = new List<UpdateElement>();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<int, Translation> pair in GetDataFiles.UpdateList)
            {
                UpdateElements.Add(new UpdateElement() { Translation = pair.Value });
            }
            ListViewUpdatesAvailable.Items.Clear();
            ListViewUpdatesAvailable.ItemsSource = UpdateElements;
            ListViewUpdatesAvailable.DisplayMemberPath = "Name";
        }
    }
}
