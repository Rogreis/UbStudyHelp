using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace UbStudyHelp.Classes.TextblockExtension
{
    /// <summary>
    /// From https://github.com/MahApps/MahApps.Metro.IconPacks
    /// Used to implement some local icons and not use the full library
    /// </summary>
    //[MarkupExtensionReturnType(ReturnType = typeof(PackUbIcons))]
    [MarkupExtensionReturnType(typeof(PackIconMaterial))]
    public class PackUbIconsExtension : BasePackIconExtension
    {
        public PackUbIconsExtension()
        {
        }

        public PackUbIconsExtension(PackUbIconsKind kind)
        {
            this.Kind = kind;
        }

        [ConstructorArgument("kind")]
        public PackUbIconsKind Kind { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.GetPackIcon<PackUbIcons, PackUbIconsKind>(this.Kind);
        }


    }
}
