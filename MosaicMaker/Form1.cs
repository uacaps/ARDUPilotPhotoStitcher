using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MosaicMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // Set the current instance to 'self' because we do that for some reason
            var self = this;

            InitializeComponent();
            Init();
            //button1.Click += Init;
        }

        private class PlottedImage
        {
            public Image Picture { get; set;}
            public Point Plot { get; set; }
            public int defaultWidth { get; set; }
            public int defaultHeight { get; set; }
        }

        private void Init()
        {
            // Set the current instance to 'self' because we do that for some reason
            var self = this;

            

            // Create an array of images to be used for re-scaling and re-creating maps when needed
            List<Image> Images = new List<Image>();


            // Modify this path as necessary. 
            // Should direct to the folder on the ArduPilot holding geocached images
            string folder = @"C:\Users\nwilliams\Documents\TestGeoImages2\";

            // Take a snapshot of the file system.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(folder);

            // This method assumes that the application has discovery permissions for all folders under the specified path.
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            fileList.ToList<System.IO.FileInfo>();

            // Select all geocoded .jpg files in ArduPilot picture directory
            // All images geotagged by ArduPilot MissionPlanner should end in "_geotog". 
            // If this is not the case, change searchTerm to the appropriate suffix
            string searchTerm = "_geotag";
            var allFiles =
                from file in fileList
                where file.Extension == ".jpg"
                let fileText = file.FullName
                where fileText.Contains(searchTerm)
                select file.FullName;

            // Create the Graphics element that will house our mosaic map
            // Use high quality interpolation for scaling, allowing zoom and pan without sacrificing resolution
            ///PictureBox Mosaic = new PictureBox();
            ImageBox imageBox = new ImageBox();

            // Iterate through each file we found in ArduPilots image directory, creating an image of each file.
            // Returns updates destination coordinates
            /// TODO: 
            ///     Place them in their appropriate position on our map.
            ///     Formulas for converting Latitude and Longitude to Meters exist; use them
            ///     Consider using each pixel as one meter for scaling purposes; change if necessarry
            foreach (string file in allFiles)
            {
                Images.Add(Image.FromFile(file));
            }
            MakeMosaic(allFiles, imageBox, 800, 600, 1f, 1f);
            imageBox.Dispose();
        }

        private void MakeMosaic(IEnumerable<String> files, ImageBox g, int containerWidth, int containerHeight, float scaleX, float scaleY)
        {
            List<PlottedImage> Plotting = new List<PlottedImage>();
            bool firstImage = true;

            int upperBound = 0;
            int rightBound = 0;
            int leftBound = 0;
            int lowerBound = 0;
            
            // Initialization variables to be used and edited as the Mosaic is created
            // Points array containes X [0th spot] and Y [ 1st spot] coordinates.
            int[] Points = new int[] { containerHeight/2, containerWidth/2 };

            ///TODO:
            ///   Point X and point Y need to be derived from GPS coordinates rather than arbitrary random integers
            Random rand = new Random();
            foreach (string file in files)
            {

                //Direction is random until gps coordinates can be attained
                int temp = rand.Next(1, 4);

                switch (temp){
                    case 1:
                        if (firstImage)
                        {

                            Point centerPlot = new Point((containerWidth / 2) - (Image.FromFile(file).Width / 2),
                                                        (containerHeight / 2) - (Image.FromFile(file).Height / 2));

                            Plotting.Add(new PlottedImage
                            {
                                Picture = Image.FromFile(file),
                                Plot = centerPlot,
                                defaultHeight = Image.FromFile(file).Height,
                                defaultWidth = Image.FromFile(file).Width
                            });
                            firstImage = false;

                            upperBound = (containerHeight / 2) - (Image.FromFile(file).Height / 2);
                            lowerBound = (containerHeight / 2) - (Image.FromFile(file).Height / 2) + Image.FromFile(file).Height;
                            leftBound = (containerWidth / 2) - (Image.FromFile(file).Width / 2);
                            rightBound = (containerWidth / 2) - (Image.FromFile(file).Width / 2) + Image.FromFile(file).Width;
                        }
                        else
                        {

                            Plotting.Add(new PlottedImage
                            {
                                Picture = Image.FromFile(file),
                                Plot = new Point(rightBound, upperBound),
                                defaultHeight = Image.FromFile(file).Height,
                                defaultWidth = Image.FromFile(file).Width
                            });

                            //upperBound = upperBound - Image.FromFile(file).Height;
                            //lowerBound = lowerBound + Image.FromFile(file).Height;
                            leftBound = leftBound + Image.FromFile(file).Width;
                            rightBound = rightBound + Image.FromFile(file).Width;
                        }
                        break;

                    case 2:
                        if (firstImage)
                        {

                            Point centerPlot = new Point((containerWidth / 2) - (Image.FromFile(file).Width / 2),
                                                        (containerHeight / 2) - (Image.FromFile(file).Height / 2));

                            Plotting.Add(new PlottedImage
                            {
                                Picture = Image.FromFile(file),
                                Plot = centerPlot,
                                defaultHeight = Image.FromFile(file).Height,
                                defaultWidth = Image.FromFile(file).Width
                            });
                            firstImage = false;

                            upperBound = (containerHeight / 2) - (Image.FromFile(file).Height / 2);
                            lowerBound = (containerHeight / 2) - (Image.FromFile(file).Height / 2) + Image.FromFile(file).Height;
                            leftBound = (containerWidth / 2) - (Image.FromFile(file).Width / 2);
                            rightBound = (containerWidth / 2) - (Image.FromFile(file).Width / 2) + Image.FromFile(file).Width;
                        }
                        else
                        {

                            Plotting.Add(new PlottedImage
                            {
                                Picture = Image.FromFile(file),
                                Plot = new Point(leftBound - Image.FromFile(file).Width, upperBound),
                                defaultHeight = Image.FromFile(file).Height,
                                defaultWidth = Image.FromFile(file).Width
                            });
                            leftBound = leftBound - Image.FromFile(file).Width;
                            rightBound = rightBound - Image.FromFile(file).Width;
                        }
                        break;

                    case 3:
                        if (firstImage)
                        {

                            Point centerPlot = new Point((containerWidth / 2) - (Image.FromFile(file).Width / 2),
                                                        (containerHeight / 2) - (Image.FromFile(file).Height / 2));

                            Plotting.Add(new PlottedImage
                            {
                                Picture = Image.FromFile(file),
                                Plot = centerPlot,
                                defaultHeight = Image.FromFile(file).Height,
                                defaultWidth = Image.FromFile(file).Width
                            });
                            firstImage = false;

                            upperBound = (containerHeight / 2) - (Image.FromFile(file).Height / 2);
                            lowerBound = (containerHeight / 2) - (Image.FromFile(file).Height / 2) + Image.FromFile(file).Height;
                            leftBound = (containerWidth / 2) - (Image.FromFile(file).Width / 2);
                            rightBound = (containerWidth / 2) - (Image.FromFile(file).Width / 2) + Image.FromFile(file).Width;
                        }
                        else
                        {

                            Plotting.Add(new PlottedImage
                            {
                                Picture = Image.FromFile(file),
                                Plot = new Point(leftBound, lowerBound),
                                defaultHeight = Image.FromFile(file).Height,
                                defaultWidth = Image.FromFile(file).Width
                            });
                            upperBound = upperBound + Image.FromFile(file).Height;
                            lowerBound = lowerBound + Image.FromFile(file).Height;
                        }
                        break;
                    case 4:
                        if (firstImage)
                        {

                            Point centerPlot = new Point((containerWidth / 2) - (Image.FromFile(file).Width / 2),
                                                        (containerHeight / 2) - (Image.FromFile(file).Height / 2));

                            Plotting.Add(new PlottedImage
                            {
                                Picture = Image.FromFile(file),
                                Plot = centerPlot,
                                defaultHeight = Image.FromFile(file).Height,
                                defaultWidth = Image.FromFile(file).Width
                            });
                            firstImage = false;

                            upperBound = (containerHeight / 2) - (Image.FromFile(file).Height / 2);
                            lowerBound = (containerHeight / 2) - (Image.FromFile(file).Height / 2) + Image.FromFile(file).Height;
                            leftBound = (containerWidth / 2) - (Image.FromFile(file).Width / 2);
                            rightBound = (containerWidth / 2) - (Image.FromFile(file).Width / 2) + Image.FromFile(file).Width;
                        }
                        else
                        {

                            Plotting.Add(new PlottedImage
                            {
                                Picture = Image.FromFile(file),
                                Plot = new Point(leftBound, upperBound),
                                defaultHeight = Image.FromFile(file).Height,
                                defaultWidth = Image.FromFile(file).Width
                            });
                            lowerBound = lowerBound - Image.FromFile(file).Height;
                            upperBound = upperBound - Image.FromFile(file).Height;
                        }
                        break;


                }
            }

            foreach (PlottedImage piece in Plotting)
            {
                g.DrawImage(piece.Picture);
            }
        }

        #region  Event Handlers


        private void showImageRegionToolStripButton_Click(object sender, EventArgs e)
        {
          imageBox.Invalidate();
        }

        #endregion  Event Handlers

        #region  Private Methods

        private void DrawBox(Graphics graphics, Color color, Rectangle rectangle)
        {
          int offset;
          int penWidth;

          offset = 9;
          penWidth = 2;

          using (SolidBrush brush = new SolidBrush(Color.FromArgb(64, color)))
            graphics.FillRectangle(brush, rectangle);

          using (Pen pen = new Pen(color, penWidth))
          {
            pen.DashStyle = DashStyle.Dot;
            graphics.DrawLine(pen, rectangle.Left, rectangle.Top - offset, rectangle.Left, rectangle.Bottom + offset);
            graphics.DrawLine(pen, rectangle.Left + rectangle.Width, rectangle.Top - offset, rectangle.Left + rectangle.Width, rectangle.Bottom + offset);
            graphics.DrawLine(pen, rectangle.Left - offset, rectangle.Top, rectangle.Right + offset, rectangle.Top);
            graphics.DrawLine(pen, rectangle.Left - offset, rectangle.Bottom, rectangle.Right + offset, rectangle.Bottom);
          }
        }

        #endregion  Private Methods
    }
}
