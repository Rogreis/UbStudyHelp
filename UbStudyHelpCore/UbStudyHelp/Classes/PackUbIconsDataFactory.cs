using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Text;

namespace UbStudyHelp.Classes
{
    public static class PackUbIconsDataFactory
    {

        public static Lazy<IDictionary<PackUbIconsKind, string>> DataIndex { get; }

        static PackUbIconsDataFactory()
        {
            if (DataIndex == null)
            {
                DataIndex = new Lazy<IDictionary<PackUbIconsKind, string>>(PackUbIconsDataFactory.Create);
            }
        }

        public static IDictionary<PackUbIconsKind, string> Create()
        {
            return new Dictionary<PackUbIconsKind, string>
                   {
                       {PackUbIconsKind.None, ""},
                       {PackUbIconsKind.TableOfContents, "M3,9H17V7H3V9M3,13H17V11H3V13M3,17H17V15H3V17M19,17H21V15H19V17M19,7V9H21V7H19M19,13H21V11H19V13Z"},
                       {PackUbIconsKind.BookSearchOutline, ""},
                       {PackUbIconsKind.HandIndex, ""},
                       {PackUbIconsKind.TrainTrack, ""},
                       {PackUbIconsKind.Fonts, ""},
                       {PackUbIconsKind.Help, ""},
                       {PackUbIconsKind.HelpSettingsMD, ""}
            };
        }

    }
}
