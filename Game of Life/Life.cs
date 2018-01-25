using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Game_of_Life
{
    class Life
    {
        public delegate void LifeEventHandler();
        public event LifeEventHandler OnStateChange;

        PictureBox pb;
        float x = 20,y = 15;
        int zoom;
        bool multicolor;
        List<HashSet<Point>> states;
        Point delta = new Point();
        private HashSet<Point> currentState
        {
            get { return states[states.Count - 1]; }
            set { states[states.Count - 1] = value; }
        }
        public Life(PictureBox pb, int zoom = 25)
        {
            this.pb = pb;
            pb.Paint += Pb_Paint;
            pb.MouseWheel += Pb_MouseWheel;
            pb.MouseEnter += Pb_MouseEnter;
            pb.MouseDown += Pb_MouseDown;
            pb.MouseUp += Pb_MouseUp;
            OnStateChange += Life_OnStateChange;
            Interactive = true;
            this.zoom = zoom;
            Reset();
        }
        private void Life_OnStateChange()
        {
            pb.Refresh();
        }
        private void Pb_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Interactive) return;
            if (Math.Abs(delta.X - e.X) > zoom / 4 || Math.Abs(delta.Y - e.Y) > zoom / 4)
            {
                x += ((float)delta.X - e.X) / zoom;
                y += ((float)delta.Y - e.Y) / zoom;
            }
            else
            {
                int i = (int)Math.Floor((e.X + x * zoom - pb.Width / 2) / zoom), j = (int)Math.Floor((e.Y + y * zoom - pb.Height / 2) / zoom);
                if (currentState.Contains(new Point(i, j))) currentState.Remove(new Point(i, j));
                else currentState.Add(new Point(i, j));
            }
            OnStateChange.Invoke();
        }
        private void Pb_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Interactive) return;
            delta = e.Location;
        }
        private void Pb_MouseEnter(object sender, EventArgs e) { pb.Focus(); }
        private void Pb_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!Interactive) return;
            zoom = Math.Max(5, zoom + Math.Sign(e.Delta));
            OnStateChange.Invoke();
        }
        private void Pb_Paint(object sender, PaintEventArgs e)
        {
            for (int i = (int)Math.Floor(x - pb.Width / 2 / zoom)-1; i <= Math.Ceiling(pb.Width / 2 / zoom + x); i++)
                for (int j = (int)Math.Floor(y - pb.Height / 2 / zoom)-1; j <= (int)Math.Ceiling(pb.Height / 2 / zoom + y); j++)
                    e.Graphics.FillRectangle(color(i, j),
                        Math.Max(0,i * zoom - x * zoom + pb.Width / 2),
                        Math.Max(0,j * zoom - y * zoom + pb.Height / 2),
                        i * zoom - x * zoom + pb.Width / 2 + zoom, 
                        j * zoom - y * zoom + pb.Height / 2 + zoom);

            for (int i = (int)Math.Floor(x - pb.Width / 2 / zoom)-1; i <= Math.Ceiling(pb.Width / 2 / zoom + x); i++)
                e.Graphics.DrawLine(Pens.LightGray, i*zoom - x * zoom + pb.Width / 2, 0, i*zoom - x * zoom + pb.Width / 2, pb.Height);
            for (int i = (int)(y - pb.Height / 2 / zoom)-1; i <= Math.Ceiling(pb.Height / 2 / zoom + y); i++)
                e.Graphics.DrawLine(Pens.LightGray, 0, i*zoom - y * zoom + pb.Height / 2, pb.Width, i*zoom - y * zoom + pb.Height / 2);

        }
        public void Reset()
        {
            x = y = 0; zoom = 25;
            states = new List<HashSet<Point>>();
            states.Add(new HashSet<Point>());
            OnStateChange.Invoke();
        }
        private int aliveNeighbors(int x, int y)
        {
            int s = 0;
            int[] d = { -1, 0, 1 };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if ((i != 1 || j != 1) && currentState.Contains(new Point(x + d[i], y + d[j]))) s++;
            return s;
        }
        private int aliveNeighbors(Point p)
        {
            return aliveNeighbors(p.X, p.Y);
        }
        public void Next()
        {
            if (!Interactive) return;
            int[] d = { -1, 0, 1 };
            HashSet<Point> tmp = new HashSet<Point>();
            foreach (Point p in currentState)
            {
                if (aliveNeighbors(p) == 2 || aliveNeighbors(p) == 3) tmp.Add(p);
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        if ((i != 1 || j != 1) && aliveNeighbors(new Point(p.X + d[i], p.Y + d[j])) == 3)
                            tmp.Add(new Point(p.X + d[i], p.Y + d[j]));
            }
            states.Add(tmp);
            OnStateChange.Invoke();
        }
        public bool Reversible { get { return states.Count > 1; } }
        public void Previous()
        {
            if (!Interactive) return;
            if (!Reversible) return;
            states.RemoveAt(states.Count - 1);
            OnStateChange.Invoke();
        }
        public bool MultiColor { get { return multicolor; } set { multicolor = value; pb.Refresh(); } }
        public bool Interactive { get; set; }
        private Brush color(int i, int j)
        {
            if (!MultiColor)
            {
                if (currentState.Contains(new Point(i, j))) return new Pen(Color.RoyalBlue).Brush;
                else return new Pen(Color.White).Brush;
            }
            if (currentState.Contains(new Point(i, j)) && (aliveNeighbors(i, j) == 2 || aliveNeighbors(i, j) == 3)) return new Pen(Color.RoyalBlue).Brush;
            if (currentState.Contains(new Point(i, j))) return new Pen(Color.FromArgb(90, 0, 200)).Brush;
            if (aliveNeighbors(i, j) == 3) return new Pen(Color.LightBlue).Brush;
            return new Pen(Color.White).Brush;
        }
        public void Export(StreamWriter sw)
        {
            /*StringBuilder sb = new StringBuilder(x + "\n" + y + "\n");
            for (int k = 0; k < states.Count; k++)
                for (int i = 0; i < x; i++)
                    for (int j = 0; j < y; j++)
                        sb.Append((states[k][i, j] ? 1 : 0) + "\n");
            sw.Write(Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString())));
            sw.Close();*/
            OnStateChange.Invoke();
        }
        public void Import(StreamReader sr)
        {
            /*string[] data = Encoding.UTF8.GetString(Convert.FromBase64String(sr.ReadToEnd())).Split('\n');
            Reset(int.Parse(data[0]), int.Parse(data[1]));
            for (int i = 1; i < (data.Length - 3) / (x * y); i++) states.Add(new bool[x, y]);
            for (int i = 2; i < data.Length - 1; i++) states[(i - 2) / (x * y)][((i - 2) % (x * y)) / x, (i - 2) % x] = (int.Parse(data[i]) == 1);*/
            OnStateChange.Invoke();
        }
    }
}