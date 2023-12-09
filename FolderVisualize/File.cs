using System;
using System.Security.Cryptography.Xml;

namespace FolderVisualize
{
    class File : Component
    {
        public int Size { get; private set; }
        public string Extension { get; private set; }

        public File(string name, int size, string extension) : base(name)
        {
            Size = size;
            Extension = extension;
        }

        public override int CalculateSize()
        {
            return Size;
        }
        // In the File class
        public override void Visualize(Graphics g, int depth, int x, ref int y, Font font, Brush brush)
        {
            if (Component.Mode == VisualizationMode.Horizontal)
            {
                SizeF textSize = g.MeasureString(Name, font);
                const int horizontalPadding = 20;
                const int verticalPadding = 8;
                const int verticalOffset = 30; // Space between items vertically

                // Calculate box size and location
                Size boxSize = new Size((int)textSize.Width + 2 * horizontalPadding, (int)textSize.Height + 2 * verticalPadding);
                Point boxLocation = new Point(x, y);
                Rectangle boxRectangle = new Rectangle(boxLocation, boxSize);

                // Draw the file
                g.FillRectangle(Brushes.LightBlue, boxRectangle);
                g.DrawRectangle(Pens.Black, boxRectangle);
                g.DrawString(Name, font, brush, boxLocation.X + horizontalPadding, boxLocation.Y + verticalPadding);

                // Update y for the next element
                y += boxSize.Height + verticalOffset;
            }
            else
            {
                SizeF textSize = g.MeasureString(Name, font);
                const int horizontalPadding = 20;
                const int verticalPadding = 8;
                const int verticalOffset = 30; // Space between items vertically

                // Adjust the x position to account for the depth in the tree
                int adjustedX = x + (depth * horizontalPadding);

                // Calculate box size and location for vertical layout
                Size boxSize = new Size((int)textSize.Width + 2 * horizontalPadding, (int)textSize.Height + 2 * verticalPadding);
                Point boxLocation = new Point(adjustedX, y);
                Rectangle boxRectangle = new Rectangle(boxLocation, boxSize);

                // Draw the file
                g.FillRectangle(Brushes.LightBlue, boxRectangle);
                g.DrawRectangle(Pens.Black, boxRectangle);
                g.DrawString(Name, font, brush, boxLocation.X + horizontalPadding, boxLocation.Y + verticalPadding);

                // Update y for the next element
                y += boxSize.Height + verticalOffset;
            }
        }



























    }
}
