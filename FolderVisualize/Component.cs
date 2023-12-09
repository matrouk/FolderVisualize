using System;
using System.Drawing;

namespace FolderVisualize
{
    public enum VisualizationMode
    {
        Horizontal,
        Vertical
    }

    public abstract class Component
    {
        public static int PanelWidth { get; set; }
        public static VisualizationMode Mode { get; set; } = VisualizationMode.Horizontal; // Default to horizontal mode.
        public static int startingYPosition { get; set; }


        public string Name { get; protected set; }

        protected Component(string name)
        {
            Name = name;
        }

        public abstract int CalculateSize();
        public abstract void Visualize(Graphics g, int depth, int x, ref int y, Font font, Brush brush);
    }
}
