using System;
using System.Collections.Generic;
using System.Drawing;
using static System.Windows.Forms.LinkLabel;

namespace FolderVisualize
{
    public class Folder : Component
    {
        public List<Component> children = new List<Component>(); // Change access modifier to public

        public Folder(string name) : base(name)
        {
        }

        public void Add(Component component)
        {
            children.Add(component);
        }

        public void Remove(Component component)
        {
            children.Remove(component);
        }

        public override int CalculateSize()
        {
            int totalSize = 0;
            foreach (var child in children)
            {
                totalSize += child.CalculateSize();
            }
            return totalSize;
        }
        public override void Visualize(Graphics g, int depth, int x, ref int y, Font font, Brush brush)
        {
            if (Component.Mode == VisualizationMode.Horizontal)
            {
                SizeF textSize = g.MeasureString(Name, font);
                const int horizontalPadding = 30;
                const int verticalPadding = 8;
                const int horizontalOffset = 200; // Indentation for each depth level
                const int verticalOffset = 50; // Space between items vertically

                // Calculate box size and location
                Size boxSize = new Size((int)textSize.Width + 2 * horizontalPadding, (int)textSize.Height + 2 * verticalPadding);
                Point boxLocation = new Point(x, y);
                Rectangle boxRectangle = new Rectangle(boxLocation, boxSize);

                // Draw the folder
                g.DrawRectangle(Pens.Black, boxRectangle);
                g.DrawString(Name, font, brush, boxLocation.X + horizontalPadding, boxLocation.Y + verticalPadding);

                // Update y to the bottom of the folder
                y += boxSize.Height + verticalPadding;

                //if there is more children
                if (children.Count > 0)
                {
                    int verticalLineStartY = boxLocation.Y + boxSize.Height;
                    int verticalLineEndY = y;

                    // Vertical line's X is at the center of the folder
                    int verticalLineX = boxLocation.X + boxSize.Width / 2;

                    foreach (var child in children)
                    {
                        // Calculate child's starting Y position
                        int childY = y + verticalOffset / 2;

                        // Draw horizontal line to the child
                        g.DrawLine(Pens.Black, verticalLineX, childY, x + horizontalOffset, childY);

                        // Visualize the child
                        child.Visualize(g, depth + 1, x + horizontalOffset, ref y, font, brush);

                        // Update end Y position for the vertical line after visualizing the child
                        verticalLineEndY = y;
                    }

                    // Draw the vertical connecting line
                    g.DrawLine(Pens.Black, verticalLineX, verticalLineStartY, verticalLineX, verticalLineEndY);
                }
            }
            else
            {
                SizeF textSize = g.MeasureString(Name, font);
                const int horizontalPadding = 30;
                const int verticalPadding = 8;
                const int verticalSpacing = 50;
                const int siblingOffset = 100;

                Size boxSize = new Size((int)textSize.Width + 2 * horizontalPadding, (int)textSize.Height + 2 * verticalPadding);

                // Center the top folder 
                if (depth == 0)
                {
                    x = (Component.PanelWidth - boxSize.Width) / 2;
                }

                Point boxLocation = new Point(x, y);
                Rectangle boxRectangle = new Rectangle(boxLocation, boxSize);

                g.FillRectangle(Brushes.White, boxRectangle);
                g.DrawRectangle(Pens.Black, boxRectangle);
                g.DrawString(Name, font, brush, boxLocation.X + horizontalPadding, boxLocation.Y + verticalPadding);

                // Determine the y-coordinate for the children
                int childY = y + boxSize.Height + verticalSpacing;

                if (children.Count > 0)
                {
                    // Calculate total width of all children
                    int totalChildrenWidth = children.Sum(c => (int)g.MeasureString(c.Name, font).Width + 2 * horizontalPadding + siblingOffset) - siblingOffset;

                    // Start children at x-coordinate that will center them beneath the parent
                    int childrenStartX = x + (boxSize.Width / 2) - (totalChildrenWidth / 2);

                    foreach (var child in children)
                    {
                        SizeF childTextSize = g.MeasureString(child.Name, font);
                        Size childBoxSize = new Size((int)childTextSize.Width + 2 * horizontalPadding, (int)childTextSize.Height + 2 * verticalPadding);

                        // Center the child node horizontally beneath its parent node
                        int childX = childrenStartX + (childBoxSize.Width / 2) - (int)(childTextSize.Width / 2);

                        // Draw line from parent to child
                        g.DrawLine(Pens.Black, boxLocation.X + boxSize.Width / 2, boxLocation.Y + boxSize.Height, childX, childY);

                        // Visualize the child node
                        int nextChildY = childY; // Use a separate variable to hold the modified y-coordinate for the next child
                        child.Visualize(g, depth + 1, childX, ref nextChildY, font, brush);

                        // Increment childrenStartX to place the next child to the right
                        childrenStartX += childBoxSize.Width + siblingOffset;
                    }

                    // After all children have been visualized, update y to the bottom of the children
                    y = childY;
                }
                else
                {
                    // If there are no children, just update y below this node
                    y += boxSize.Height + verticalSpacing;
                }
            }




        }


        private int CalculateVerticalLineEndY(Graphics g, Font font, int startY)
        {
            int maxY = startY;
            foreach (var child in children)
            {
                SizeF childSize = g.MeasureString(child.Name, font);
                int childHeight = (int)childSize.Height + 30; // 30 is the vertical offset between items
                maxY += childHeight;
            }
            return maxY;
        }
    }
}
