using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace WpfAppTest
{


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation", IsNullable = false)]
    public partial class Canvas
    {

        private CanvasCanvasLayoutTransform canvasLayoutTransformField;

        private CanvasPath pathField;

        private string nameField;

        private byte widthField;

        private byte heightField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Canvas.LayoutTransform")]
        public CanvasCanvasLayoutTransform CanvasLayoutTransform
        {
            get
            {
                return this.canvasLayoutTransformField;
            }
            set
            {
                this.canvasLayoutTransformField = value;
            }
        }

        /// <remarks/>
        public CanvasPath Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation")]
    public partial class CanvasCanvasLayoutTransform
    {

        private CanvasCanvasLayoutTransformMatrixTransform matrixTransformField;

        /// <remarks/>
        public CanvasCanvasLayoutTransformMatrixTransform MatrixTransform
        {
            get
            {
                return this.matrixTransformField;
            }
            set
            {
                this.matrixTransformField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation")]
    public partial class CanvasCanvasLayoutTransformMatrixTransform
    {

        private string matrixField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Matrix
        {
            get
            {
                return this.matrixField;
            }
            set
            {
                this.matrixField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation")]
    public partial class CanvasPath
    {

        private string dataField;

        private string strokeField;

        private string fillField;

        private string stretchField;

        private byte widthField;

        private byte heightField;

        private string snapsToDevicePixelsField;

        private string useLayoutRoundingField;

        private byte strokeThicknessField;

        private string strokeStartLineCapField;

        private string strokeEndLineCapField;

        private string strokeLineJoinField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Stroke
        {
            get
            {
                return this.strokeField;
            }
            set
            {
                this.strokeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Fill
        {
            get
            {
                return this.fillField;
            }
            set
            {
                this.fillField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Stretch
        {
            get
            {
                return this.stretchField;
            }
            set
            {
                this.stretchField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SnapsToDevicePixels
        {
            get
            {
                return this.snapsToDevicePixelsField;
            }
            set
            {
                this.snapsToDevicePixelsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UseLayoutRounding
        {
            get
            {
                return this.useLayoutRoundingField;
            }
            set
            {
                this.useLayoutRoundingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte StrokeThickness
        {
            get
            {
                return this.strokeThicknessField;
            }
            set
            {
                this.strokeThicknessField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StrokeStartLineCap
        {
            get
            {
                return this.strokeStartLineCapField;
            }
            set
            {
                this.strokeStartLineCapField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StrokeEndLineCap
        {
            get
            {
                return this.strokeEndLineCapField;
            }
            set
            {
                this.strokeEndLineCapField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StrokeLineJoin
        {
            get
            {
                return this.strokeLineJoinField;
            }
            set
            {
                this.strokeLineJoinField = value;
            }
        }
    }




    public class CanvasIcon
    {
        /*
         * 
        <Canvas x:Name="xxxx" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        Width="16" 
        Height="16">
            <Canvas.LayoutTransform>
                <MatrixTransform Matrix="Identity" />
            </Canvas.LayoutTransform>
            <Path Data="M256,8C119,8,8,119,8,256S119,504,256,504,504,393,504,256,393,8,256,8Zm48.2,326.1h-181L207.9,178h181Z"
          Stroke="{x:Null}"
          Fill="#FF000000"
          Stretch="Uniform"
          Width="32"
          Height="32"
          SnapsToDevicePixels="False"
          UseLayoutRounding="False"
          StrokeThickness="0"
          StrokeStartLineCap="flat"
          StrokeEndLineCap="flat"
          StrokeLineJoin="miter"/>
        </Canvas>
         * */

        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public MatrixTransform MatrixTransform { get; set; }
        public string PathData { get; set; }

    }

    public static class CanvasIcons
    {
        public static List<CanvasIcon> Icons = new List<CanvasIcon>();

        public static void Load()
        {
            string folderData = @"C:\Trabalho\GitHub\Rogerio\UbStudyHelp\Images\XamlIcons";
            foreach (string pathFile in Directory.GetFiles(folderData, "*.xaml"))
            {
                string xml = File.ReadAllText(pathFile);
                var serializer = new XmlSerializer(typeof(Canvas));
                using (var reader = new StringReader(xml))
                {
                    var canvas = (Canvas)serializer.Deserialize(reader);
                    CanvasIcon iconData = new CanvasIcon();
                    iconData.Name = canvas.Name;
                    iconData.Width = canvas.Width;
                    iconData.Height = canvas.Height;

                    if (canvas.CanvasLayoutTransform.MatrixTransform.Matrix == "Identity")
                    {
                        iconData.MatrixTransform = (MatrixTransform)MatrixTransform.Identity;
                    }
                    else
                    {
                        iconData.MatrixTransform = (MatrixTransform)MatrixTransform.Identity;
                        //char[] sep = { ',' };
                        //string[] parts = canvas.CanvasLayoutTransform.MatrixTransform.Matrix.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        //iconData.MatrixTransform = new MatrixTransform(new Matrix(Convert.ToDouble(parts[0]), Convert.ToDouble(parts[1]), Convert.ToDouble(parts[2]),
                        //                                                          Convert.ToDouble(parts[3]), Convert.ToDouble(parts[4]), Convert.ToDouble(parts[5])));
                    }
                    iconData.PathData = canvas.Path.Data;
                    Icons.Add(iconData);
                }
            }
        }
    }


}
