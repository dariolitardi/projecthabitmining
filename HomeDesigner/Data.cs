// Decompiled with JetBrains decompiler
// Type: HomeDesigner.Data
// Assembly: HomeDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3850526B-3877-4CBB-8C11-EAD333A0DB1D
// Assembly location: C:\Users\Dario\Desktop\HomeSensorSimulator\HomeSensorSimulator\HomeDesigner.exe

using SensorSim.Data;
using SensorSim.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;

using System.Threading; 
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace HomeDesigner
{
    internal class Data
    {
        static ReaderWriterLock locker = new ReaderWriterLock();

        private char[] splitChars = new char[4]
        {
            ' ',
            '\n',
            '\t',
            '\r'
        };

        public static bool inEsecuzione;
        public List<Position> pos;
        public List<Pair<Position, Position>> wall;
        public List<Sensor> sensor;
        public static int[,] directions;
        public String pathdir;
        public List<KeyValuePair<String, List<KeyValuePair<int, Position>>>> datasetFile;
        public void setPath()
        {
            var now = DateTime.Now;
            DateTime time = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            String[] arr = time.ToString("dd/MM/yyyy").Split('/');
            String s = arr[0] + "." + arr[1] + "." + arr[2];
            String[] arrTime = time.ToString("HH:mm:ss.fff").Split(':');
            String t = arrTime[0] + "." + arrTime[1] + "." + arrTime[2];

            pathdir = "Log\\" + "sim_" + s + "_" + t;
            DirectoryInfo di = Directory.CreateDirectory(pathdir);
            datasetFile = new List<KeyValuePair<String, List<KeyValuePair<int, Position>>>>();
        }

        public Data()
        {

            this.reset();
        }

        public void reset()
        {

            this.pos = new List<Position>();
            this.wall = new List<Pair<Position, Position>>();
            this.sensor = new List<Sensor>();

        }

        public bool loadHouse(string filePath)
        {
            try
            {
                this.reset();
                IEnumerator enumerator =
                    ((IEnumerable<string>)File.ReadAllText("Data/House_models/" + filePath + "/house.txt")
                        .Split(this.splitChars, StringSplitOptions.RemoveEmptyEntries))
                    .Where<string>((Func<string, bool>)(item => item[0] != ';')).ToArray<string>().GetEnumerator();
                enumerator.MoveNext();
                while (enumerator.MoveNext() && !((string)enumerator.Current).ToLower().Equals("walls"))
                {
                    Position position = new Position();
                    position.name = (string)enumerator.Current;
                    enumerator.MoveNext();
                    position.x = (double)int.Parse((string)enumerator.Current);
                    enumerator.MoveNext();
                    position.y = (double)int.Parse((string)enumerator.Current);
                    this.pos.Add(position);
                }

                while (enumerator.MoveNext() && !((string)enumerator.Current).Equals("Sensors"))
                {
                    string current = (string)enumerator.Current;
                    enumerator.MoveNext();
                    this.wall.Add(new Pair<Position, Position>(this.findPos(current),
                        this.findPos((string)enumerator.Current)));
                }

                while (enumerator.MoveNext() && enumerator.Current != null)
                {
                    Sensor sensor = new Sensor();
                    sensor.name = (string)enumerator.Current;
                    enumerator.MoveNext();
                    sensor.x = double.Parse((string)enumerator.Current);
                    enumerator.MoveNext();
                    sensor.y = double.Parse((string)enumerator.Current);
                    enumerator.MoveNext();
                    sensor.orinentation = double.Parse((string)enumerator.Current);
                    enumerator.MoveNext();
                    sensor.type = (string)enumerator.Current;
                    this.sensor.Add(sensor);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void readPositions(String path, Dictionary<string, List<KeyValuePair<Position, Color>>> dictionary, Color color)
        {
            try
            {
                String lineLog;
                FileStream fs = new FileStream(path, FileMode.Open);

                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr1 = new StreamReader(fs);

                //Read the first line of text
                lineLog = sr1.ReadLine();
                //Continue to read until you reach end of file
                int i = 0;
                while (lineLog != null)
                {


                    String[] parsedLine = lineLog.Split(' ');
                    Position position = new Position();
                    String ts = parsedLine[0];

                    position.x = (double)Convert.ToDouble(parsedLine[1]);
                    position.y = (double)Convert.ToDouble(parsedLine[2]);

                    if (dictionary.ContainsKey(ts))
                    {
                        KeyValuePair<Position, Color> keyValuePair = new KeyValuePair<Position, Color>(position, color);
                        dictionary[ts].Add(keyValuePair);


                    }
                    else
                    {
                        List<KeyValuePair<Position, Color>> lista = new List<KeyValuePair<Position, Color>>();
                        KeyValuePair<Position, Color> keyValuePair = new KeyValuePair<Position, Color>(position, color);
                        lista.Add(keyValuePair);
                        dictionary.Add(ts, lista);

                    }




                    lineLog = sr1.ReadLine();

                }





                fs.Close();

                sr1.Close();
                return;

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return;
            }
            finally
            {
                Console.WriteLine("Executing finally block.");

            }

        }



        private Position findPos(string name)
        {
            foreach (Position po in this.pos)
            {
                if (po.name.Equals(name))
                    return po;
            }

            return (Position)null;
        }

        public void saveHouse(string filePath, string posString, string wallString, string sensorString)
        {
            Directory.CreateDirectory("Data/House_models/" + filePath);
            TextWriter textWriter = (TextWriter)new StreamWriter("Data/House_models/" + filePath + "/house.txt");
            textWriter.WriteLine("Positions");
            textWriter.WriteLine(posString);
            textWriter.WriteLine("Walls");
            textWriter.WriteLine(wallString);
            textWriter.WriteLine("Sensors");
            textWriter.WriteLine(sensorString);
            textWriter.Close();
        }

        public List<Position> costruisciMuri(float distanza)
        {
            List<Position> muri = new List<Position>();
            foreach (Pair<Position, Position> pair in wall)
            {
                Position first = pair.First;
                Position second = pair.Second;
                if (first.x == second.x)
                {


                    muri.Add(first);
                    muri.Add(second);
                    if (first.y < second.y)
                    {


                        float n = (int)first.y + distanza;
                        for (float j = n; j < second.y; j += distanza)
                        {

                            Position p3 = new Position();
                            p3.x = first.x;
                            p3.y = j;
                            muri.Add(p3);
                        }
                    }
                    else if (first.y > second.y)
                    {


                        float n = (int)first.y - distanza;
                        for (float j = n; j > second.y; j -= distanza)
                        {

                            Position p3 = new Position();
                            p3.x = first.x;
                            p3.y = j;
                            muri.Add(p3);
                        }
                    }
                }
                else if (first.y == second.y)
                {


                    muri.Add(first);
                    muri.Add(second);
                    if (first.x < second.x)
                    {
                        float m = (int)(first.x) + distanza;
                        for (float i = m; i < second.x; i += distanza)
                        {
                            Position p3 = new Position();
                            p3.x = i;
                            p3.y = first.y;
                            muri.Add(p3);
                        }
                    }
                    else if (first.x > second.x)
                    {
                        float m = (int)(first.x) - distanza;
                        for (float i = m; i > second.x; i -= distanza)
                        {
                            Position p3 = new Position();
                            p3.x = i;
                            p3.y = first.y;
                            muri.Add(p3);
                        }
                    }
                }

            }

            return muri;
        }

        public void simulationHandler(List<KeyValuePair<int, string>> utenti, string tipoTraiettoria, int giorniSimulazione)
        {
            int lunghezzaAscissa;
            int lunghezzaOrdinata;
            float distanza;

            List<KeyValuePair<int, Position>> listaOggetti = new List<KeyValuePair<int, Position>>();
            foreach (Position p1 in pos)
            {
                Position p = new Position();
                p.x = p1.x;
                p.y = p1.y;

                String s = p1.name;

                switch (s)
                {
                  
                  
             
                
                 
                    case "coat_hangers":
                        listaOggetti.Add(new KeyValuePair<int, Position>(0, p));


                        break;
                    case "table":
                        listaOggetti.Add(new KeyValuePair<int, Position>(1, p));



                        break;

                    case "sofa":
                        listaOggetti.Add(new KeyValuePair<int, Position>(2, p));




                        break;

                }
            }
            for (int i = 0; i < utenti.Count; i++)
            {
                int contaGiorni=0;
                float vel = (float)Convert.ToDouble(utenti[i].Value.Split(' ')[0]);
                bool flag = Convert.ToBoolean(utenti[i].Value.Split(' ')[1]);

                if (vel == 0.25f && flag == false)
                {
                    lunghezzaAscissa = 66;
                    lunghezzaOrdinata = 84;
                    distanza = 25;
                }
                else if (vel == 0.5f && flag == false)
                {
                    lunghezzaAscissa = 33;
                    lunghezzaOrdinata = 42;
                    distanza = 50;


                } else if (vel == 1f && flag == false)
                {
                    lunghezzaAscissa = 33;
                    lunghezzaOrdinata = 42;
                    distanza = 50;

                } else if (vel == 1f && flag == true)
                {
                    lunghezzaAscissa = 33;
                    lunghezzaOrdinata = 42;
                    distanza = 50;

                }
                else
                {
                    lunghezzaAscissa = 0;
                    lunghezzaOrdinata = 0;
                    distanza = 0;
                }

                this.generatePathLog(pathdir, i + 1, lunghezzaAscissa, lunghezzaOrdinata, 
                    distanza, flag, vel, tipoTraiettoria, listaOggetti, contaGiorni, giorniSimulazione);


            }


            MessageBox.Show("Simulation done!");



        }



        private String converti(List<KeyValuePair<int, Position>> lista)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<int, Position> keyValuePair in lista)
            {
                stringBuilder.Append(keyValuePair.Value.x + " " + keyValuePair.Value.y + " ");
            }

            return stringBuilder.ToString();
        }

        private List<KeyValuePair<int, Position>> posizioniUguali(List<KeyValuePair<int, Position>> lista)
        {
            List<KeyValuePair<int, Position>> positions = new List<KeyValuePair<int, Position>>();
            positions.Add(new KeyValuePair<int, Position>(lista[0].Key, lista[0].Value));

            for (int i = 0; i < lista.Count; i++)
            {

                if (i + 1 < lista.Count)
                {
                    if (contenuto(i + 1, lista, lista[i]))
                        positions.Add(new KeyValuePair<int, Position>(lista[i].Key, lista[i].Value));
                }

            }

            if (positions.Count == 0 || positions.Count == 1)
                return null;
            else
                return positions;

        }

        private bool contenuto(int j, List<KeyValuePair<int, Position>> lista, KeyValuePair<int, Position> p)
        {
            for (int i = j; i < lista.Count; i++)
            {
                if (p.Value.x == lista[i].Value.x && p.Value.y == lista[i].Value.y)
                    return true;

            }

            return false;
        }
        public void readPathLog(int idFile)
        {

            FileStream fs = new FileStream(pathdir + "\\PathLog" + idFile + ".txt", FileMode.Open);

            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr1 = new StreamReader(fs);

            //Read the first line of text
            String lineLog = sr1.ReadLine();
            //Continue to read until you reach end of file
            int i = 0;
            int idUser = idFile - 1;
            while (lineLog != null)
            {
                if (i < 86300)
                {
                    String[] parsedLine = lineLog.Split(' ');
                    Position p = new Position();
                    p.x = Convert.ToDouble(parsedLine[1]);
                    p.y = Convert.ToDouble(parsedLine[2]);

                    datasetFile[i].Value.Add(new KeyValuePair<int, Position>(idUser, p));
                    i++;
                    lineLog = sr1.ReadLine();
                } else
                    break;
            }
            sr1.Close();
            fs.Close();

        }
        public Bitmap drawPicture(bool drawPlaces, bool drawSensors, bool drawWalls)
        {


            Bitmap bitmap = new Bitmap("Resources/grid_25.PNG");
            Graphics graphics = Graphics.FromImage((Image)bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen pen1 = new Pen(Brushes.Blue, 3f);
            foreach (Pair<Position, Position> pair in this.wall)
                graphics.DrawLine(pen1, (float)pair.First.x / 2f, (float)pair.First.y / 2f,
                    (float)pair.Second.x / 2f,
                    (float)pair.Second.y / 2f);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Far;
            Pen pen2 = new Pen(Brushes.Black, 2f);
            foreach (Position po in this.pos)
            {
                int result;
                if ((!po.name.StartsWith("p") || !int.TryParse(po.name.Substring(1), out result) || drawWalls) &&
                    (po.name.StartsWith("p") && int.TryParse(po.name.Substring(1), out result) || drawPlaces))
                {
                    graphics.DrawEllipse(pen2, (float)(po.x / 2.0 - 5.0), (float)(po.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(po.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Black,
                        new PointF((float)po.x / 2f, (float)po.y / 2f), format);
                }
            }

            Pen pen3 = new Pen(Brushes.Green, 2f);
            Pen pen55 = new Pen(Brushes.Red, 2f);

            pen55.DashCap = System.Drawing.Drawing2D.DashCap.Round;

            // Create a custom dash pattern.
            pen55.DashPattern = new float[] { 4.0F, 2.0F, 2.0F, 2.0F };
            if (drawSensors)
            {
                foreach (Sensor sensor in this.sensor)
                {
                    graphics.DrawEllipse(pen3, (float)(sensor.x / 2.0 - 5.0), (float)(sensor.y / 2.0 - 5.0), 10f,
                        10f);
                    graphics.DrawString(sensor.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Green,
                        new PointF((float)sensor.x / 2f, (float)sensor.y / 2f), format);
                    graphics.DrawLine(pen3, (float)sensor.x / 2f, (float)sensor.y / 2f,
                        (float)(sensor.x / 2.0 + Math.Sin(sensor.orinentation / 180.0 * 3.14) * 10.0),
                        (float)(sensor.y / 2.0 - Math.Cos(sensor.orinentation / 180.0 * 3.14) * 10.0));
                    DrawCircle(graphics, pen55, (float)sensor.x / 2f, (float)sensor.y / 2f, 50);
                }
            }

            return bitmap;

        }
        public void DrawCircle(Graphics g, Pen pen,
            float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }
      
        public Bitmap DrawSimulation(Form form, String folderPath, PictureBox pictureBox1, bool drawPlaces, bool drawSensors, bool drawWalls,
            DateTime timeInizio, DateTime timeFine, float zoomFactor)
        {
            int c = 0;
            DateTime timeInizioApp = timeInizio;
            Bitmap bitmap = new Bitmap("Resources/grid_25.PNG");

            Graphics graphics = Graphics.FromImage((Image)bitmap);

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen pen1 = new Pen(Brushes.Blue, 3f);
            foreach (Pair<Position, Position> pair in this.wall)
                graphics.DrawLine(pen1, (float)pair.First.x / 2f, (float)pair.First.y / 2f, (float)pair.Second.x / 2f,
                    (float)pair.Second.y / 2f);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Far;
            Pen pen2 = new Pen(Brushes.Black, 2f);
            foreach (Position po in this.pos)
            {
                int result;
                if ((!po.name.StartsWith("p") || !int.TryParse(po.name.Substring(1), out result) || drawWalls) &&
                    (po.name.StartsWith("p") && int.TryParse(po.name.Substring(1), out result) || drawPlaces))
                {
                    graphics.DrawEllipse(pen2, (float)(po.x / 2.0 - 5.0), (float)(po.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(po.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Black,
                        new PointF((float)po.x / 2f, (float)po.y / 2f), format);
                }
            }

            Pen pen3 = new Pen(Brushes.Green, 2f);
            if (drawSensors)
            {
          
                foreach (Sensor sensor in this.sensor)
                {
                    graphics.DrawEllipse(pen3, (float)(sensor.x / 2.0 - 5.0), (float)(sensor.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(sensor.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Green,
                        new PointF((float)sensor.x / 2f, (float)sensor.y / 2f), format);
                    graphics.DrawLine(pen3, (float)sensor.x / 2f, (float)sensor.y / 2f,
                        (float)(sensor.x / 2.0 + Math.Sin(sensor.orinentation / 180.0 * 3.14) * 10.0),
                        (float)(sensor.y / 2.0 - Math.Cos(sensor.orinentation / 180.0 * 3.14) * 10.0));
                }
            }



            Bitmap bitmapOriginal = bitmap;
            Graphics graphicsOriginal = graphics;

            int incrementoTM = 0;
            string[] fileArray = Directory.GetFiles(folderPath, "*.txt");
            Dictionary<string, List<KeyValuePair<Position, Color>>> dictionary = new Dictionary<string, List<KeyValuePair<Position, Color>>>();
            dictionary.Clear();

            Random rand = new Random();
            int i = 0;

            foreach (String filename in fileArray)
            {
                if (filename.Contains("PathLog"))
                {


                    Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));

                    readPositions(filename, dictionary, color);

                }
            }

            int incremento = 0;

            graphics.DrawLine(pen3, 0, 0, 6.25f, 6.25f);

            List<KeyValuePair<Position, Color>> list;
            List<KeyValuePair<Position, Color>> list2;
            bool cancellaLinee = false;

            list = dictionary[timeInizio.ToString("HH:mm:ss.fff")];
            while (timeInizio < (timeFine))
            {
                if (Form1.stopSimulazione)
                {
                    Thread thread = Thread.CurrentThread;

                    bitmap = drawPicture(drawPlaces, false, drawWalls);
                    graphics = Graphics.FromImage((Image)bitmap);
                    pictureBox1.Invoke((MethodInvoker)delegate
                    {



                        pictureBox1.Image = (Image)bitmap;

                        pictureBox1.Refresh(); //questo refresh serve per restiture al gui thread un risultato parziale e temporaneo dell'immagine

                    });
                    thread.Abort();

                    return bitmap;
                }

                if (Form1.pauseSimulazione)
                {
                    while (true)
                    {
                        if (Form1.restartSimulazione)
                        {
                            break;
                        } else if (Form1.stopSimulazione)
                        {
                            Thread thread = Thread.CurrentThread;

                            bitmap = drawPicture(drawPlaces, false, drawWalls);
                            graphics = Graphics.FromImage((Image)bitmap);
                            pictureBox1.Invoke((MethodInvoker)delegate
                            {



                                pictureBox1.Image = (Image)bitmap;

                                pictureBox1.Refresh(); //questo refresh serve per restiture al gui thread un risultato parziale e temporaneo dell'immagine

                            });
                            thread.Abort();

                            return bitmap;
                        }


                    }
                }
                if (cancellaLinee)
                {
                    incremento++;
                    incrementoTM += 1000;
                    bitmap = Form1.bitmapHouse;



                }

                timeInizio = timeInizio.AddMilliseconds(1000);

                list2 = dictionary[timeInizio.ToString("HH:mm:ss.fff")];

                foreach (KeyValuePair<Position, Color> keyValuePair in list)
                {

                    ThreadStart threadDelegate = delegate { bitmap = DrawerHandler(keyValuePair, list2, graphics, pictureBox1, cancellaLinee, dictionary, timeInizioApp, incremento, bitmap, graphicsOriginal, drawPlaces, drawSensors, drawWalls, "simulation", incrementoTM); ; };
                    Thread newThread = new Thread(threadDelegate);
                    newThread.Start();


                    pictureBox1.Invoke((MethodInvoker)delegate
                    {



                        pictureBox1.Image = (Image)bitmap;

                        pictureBox1.Refresh(); //questo refresh serve per restiture al gui thread un risultato parziale e temporaneo dell'immagine

                    });


                    newThread.Join();

                    // ogni linea viene graficata ogni mezzo secondo



                }


                Task.Delay(1000).Wait();
                if (c < 10)
                {
                    cancellaLinee = false;
                    c++;

                }
                else
                {


                    cancellaLinee = true;
                }

                list = list2;



            }


            list.Clear();
            dictionary.Clear();
            return bitmap;

        }


        public Bitmap DrawPrediction(Form form, String folderPath, PictureBox pictureBox1, bool drawPlaces, bool drawSensors, bool drawWalls,
          DateTime timeInizio, DateTime timeFine, float zoomFactor)
        {
            int c = 0;
            DateTime timeInizioApp = timeInizio;
            Bitmap bitmap = new Bitmap("Resources/grid_25.PNG");

            Graphics graphics = Graphics.FromImage((Image)bitmap);

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen pen1 = new Pen(Brushes.Blue, 3f);
            foreach (Pair<Position, Position> pair in this.wall)
                graphics.DrawLine(pen1, (float)pair.First.x / 2f, (float)pair.First.y / 2f, (float)pair.Second.x / 2f,
                    (float)pair.Second.y / 2f);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Far;
            Pen pen2 = new Pen(Brushes.Black, 2f);
            foreach (Position po in this.pos)
            {
                int result;
                if ((!po.name.StartsWith("p") || !int.TryParse(po.name.Substring(1), out result) || drawWalls) &&
                    (po.name.StartsWith("p") && int.TryParse(po.name.Substring(1), out result) || drawPlaces))
                {
                    graphics.DrawEllipse(pen2, (float)(po.x / 2.0 - 5.0), (float)(po.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(po.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Black,
                        new PointF((float)po.x / 2f, (float)po.y / 2f), format);
                }
            }

            Pen pen3 = new Pen(Brushes.Green, 2f);
            if (drawSensors)
            {
                foreach (Sensor sensor in this.sensor)
                {
                    graphics.DrawEllipse(pen3, (float)(sensor.x / 2.0 - 5.0), (float)(sensor.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(sensor.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Green,
                        new PointF((float)sensor.x / 2f, (float)sensor.y / 2f), format);
                    graphics.DrawLine(pen3, (float)sensor.x / 2f, (float)sensor.y / 2f,
                        (float)(sensor.x / 2.0 + Math.Sin(sensor.orinentation / 180.0 * 3.14) * 10.0),
                        (float)(sensor.y / 2.0 - Math.Cos(sensor.orinentation / 180.0 * 3.14) * 10.0));
                }
            }



            Bitmap bitmapOriginal = bitmap;
            Graphics graphicsOriginal = graphics;

            int incrementoTM = 0;
            string[] fileArray = Directory.GetFiles(folderPath, "*.txt");
            Dictionary<string, List<KeyValuePair<Position, Color>>> dictionary = new Dictionary<string, List<KeyValuePair<Position, Color>>>();
            dictionary.Clear();

            Random rand = new Random();
            int i = 0;

            foreach (String filename in fileArray)
            {
                if (filename.Contains("PathLog"))
                {


                    Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));

                    readPositions(filename, dictionary, color);

                }
            }

            int incremento = 0;

            graphics.DrawLine(pen3, 0, 0, 6.25f, 6.25f);

            List<KeyValuePair<Position, Color>> list;
            List<KeyValuePair<Position, Color>> list2;
            bool cancellaLinee = false;

            list = dictionary[timeInizio.ToString("HH:mm:ss.fff")];
            while (timeInizio < (timeFine))
            {
                if (Form1.stopSimulazione)
                {
                    bitmap = drawPicture(drawPlaces, false, drawWalls);
                    return bitmap;
                }

                if (Form1.pauseSimulazione)
                {
                    while (true)
                    {
                        if (Form1.restartSimulazione)
                        {
                            break;
                        }
                        else if (Form1.stopSimulazione)
                        {
                            bitmap = drawPicture(drawPlaces, false, drawWalls);
                            return bitmap;
                        }


                    }
                }
                if (cancellaLinee)
                {
                    incremento++;
                    incrementoTM += 1000;
                    bitmap = Form1.bitmapHouse;



                }

                timeInizio = timeInizio.AddMilliseconds(1000);

                list2 = dictionary[timeInizio.ToString("HH:mm:ss.fff")];

                foreach (KeyValuePair<Position, Color> keyValuePair in list)
                {

                    ThreadStart threadDelegate = delegate { bitmap = DrawerHandler(keyValuePair, list2, graphics, pictureBox1, cancellaLinee, dictionary, timeInizioApp, incremento, bitmap, graphicsOriginal, drawPlaces, drawSensors, drawWalls, "simulation", incrementoTM); ; };
                    Thread newThread = new Thread(threadDelegate);
                    newThread.Start();


                    pictureBox1.Invoke((MethodInvoker)delegate
                    {



                        pictureBox1.Image = (Image)bitmap;

                        pictureBox1.Refresh(); //questo refresh serve per restiture al gui thread un risultato parziale e temporaneo dell'imagine

                    });


                    newThread.Join();

                    // ogni linea viene graficata ogni mezzo secondo



                }


                Task.Delay(1000).Wait();
                if (c < 10)
                {
                    cancellaLinee = false;
                    c++;

                }
                else
                {


                    cancellaLinee = true;
                }

                list = list2;



            }


            list.Clear();
            dictionary.Clear();
            return bitmap;

        }

        private static DateTime timeInizioAppoggio;

        public Bitmap DrawerHandler(KeyValuePair<Position, Color> entry, List<KeyValuePair<Position, Color>> lista, Graphics graphics, PictureBox pictureBox, bool cancellaLinee,
         Dictionary<string, List<KeyValuePair<Position, Color>>> dictionary, DateTime timeInizioApp, int incremento,
            Bitmap bitmap, Graphics graphicsOriginal, bool drawPlaces, bool drawSensors, bool drawWalls, string tipoGraficazione, int incrementoTM)
        {
            timeInizioAppoggio = timeInizioApp;
            foreach (KeyValuePair<Position, Color> keyValuePair in lista)
            {


                if (keyValuePair.Value.Equals(entry.Value))
                {
                    if (cancellaLinee)
                    {

                        bitmap = drawPicture(drawPlaces, false, drawWalls);

                        graphics = Graphics.FromImage((Image)bitmap);
                        timeInizioApp = timeInizioApp.AddMilliseconds(incrementoTM);
                        List<KeyValuePair<Position, Color>> list1;
                        List<KeyValuePair<Position, Color>> list2;
                        list1 = dictionary[timeInizioApp.ToString("HH:mm:ss.fff")];


                        for (int g = incremento; g < incremento + 10; g++)
                        {

                            timeInizioApp = timeInizioApp.AddMilliseconds(1000);

                            list2 = dictionary[timeInizioApp.ToString("HH:mm:ss.fff")];


                            foreach (KeyValuePair<Position, Color> k in list1)
                            {


                                DrawerHandlerPastLines(k, list2, graphics, tipoGraficazione);


                            }
                            list1 = list2;


                        }
                    }
                    else
                    {
                        Pen pen = new Pen(keyValuePair.Value, 3f);
                        Position p1 = entry.Key;

                        Position p2 = keyValuePair.Key;

                        if (p1.x == p2.x && p1.y == p2.y) //posizioni uguali quindi utente fermo
                        {
                            Brush brush = new SolidBrush(keyValuePair.Value);

                            graphics.FillRectangle(brush, (float)p1.x / 2f, (float)p1.y / 2f, 4f, 4f);

                        }
                        else
                        {
                            if (tipoGraficazione.Equals("prediction"))
                            {
                                pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;

                                // Create a custom dash pattern.
                                pen.DashPattern = new float[] { 4.0F, 2.0F, 2.0F, 2.0F };
                            }

                            graphics.DrawLine(pen, (float)p1.x / 2f, (float)p1.y / 2f, (float)p2.x / 2f, (float)p2.y / 2f);
                        }


                    }

                }
            }
            return bitmap;


        }

        private double CalcolaDistanza(Position position, Sensor s)
        {
            double distanza = Math.Sqrt(Math.Pow((position.x - s.x), 2) + Math.Pow((position.y - s.y), 2));
            return distanza;
        }

        private Sensor getSensor(Position position)
        {
            Sensor foundedSensor = null;
            foreach (Sensor s in sensor)
            {

                if (CalcolaDistanza(position, s) <= 100 && !isIntersectsWall(position, s))
                {

                    foundedSensor = s;
                    return foundedSensor;
                }

            }

            return foundedSensor;
        }


        public void DrawerHandlerPastLines(KeyValuePair<Position, Color> entry, List<KeyValuePair<Position, Color>> lista,
            Graphics graphics, string tipoGraficazione
        )
        {


            foreach (KeyValuePair<Position, Color> keyValuePair in lista)
            {
                if (keyValuePair.Value.Equals(entry.Value))
                {
                    Pen pen = new Pen(keyValuePair.Value, 3f);
                    Position p1 = entry.Key;

                    Position p2 = keyValuePair.Key;
                    if (p1.x == p2.x && p1.y == p2.y)
                    {
                        Brush brush = new SolidBrush(keyValuePair.Value);

                        graphics.FillRectangle(brush, (float)p1.x / 2f, (float)p1.y / 2f, 4f, 4f);

                    }
                    else
                    {

                        if (tipoGraficazione.Equals("prediction"))
                        {
                            pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;

                            // Create a custom dash pattern.
                            pen.DashPattern = new float[] { 4.0F, 2.0F, 2.0F, 2.0F };
                        }


                        graphics.DrawLine(pen, (float)p1.x / 2f, (float)p1.y / 2f, (float)p2.x / 2f,
                            (float)p2.y / 2f);

                    }

                    return;
                }
            }
        }





        public void generatePathLog(String pathdir, int u, int lunghezzaAscissa,
            int lunghezzaOrdinata, float distanza,
            bool velocitaVariabile, float vel, string tipoTraiettoria,
            List<KeyValuePair<int, Position>> listaOggetti, int contaGiorni, int giorniSimulazione)
        {

            //costruzione grafo 


            List<Position> lista = costruisciMuri(distanza);
            List<Position> muri = new List<Position>();

            foreach (Position position in lista)
            {
                Position p = new Position();
                p.x = position.x;
                p.y = position.y;
                muri.Add(p);


            }

            List<Position> nodi = new List<Position>();
            Dictionary<int, Position> d1 = new Dictionary<int, Position>();

            Position[,] matrix = new Position[lunghezzaAscissa, lunghezzaOrdinata];

            int m = 0;
            int n = 0;
            for (int i = 0; i < lunghezzaAscissa; i++)
            {
                for (int j = 0; j < lunghezzaOrdinata; j++)
                {
                    Position p = new Position();

                    p.x = (j * distanza);
                    p.y = (i * distanza);
                    if (IsWall(muri, p))
                        matrix[i, j] = null;
                    else
                    {
                        nodi.Add(p);
                        matrix[i, j] = p;

                    }

                }
            }


            List<Pair<Position, Position>> archi = new List<Pair<Position, Position>>();
            for (int i = 0; i < lunghezzaAscissa; i++)
            {
                for (int j = 0; j < lunghezzaOrdinata; j++)
                {
                    if (matrix[i, j] != null)
                    {

                        if (i + 1 < lunghezzaAscissa && j < lunghezzaOrdinata)
                        {
                            if (matrix[i + 1, j] != null)
                            {
                                Pair<Position, Position> pair =
                                    new Pair<Position, Position>(matrix[i, j], matrix[i + 1, j]);
                                archi.Add(pair);
                                /*    graphics.DrawLine(pen10, (float) matrix[i, j].x , (float) matrix[i, j].y , 
                                      (float)matrix[i + 1, j].x ,
                                      (float)matrix[i + 1, j].y );*/
                            }
                        }

                        if (i < lunghezzaAscissa && j + 1 < lunghezzaOrdinata)
                        {
                            if (matrix[i, j + 1] != null)
                            {
                                Pair<Position, Position> pair =
                                    new Pair<Position, Position>(matrix[i, j], matrix[i, j + 1]);
                                archi.Add(pair);
                                /*    graphics.DrawLine(pen10, (float) matrix[i, j ].x , (float) matrix[i, j ].y , 
                                      (float) matrix[i, j + 1].x ,
                                      (float)matrix[i, j + 1].y );*/
                            }

                        }

                        if (i + 1 < lunghezzaAscissa && j + 1 < lunghezzaOrdinata)
                        {
                            if (matrix[i + 1, j + 1] != null)
                            {
                                Pair<Position, Position> pair =
                                    new Pair<Position, Position>(matrix[i, j], matrix[i + 1, j + 1]);
                                archi.Add(pair);
                                /*  graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                    (float) matrix[i + 1, j + 1].x,
                                    (float) matrix[i + 1, j + 1].y);*/
                            }
                        }

                        if (i - 1 >= 0 && j < lunghezzaOrdinata)
                        {
                            if (matrix[i - 1, j] != null)
                            {
                                Pair<Position, Position> pair =
                                    new Pair<Position, Position>(matrix[i, j], matrix[i - 1, j]);
                                archi.Add(pair);
                                /*  graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                    (float) matrix[i - 1, j].x,
                                    (float) matrix[i - 1, j].y);*/
                            }
                        }

                        if (j - 1 >= 0 && i < lunghezzaAscissa)

                        {
                            if (matrix[i, j - 1] != null)
                            {
                                Pair<Position, Position> pair =
                                    new Pair<Position, Position>(matrix[i, j], matrix[i, j - 1]);
                                archi.Add(pair);
                                /* graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                   (float) matrix[i, j - 1].x,
                                   (float) matrix[i, j - 1].y);*/
                            }
                        }

                        if (j - 1 >= 0 && i - 1 >= 0)

                        {
                            if (matrix[i - 1, j - 1] != null)
                            {
                                Pair<Position, Position> pair =
                                    new Pair<Position, Position>(matrix[i, j], matrix[i - 1, j - 1]);
                                archi.Add(pair);
                                /*  graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                    (float) matrix[i - 1, j - 1].x,
                                    (float) matrix[i - 1, j - 1].y);*/
                            }

                        }

                        if (j + 1 < lunghezzaOrdinata && i - 1 >= 0)

                        {
                            if (matrix[i - 1, j + 1] != null)
                            {
                                Pair<Position, Position> pair =
                                    new Pair<Position, Position>(matrix[i, j], matrix[i - 1, j + 1]);
                                archi.Add(pair);
                                /*graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                  (float) matrix[i - 1, j + 1].x,
                                  (float) matrix[i - 1, j + 1].y);*/
                            }
                        }

                        if (i + 1 < lunghezzaAscissa && j - 1 >= 0)

                        {
                            if (matrix[i + 1, j - 1] != null)
                            {
                                Pair<Position, Position> pair =
                                    new Pair<Position, Position>(matrix[i, j], matrix[i + 1, j - 1]);
                                archi.Add(pair);
                                /* graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                   (float) matrix[i + 1, j - 1].x,
                                   (float) matrix[i + 1, j - 1].y);*/
                            }
                        }



                        //  graphics.FillRectangle(drawBrush, (float) matrix[i, j].x, (float) matrix[i, j].y, 3, 3);
                    }
                }
            }

            for (int i = 0; i < nodi.Count; i++)
            {
                d1.Add(i, nodi[i]);
            }
            List<int> myKeys = d1.Keys.ToList();
            var nodes = myKeys;

            List<Pair<int, int>> listaArchi = new List<Pair<int, int>>();
            foreach (Pair<Position, Position> pair in archi)
            {
                int n1 = getNodi(pair.First, d1);
                int n2 = getNodi(pair.Second, d1);
                listaArchi.Add(new Pair<int, int>(n1, n2));
            }


            var edg = new Pair<int, int>[listaArchi.Count];

            for (int j = 0; j < listaArchi.Count; j++)
            {
                edg[j] = listaArchi[j];
            }

            var graph = new Graph<int>(nodes, edg);



            TextWriter log = (TextWriter)new StreamWriter(pathdir + "\\PathLog" + u + ".txt");
            var now = DateTime.Now;
            DateTime time = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0);
            Position source, dest = null;
            int indice = 0;
            DateTime maxDate = time.AddDays(giorniSimulazione);
            maxDate.AddHours(23);
            maxDate.AddMinutes(59);
            maxDate.AddSeconds(59);
            maxDate.AddMilliseconds(0);

            while (time!=maxDate)
            {
                if (time == maxDate)
                {
                    log.Close();
                    return;
                }
              

                int s1 = 0;
                Random random = new Random();
                int idOggetto = random.Next(0, 3);
                if (indice == 0)
                {
                    indice = 1;
                    source = listaOggetti[idOggetto].Value;
                    s1 = getNodi(source, d1);
                }
                else
                {

                    source = dest;
                    source.x = dest.x;
                    source.y = dest.y;

                    s1 = getNodi(source, d1);

                }



                dest = listaOggetti[2 - idOggetto].Value;

                int iddest = listaOggetti[2 - idOggetto].Key;

                int dg = getNodi(dest, d1);


                var startVertex = s1;
                var shortestPath = ShortestPathFunction(graph, startVertex);
                var d = dg;

                List<int> path = shortestPath(d).ToList();
                if (path.Count == 1)
                {
                    continue;
                }
                for (int g = 0; g < path.Count; g++)
                {
                    if (velocitaVariabile)
                    {
                        if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                        {

                            if (contaGiorni == giorniSimulazione)
                            {
                                log.Close();
                                return;
                            }
                            contaGiorni++;

                        }

                        Random randVelocita = new Random();
                        float velocita = (float)(((randVelocita.NextDouble() * 100) / 2) / 2);
                        if (velocita < 6.25f)
                        {
                            velocita = 6.25f;
                        }

                        Position posizione0 = getPosizione(path[g], d1);
                        log.WriteLine(time.ToString("HH:mm:ss.fff")
                                      + " " + posizione0.x + " " + posizione0.y);
                        time = time.AddSeconds(1);
                        if (g + 1 < path.Count)
                        {
                            Position posizione1 = getPosizione(path[g + 1], d1);

                            double x = Math.Abs(posizione0.x - posizione1.x);
                            double y = Math.Abs(posizione0.y - posizione1.y);
                            Position posizioneNuova = new Position();
                            double xCount = posizione0.x;
                            double yCount = posizione0.y;
                            while (Math.Abs(posizione1.x - xCount) > velocita &&
                                   Math.Abs(posizione1.y - yCount) > velocita)
                            {
                                if (time == maxDate)
                                {
                                    log.Close();
                                    return;
                                }


                                if (x == 25f)
                                {
                                    if (posizione0.x < posizione1.x)
                                        xCount += velocita;
                                    else
                                        xCount -= velocita;

                                    posizioneNuova.x = xCount;
                                }

                                if (y == 25f)
                                {
                                    if (posizione0.y < posizione1.y)
                                        yCount += velocita;
                                    else
                                        yCount -= velocita;

                                    posizioneNuova.y = yCount;
                                }

                                if (x == 0f)
                                {
                                    posizioneNuova.x = xCount;

                                }

                                if (y == 0f)
                                {
                                    posizioneNuova.y = yCount;

                                }


                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + posizioneNuova.x + " " + posizioneNuova.y);

                                posizioneNuova.x = 0;
                                posizioneNuova.y = 0;

                                time = time.AddSeconds(1);
                            }
                            /*
                                                            if(Math.Abs(posizione1.x)!=Math.Abs(posizioneNuova.x) && Math.Abs(posizione1.y)!=Math.Abs(posizioneNuova.y))
                                                            {
                                                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                                                              + " " + posizione1.x + " " + posizione1.y);
                                                                time = time.AddSeconds(1);


                                                            }*/

                        }



                    }
                    else if (velocitaVariabile == false && vel == 1f)
                    {
                        if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.000"))
                        {

                            if (contaGiorni == giorniSimulazione)
                            {
                                log.Close();
                                return;
                            }
                            contaGiorni++;

                        }
                        if (g == path.Count - 1 && g % 2 != 0)
                        {
                            Position posizione = getPosizione(path[g], d1);

                            log.WriteLine(time.ToString("HH:mm:ss.fff")
                                          + " " + posizione.x + " " + posizione.y);
                            time = time.AddSeconds(1);
                        }

                        if (g % 2 == 0)
                        {


                            Position posizione = getPosizione(path[g], d1);

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    dest.name = s.name;
                                    log.WriteLine(time.ToString("HH:mm:ss.fff") + " " + dest.x.ToString() + " " + dest.y.ToString() + " " + dest.name);

                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + +posizione.x + " " + posizione.y);


                            }

                            time = time.AddSeconds(1);
                        }
                    }
                    else
                    {
                        if (time == maxDate)
                        {
                            log.Close();
                            return;
                        }

                        Position posizione = getPosizione(path[g], d1);





                        if (tipoTraiettoria.Equals("Discrete"))
                        {
                            Sensor s = getSensor(posizione);
                            if (s != null)
                            {
                                if (time == maxDate)
                                {
                                    log.Close();
                                    return;
                                }
                                log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                s.name,"ON"));



                            }

                            time = time.AddSeconds(1);


                        }
                        else
                        {





                            if (vel == 0.25f)
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + posizione.x.ToString() + " " + posizione.y.ToString());

                                time = time.AddSeconds(1);

                            } else {

                                if (g + 1 < path.Count)
                                {
                                    Position posizioneSuccessiva = getPosizione(path[g + 1], d1);
                                    Position posApp = new Position();
                                    posApp.x = posizione.x;
                                    posApp.y = posizione.y;

                                    float xoff = 10;
                                    float yoff = 10;
                                    if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }

                                    log.WriteLine(time.ToString("HH:mm:ss.fff")
                                                  + " " + posizione.x.ToString() + " " + posizione.y.ToString());


                                    if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }

                                    for (int s = 1; s <= 4; s++)
                                    {
                                        time = time.AddMilliseconds(200);

                                        if (
                                            posizioneSuccessiva.y < posizione.y)
                                        {
                                            posApp.y -= yoff;
                                        }

                                        if (posizioneSuccessiva.x < posizione.x
                                        )
                                        {
                                            posApp.x -= xoff;


                                        }

                                        if (posizioneSuccessiva.y > posizione.y)
                                        {
                                            posApp.y += xoff;


                                        }

                                        if (posizioneSuccessiva.x > posizione.x
                                        )
                                        {
                                            posApp.x += xoff;

                                        }

                                        if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.000"))
                                        {

                                            if (contaGiorni == giorniSimulazione)
                                            {
                                                log.Close();
                                                return;
                                            }
                                            contaGiorni++;

                                        }

                                        log.WriteLine(time.ToString("HH:mm:ss.fff")
                                                      + " " + posApp.x.ToString() + " " + posApp.y.ToString());




                                    }

                                    time = time.AddMilliseconds(200);
                                    if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.000"))
                                    {
                                        log.Close();
                                        return;
                                    }


                              

                                }
                            }



                        }
                    }
                }


               /* Random r;
                int minAttesa;
                int secondiAttesa;
                switch (iddest)
                {
                    case 1:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);



                        }

                        break;
                    case 0:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)

                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }
                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);


                        }

                        break;
                    case 2:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }

                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 3:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {

                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);


                        }

                        break;
                    case 4:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }

                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }


                        break;
                    case 5:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }



                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }

                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }

                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 6:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }
                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 7:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 8:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }
                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 9:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 10:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }

                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 11:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);


                        }

                        break;
                    case 12:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 13:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.000"))
                            {
                                log.Close();
                                return;
                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 14:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.000"))
                            {
                                log.Close();
                                return;
                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 15:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.000"))
                            {
                                log.Close();
                                return;
                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }

                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }

                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 16:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.000"))
                            {
                                log.Close();
                                return;
                            }



                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }

                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 17:


                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }
                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 18:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 19:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;

                    case 20:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }
                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());


                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 21:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 22:
                        r = new Random();
                        minAttesa = r.Next(1, 3);

                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 23:
                        r = new Random();
                        minAttesa = r.Next(1, 3);

                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }
                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 24:
                        r = new Random();
                        minAttesa = r.Next(1, 3);

                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 25:
                        r = new Random();
                        minAttesa = r.Next(1, 3);

                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 26:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 27:
                        r = new Random();
                        minAttesa = r.Next(1, 3);

                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 28:
                        r = new Random();
                        minAttesa = r.Next(1, 3);

                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 29:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 30:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }

                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                    case 31:
                        r = new Random();
                        minAttesa = r.Next(1, 3);
                        secondiAttesa = minAttesa * 60;
                        for (int t = 0; t < secondiAttesa; t++)
                        {
                            if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                            {

                                if (contaGiorni == giorniSimulazione)
                                {
                                    log.Close();
                                    return;
                                }
                                contaGiorni++;

                            }


                            if (tipoTraiettoria.Equals("Discrete"))
                            {
                                Sensor s = getSensor(dest);
                                if (s != null)
                                {
                                    if (time.ToString("HH:mm:ss.ffffff").Equals("23:59:59.000000"))
                                    {

                                        if (contaGiorni == giorniSimulazione)
                                        {
                                            log.Close();
                                            return;
                                        }
                                        contaGiorni++;

                                    }
                                    dest.name = s.name;

                                    log.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", time.ToString("yyyy-MM-dd"), time.ToString("HH:mm:ss.ffffff"),

                                                    dest.name, "ON"));



                                }


                            }
                            else
                            {
                                log.WriteLine(time.ToString("HH:mm:ss.fff")
                                              + " " + dest.x.ToString() + " " + dest.y.ToString());

                            }
                            time = time.AddSeconds(1);

                        }

                        break;
                }
                */
                if (time.ToString("HH:mm:ss.fff").Equals("23:59:59.0000"))
                {

                    if (contaGiorni == giorniSimulazione)
                    {
                        log.Close();
                        return;
                    }
                    contaGiorni++;

                }

            }



        }
    



       
        private Position getPosizione(int n, Dictionary<int, Position> nodi)
        {
            foreach (KeyValuePair<int, Position> keyValuePair in nodi)
            {
                
                if (keyValuePair.Key==n)
                    return keyValuePair.Value;
                
            }

            return null;
        }
        private int getNodi(Position p, Dictionary<int, Position> dictionary)
        {
            int key = -1;
            int c = 0;
     
            foreach (KeyValuePair<int, Position> keyValuePair in dictionary)
            {


                if (keyValuePair.Value.x == p.x &&
                     keyValuePair.Value.y ==p.y)
                {
                    key = keyValuePair.Key;
                    break;
                }

                c++;
            }

          
                return key;
        }
  
        public bool IsWall(List<Position> muri, Position p)
        {
            foreach (Position position in muri)
            {
                if (position.x== p.x && position.y==p.y)
                    return true;
            }

            return false;
        }

 
        public Func<T, IEnumerable<T>> ShortestPathFunction<T>(Graph<T> graph, T start) {
            var previous = new Dictionary<T, T>();
            var queue = new Queue<T>();
            queue.Enqueue(start);
         
            while (queue.Count > 0) {
                var vertex = queue.Dequeue();
                
                foreach(var neighbor in graph.AdjacencyList[vertex]) {
                  
                    if (previous.ContainsKey(neighbor))
                        continue;
            
                    previous[neighbor] = vertex;
                    queue.Enqueue(neighbor);
                }
            }

            Func<T, IEnumerable<T>> shortestPath = v => {
                var path = new List<T>{};

                var current = v;
                while (!current.Equals(start)) {
                    path.Add(current);
                    current = previous[current];
                };

                path.Add(start);
                path.Reverse();

                return path;
            };

            return shortestPath;
        }
   
        private bool isIntersectsWall(Position p1, Position p2)
        {
            foreach (Pair<Position, Position> pair in this.wall)
            {
                if ((p1.x != pair.First.x || p1.y != pair.First.y) && (p1.x != pair.Second.x || p1.y != pair.Second.y) && (p2.x != pair.First.x || p2.y != pair.First.y) && (p2.x != pair.Second.x || p2.y != pair.Second.y))
                {
                    Position first = pair.First;
                    Position second = pair.Second;
                    double num1 = (p1.x - p2.x) * (first.y - second.y) - (p1.y - p2.y) * (first.x - second.x);
                    if (num1 != 0.0)
                    {
                        double num2 = ((p1.x * p2.y - p1.y * p2.x) * (first.x - second.x) - (p1.x - p2.x) * (first.x * second.y - first.y * second.x)) / num1;
                        double num3 = ((p1.x * p2.y - p1.y * p2.x) * (first.y - second.y) - (p1.y - p2.y) * (first.x * second.y - first.y * second.x)) / num1;
                        if ((p1.x <= num2 && p2.x >= num2 || p1.x >= num2 && p2.x <= num2 || p1.x == p2.x) && (p1.y <= num3 && p2.y >= num3 || p1.y >= num3 && p2.y <= num3 || p1.y == p2.y) && (first.x <= num2 && second.x >= num2 || first.x >= num2 && second.x <= num2 || first.x == second.x) && (first.y <= num3 && second.y >= num3 || first.y >= num3 && second.y <= num3 || first.y == second.y))
                            return true;
                    }
                }
            }
            return false;
        }


    }
  
  

}