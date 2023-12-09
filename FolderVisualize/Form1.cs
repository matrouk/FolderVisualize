using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FolderVisualize
{
    public partial class Form1 : Form
    {
        private Folder topFolder;
        private Font drawingFont = new Font("Arial", 12);
        private Brush drawingBrush = Brushes.Black;
        private int startingYPosition = 20;

        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint,
                     true);
            UpdateStyles();
            this.AutoScroll = true;
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            panel1.Scroll += new ScrollEventHandler(panel1_Scroll);
            Component.startingYPosition = 20;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the top-level folder on form load
            topFolder = new Folder("Top Folder");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Component.Mode = VisualizationMode.Horizontal;
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = folderDialog.SelectedPath;
                    topFolder = new Folder(Path.GetFileName(selectedFolder));
                    TraverseFolder(selectedFolder, topFolder);
                    panel1.Invalidate(); // This will trigger the panel1_Paint event
                }
            }
        }

        private void TraverseFolder(string folderPath, Folder parentFolder)
        {
            foreach (string subfolderPath in Directory.GetDirectories(folderPath))
            {
                Folder subfolder = new Folder(Path.GetFileName(subfolderPath));
                parentFolder.Add(subfolder);
                TraverseFolder(subfolderPath, subfolder);
            }

            foreach (string filePath in Directory.GetFiles(folderPath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                parentFolder.Add(new File(fileInfo.Name, (int)fileInfo.Length, fileInfo.Extension));
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Component.PanelWidth = panel1.ClientSize.Width;

            Graphics g = e.Graphics;

            // Apply a translation only once to account for the scrolling.
            g.TranslateTransform(panel1.AutoScrollPosition.X, panel1.AutoScrollPosition.Y);

            // Start with a clean background. No need to call Clear twice.
            g.Clear(panel1.BackColor);

            // Calculate the starting y-coordinate, considering the scroll position.
            int y = startingYPosition - panel1.AutoScrollPosition.Y;

            // Draw the folder structure starting from the adjusted y-coordinate.
            topFolder.Visualize(g, 0, 20, ref y, drawingFont, drawingBrush);

            // After drawing, calculate the total size needed for the content.
            int totalHeight = CalculateTotalHeight(topFolder, 0, g) + panel1.AutoScrollPosition.Y;
            int totalWidth = CalculateTotalWidth(topFolder, 0, g) + panel1.AutoScrollPosition.X;

            // Ensure the minimum scrollable size is always larger than the panel size
            totalHeight = Math.Max(totalHeight, panel1.Height + 1);
            totalWidth = Math.Max(totalWidth, panel1.Width + 1);

            // Set the AutoScrollMinSize based on the total size required for the visualized content.
            panel1.AutoScrollMinSize = new Size(totalWidth, totalHeight);
        }







        private int CalculateTotalHeight(Folder folder, int depth, Graphics g)
        {
            int totalHeight = 0;
            SizeF textSize = g.MeasureString(folder.Name, drawingFont);
            int elementHeight = (int)textSize.Height + 10; // Add padding

            totalHeight += elementHeight;

            foreach (var child in folder.children)
            {
                if (child is Folder childFolder)
                {
                    totalHeight += CalculateTotalHeight(childFolder, depth + 1, g);
                }
                else
                {
                    totalHeight += elementHeight;
                }
            }

            return totalHeight;
        }
        private int CalculateTotalWidth(Folder folder, int depth, Graphics g)
        {
            int maxWidth = 0;
            SizeF textSize = g.MeasureString(folder.Name, drawingFont);
            int elementWidth = (int)textSize.Width + 40 + (depth * 20); // Add padding and depth indentation

            maxWidth = Math.Max(maxWidth, elementWidth);

            foreach (var child in folder.children)
            {
                if (child is Folder childFolder)
                {
                    maxWidth = Math.Max(maxWidth, CalculateTotalWidth(childFolder, depth + 1, g));
                }
                else
                {
                    SizeF childTextSize = g.MeasureString(child.Name, drawingFont);
                    elementWidth = (int)childTextSize.Width + 40 + ((depth + 1) * 20);
                    maxWidth = Math.Max(maxWidth, elementWidth);
                }
            }

            return maxWidth;
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            // Forces immediate repaint of the control.
            panel1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Open folder browser dialog
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected folder path
                    string selectedFolder = folderDialog.SelectedPath;

                    // Set visualization mode to vertical
                    Component.Mode = VisualizationMode.Vertical;

                    // Build the folder structure
                    topFolder = new Folder(Path.GetFileName(selectedFolder));
                    TraverseFolder(selectedFolder, topFolder);

                    // Invalidate the panel to redraw
                    panel1.Invalidate(); // This will trigger the panel1_Paint event
                }
            }
        }

    }
}
