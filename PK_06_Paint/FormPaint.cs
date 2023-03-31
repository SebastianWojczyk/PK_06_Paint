using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PK_06_Paint
{
    public partial class FormPaint : Form
    {
        List<Point> points = new List<Point>();
        Graphics graphics;

        List<Image> history;
        public FormPaint()
        {
            InitializeComponent();
            nowyToolStripMenuItem_Click(null, null);
        }
        private void FormPaint_MouseMove(object sender, MouseEventArgs e)
        {
            this.Text = e.Location.ToString() + e.Button;
        }
        private void pictureBoxImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                points = new List<Point>();
                points.Add(e.Location);

                while (undoCount > 0)
                {
                    history.Remove(history.Last());
                    undoCount--;
                }
            }
        }
        private void pictureBoxImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                points.Add(e.Location);
                Pen myPen = new Pen(buttonColorPen.BackColor, (int)numericUpDownWidthPen.Value);
                Brush myBrush = new SolidBrush(buttonColorFill.BackColor);

                Draw(myPen, myBrush);

                history.Add(new Bitmap(pictureBoxImage.Image));
                pictureBoxImage.Refresh();
            }
        }

        private void Draw(Pen myPen, Brush myBrush)
        {
            if (radioButtonLine.Checked)
            {
                graphics.DrawLine(myPen, points.First(), points.Last());
            }
            else if (radioButtonRectangle.Checked)
            {
                if (checkBoxFill.Checked)
                {
                    graphics.FillRectangle(myBrush,
                                           Math.Min(points.First().X, points.Last().X),
                                           Math.Min(points.First().Y, points.Last().Y),
                                           Math.Abs(points.First().X - points.Last().X),
                                           Math.Abs(points.First().Y - points.Last().Y));
                }
                graphics.DrawRectangle(myPen,
                                       Math.Min(points.First().X, points.Last().X),
                                       Math.Min(points.First().Y, points.Last().Y),
                                       Math.Abs(points.First().X - points.Last().X),
                                       Math.Abs(points.First().Y - points.Last().Y));
            }
            else if (radioButtonEllipse.Checked)
            {
                if (checkBoxFill.Checked)
                {
                    graphics.FillEllipse(myBrush,
                                         Math.Min(points.First().X, points.Last().X),
                                         Math.Min(points.First().Y, points.Last().Y),
                                         Math.Abs(points.First().X - points.Last().X),
                                         Math.Abs(points.First().Y - points.Last().Y));
                }
                graphics.DrawEllipse(myPen,
                                     Math.Min(points.First().X, points.Last().X),
                                     Math.Min(points.First().Y, points.Last().Y),
                                     Math.Abs(points.First().X - points.Last().X),
                                     Math.Abs(points.First().Y - points.Last().Y));
            }
            else if (radioButtonCurve.Checked)
            {
                graphics.DrawCurve(myPen, points.ToArray());
            }
        }

        private void pictureBoxImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                points.Add(e.Location);

                pictureBoxImage.Image = new Bitmap(history.Last());
                graphics = Graphics.FromImage(pictureBoxImage.Image);

                Pen myPen = new Pen(Color.Black, (int)numericUpDownWidthPen.Value);
                Brush myBrush = new SolidBrush(Color.LightGray);
                Draw(myPen, myBrush);

                pictureBoxImage.Refresh();
            }
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = (sender as Button).BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                (sender as Button).BackColor = cd.Color;
            }
        }

        private void nowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBoxImage.Image = new Bitmap(pictureBoxImage.Width, pictureBoxImage.Height);
            graphics = Graphics.FromImage(pictureBoxImage.Image);
            graphics.Clear(Color.White);
            history = new List<Image>();
            undoCount = 0;
            history.Add(new Bitmap(pictureBoxImage.Image));
        }

        private void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Bitmapa|*.bmp|JPEG|*.jpg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBoxImage.Image = new Bitmap(ofd.FileName);
                graphics = Graphics.FromImage(pictureBoxImage.Image);
                history = new List<Image>();
                undoCount = 0;
                history.Add(new Bitmap(pictureBoxImage.Image));
            }
        }

        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Bitmapa|*.bmp|JPEG|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                String extension = Path.GetExtension(sfd.FileName);
                switch (extension)
                {
                    case ".bmp":
                        pictureBoxImage.Image.Save(sfd.FileName, ImageFormat.Bmp);
                        break;
                    case ".jpg":
                        pictureBoxImage.Image.Save(sfd.FileName, ImageFormat.Jpeg);
                        break;

                }
            }
        }

        int undoCount = 0;
        private void cofnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoCount < history.Count - 1)
            {
                undoCount++;
                pictureBoxImage.Image = new Bitmap(history[history.Count - 1 - undoCount]);
                graphics = Graphics.FromImage(pictureBoxImage.Image);
            }
        }

        private void ponówToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoCount > 0)
            {
                undoCount--;
                pictureBoxImage.Image = new Bitmap(history[history.Count - 1 - undoCount]);
                graphics = Graphics.FromImage(pictureBoxImage.Image);
            }
        }
    }
}
