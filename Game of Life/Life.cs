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
        float zoom;
        bool multicolor;
        bool moving;
        Point delta = new Point();

        bool[] birth = { false, false, false, true, false, false, false, false, false };
        bool[] survival = { false, false, true, true, false, false, false, false, false };
        List<HashSet<Cell>> states;
        private HashSet<Cell> currentState
        {
            get { return states[states.Count - 1]; }
            set { states[states.Count - 1] = value; }
        }
        public Life(PictureBox pb)
        {
            this.pb = pb;
            pb.Paint += Pb_Paint;
            pb.MouseWheel += Pb_MouseWheel;
            pb.MouseEnter += Pb_MouseEnter;
            pb.MouseDown += Pb_MouseDown;
            pb.MouseUp += Pb_MouseUp;
            OnStateChange += Life_OnStateChange;
            pb.MouseMove += Pb_MouseMove;
            Interactive = true;
            Reset();
        }
        private void Pb_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Interactive) return;
            if (e.Button != MouseButtons.Left) return;
            moving = true;
            x += ((float)delta.X - e.X) / zoom;
            y += ((float)delta.Y - e.Y) / zoom;
            delta = e.Location;
            OnStateChange.Invoke();
        }
        private void Life_OnStateChange()
        {
            pb.Refresh();
        }
        private void Pb_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Interactive) return;
            if (moving) { moving = false; return; }
            int i = (int)Math.Floor((e.X + x * zoom - pb.Width / 2) / zoom), j = (int)Math.Floor((e.Y + y * zoom - pb.Height / 2) / zoom);
            if (currentState.Contains(new Cell(i, j))) currentState.Remove(new Cell(i, j));
            else currentState.Add(new Cell(i, j));
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
                    e.Graphics.FillRectangle(color(new Cell(i, j)),
                        Math.Max(0,i * zoom - x * zoom + pb.Width / 2),
                        Math.Max(0,j * zoom - y * zoom + pb.Height / 2),
                        i * zoom - x * zoom + pb.Width / 2 + zoom, 
                        j * zoom - y * zoom + pb.Height / 2 + zoom);
            if (zoom < 5) return;
            for (int i = (int)Math.Floor(x - pb.Width / 2 / zoom)-1; i <= Math.Ceiling(pb.Width / 2 / zoom + x); i++)
                e.Graphics.DrawLine(Pens.LightGray, i*zoom - x * zoom + pb.Width / 2, 0, i*zoom - x * zoom + pb.Width / 2, pb.Height);
            for (int i = (int)(y - pb.Height / 2 / zoom)-1; i <= Math.Ceiling(pb.Height / 2 / zoom + y); i++)
                e.Graphics.DrawLine(Pens.LightGray, 0, i*zoom - y * zoom + pb.Height / 2, pb.Width, i*zoom - y * zoom + pb.Height / 2);

        }
        public void Reset()
        {
            x = y = 0; zoom = pb.Width / 20;
            states = new List<HashSet<Cell>>();
            states.Add(new HashSet<Cell>(Cell.Comparer()));
            OnStateChange.Invoke();
        }
        private int aliveNeighbors(Cell c)
        {
            int s = 0;
            foreach (Cell n in c.Neighbors)
                if (currentState.Contains(n)) s++;
            return s;
        }
        public void Next()
        {
            if (!Interactive) return;
            HashSet<Cell> tmp = new HashSet<Cell>(Cell.Comparer());
            foreach (Cell c in currentState)
            {
                if (survival[aliveNeighbors(c)]) tmp.Add(c);
                foreach (Cell n in c.Neighbors)
                {
                    if (!currentState.Contains(n) && birth[aliveNeighbors(n)])
                        tmp.Add(n);
                }
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
        public bool[] BirthRule { get { return birth; } set { birth = value; } }
        public bool[] SurvivalRule { get { return survival; } set { survival = value; } }
        private Brush color(Cell c)
        {
            if (!MultiColor)
            {
                if (currentState.Contains(c)) return new Pen(Color.RoyalBlue).Brush;
                else return new Pen(Color.White).Brush;
            }
            if (currentState.Contains(c) && survival[aliveNeighbors(c)]) return new Pen(Color.RoyalBlue).Brush;
            if (currentState.Contains(c)) return new Pen(Color.FromArgb(90, 0, 200)).Brush;
            if (birth[aliveNeighbors(c)]) return new Pen(Color.LightBlue).Brush;
            return new Pen(Color.White).Brush;
        }
        public void Export(StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder(x + "A" + y + "A" + zoom / pb.Width + "A");
            for (int k = 0; k < states.Count; k++)
            {
                foreach (Point p in states[k])
                    sb.Append(p.X + "B" + p.Y + "A");
                sb.Append("FA");
            }
            while (sb.ToString().Length % 4 != 0) sb.Append("E");
            byte[] raw = Convert.FromBase64String(sb.ToString().Replace('-', 'C').Replace(',', 'D'));
            sw.BaseStream.Write(raw, 0, raw.Length);
            sw.Close();
        }
        public void Import(StreamReader sr)
        {
            byte[] raw = new byte[sr.BaseStream.Length];
            sr.BaseStream.Read(raw, 0, (int)sr.BaseStream.Length);
            string[] data = Convert.ToBase64String(raw).Replace('C', '-').Replace('D', ',').Split('A');
            x = float.Parse(data[0]);
            y = float.Parse(data[1]);
            zoom = float.Parse(data[2]) * pb.Width;
            states = new List<HashSet<Cell>>();
            states.Add(new HashSet<Cell>(Cell.Comparer()));
            for (int i = 3; i < data.Length - 1; i++)
            {
                if (data[i] == "F") { states.Add(new HashSet<Cell>(Cell.Comparer())); continue; }
                currentState.Add(new Cell(
                    int.Parse(data[i].Split('B')[0]), 
                    int.Parse(data[i].Split('B')[1])));
            }
            states.RemoveAt(states.Count - 1);
            OnStateChange.Invoke();
        }
    }
}