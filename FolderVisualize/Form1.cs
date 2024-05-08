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
        private float _zoomScale = 1.0f;
        private bool _ctrlKeyPressed = false;

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
            this.MouseWheel += OnMouseWheel;
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            this.KeyPreview = true; // This ensures that the form receives key events first

            // Focus the panel on form load
            this.Load += (s, e) => { panel1.Focus(); };

            // Attach mouse wheel event handler to panel1
            panel1.MouseWheel += OnMouseWheel;

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

            // Apply the zoom scale to the graphics object
            g.ScaleTransform(_zoomScale, _zoomScale);

            // Apply a translation to account for the scrolling, scaled by the current zoom level
            g.TranslateTransform(panel1.AutoScrollPosition.X / _zoomScale, panel1.AutoScrollPosition.Y / _zoomScale);

            g.Clear(panel1.BackColor);

            // Adjust the starting position based on the zoom scale
            int y = (int)((startingYPosition - panel1.AutoScrollPosition.Y) / _zoomScale);

            topFolder.Visualize(g, 0, 20, ref y, drawingFont, drawingBrush);

            // Use the zoom scale to adjust the total size needed for the content
            int totalHeight = (int)(CalculateTotalHeight(topFolder, 0, g) / _zoomScale) + panel1.AutoScrollPosition.Y;
            int totalWidth = (int)(CalculateTotalWidth(topFolder, 0, g) / _zoomScale) + panel1.AutoScrollPosition.X;

            totalHeight = Math.Max(totalHeight, panel1.Height + 1);
            totalWidth = Math.Max(totalWidth, panel1.Width + 1);

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

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            // Check if Control key is pressed
            if (_ctrlKeyPressed)
            {
                // Adjust the zoom scale by 0.1f per wheel delta
                _zoomScale += e.Delta * 0.001f; // Change this value to zoom more or less per wheel notch
                _zoomScale = Math.Max(0.1f, _zoomScale); // Prevent zooming out too much

                panel1.Refresh();

                // Redraw the component with the new zoom scale
                panel1.Refresh(); // Triggers the Paint event where you redraw everything
            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                _ctrlKeyPressed = true;
            }
            else if (_ctrlKeyPressed && (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add))
            {
                _zoomScale += 0.1f;
                panel1.Refresh(); // Force immediate repaint of the panel
            }
            else if (_ctrlKeyPressed && (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract))
            {
                _zoomScale -= 0.1f;
                _zoomScale = Math.Max(0.1f, _zoomScale);
                panel1.Refresh(); // Force immediate repaint of the panel
            }
        }


        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // Reset the Control key state when it is released
            if (e.KeyCode == Keys.ControlKey)
            {
                _ctrlKeyPressed = false;
            }
        }

    }
}
