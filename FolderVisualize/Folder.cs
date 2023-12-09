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
            else {
                SizeF textSize = g.MeasureString(Name, font);
                const int horizontalPadding = 30;
                const int verticalPadding = 8;
                const int verticalSpacing = 50; // Increased space for top-level folders
                int siblingOffset = 100; // Horizontal space between sibling folders/files

                // Calculate box size for the folder
                Size boxSize = new Size((int)textSize.Width + 2 * horizontalPadding, (int)textSize.Height + 2 * verticalPadding);

                // Center the top folder horizontally by calculating the required offset
                if (depth == 0)
                {
                    int totalChildrenWidth = children.Sum(child => (int)g.MeasureString(child.Name, font).Width + 2 * horizontalPadding);
                    int totalOffset = (children.Count - 1) * siblingOffset;
                    x = (Component.PanelWidth - (totalChildrenWidth + totalOffset)) / 2;
                }

                Point boxLocation = new Point(x, y);
                Rectangle boxRectangle = new Rectangle(boxLocation, boxSize);

                // Draw the folder
                g.FillRectangle(Brushes.White, boxRectangle);
                g.DrawRectangle(Pens.Black, boxRectangle);
                g.DrawString(Name, font, brush, boxLocation.X + horizontalPadding, boxLocation.Y + verticalPadding);

                // Update y to the bottom of the folder
                y += boxSize.Height + verticalPadding;

                // Only proceed if there are children to visualize
                if (children.Count > 0)
                {
                    // Start the first child at the center of the top folder minus half the total width of all children
                    int childrenStartX = boxLocation.X;

                    // Iterate over each child to visualize them
                    foreach (var child in children)
                    {
                        SizeF childTextSize = g.MeasureString(child.Name, font);
                        Size childBoxSize = new Size((int)childTextSize.Width + 2 * horizontalPadding, (int)childTextSize.Height + 2 * verticalPadding);

                        // Calculate the middle point for the connecting line
                        int lineStartX = boxLocation.X + boxSize.Width / 2;
                        int lineEndX = childrenStartX + childBoxSize.Width / 2;

                        // Draw the line from the current folder to the child folder
                        g.DrawLine(Pens.Black, lineStartX, boxLocation.Y + boxSize.Height, lineEndX, y + verticalSpacing / 2);

                        // Visualize the child
                        int childY = y + verticalSpacing;
                        child.Visualize(g, depth + 1, childrenStartX, ref childY, font, brush);

                        // Update childrenStartX for the next child
                        childrenStartX += childBoxSize.Width + siblingOffset;
                    }

                    // After all children are visualized, update y to the bottom of the last child
                    y += children.Max(c => (int)g.MeasureString(c.Name, font).Height + 2 * verticalPadding) + verticalSpacing;

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
