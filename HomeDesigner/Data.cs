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
        public List<KeyValuePair<String, List<KeyValuePair<int,Position> >>>datasetFile;
        public void setPath()
        {

            Random r = new Random();
            double c = r.NextDouble() * 1000;
            pathdir = "Log\\" + "sim_" + c;
            DirectoryInfo di = Directory.CreateDirectory(pathdir);
            datasetFile=new  List<KeyValuePair<String, List<KeyValuePair<int,Position> >>>();
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
                    ((IEnumerable<string>) File.ReadAllText("Data/House_models/" + filePath + "/house.txt")
                        .Split(this.splitChars, StringSplitOptions.RemoveEmptyEntries))
                    .Where<string>((Func<string, bool>) (item => item[0] != ';')).ToArray<string>().GetEnumerator();
                enumerator.MoveNext();
                while (enumerator.MoveNext() && !((string) enumerator.Current).ToLower().Equals("walls"))
                {
                    Position position = new Position();
                    position.name = (string) enumerator.Current;
                    enumerator.MoveNext();
                    position.x = (double) int.Parse((string) enumerator.Current);
                    enumerator.MoveNext();
                    position.y = (double) int.Parse((string) enumerator.Current);
                    this.pos.Add(position);
                }

                while (enumerator.MoveNext() && !((string) enumerator.Current).Equals("Sensors"))
                {
                    string current = (string) enumerator.Current;
                    enumerator.MoveNext();
                    this.wall.Add(new Pair<Position, Position>(this.findPos(current),
                        this.findPos((string) enumerator.Current)));
                }

                while (enumerator.MoveNext() && enumerator.Current != null)
                {
                    Sensor sensor = new Sensor();
                    sensor.name = (string) enumerator.Current;
                    enumerator.MoveNext();
                    sensor.x = double.Parse((string) enumerator.Current);
                    enumerator.MoveNext();
                    sensor.y = double.Parse((string) enumerator.Current);
                    enumerator.MoveNext();
                    sensor.orinentation = double.Parse((string) enumerator.Current);
                    enumerator.MoveNext();
                    sensor.type = (string) enumerator.Current;
                    this.sensor.Add(sensor);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private List<Position> readPositions(String path)
        {
            try
            {
                List<Position> map = new List<Position>();
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
                    if (i < 300)
                    {

                        String[] parsedLine = lineLog.Split(' ');
                        Position position = new Position();
                        float x0 = float.Parse(parsedLine[1], CultureInfo.InvariantCulture);
                        float y0 = float.Parse(parsedLine[2], CultureInfo.InvariantCulture);


                        position.x = (double) x0;
                        position.y = (double) y0;

                        map.Add(position);


                        lineLog = sr1.ReadLine();
                        i++;
                    }else
                        break;
                }

                fs.Close();

                sr1.Close();
                return map;

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return null;
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

            return (Position) null;
        }

        public void saveHouse(string filePath, string posString, string wallString, string sensorString)
        {
            Directory.CreateDirectory("Data/House_models/" + filePath);
            TextWriter textWriter = (TextWriter) new StreamWriter("Data/House_models/" + filePath + "/house.txt");
            textWriter.WriteLine("Positions");
            textWriter.WriteLine(posString);
            textWriter.WriteLine("Walls");
            textWriter.WriteLine(wallString);
            textWriter.WriteLine("Sensors");
            textWriter.WriteLine(sensorString);
            textWriter.Close();
        }

        public List<Position> costruisciMuri()
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


                        int n = (int) first.y + 50;
                        for (int j = n; j < second.y; j += 50)
                        {

                            Position p3 = new Position();
                            p3.x = first.x;
                            p3.y = j;
                            muri.Add(p3);
                        }
                    }
                    else if (first.y > second.y)
                    {


                        int n = (int) first.y - 50;
                        for (int j = n; j > second.y; j -= 50)
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
                        int m = (int) (first.x) + 50;
                        for (int i = m; i < second.x; i += 50)
                        {
                            Position p3 = new Position();
                            p3.x = i;
                            p3.y = first.y;
                            muri.Add(p3);
                        }
                    }
                    else if (first.x > second.x)
                    {
                        int m = (int) (first.x) - 50;
                        for (int i = m; i > second.x; i -= 50)
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

        public void simulationHandler(int numeroPersone)
        {
            //Crea il primo thread
            List<Thread> lista= new List<Thread>();
            for (int i = 0; i < numeroPersone; i++)
            {


                ThreadStart threadDelegate = delegate { this.generatePathLog(pathdir, i+1); };
                Thread newThread = new Thread(threadDelegate);
                newThread.Start();
                newThread.Join();
            }



           
          
            ThreadStart threadDelegate3 = delegate { this.generateDataset(numeroPersone);            }; 
            Thread newThread3 = new Thread(threadDelegate3);
            newThread3.Start();
            newThread3.Join();

        }

        public void generateDataset(int nFile)
        {
         
            try
            {
                locker.AcquireWriterLock(int.MaxValue); 
         
            TextWriter logDataset = (TextWriter) new StreamWriter(pathdir+"\\PathsDataset.txt");
            var now = DateTime.Now;
            DateTime time = new DateTime(now.Year, now.Month, now.Day,0,0,0);

            logDataset.WriteLine("timestamp u1 u2 crossing");

            for (int i = 0; i < 86300; i++)
            {
                
                datasetFile.Add(new KeyValuePair<String,List<KeyValuePair<int,Position>>>( time.ToString("HH:mm:ss"),new List<KeyValuePair<int,Position>>()));
                time = time.AddSeconds(1);
            }
            for (int i = 1; i <= nFile; i++)
            {
                readPathLog(i);
                
            }

         
//qui faccio il controllo degli incroci
         
          
                
                for (int j=0; j<datasetFile.Count; j++)
                {
                    StringBuilder stringBuilder= new StringBuilder();

                    if (j != 0 && j+1<datasetFile.Count)
                    {
                       
                        List<KeyValuePair<int,Position>> list= posizioniUguali(datasetFile[j].Value);
                        if (list != null)
                        {
                            List<KeyValuePair<int,Position>> list1=new List<KeyValuePair<int, Position>>() ;
                            List<KeyValuePair<int,Position>> list2=  new List<KeyValuePair<int, Position>>();

                           
                            
                          
                                list1.Add(datasetFile[j - 1].Value[0]);
                            list1.Add(datasetFile[j - 1].Value[1]);
                            list2.Add(datasetFile[j + 1].Value[0]);
                            list2.Add(datasetFile[j + 1].Value[1]);

                            
                            stringBuilder.Append("{");
                                //prodotto cartesiano per gli incroci
                            for(int m=0; m<list1.Count; m++)
                            {
                                for(int n=0; n<list2.Count; n++)
                                {
                                    if(m==list1.Count-1 && n==list2.Count-1)
                                        stringBuilder.Append("(("+list1[m].Value.x+"," 
                                                             +list1[m].Value.y+"),("+
                                                             list2[n].Value.x+","+
                                                             list2[n].Value.y+"))");
                                    else
                                    stringBuilder.Append("(("+list1[m].Value.x+"," 
                                                         +list1[m].Value.y+"),("+
                                                         list2[n].Value.x+","+
                                    list2[n].Value.y+")),");
                                   
                                } 
                            }
                            stringBuilder.Append("}");


                        }
                     
                    }

                    String s = converti(datasetFile[j].Value);
                    logDataset.WriteLine(datasetFile[j].Key+ " "+ s+stringBuilder.ToString());
                }
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
               
            
        }

        private String converti(List<KeyValuePair<int,Position> >lista)
        {
            StringBuilder stringBuilder= new StringBuilder();
            foreach (KeyValuePair<int,Position> keyValuePair in lista)
            {
                stringBuilder.Append(keyValuePair.Value.x + " " + keyValuePair.Value.y+" ");
            }

            return stringBuilder.ToString();
        }
    

        private   List<KeyValuePair<int,Position>> posizioniUguali( List<KeyValuePair<int,Position> >lista)
        {
            List<KeyValuePair<int,Position> >positions= new List<KeyValuePair<int,Position>>();
            positions.Add(new KeyValuePair<int,Position> (lista[0].Key,lista[0].Value));

            for (int i = 0; i < lista.Count; i++)
            {

                if (i + 1 < lista.Count)
                {
                    if (contenuto(i+1,lista,   lista[i]))
                        positions.Add(new KeyValuePair<int,Position> (lista[i].Key,lista[i].Value));
                }

            }

            if (positions.Count == 0 || positions.Count ==1)
                return null;
            else
            return positions;

        }

        private bool contenuto(int j, List<KeyValuePair<int,Position> >lista,  KeyValuePair<int, Position> p)
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
            
            FileStream fs = new FileStream(pathdir+"\\PathLog"+idFile+".txt", FileMode.Open);

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
                    Position p=new Position();
                    p.x = Convert.ToDouble(parsedLine[1]);
                    p.y = Convert.ToDouble(parsedLine[2]);

                    datasetFile[i].Value.Add(new KeyValuePair<int, Position>(idUser, p));
                    i++;
                    lineLog = sr1.ReadLine();
                }else
                    break;
            } 
            sr1.Close();
            fs.Close();
          
        }
        public Bitmap drawPicture(bool drawPlaces, bool drawSensors, bool drawWalls)
        {


            Bitmap bitmap = new Bitmap("Resources/grid_25.PNG");
            Graphics graphics = Graphics.FromImage((Image) bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen pen1 = new Pen(Brushes.Blue, 3f);
            foreach (Pair<Position, Position> pair in this.wall)
                graphics.DrawLine(pen1, (float) pair.First.x / 2f, (float) pair.First.y / 2f,
                    (float) pair.Second.x / 2f,
                    (float) pair.Second.y / 2f);
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
                    graphics.DrawEllipse(pen2, (float) (po.x / 2.0 - 5.0), (float) (po.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(po.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Black,
                        new PointF((float) po.x / 2f, (float) po.y / 2f), format);
                }
            }

            Pen pen3 = new Pen(Brushes.Green, 2f);
            if (drawSensors)
            {
                foreach (Sensor sensor in this.sensor)
                {
                    graphics.DrawEllipse(pen3, (float) (sensor.x / 2.0 - 5.0), (float) (sensor.y / 2.0 - 5.0), 10f,
                        10f);
                    graphics.DrawString(sensor.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Green,
                        new PointF((float) sensor.x / 2f, (float) sensor.y / 2f), format);
                    graphics.DrawLine(pen3, (float) sensor.x / 2f, (float) sensor.y / 2f,
                        (float) (sensor.x / 2.0 + Math.Sin(sensor.orinentation / 180.0 * 3.14) * 10.0),
                        (float) (sensor.y / 2.0 - Math.Cos(sensor.orinentation / 180.0 * 3.14) * 10.0));
                }
            }

            return bitmap;
        
    }
        
    public Bitmap drawPrediction(Dictionary<string, List<KeyValuePair<Position,Color>>> dictionary,bool drawPlaces, bool drawSensors, bool drawWalls)
        {
        

            Bitmap bitmap = new Bitmap("Resources/grid_25.PNG");
            Graphics graphics = Graphics.FromImage((Image) bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen pen1 = new Pen(Brushes.Blue, 3f);
            foreach (Pair<Position, Position> pair in this.wall)
                graphics.DrawLine(pen1, (float) pair.First.x / 2f, (float) pair.First.y / 2f, (float) pair.Second.x / 2f,
                    (float) pair.Second.y / 2f);
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
                    graphics.DrawEllipse(pen2, (float) (po.x / 2.0 - 5.0), (float) (po.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(po.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Black,
                        new PointF((float) po.x / 2f, (float) po.y / 2f), format);
                }
            }
          
            Pen pen3 = new Pen(Brushes.Green, 2f);
            if (drawSensors)
            {
                foreach (Sensor sensor in this.sensor)
                {
                    graphics.DrawEllipse(pen3, (float) (sensor.x / 2.0 - 5.0), (float) (sensor.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(sensor.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Green,
                        new PointF((float) sensor.x / 2f, (float) sensor.y / 2f), format);
                    graphics.DrawLine(pen3, (float) sensor.x / 2f, (float) sensor.y / 2f,
                        (float) (sensor.x / 2.0 + Math.Sin(sensor.orinentation / 180.0 * 3.14) * 10.0),
                        (float) (sensor.y / 2.0 - Math.Cos(sensor.orinentation / 180.0 * 3.14) * 10.0));
                }
            }

       
            Pen pen10 = new Pen(Brushes.Brown, 3f);
            Position p1, p2 = null;
            // Create rectangle to bound ellipse.
            Rectangle rect;
            List< List<KeyValuePair<Position,Color> >> listona=new List< List<KeyValuePair<Position,Color> >>();
            int j = 0;
            var now = DateTime.Now;
            DateTime time = new DateTime(now.Year, now.Month, now.Day,0,0,0);
            List<KeyValuePair<Position,Color>> list;
            List<KeyValuePair<Position,Color>> list2;

            list = dictionary[time.ToString("HH:mm:ss")];
            for(int h=1; h<dictionary.Count; h++)
            {                
                time = time.AddSeconds(1);

                list2 = dictionary[time.ToString("HH:mm:ss")];
                foreach (KeyValuePair<Position, Color> keyValuePair in list)
                {
                    DrawerHandler(keyValuePair, list2,graphics);
                }

                list = list2;


            }

            
           
            return bitmap;

        }
        
        private void DrawerHandler(KeyValuePair<Position,Color> entry, List<KeyValuePair<Position, Color>> lista,Graphics graphics)
        {
            foreach (KeyValuePair<Position, Color> keyValuePair in lista)
            {
                if (keyValuePair.Value.Equals(entry.Value))
                {
                    Pen pen = new Pen(keyValuePair.Value, 3f);
                    Position p1 = entry.Key;

                    Position p2 = keyValuePair.Key;
                   
                    graphics.DrawLine(pen, (float) p1.x, (float) p1.y, (float) p2.x, (float) p2.y);
                    return;
                }
            }
        }
 
         

    public Bitmap drawSimulate(bool drawPlaces, bool drawSensors, bool drawWalls)
        {
        

            Bitmap bitmap = new Bitmap("Resources/grid_25.PNG");
            Graphics graphics = Graphics.FromImage((Image) bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen pen1 = new Pen(Brushes.Blue, 3f);
            foreach (Pair<Position, Position> pair in this.wall)
                graphics.DrawLine(pen1, (float) pair.First.x / 2f, (float) pair.First.y / 2f, (float) pair.Second.x / 2f,
                    (float) pair.Second.y / 2f);
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
                    graphics.DrawEllipse(pen2, (float) (po.x / 2.0 - 5.0), (float) (po.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(po.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Black,
                        new PointF((float) po.x / 2f, (float) po.y / 2f), format);
                }
            }
          
            Pen pen3 = new Pen(Brushes.Green, 2f);
            if (drawSensors)
            {
                foreach (Sensor sensor in this.sensor)
                {
                    graphics.DrawEllipse(pen3, (float) (sensor.x / 2.0 - 5.0), (float) (sensor.y / 2.0 - 5.0), 10f, 10f);
                    graphics.DrawString(sensor.name, new Font("Times", 12f, FontStyle.Bold), Brushes.Green,
                        new PointF((float) sensor.x / 2f, (float) sensor.y / 2f), format);
                    graphics.DrawLine(pen3, (float) sensor.x / 2f, (float) sensor.y / 2f,
                        (float) (sensor.x / 2.0 + Math.Sin(sensor.orinentation / 180.0 * 3.14) * 10.0),
                        (float) (sensor.y / 2.0 - Math.Cos(sensor.orinentation / 180.0 * 3.14) * 10.0));
                }
            }

       

            List<Position >map = readPositions(pathdir+"\\PathLog1.txt");
            if (map == null)
                return bitmap;
            Pen pen10 = new Pen(Brushes.Brown, 3f);
            Position p1, p2 = null;
            // Create rectangle to bound ellipse.
            Rectangle rect;
             
            // Create start and sweep angles on ellipse.
            float startAngle = 90f;
            float sweepAngle = 270f;
            List<float> d1=new List<float>();
            d1.Add(startAngle);
            d1.Add(sweepAngle);

            float startAngle1 = 270f;
            float sweepAngle1 = 90f;
            List<float> d2=new List<float>();
            d2.Add(startAngle);
            d2.Add(sweepAngle);
            List<float> d3=new List<float>();
            d3.Add(0);
            d3.Add(180);
            List<float> d4=new List<float>();
            d4.Add(180);
            d4.Add(360);
            // Draw arc to screen.
            
            for (int i = 0; i <map.Count; i++)
            {
                p1 = map[i];
                if (i + 1 < map.Count)
                {
                    p2 = map[i + 1];
                   
                    graphics.DrawLine(pen10,(float) p1.x,(float)p1.y,(float) p2.x,(float)p2.y );
                

                }else
                    break;

            }
            
            List<Position >map2 = readPositions(pathdir+"\\PathLog2.txt");
            if (map2 == null)
                return bitmap;
            Pen pen = new Pen(Brushes.Gold, 3f);
            Position p11, p21 = null;
            // Create rectangle to bound ellipse.
            
            for (int i = 0; i < map2.Count; i++)
            {
                p11 = map2[i];
                if (i + 1 <map2.Count)
                {
                    p21 = map2[i + 1];
                    graphics.DrawLine(pen,(float) p11.x,(float)p11.y,(float) p21.x,(float)p21.y );


                }else
                    break;

            }
         
            
           
            return bitmap;

        }

     

        public  void generatePathLog( String  pathdir, int u )
        {
        
         //costruzione grafo 
            
      

           
         
            List<Position> lista = costruisciMuri();
            List<Position> muri = new List<Position>();

            foreach (Position position in lista)
            {
                Position p= new Position();
                p.x = position.x / 2f;
                p.y = position.y / 2f;
                muri.Add(p);


            }
            List<Position> nodi= new List<Position>();
            Dictionary<int,Position > d1= new Dictionary<int, Position>();

            Position [,] matrix= new Position[33,42];
      
            int m=0;
            int n=0;
            for (int i = 0; i < 33; i++)
            {
                for (int j = 0; j < 42; j++)
                {
                    Position p= new Position();

                    p.x = (j * 50)/2f;
                    p.y = (i * 50)/2f;
                    if (IsWall(muri, p))
                        matrix[i, j] = null;
                    else
                    {
                        nodi.Add(p);
                        matrix[i, j] = p;

                    }

                }
            }

    
            List<Pair<Position,Position>> archi = new List<Pair<Position,Position>> (); 
            for (int i = 0; i < 33; i++)
            {
                for (int j = 0; j < 42; j++)
                {
                    if (matrix[i, j] != null)
                    {
            
                        if (i + 1 < 33 && j < 42)
                        {
                            if (matrix[i + 1, j] != null)
                            {
                                Pair<Position, Position> pair = new Pair<Position, Position>(matrix[i, j], matrix[i + 1, j]);
                                archi.Add(pair);
                                /*    graphics.DrawLine(pen10, (float) matrix[i, j].x , (float) matrix[i, j].y , 
                                      (float)matrix[i + 1, j].x ,
                                      (float)matrix[i + 1, j].y );*/
                            }
                        }
                        if (i < 33 && j + 1 < 42)
                        {
                            if (matrix[i, j + 1] != null)
                            {
                                Pair<Position, Position> pair = new Pair<Position, Position>(matrix[i, j], matrix[i, j + 1]);
                                archi.Add(pair);
                                /*    graphics.DrawLine(pen10, (float) matrix[i, j ].x , (float) matrix[i, j ].y , 
                                      (float) matrix[i, j + 1].x ,
                                      (float)matrix[i, j + 1].y );*/
                            }
              
                        }
                        if (i + 1 < 33 && j + 1 < 42)
                        {
                            if (matrix[i + 1, j + 1] != null)
                            {
                                Pair<Position, Position> pair = new Pair<Position, Position>(matrix[i, j], matrix[i + 1, j + 1]);
                                archi.Add(pair);
                                /*  graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                    (float) matrix[i + 1, j + 1].x,
                                    (float) matrix[i + 1, j + 1].y);*/
                            }
                        }
                        if (i - 1 >= 0 && j < 42)
                        {
                            if (matrix[i - 1, j] != null)
                            {
                                Pair<Position, Position> pair = new Pair<Position, Position>(matrix[i, j], matrix[i - 1, j ]);
                                archi.Add(pair);
                                /*  graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                    (float) matrix[i - 1, j].x,
                                    (float) matrix[i - 1, j].y);*/
                            }
                        }
                        if (j - 1 >= 0 && i < 33)

                        {
                            if (matrix[i, j - 1] != null)
                            {
                                Pair<Position, Position> pair = new Pair<Position, Position>(matrix[i, j], matrix[i, j - 1]);
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
                                Pair<Position, Position> pair = new Pair<Position, Position>(matrix[i, j],matrix[i - 1, j - 1]);
                                archi.Add(pair);
                                /*  graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                    (float) matrix[i - 1, j - 1].x,
                                    (float) matrix[i - 1, j - 1].y);*/
                            }
            
                        }
                        if (j + 1 < 42 && i - 1 >= 0)

                        {
                            if (matrix[i - 1, j + 1] != null)
                            {
                                Pair<Position, Position> pair = new Pair<Position, Position>(matrix[i, j],matrix[i - 1, j + 1]);
                                archi.Add(pair);
                                /*graphics.DrawLine(pen10, (float) matrix[i, j].x, (float) matrix[i, j].y,
                                  (float) matrix[i - 1, j + 1].x,
                                  (float) matrix[i - 1, j + 1].y);*/
                            }
                        }
                        if (i + 1 < 33 && j - 1 >= 0)

                        {
                            if (matrix[i + 1, j - 1] != null)
                            {
                                Pair<Position, Position> pair = new Pair<Position, Position>(matrix[i, j],matrix[i + 1, j - 1]);
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

            for (int i=0;  i < nodi.Count; i++)
            {
                d1.Add(i,nodi[i]);
            }
            List<int> myKeys = d1.Keys.ToList();
            var nodes = myKeys;

            List<Pair<int,int>> listaArchi= new List<Pair<int, int>>();
            foreach (Pair<Position,Position> pair in archi)
            {
                int n1 = getNodi(pair.First, d1);
                int n2 = getNodi(pair.Second, d1);
                listaArchi.Add(new Pair<int, int>(n1,n2));
            }

     
            var edg= new Pair<int,int>[listaArchi.Count];

            for (int j = 0; j < listaArchi.Count; j++)
            {
                edg[j] = listaArchi[j];
            }

            var graph = new Graph<int>(nodes, edg);


            List<KeyValuePair<int, Position>> listaOggetti = new List<KeyValuePair<int, Position>>();
            foreach (Position p1 in pos)
            {
                Position p=new Position();
                p.x = p1.x/2f;
                p.y = p1.y/2f;

                String s=p1.name;
                switch (s)
                {
                  case "bathroom_sink":
                      listaOggetti.Add(new KeyValuePair<int, Position>(1,p));

                     break;
                      case "bed" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(0,p));

                          break;
                      case "computer_chair":
                          listaOggetti.Add(new KeyValuePair<int, Position>(2,p));

                          break;
                      case "computer":
                          listaOggetti.Add(new KeyValuePair<int, Position>(3,p));

                          break;
                      case "dining_chair" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(4,p));

                   
                          break;
                      case "dining_table" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(5,p));

                          break;
                      case "entrace" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(6,p));

                          break;
                      case "fridge" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(7,p));

                          break;
                      case "kitchen_sink" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(8,p));

                          break;
                      case "micro" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(9,p));

                          break;
                      case "outside" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(10,p));

                          break;
                      case "start" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(11,p));

                          break;
                      case "tv" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(12,p));

                          break;
                      case "tv_chair" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(13,p));

                      
                          break;
                      case "wardrobe" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(14,p));

                          break;
                      case "wc" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(15,p));

                          
                          break;
                      case "bathtub" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(16,p));

                          break;
                      case "oven" :
                       

                          listaOggetti.Add(new KeyValuePair<int, Position>(17,p));

                  
                          break;
                      case "kitchen_shelf" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(18,p));

                          break;
                      case "shoe_shelf" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(19,p));

                      
                          break;

                      case "exercise_place" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(20,p));

                          break;
                      case "chair" :
                          listaOggetti.Add(new KeyValuePair<int, Position>(21,p));

