using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.IconPacks;



namespace UbStudyHelp.Classes
{
    public enum PackUbIconsKind
    {
        [Description("Empty placeholder")] None,
        [Description("Table of Contents Icon")] TableOfContents,
        [Description("Book Search Icon")] BookSearchOutline,
        [Description("Index Icon")] HandIndex,
        [Description("Track con")] TrainTrack,
        [Description("Font Icon")] Fonts,
        [Description("Help Icon")] Help,
        [Description("Settings Icon")] HelpSettingsMD
    }


    public class PackUbIcons : PackIconControlBase  //PackIconControl<PackUbIconsKind>
    {
        public static readonly DependencyProperty KindProperty
            = DependencyProperty.Register(nameof(Kind), 
                    typeof(PackUbIconsKind), 
                    typeof(PackUbIcons), 
                    new PropertyMetadata(default(PackUbIconsKind), KindPropertyChangedCallback));


        /// <summary>
        /// Gets or sets the icon to display.
        /// </summary>
        public PackUbIconsKind Kind
        {
            get { return (PackUbIconsKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }


        public PackUbIcons()
        {
            //this.DefaultStyleKey = typeof(PackUbIcons);
        }

        static PackUbIcons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackUbIcons), new FrameworkPropertyMetadata(typeof(PackUbIcons)));
        }


        protected override void SetKind<TKind>(TKind iconKind)
        {
            //BindingOperations.SetBinding(this, PackUbIcons.KindProperty, new Binding() { Source = iconKind, Mode = BindingMode.OneTime });
            this.SetCurrentValue(KindProperty, iconKind);
        }

        protected override void UpdateData()
        {
            if (Kind != default(PackUbIconsKind))
            {
                string data = null;
                PackUbIconsDataFactory.DataIndex.Value?.TryGetValue(Kind, out data);
                this.Data = data;
            }
            else
            {
                this.Data = null;
            }
        }




        private static void KindPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((PackUbIcons)dependencyObject).UpdateData();
            }
        }
    }
}
