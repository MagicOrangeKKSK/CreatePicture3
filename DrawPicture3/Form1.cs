using DrawPicture3.AST;
using DrawPicture3.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawPicture3
{
    public partial class Form1 : Form
    {
        public static Settings Settings;
        private DnaDrawing currentDrawing;
        private double errorLevel = double.MaxValue;
        private int generation;
        private DnaDrawing guiDrawing;
        private bool isRunning;
        private DateTime lastRepaint = DateTime.MinValue;
        private int lastSelected;
        private TimeSpan repaintIntervall = new TimeSpan(0, 0, 0, 0, 0);
        private int repaintOnSelectedSteps = 3;
        private int selected;
        private Color[,] sourceColors;
        private Thread thread;

        public Form1()
        {
            InitializeComponent();
        

            if (Settings == null)
                Settings = new Settings();


            panel1.Width = trackBarScale.Value * pictureBox1.Width;
            panel1.Height = trackBarScale.Value * pictureBox1.Height;
        }

        public void StartEvolution()
        {
            SetupSourceColorMatrix();
            if (currentDrawing == null)
            {
                currentDrawing = GetNewInitializedDrawing();
            }
            lastSelected = 0;

            while (isRunning)
            {
                DnaDrawing newDrawing;
                lock (currentDrawing)
                {
                    newDrawing = currentDrawing.Clone();
                }
                newDrawing.Mutate();

                if (newDrawing.IsDirty)
                {
                    generation++;

                    double newErrorLevel = FitnessCalculator.GetDrawingFitness(newDrawing, sourceColors);

                    if (newErrorLevel <= errorLevel)
                    {
                        selected++;
                        lock (currentDrawing)
                        {
                            currentDrawing = newDrawing;
                        }
                        errorLevel = newErrorLevel;
                    }
                }
            }
        }

        public void SetupSourceColorMatrix()
        {
            sourceColors = new Color[Tools.MaxWidth, Tools.MaxHeight];
            var sourceImage = pictureBox1.Image as Bitmap;

            if (sourceImage == null)
                throw new NotSupportedException("请选择图片！");

            for (int y = 0; y < Tools.MaxHeight; y++)
            {
                for (int x = 0; x < Tools.MaxWidth; x++)
                {
                    Color c = sourceImage.GetPixel(x, y);
                    sourceColors[x, y] = c;
                }
            }
        }


        private static DnaDrawing GetNewInitializedDrawing()
        {
            var drawing = new DnaDrawing();
            drawing.Init();
            return drawing;
        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImage();
        }

        private void OpenImage()
        {
            Stop();

            string fileName = FileUtil.GetOpenFileName(FileUtil.ImgExtension);
            if (string.IsNullOrEmpty(fileName))
                return;

            pictureBox1.Image = Image.FromFile(fileName);

            Tools.MaxHeight = pictureBox1.Height;
            Tools.MaxWidth = pictureBox1.Width;

            panel1.Width = trackBarScale.Value* pictureBox1.Width;
            panel1.Height = trackBarScale.Value*pictureBox1.Height;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isRunning)
                Stop();
            else
                Start();
        }

        private void Start()
        {
            btnStart.Text = "Stop";
            isRunning = true;
            tmrRedraw.Enabled = true;

            if (thread != null)
                KillThread();

            thread = new Thread(StartEvolution)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };

            thread.Start();
        }

        private void KillThread()
        {
            if (thread != null)
            {
                thread.Abort();
            }
            thread = null;
        }

        private void Stop()
        {
            if (isRunning)
                KillThread();

            btnStart.Text = "Start";
            isRunning = false;
            tmrRedraw.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currentDrawing == null)
                return;
            int polygons = currentDrawing.Polygons.Count;
            int points = currentDrawing.PointCount;
            double avg = 0;
            if (polygons != 0)
                avg = points / polygons;

            LabelFitness.Text = errorLevel.ToString();
            LabelGeneration.Text = generation.ToString();
            LabelSelected.Text = selected.ToString();
            LabelPointCount.Text = points.ToString();
            LabelPolygons.Text = polygons.ToString();
            LabelPointsPerPolygons.Text = avg.ToString();

            bool shouldRepaint = false;
            if (repaintIntervall.Ticks > 0)
                if (lastRepaint < DateTime.Now - repaintIntervall)
                    shouldRepaint = true;

            if (repaintOnSelectedSteps > 0)
                if (lastSelected + repaintOnSelectedSteps < selected)
                    shouldRepaint = true;

            if (shouldRepaint)
            {
                lock (currentDrawing)
                {
                    guiDrawing = currentDrawing.Clone();
                }
                panel1.Invalidate();
                lastRepaint = DateTime.Now;
                lastSelected = selected;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (guiDrawing == null)
            {
                e.Graphics.Clear(Color.Black);
                return;
            }
            using (var backBuffer = new Bitmap(trackBarScale.Value * pictureBox1.Width,
                                               trackBarScale.Value * pictureBox1.Height, 
                                               PixelFormat.Format24bppRgb))
            {
                using(Graphics backGraphics = Graphics.FromImage(backBuffer))
                {
                    backGraphics.SmoothingMode = SmoothingMode.HighQuality;
                    Renderer.Render(guiDrawing, backGraphics, trackBarScale.Value );
                    e.Graphics.DrawImage(backBuffer, 0, 0);
                }
            }
        }

        private void trackBarScale_Scroll(object sender, EventArgs e)
        {
            panel1.Width = trackBarScale.Value * pictureBox1.Width;
            panel1.Height = trackBarScale.Value * pictureBox1.Height;
        }
    }
   
}