break;
                }
            }
         Position p22= new Position();
            p22.x = 200/2f;
            p22.y = 250/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(22,p22));

            Position p23= new Position();
            p23.x = 450/2f;
            p23.y = 250/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(23,p23));

            Position p24= new Position();
            p24.x = 200/2f;
            p24.y = 550/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(24,p24));

            Position p25= new Position();
            p25.x = 450/2f;


            p25.y = 550/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(25,p25));

            Position p26= new Position();
            p26.x = 450/2f;
            p26.y = 850/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(26,p26));

            Position p27= new Position();
            p27.x = 200/2f;
            p27.y = 850/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(27,p27));

            Position p28= new Position();
            p28.x = 450/2f;
            p28.y = 1100/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(28,p28));

            Position p29= new Position();
            p29.x = 750/2f;
            p29.y = 900/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(29,p29));

            Position p30= new Position();
            p30.x = 200/2f;
            p30.y = 1100/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(30,p30));

            Position p31= new Position();
            p31.x = 1150/2f;
            p31.y = 900/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(31,p31));

            Position p32= new Position();
            p32.x = 750/2f;
            p32.y = 1150/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(32,p32));

            Position p33= new Position();
            p33.x = 1150/2f;
            p33.y = 1150/2f;

            listaOggetti.Add(new KeyValuePair<int, Position>(33,p33));
            Position p34= new Position();
            p34.x = 1550/2f;
            p34.y = 550/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(34,p34));

            Position p35= new Position();
            p35.x = 1500/2f;
            p35.y = 650/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(35,p35));

            Position p36= new Position();
            p36.x = 1750/2f;
            p36.y = 650/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(36,p36));

            Position p37= new Position();
            p37.x = 1900/2f;
            p37.y = 350/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(37,p37));

            Position p38= new Position();
            p38.x = 1550/2f;
            p38.y = 350/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(38,p38));

            Position p39= new Position();
            p39.x = 1900/2f;
            p39.y = 850/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(39,p39));

            Position p40= new Position();
            p40.x = 1900/2f;
            p40.y = 1200/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(40,p40));

            Position p41= new Position();
            p41.x = 1500/2f;
            p41.y = 850/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(41,p41));

            Position p42= new Position();
            p42.x = 1500/2f;
                p42.y = 1200/2f;

            listaOggetti.Add(new KeyValuePair<int, Position>(42,p42));
            Position p43= new Position();
            p43.x = 750/2f;
            p43.y = 350/2f;
            
            listaOggetti.Add(new KeyValuePair<int, Position>(43,p43));

            Position p44= new Position();
            p44.x = 1150/2f;
            p44.y = 350/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(44,p44));

            Position p45= new Position();

            p45.x = 750/2f;
            p45.y = 600/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(45,p45));

            Position p46= new Position();

            p46.x = 1150/2f;
            p46.y = 600/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(46,p46));

            Position p47= new Position();

            p47.x = 300/2f;
            p47.y = 600/2f;
            listaOggetti.Add(new KeyValuePair<int, Position>(47,p47));
         
  
            TextWriter log = (TextWriter) new StreamWriter(pathdir+"\\PathLog"+u+".txt");
            var now = DateTime.Now;
            DateTime time = new DateTime(now.Year, now.Month, now.Day,0,0,0);
            Position source,dest = null;
           
            

            for (int i = 0; i < 86400; i++)
            {
                int s1=0;
                Random random = new Random();
                int idOggetto = random.Next(0, 48);
                if (i == 0)
                {
                    source = listaOggetti[idOggetto].Value;
                    s1 = getNodi(source, d1);
                }
                else
                {

                    source = dest;

                    s1 = getNodi(source, d1);

                }

                Random random2 = new Random();
                int idOggetto2 = random2.Next(0, 48);
                 dest = listaOggetti[47-idOggetto2].Value;
             

                int dg = getNodi(dest, d1);
                var startVertex = s1;
                var shortestPath = ShortestPathFunction(graph, startVertex);
                var d = dg;

                List<int> path= shortestPath(d).ToList();
                
                if (i == 0)
                {
                    
                    for (int g = 0; g < path.Count; g++)
                    {Position posizione = getPosizione(path[g], d1);
                        if (i < 86400)
                        {

                            log.WriteLine(time.ToString("HH:mm:ss")
                                          + " " + posizione.x + " " + posizione.y);
                            time = time.AddSeconds(1);

                           
                        }
                    }
                    if (i + path.Count-1 < 86400)
                    i = i + path.Count-1;
                    else
                    {
                      
                        return;
                    }


                }
                else
                {

                    for (int g = 1; g < path.Count; g++)
                    {

                        if (i < 86400)
                        {

                            Position posizione = getPosizione(path[g], d1);

                            log.WriteLine(time.ToString("HH:mm:ss")
                                          + " " + posizione.x + " " + posizione.y);
                            time = time.AddSeconds(1);


                        }
                    }

                    if (i + path.Count - 2 < 86400)
                    {
                        i = i + path.Count - 2;
                    }
                 else
                    {
                        return;
                    }
                }


            }
          
            log.Close();
          
             
                
  
            
        

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
            foreach (KeyValuePair<int, Position> keyValuePair in dictionary)
            {
             
                    
                    if (keyValuePair.Value.x==p.x && keyValuePair.Value.y==p.y)
                        return keyValuePair.Key;
                
            }
            

            return -1;
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