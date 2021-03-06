﻿// Decompiled with JetBrains decompiler
// Type: HomeDesigner.Form1
// Assembly: HomeDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3850526B-3877-4CBB-8C11-EAD333A0DB1D
// Assembly location: C:\Users\Dario\Desktop\HomeSensorSimulator\HomeSensorSimulator\HomeDesigner.exe

using SensorSim.Data;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;
using System.Text;

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Devart.Data.SQLite;
using Devart.Data;

namespace HomeDesigner
{
  public class Form1 : Form{

    private IContainer components = (IContainer) null;
    private PictureBox pictureBox1;
    private Label label1;
    private TextBox posTextBox;
    private Label label2;
    private Label label3;
    private TextBox wallTextBox;
    private Button open;
    private Button save;
    private Button disegna;
    private Button disegnaStop;
    private Button predizioneStop;
    private Label labelSim;
    private Label labelPred;
    private CheckBox adjustCheckBox;

    private Button pausePredizioneButton;
    private Button pauseSimulazioneButton;

    private Button simula;
    private Button zoom;
    private Button zoomMeno;
    private Button disegnaPredizione;
    private Button doEdits;
    private CheckBox sensorCheckBox;
    private TextBox orientationtb;
    private TextBox sensortypetb;
    private Label label4;
    private Label label5;
    private GroupBox groupBox1;
    private Label label6;
    private TextBox sensornametb;
    private Label label7;
    private TextBox sensortb;
    private Button loadSensorbtn;
    private GroupBox groupBox2;
    private CheckBox wallpcb;
    private CheckBox sensorpcb;
    private CheckBox placepcb;
    private Panel panel1;
    private ComboBox loadcb;
    private ComboBox savecb;
    private ToolTip toolTip1;
    private Label label8;
    private Button button1;
    private static bool isLoaded;
    private Task taskSimulazione;
    private Task taskPredizione;
    public float zoomFactor;
    private Label label9;
    private Label simulationLabel;
    private Label designLabel;
    private CheckBox showSensors;

    private Label label10;
    private ComboBox posNamecb;
    private Label simulationBox;
    public static Bitmap bitmapHouse;
    private HomeDesigner.Data data;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
    
      this.WindowState = FormWindowState.Maximized;
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
      this.label1 = new Label();
      this.posTextBox = new TextBox();
      this.label2 = new Label();
      this.label3 = new Label();
      this.wallTextBox = new TextBox();
      this.open = new Button();
      this.save = new Button();
      this.disegna = new Button();
      this.disegnaPredizione = new Button();
      this.disegnaStop = new Button();
      this.predizioneStop = new Button();
      this.pausePredizioneButton= new Button();
      this.pauseSimulazioneButton= new Button();
      this.simula = new Button();
      this.zoom = new Button();
      this.zoomMeno = new Button();
      this.showSensors = new CheckBox();

      this.labelSim=new Label();
      this.labelPred=new Label();

      this.doEdits = new Button();
      this.pictureBox1 = new PictureBox();
      this.adjustCheckBox = new CheckBox();
      this.sensorCheckBox = new CheckBox();
      this.orientationtb = new TextBox();
      this.sensortypetb = new TextBox();
      this.label4 = new Label();
      this.label5 = new Label();
      this.groupBox1 = new GroupBox();
      this.loadSensorbtn = new Button();
      this.label6 = new Label();
      this.sensornametb = new TextBox();
      this.label7 = new Label();
      this.sensortb = new TextBox();
      this.groupBox2 = new GroupBox();
      this.wallpcb = new CheckBox();
      this.sensorpcb = new CheckBox();
      this.placepcb = new CheckBox();
      this.panel1 = new Panel();
      this.loadcb = new ComboBox();
      this.savecb = new ComboBox();
      this.toolTip1 = new ToolTip(this.components);
      this.label8 = new Label();
      this.button1 = new Button();
      this.label9 = new Label();
      this.simulationLabel = new Label();
      this.designLabel = new Label();

      this.label10 = new Label();
      this.posNamecb = new ComboBox();
      this.simulationBox = new Label();

      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.label1.AutoSize = true;
      this.label1.Location = new Point(455, 43);
      this.label1.Name = "label1";
      this.label1.Size = new Size(72, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "position name";
      this.posTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
      this.posTextBox.Location = new Point(457, 242);
      this.posTextBox.Multiline = true;
      this.posTextBox.Name = "posTextBox";
      this.posTextBox.ScrollBars = ScrollBars.Vertical;
      this.posTextBox.Size = new Size(113, 327);
      this.posTextBox.TabIndex = 3;
      this.toolTip1.SetToolTip((Control) this.posTextBox, "Format per line:\r\n[position_name] [X_coordinate] [Y_coordinate]");
      this.label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.label2.AutoSize = true;
      this.label2.Location = new Point(456, 214);
      this.label2.Name = "label2";
      this.label2.Size = new Size(109, 26);
      this.label2.TabIndex = 4;
      this.label2.Text = "positions\r\n(edit in textbox below)";
      this.label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.label3.AutoSize = true;
      this.label3.Location = new Point(596, 102);
      this.label3.Name = "label3";
      this.label3.Size = new Size(121, 26);
      this.label3.TabIndex = 7;
      this.label3.Text = "walls\r\n(define in textbox below)";
      this.wallTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
      this.wallTextBox.Location = new Point(599, 132);
      this.wallTextBox.Multiline = true;
      this.wallTextBox.Name = "wallTextBox";
      this.wallTextBox.ScrollBars = ScrollBars.Vertical;
      this.wallTextBox.Size = new Size(113, 437);
      this.wallTextBox.TabIndex = 6;
      this.toolTip1.SetToolTip((Control) this.wallTextBox, "Format per line:\r\n[start_position_name] [end_position_name]");
      this.open.Location = new Point(142, 37);
      this.open.Name = "open";
      this.open.Size = new Size(70, 31);
      this.open.TabIndex = 9;
      this.open.Text = "Load";
      this.open.UseVisualStyleBackColor = true;
      this.open.Click += new EventHandler(this.button2_Click);
      this.save.Location = new Point(350, 37);
      this.save.Name = "save";
      this.save.Size = new Size(70, 31);
      this.save.TabIndex = 5;
      this.save.Text = "Save";
      this.save.UseVisualStyleBackColor = true;
      this.save.Click += new EventHandler(this.button1_Click);
      
      this.showSensors.Location = new Point(1170, 15);
      this.showSensors.Name = "showsensors";
      this.showSensors.Size = new Size(100, 50);
      this.showSensors.TabIndex = 5;
      this.showSensors.Text = "show sensors";
      this.showSensors.UseVisualStyleBackColor = true;
      this.showSensors.CheckedChanged += new EventHandler(this.checkBox1_ShowSensors);      
      this.disegna.Location = new Point(540, 38);
      this.disegna.Name = "disegna";
      this.disegna.Size = new Size(48, 48);
      this.disegna.TabIndex = 5;
      this.disegna.UseVisualStyleBackColor = true;
      this.disegna.Image = System.Drawing.Image.FromFile("Resources/play_image.PNG");
      GraphicsPath graphicsPathDisegna = new GraphicsPath();
      graphicsPathDisegna.AddEllipse(1, 1, disegna.Width - 4, disegna.Height - 4);
      disegna.Region = new Region(graphicsPathDisegna);
      this.disegna.Click += new EventHandler(this.buttonDisegna_Click);
      this.disegnaStop.Location = new Point(590, 40);
      this.disegna.Name = "disegnaStop";
      this.disegnaStop.Size = new Size(47, 47);
      this.disegnaStop.TabIndex = 5;
      this.disegnaStop.UseVisualStyleBackColor = true;
      this.disegnaStop.Image = System.Drawing.Image.FromFile("Resources/stop_image.PNG");
      GraphicsPath gp = new GraphicsPath();
      gp.AddEllipse(1, 1, disegnaStop.Width - 4, disegnaStop.Height - 4);
      disegnaStop.Region = new Region(gp);
      this.disegnaStop.Click += new EventHandler(this.buttonStopSimulazione);
      
      this.pauseSimulazioneButton.Location = new Point(640, 40);
      this.pauseSimulazioneButton.Name = "pauseSimulazione";
      this.pauseSimulazioneButton.Size = new Size(47, 47);
      this.pauseSimulazioneButton.TabIndex = 5;
      this.pauseSimulazioneButton.UseVisualStyleBackColor = true;
      this.pauseSimulazioneButton.Image = System.Drawing.Image.FromFile("Resources/pause_image.PNG");
      GraphicsPath gpause = new GraphicsPath();
      gpause.AddEllipse(1, 1, pauseSimulazioneButton.Width - 4, pauseSimulazioneButton.Height - 4);
      pauseSimulazioneButton.Region = new Region(gpause);
      this.pauseSimulazioneButton.Click += new EventHandler(this.buttonPauseSimulazione);
      
      this.pausePredizioneButton.Location = new Point(788, 38);
      this.pausePredizioneButton.Name = "pausePredizioneButton";
      this.pausePredizioneButton.Size = new Size(47, 47);
      this.pausePredizioneButton.TabIndex = 5;
      this.pausePredizioneButton.UseVisualStyleBackColor = true;
      this.pausePredizioneButton.Image = System.Drawing.Image.FromFile("Resources/pause_image.PNG");
      GraphicsPath gpausePred = new GraphicsPath();
      gpausePred.AddEllipse(1, 1, pausePredizioneButton.Width - 4, pausePredizioneButton.Height - 4);
      pausePredizioneButton.Region = new Region(gpausePred);
      this.pausePredizioneButton.Click += new EventHandler(this.buttonPausePredizione);
      
      this.predizioneStop.Location = new Point(738, 40);
      this.predizioneStop.Name = "predizioneStop";
      this.predizioneStop.Size = new Size(47, 47);
      this.predizioneStop.TabIndex = 5;
      this.predizioneStop.UseVisualStyleBackColor = true;
      this.predizioneStop.Image = System.Drawing.Image.FromFile("Resources/stop_image.PNG");
      GraphicsPath gp2 = new GraphicsPath();
      gp2.AddEllipse(1, 1, predizioneStop.Width - 4, predizioneStop.Height - 4);
      predizioneStop.Region = new Region(gp2);
      this.predizioneStop.Click += new EventHandler(this.buttonStopPredizione);
      this.disegnaPredizione.Location = new Point(690, 40);
      this.disegnaPredizione.Name = "disegnaPredizione";
      this.disegnaPredizione.Size = new Size(48, 48);
      this.disegnaPredizione.TabIndex = 5;
      this.disegnaPredizione.Image = System.Drawing.Image.FromFile("Resources/play_image.PNG");
      this.disegnaPredizione.UseVisualStyleBackColor = true;
      GraphicsPath graphicsPathDisegnaPredizione = new GraphicsPath();
      graphicsPathDisegnaPredizione.AddEllipse(1, 1, disegnaPredizione.Width - 4, disegnaPredizione.Height - 4);
      disegnaPredizione.Region = new Region(graphicsPathDisegnaPredizione);
      this.disegnaPredizione.Click += new EventHandler(this.buttonDisegnaPredizione_Click);
      
      this.simula.Location = new Point(450, 45);
      this.simula.Name = "simula";
      this.simula.Size = new Size(70, 31);
      this.simula.TabIndex = 5;
      this.simula.Text = "Simulate";
      this.simula.UseVisualStyleBackColor = true;
      this.simula.Click += new EventHandler(this.buttonSimula_Click);
      this.zoom.Location = new Point(1000, 10);
      this.zoom.Name = "zoomButton";
      this.zoom.Size = new Size(50, 50);
      this.zoom.TabIndex = 5;
      this.zoom.Image = System.Drawing.Image.FromFile("Resources/zoompiu.PNG");
      GraphicsPath p = new GraphicsPath();
      p.AddEllipse(1, 1, zoom.Width - 4, zoom.Height - 4);
      zoom.Region = new Region(p);
      this.zoom.UseVisualStyleBackColor = true;
      this.zoom.Click += new EventHandler(this.zoomButton_Click);
      this.zoomMeno.Location = new Point(1080, 10);
      this.zoomMeno.Name = "zoomButtonMeno";
     
      this.zoomMeno.Size = new Size(50, 50);
      this.zoomMeno.TabIndex = 5;
      this.zoomMeno.UseVisualStyleBackColor = true;
      this.zoomMeno.Image = System.Drawing.Image.FromFile("Resources/zoomout.PNG");
      GraphicsPath p1 = new GraphicsPath();
      p1.AddEllipse(1, 1, zoomMeno.Width - 4, zoomMeno.Height - 4);
      zoomMeno.Region = new Region(p1);
      this.zoomMeno.Click += new EventHandler(this.zoomMenoButton_Click);
      this.doEdits.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.doEdits.Location = new Point(588, 588);
      this.doEdits.Name = "doEdits";
      this.doEdits.Size = new Size(137, 42);
      this.doEdits.TabIndex = 12;
      this.doEdits.Text = "Apply textbox edits";
      this.doEdits.UseVisualStyleBackColor = true;
      this.doEdits.Click += new EventHandler(this.doEdits_Click);
      this.pictureBox1.Image = (Image) componentResourceManager.GetObject("pictureBox1.Image");
      this.pictureBox1.InitialImage = (Image) componentResourceManager.GetObject("pictureBox1.InitialImage");
      this.pictureBox1.Location = new Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(1038, 811);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.MouseClick += new MouseEventHandler(this.pictureBox1_MouseClick);
      this.adjustCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.adjustCheckBox.AutoSize = true;
      this.adjustCheckBox.Checked = true;
      this.adjustCheckBox.CheckState = CheckState.Checked;
      this.adjustCheckBox.Location = new Point(599, 64);
      this.adjustCheckBox.Name = "adjustCheckBox";
      this.adjustCheckBox.Size = new Size(108, 17);
      this.adjustCheckBox.TabIndex = 13;
      this.adjustCheckBox.Text = "adjust new points";
      this.adjustCheckBox.UseVisualStyleBackColor = true;
      
      this.sensorCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.sensorCheckBox.AutoSize = true;
      this.sensorCheckBox.Location = new Point(732, 64);
      this.sensorCheckBox.Name = "sensorCheckBox";
      this.sensorCheckBox.Size = new Size(78, 17);
      this.sensorCheckBox.TabIndex = 14;
      this.sensorCheckBox.Text = "add sensor";
      this.sensorCheckBox.UseVisualStyleBackColor = true;
      this.sensorCheckBox.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged);
      this.orientationtb.Location = new Point(9, 90);
      this.orientationtb.Name = "orientationtb";
      this.orientationtb.Size = new Size(113, 20);
      this.orientationtb.TabIndex = 15;
      this.orientationtb.Text = "0";
      this.sensortypetb.Location = new Point(9, 136);
      this.sensortypetb.Name = "sensortypetb";
      this.sensortypetb.Size = new Size(113, 20);
      this.sensortypetb.TabIndex = 16;
      this.sensortypetb.Text = "type1";
      this.label4.AutoSize = true;
      this.label4.Location = new Point(6, 74);
      this.label4.Name = "label4";
      this.label4.Size = new Size(92, 13);
      this.label4.TabIndex = 17;
      this.label4.Text = "orientation (0-360)";
      this.label4.Click += new EventHandler(this.label4_Click);
      this.label5.AutoSize = true;
      this.label5.Location = new Point(6, 120);
      this.label5.Name = "label5";
      this.label5.Size = new Size(61, 13);
      this.label5.TabIndex = 18;
      this.label5.Text = "sensor type";
      this.groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.groupBox1.Controls.Add((Control) this.loadSensorbtn);
      this.groupBox1.Controls.Add((Control) this.label6);
      this.groupBox1.Controls.Add((Control) this.sensornametb);
      this.groupBox1.Controls.Add((Control) this.label4);
      this.groupBox1.Controls.Add((Control) this.label5);
      this.groupBox1.Controls.Add((Control) this.orientationtb);
      this.groupBox1.Controls.Add((Control) this.sensortypetb);
      this.groupBox1.Enabled = false;
      this.groupBox1.Location = new Point(731, 102);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(135, 165);
      this.groupBox1.TabIndex = 19;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "sensor properties";
      this.loadSensorbtn.Location = new Point(72, 14);
      this.loadSensorbtn.Name = "loadSensorbtn";
      this.loadSensorbtn.Size = new Size(50, 20);
      this.loadSensorbtn.TabIndex = 21;
      this.loadSensorbtn.Text = "Load it";
      this.loadSensorbtn.UseVisualStyleBackColor = true;
      this.loadSensorbtn.Click += new EventHandler(this.loadSensorbtn_Click);
      this.label6.AutoSize = true;
      this.label6.Location = new Point(6, 19);
      this.label6.Name = "label6";
      this.label6.Size = new Size(91, 26);
      this.label6.TabIndex = 20;
      this.label6.Text = "sensor name\r\n(autogen if empty)";
      this.sensornametb.Location = new Point(9, 47);
      this.sensornametb.Name = "sensornametb";
      this.sensornametb.Size = new Size(113, 20);
      this.sensornametb.TabIndex = 19;
      this.label7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.label7.AutoSize = true;
      this.label7.Location = new Point(738, 278);
      this.label7.Name = "label7";
      this.label7.Size = new Size(109, 26);
      this.label7.TabIndex = 20;
      this.label7.Text = "sensors\r\n(edit in textbox below)";
      this.sensortb.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
      this.sensortb.Location = new Point(740, 307);
      this.sensortb.Multiline = true;
      this.sensortb.Name = "sensortb";
      this.sensortb.ScrollBars = ScrollBars.Vertical;
      this.sensortb.Size = new Size(113, 262);
      this.sensortb.TabIndex = 21;
      this.toolTip1.SetToolTip((Control) this.sensortb, "Format per line:\r\n[sensor_name] [X_coordinate] [Y_coordinate] [orientation] [sensor_type]");
      this.groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.groupBox2.Controls.Add((Control) this.wallpcb);
      this.groupBox2.Controls.Add((Control) this.sensorpcb);
      this.groupBox2.Controls.Add((Control) this.placepcb);
      this.groupBox2.Location = new Point(457, 97);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new Size(113, 103);
      this.groupBox2.TabIndex = 22;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "shown points";
      this.wallpcb.AutoSize = true;
      this.wallpcb.Checked = true;
      this.wallpcb.CheckState = CheckState.Checked;
      this.wallpcb.Location = new Point(16, 73);
      this.wallpcb.Name = "wallpcb";
      this.wallpcb.Size = new Size(70, 17);
      this.wallpcb.TabIndex = 2;
      this.wallpcb.Text = "wall ends";
      this.wallpcb.UseVisualStyleBackColor = true;
      this.wallpcb.CheckedChanged += new EventHandler(this.placepcb_CheckedChanged);
      this.sensorpcb.AutoSize = true;
      this.sensorpcb.Checked = true;
      this.sensorpcb.CheckState = CheckState.Checked;
      this.sensorpcb.Location = new Point(16, 49);
      this.sensorpcb.Name = "sensorpcb";
      this.sensorpcb.Size = new Size(62, 17);
      this.sensorpcb.TabIndex = 1;
      this.sensorpcb.Text = "sensors";
      this.sensorpcb.UseVisualStyleBackColor = true;
      this.sensorpcb.CheckedChanged += new EventHandler(this.placepcb_CheckedChanged);
      this.placepcb.AutoSize = true;
      this.placepcb.Checked = true;
      this.placepcb.CheckState = CheckState.Checked;
      this.placepcb.Location = new Point(16, 25);
      this.placepcb.Name = "placepcb";
      this.placepcb.Size = new Size(57, 17);
      this.placepcb.TabIndex = 0;
      this.placepcb.Text = "places";
      this.placepcb.UseVisualStyleBackColor = true;
      this.placepcb.CheckedChanged += new EventHandler(this.placepcb_CheckedChanged);
      this.panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.panel1.AutoScroll = true;
      this.panel1.Controls.Add((Control) this.pictureBox1);
      this.panel1.Location = new Point(15, 86);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(412, 546);
      this.panel1.TabIndex = 23;
      panel1.BackColor=Color.MidnightBlue;
      this.loadcb.DropDownStyle = ComboBoxStyle.DropDownList;
      this.loadcb.FormattingEnabled = true;
      this.loadcb.Location = new Point(15, 42);
      this.loadcb.Name = "loadcb";
      this.loadcb.Size = new Size(121, 21);
      this.loadcb.TabIndex = 24;
      this.savecb.FormattingEnabled = true;
      this.savecb.Location = new Point(225, 41);
      this.savecb.Name = "savecb";
      this.savecb.Size = new Size(121, 21);
      this.savecb.TabIndex = 25;
      this.savecb.SelectedIndexChanged += new EventHandler(this.comboBox1_SelectedIndexChanged);
      this.toolTip1.AutoPopDelay = 32000;
      this.toolTip1.InitialDelay = 500;
      this.toolTip1.ReshowDelay = 100;
      this.label8.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.label8.AutoSize = true;
      this.label8.Location = new Point(756, 634);
      this.label8.Name = "label8";
      this.label8.Size = new Size(137, 13);
      this.label8.TabIndex = 26;
      this.label8.Text = "use mouseover info for help";
      this.button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.button1.Location = new Point(805, 604);
      this.button1.Name = "button1";
      this.button1.Size = new Size(81, 25);
      this.button1.TabIndex = 27;
      this.button1.Text = "About";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click_1);
      this.label9.AutoSize = true;
      this.label9.Location = new Point(12, 23);
      this.label9.Name = "label9";
      this.label9.Size = new Size(139, 13);
      this.label9.TabIndex = 28;
      this.label9.Text = "Choose home name for load";
      this.simulationLabel.AutoSize = true;
      this.simulationLabel.Location = new Point(450, 1);
      this.simulationLabel.Name = "simulationLabel";
      this.simulationLabel.Size = new Size(139, 13);
      this.simulationLabel.TabIndex = 28;
      this.simulationLabel.Text = "Simulation";
      this.simulationLabel.Font = new Font("Microsoft JhengHei", 10,FontStyle.Regular);
      this.designLabel.AutoSize = true;
      this.designLabel.Location = new Point(600, 1);
      this.designLabel.Name = "simulationLabel";
      this.designLabel.Size = new Size(139, 13);
      this.designLabel.TabIndex = 28;
      this.designLabel.Text = "Designer";
      this.designLabel.Font = new Font("Microsoft JhengHei", 12,FontStyle.Regular);
      this.simulationBox.AutoSize = true;
      this.simulationBox.Location = new Point(680, 4);
      this.simulationBox.Name = "simulationLabel";
      this.simulationBox.Size = new Size(139, 13);
      this.simulationBox.TabIndex = 28;
      this.simulationBox.Text = "";
      this.simulationBox.Font = new Font("Microsoft JhengHei", 9,FontStyle.Regular);
      this.labelSim.AutoSize = true;
      this.labelSim.Location = new Point(600, 22);
      this.labelSim.Name = "simLabel";
      this.labelSim.Size = new Size(139, 13);
      this.labelSim.TabIndex = 28;
      this.labelSim.Text = "Simulation";
      this.labelSim.Font = new Font("Microsoft JhengHei", 9,FontStyle.Regular);
      this.labelPred.AutoSize = true;
      this.labelPred.Location = new Point(690, 22);
      this.labelPred.Name = "predLabel";
      this.labelPred.Size = new Size(139, 13);
      this.labelPred.TabIndex = 28;
      this.labelPred.Text = "Prediction";
      this.labelPred.Font = new Font("Microsoft JhengHei", 9,FontStyle.Regular);
      this.label10.AutoSize = true;
      this.label10.Location = new Point(222, 23);
      this.label10.Name = "label10";
      this.label10.Size = new Size(142, 13);
      this.label10.TabIndex = 29;
      this.label10.Text = "Choose home name for save";
      this.posNamecb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.posNamecb.FormattingEnabled = true;
      this.posNamecb.Location = new Point(457, 61);
      this.posNamecb.Name = "posNamecb";
      this.posNamecb.Size = new Size(121, 21);
      this.posNamecb.TabIndex = 30;
      this.posNamecb.SelectedIndexChanged += new EventHandler(this.cb1_SelectedIndexChanged);
      
  
     
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoScroll = true;
      this.ClientSize = new Size(891, 651);
      this.Controls.Add((Control) this.posNamecb);
      this.Controls.Add((Control) this.simulationBox);

      this.Controls.Add((Control) this.label10);
      this.Controls.Add((Control) this.label9);
      this.Controls.Add((Control) this.simulationLabel);
      this.Controls.Add((Control) this.designLabel);

      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.label8);
      this.Controls.Add((Control) this.savecb);
      this.Controls.Add((Control) this.loadcb);
      this.Controls.Add((Control) this.panel1);
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.sensortb);
      this.Controls.Add((Control) this.label7);
      this.Controls.Add((Control) this.zoom);
      this.Controls.Add((Control) this.zoomMeno);
      this.Controls.Add((Control) this.pauseSimulazioneButton);
      this.Controls.Add((Control) this.pausePredizioneButton);

      this.Controls.Add((Control) this.groupBox1);
      this.Controls.Add((Control) this.sensorCheckBox);
      this.Controls.Add((Control) this.adjustCheckBox);
      this.Controls.Add((Control) this.doEdits);
      this.Controls.Add((Control) this.open);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.wallTextBox);
      this.Controls.Add((Control) this.save);
      this.Controls.Add((Control) this.disegna);
      this.Controls.Add((Control) this.disegnaPredizione);
      this.Controls.Add((Control) this.disegnaStop);
      this.Controls.Add((Control) this.predizioneStop);
      this.Controls.Add((Control) this.labelSim);

      this.Controls.Add((Control) this.labelPred);
      this.Controls.Add((Control) this.showSensors);

      this.Controls.Add((Control) this.simula);

      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.posTextBox);
      this.Controls.Add((Control) this.label1);
      this.HelpButton = true;
      this.Name = nameof (Form1);
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Home Sensor Simulator - Home Designer";
      this.Load += new EventHandler(this.Form1_Load);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

      this.BackColor=Color.AliceBlue;
    }

    public Form1()
    {
      this.InitializeComponent();
      this.zoomFactor = 1;
      this.data = new HomeDesigner.Data();
      this.reloadcbs();
      string[] strArray = File.ReadAllText("Data/House_models/obligatory_places.txt").Split(new char[4]
      {
        ' ',
        '\n',
        '\t',
        '\r'
      }, StringSplitOptions.RemoveEmptyEntries);
      this.posNamecb.Items.Add((object) "[wall_position]");
      foreach (object obj in strArray)
        this.posNamecb.Items.Add(obj);
      this.posNamecb.SelectedItem = (object) "[wall_position]";
      //carica le simulazioni
      string root = @"C:\Users\Dario\Desktop\HomeDesigner\bin\Debug\Log";

      // Get all subdirectories

      string[] subdirectoryEntries = Directory.GetDirectories(root);
     
     


   
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private void reloadcbs()
    {
      char[] separator = new char[2]{ '\\', '/' };
      string[] directories = Directory.GetDirectories("Data\\House_models\\", "*", SearchOption.TopDirectoryOnly);
      this.loadcb.Items.Clear();
      foreach (string str in directories)
      {
        if (!str.Split(separator, StringSplitOptions.RemoveEmptyEntries)[2].Equals("_system_tmp"))
          this.loadcb.Items.Add((object) str.Split(separator, StringSplitOptions.RemoveEmptyEntries)[2]);
      }
      this.loadcb.SelectedIndex = 0;
      this.savecb.Items.Clear();
      foreach (string str in directories)
      {
        if (!str.Split(separator, StringSplitOptions.RemoveEmptyEntries)[2].Equals("_system_tmp"))
          this.savecb.Items.Add((object) str.Split(separator, StringSplitOptions.RemoveEmptyEntries)[2]);
      }
      this.savecb.SelectedIndex = -1;
    }

    public static bool stopSimulazione;
    public static bool pauseSimulazione;
    public static bool restartSimulazione;
    public static bool pausePredizione;
    public static bool restartPredizione;
    private   void buttonStopSimulazione(object sender, EventArgs e)
    {
      
      stopSimulazione = true;



    }
    private   void buttonPausePredizione(object sender, EventArgs e)
    {
      if (!pausePredizione)
      {

        pausePredizione = true;
        restartPredizione = false;
      }
      else
      {

        pausePredizione = false;
        restartPredizione = true;
      }


    }
    private   void buttonPauseSimulazione(object sender, EventArgs e)
    {
      if (!pauseSimulazione)
      {

        pauseSimulazione = true;
        restartSimulazione = false;
      }
      else
      {

        pauseSimulazione = false;
        restartSimulazione = true;
      }


    }
    public static bool stopPredizione;
    private   void buttonStopPredizione(object sender, EventArgs e)
    {
      
      stopPredizione = true;



    }
    private void button2_Click(object sender, EventArgs e)
    {
      isLoaded = true;
      this.data.loadHouse(this.loadcb.Text);
      this.savecb.SelectedIndex = this.loadcb.SelectedIndex;
      this.refresh();
    }

    private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
    {
      double cx = (double) e.X;
      double cy = (double) e.Y;
      if (this.adjustCheckBox.Checked)
      {
        cx = Math.Round(cx / 25.0) * 25.0;
        cy = Math.Round(cy / 25.0) * 25.0;
      }
      if (this.sensorCheckBox.Checked)
        this.putSensor(cx, cy);
      else
        this.putPosition(cx, cy);
      this.refresh();
    }

    private void putSensor(double cx, double cy)
    {
      int num1 = 2;
      bool flag1 = false;
      string str;
      if (this.sensornametb.Text.Equals(""))
      {
        int num2 = 0;
        bool flag2 = true;
        while (flag2)
        {
          ++num2;
          flag2 = false;
          foreach (Position position in this.data.sensor)
          {
            if (position.name.Equals("s" + num2.ToString()))
              flag2 = true;
          }
        }
        str = "s" + num2.ToString();
      }
      else
        str = this.sensornametb.Text;
      foreach (Sensor sensor in this.data.sensor)
      {
        if (sensor.name.Equals(str))
        {
          try
          {
            sensor.x = cx * (double) num1;
            sensor.y = cy * (double) num1;
            sensor.orinentation = double.Parse(this.orientationtb.Text);
            sensor.type = this.sensortypetb.Text.ToString();
          }
          catch (Exception ex)
          {
            int num2 = (int) MessageBox.Show("syntax error in sensor data");
          }
          flag1 = true;
        }
      }
      if (flag1)
        return;
      try
      {
        Sensor sensor = new Sensor();
        sensor.name = str;
        sensor.x = cx * (double) num1;
        sensor.y = cy * (double) num1;
        sensor.orinentation = double.Parse(this.orientationtb.Text);
        sensor.type = this.sensortypetb.Text.ToString();
        this.data.sensor.Add(sensor);
      }
      catch (Exception ex)
      {
        int num2 = (int) MessageBox.Show("syntax error in sensor data");
      }
    }

    private void putPosition(double cx, double cy)
    {
      int num1 = 2;
      bool flag1 = false;
      if (this.posNamecb.Text.Equals(""))
      {
        int num2 = (int) MessageBox.Show("Choose name for position!");
      }
      else
      {
        string str;
        if (this.posNamecb.Text.Equals("[wall_position]"))
        {
          int num3 = 0;
          bool flag2 = true;
          while (flag2)
          {
            ++num3;
            flag2 = false;
            foreach (Position po in this.data.pos)
            {
              if (po.name.Equals("p" + num3.ToString()))
                flag2 = true;
            }
          }
          str = "p" + num3.ToString();
        }
        else
          str = this.posNamecb.Text;
        foreach (Position po in this.data.pos)
        {
          if (po.name.Equals(this.posNamecb.Text))
          {
            po.x = cx * (double) num1;
            po.y = cy * (double) num1;
            flag1 = true;
          }
        }
        if (flag1)
          return;
        this.data.pos.Add(new Position()
        {
          name = str,
          x = cx * (double) num1,
          y = cy * (double) num1
        });
      }
    }

    public void refreshSimulazione()
    {
      
      string folderPath = "";
       FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
       folderBrowserDialog1.SelectedPath = "C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log";
      String intervallo;
      if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
        FormInfo forminfo=new FormInfo();
        forminfo.ShowDialog();
         intervallo=forminfo.SelectedInterval;
         if (intervallo.Equals("[interval_of_time]"))
         {
           MessageBox.Show("Before the plotting of the simulation, choose an interval of time to draw!");
            return;
         }
         
         folderPath = folderBrowserDialog1.SelectedPath ;
         if (!folderPath.Contains("C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim"))
         {
           MessageBox.Show("You must choose a valid simulation path!");

           return;
         }
         simulationBox.Text = " ";

         simulationBox.Text+=" "+ folderPath.Substring(50);
         folderBrowserDialog1.Dispose();
         folderBrowserDialog1.Reset();
       }
      else
      {
        return;
      }
     String t0= intervallo.Split('-')[0];
      var now = DateTime.Now;
      int hours = Convert.ToInt32(t0.Split(':')[0]);
      int min = Convert.ToInt32(t0.Split(':')[1]);
      int sec = Convert.ToInt32(t0.Split(':')[2]);

  
      DateTime timeInizio= new DateTime(now.Year, now.Month, now.Day,hours,min,sec);
      String t1= intervallo.Split('-')[1];
       hours = Convert.ToInt32(t1.Split(':')[0]);
       min = Convert.ToInt32(t1.Split(':')[1]);
       sec = Convert.ToInt32(t1.Split(':')[2]);
      DateTime timeFine=      new DateTime(now.Year, now.Month, now.Day,hours,min,sec);

      this.posTextBox.Text = Printer.printPos(this.data.pos);
      this.wallTextBox.Text = Printer.printWall(this.data.wall);
      this.sensortb.Text = Printer.printSensor(this.data.sensor);
     
     
      stopSimulazione = false;
      pauseSimulazione = false;
      Task task = Task.Run( () => this.pictureBox1.Image = (Image)this.data.DrawSimulation(this, folderPath,
          this.pictureBox1, this.placepcb.Checked, false,
          this.wallpcb.Checked, timeInizio, timeFine, this.zoomFactor));
      if (task.IsCompleted)
      {
        task.Dispose();
        task = null;
      }
        
        
    


    }
   
    public void refreshPrediction( Dictionary<string, List<KeyValuePair<Position,Color>>> dictionary, DateTime timeInizio, DateTime timeFine )
    {
     
        this.posTextBox.Text = Printer.printPos(this.data.pos);
        this.wallTextBox.Text = Printer.printWall(this.data.wall);
        this.sensortb.Text = Printer.printSensor(this.data.sensor);
            string folderPath = "";

            stopPredizione = false;
         Task task = Task.Run( () => this.pictureBox1.Image = (Image) this.data.DrawSimulation(this, folderPath,
          this.pictureBox1, this.placepcb.Checked,false,
          this.wallpcb.Checked, timeInizio, timeFine, this.zoomFactor));
            if (task.IsCompleted)
        {
          dictionary.Clear();
          task.Dispose();
          task = null;
        }


      

    }

    public void refresh()
    {
      this.posTextBox.Text = Printer.printPos(this.data.pos);
      this.wallTextBox.Text = Printer.printWall(this.data.wall);
      this.sensortb.Text = Printer.printSensor(this.data.sensor);
      bitmapHouse=this.data.drawPicture(this.placepcb.Checked, this.sensorpcb.Checked, this.wallpcb.Checked);
      this.pictureBox1.Image =(Image) this.data.drawPicture(this.placepcb.Checked,false, this.wallpcb.Checked);
      
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (this.savecb.Text.Equals(""))
      {
        int num1 = (int) MessageBox.Show("Choose name for home!");
      }
      else
      {
        string str1 = "";
        try
        {
          char[] separator = new char[4]
          {
            ' ',
            '\n',
            '\t',
            '\r'
          };
          string[] strArray1 = File.ReadAllText("Data/House_models/obligatory_places.txt").Split(separator, StringSplitOptions.RemoveEmptyEntries);
          string[] strArray2 = this.posTextBox.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
          foreach (string str2 in strArray1)
          {
            bool flag = false;
            foreach (string str3 in strArray2)
            {
              if (str2.Equals(str3))
                flag = true;
            }
            if (!flag)
              str1 = str1 + str2 + ", ";
          }
        }
        catch (Exception ex)
        {
        }
        if (str1.Equals(""))
        {
          this.data.saveHouse(this.savecb.Text, this.posTextBox.Text, this.wallTextBox.Text, this.sensortb.Text);
          this.reloadcbs();
        }
        else
        {
          int num2 = (int) MessageBox.Show("Obligatory position(s) " + str1.Remove(str1.Length - 2) + " missing. Please add!", "Error");
        }
      }
    }

    private void doEdits_Click(object sender, EventArgs e)
    {
      this.data.saveHouse("_system_tmp", this.posTextBox.Text, this.wallTextBox.Text, this.sensortb.Text);
      if (this.data.loadHouse("_system_tmp"))
      {
        this.refresh();
      }
      else
      {
        int num = (int) MessageBox.Show("syntax error");
      }
    }

    private void label4_Click(object sender, EventArgs e)
    {
    }
    private void checkBox1_ShowSensors(object sender, EventArgs e)
    {
      if (this.showSensors.Checked)
        this.pictureBox1.Image = (Image) this.data.drawPicture(this.placepcb.Checked, true, this.wallpcb.Checked);
      else
        this.pictureBox1.Image = (Image) this.data.drawPicture(this.placepcb.Checked, false, this.wallpcb.Checked);
    }
    
    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      if (this.sensorCheckBox.Checked)
        this.groupBox1.Enabled = true;
      else
        this.groupBox1.Enabled = false;
    }

    private void loadSensorbtn_Click(object sender, EventArgs e)
    {
      foreach (Sensor sensor in this.data.sensor)
      {
        if (sensor.name.Equals(this.sensornametb.Text))
        {
          this.orientationtb.Text = sensor.orinentation.ToString();
          this.sensortypetb.Text = sensor.type;
          break;
        }
      }
    }

    private void placepcb_CheckedChanged(object sender, EventArgs e)
    {
      this.pictureBox1.Image = (Image) this.data.drawPicture(this.placepcb.Checked, this.sensorpcb.Checked, this.wallpcb.Checked);
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    private void buttonDisegnaPredizione_Click(object sender, EventArgs e)
    {
      if (!isLoaded)
      {
        MessageBox.Show("First of all you must load an house!");
        return;
      }
      string folderPath = "";
      FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
      folderBrowserDialog1.SelectedPath = "C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log";
      String intervallo;
      if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
        FormInfo forminfo=new FormInfo();
        forminfo.ShowDialog();
        intervallo=forminfo.SelectedInterval;
        if (intervallo.Equals("[interval_of_time]"))
        {
          MessageBox.Show("Before the plotting of the simulation, choose an interval of time to draw!");
          return;
        }
         
        folderPath = folderBrowserDialog1.SelectedPath ;
        if (!folderPath.Contains("C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim"))
        {
          MessageBox.Show("You must choose a valid simulation path!");

          return;
        }
        simulationBox.Text = " ";

        simulationBox.Text+=" "+ folderPath.Substring(50);
        folderBrowserDialog1.Dispose();
        folderBrowserDialog1.Reset();
      }
      else
      {
        return;
      }
      String t0= intervallo.Split('-')[0];
      var now = DateTime.Now;
      int hours = Convert.ToInt32(t0.Split(':')[0]);
      int min = Convert.ToInt32(t0.Split(':')[1]);
      int sec = Convert.ToInt32(t0.Split(':')[2]);

  
      DateTime timeInizio= new DateTime(now.Year, now.Month, now.Day,hours,min,sec);
      String t1= intervallo.Split('-')[1];
      hours = Convert.ToInt32(t1.Split(':')[0]);
      min = Convert.ToInt32(t1.Split(':')[1]);
      sec = Convert.ToInt32(t1.Split(':')[2]);
      DateTime timeFine=      new DateTime(now.Year, now.Month, now.Day,hours,min,sec);

      string[] fileArray = Directory.GetFiles(folderPath);
      String filepath = "";
      foreach (String filename in fileArray)
      {
        if (filename.Contains("trajectoriesDB"))
        {
          filepath = filename;
        }
      }

      if (filepath.Equals(""))
      {
        MessageBox.Show("The reconstruct paths are not present in the log folder!");
          return;
      }
      
      string mySelectQuery = "SELECT * FROM segmento";
      SQLiteConnection sqConnection = new SQLiteConnection("DataSource="+filepath);
      SQLiteCommand sqCommand = new SQLiteCommand(mySelectQuery,sqConnection);
      sqConnection.Open();
      SQLiteDataReader sqReader = sqCommand.ExecuteReader();
      Random rand = new Random();
      Dictionary<int, Color> idseg=new Dictionary<int, Color>();
      try
      {
        while (sqReader.Read())
        {
          Color color= Color.FromArgb(rand.Next(256),rand.Next(256),rand.Next(256));
          idseg.Add(sqReader.GetInt32(0), color);
        }
        

      }
      finally
      {
        // always call Close when done reading.
        sqReader.Close();
        // always call Close when done reading.
        sqConnection.Close();
      }


      Dictionary<string, List<KeyValuePair<Position,Color>>> dictionary= getPosizioni(idseg,  filepath);
     


      this.refreshPrediction(dictionary,timeInizio, timeFine);

    }

    private  Dictionary<string, List<KeyValuePair<Position,Color>>>  getPosizioni(Dictionary<int, Color>  idseg,String filepath)
    {
      string mySelectQuery = "SELECT valore.timestamp_pos, valore.posizione" +
     ",valore_segmento.id_segmento FROM valore INNER JOIN valore_segmento ON valore_segmento.id_valore = valore.id_valore";
     // TextWriter textWriter = (TextWriter) new StreamWriter("C:\\Users\\Dario\\Desktop\\HomeDesigner\\bin\\Debug\\Log\\sim_354,471915566582\\prova.txt");
      SQLiteConnection sqConnection = new SQLiteConnection("DataSource="+filepath);
      SQLiteCommand sqCommand = new SQLiteCommand(mySelectQuery,sqConnection);
      sqConnection.Open();
      SQLiteDataReader sqReader = sqCommand.ExecuteReader();
      List<String> list= new List<string>();
     //textWriter.WriteLine(sqConnection.ToString());
      try
      {
        while (sqReader.Read())
        {
          list.Add( sqReader.GetString(0).ToString()+" "
                                                    +sqReader.GetString(1).ToString()+" "+sqReader.GetInt32(2).ToString());
        
        }
      }
      finally
      {
        // always call Close when done reading.
        sqReader.Close();
        // always call Close when done reading.
        sqConnection.Close();
      }
    Dictionary<string, List<KeyValuePair<Position,Color>>> dictionary = new Dictionary<string, List<KeyValuePair<Position,Color>>>();
      String ts = list[0].Split(' ')[0];
      int idsegmento=Convert.ToInt32(list[0].Split(' ')[3]);

      List<KeyValuePair<Position,Color>> value= new List<KeyValuePair<Position,Color>>();
      Position p0=new Position();
      
    
      p0.x = double.Parse(list[0].Split(' ')[1],System.Globalization.CultureInfo.InvariantCulture) ;
      p0.y = double.Parse(list[0].Split(' ')[2],System.Globalization.CultureInfo.InvariantCulture);
      KeyValuePair<Position, Color> keyValuePair= new KeyValuePair<Position, Color>(p0,idseg[idsegmento]);

      value.Add(keyValuePair);
      dictionary.Add(ts,value);

      for (int i = 1; i < list.Count; i++ )
      {
        if (ts.Equals(list[i].Split(' ')[0]))
        {
           idsegmento=Convert.ToInt32(list[i].Split(' ')[3]);
           p0=new Position();
          p0.x = double.Parse(list[i].Split(' ')[1],System.Globalization.CultureInfo.InvariantCulture) ;
          p0.y = double.Parse(list[i].Split(' ')[2],System.Globalization.CultureInfo.InvariantCulture);
           keyValuePair= new KeyValuePair<Position, Color>(p0,idseg[idsegmento]);
          dictionary[ts].Add(keyValuePair);
        }
        else
        {
          ts = list[i].Split(' ')[0];
          idsegmento=Convert.ToInt32(list[i].Split(' ')[3]);
          p0=new Position();
          p0.x = double.Parse(list[i].Split(' ')[1],System.Globalization.CultureInfo.InvariantCulture) ;
          p0.y = double.Parse(list[i].Split(' ')[2],System.Globalization.CultureInfo.InvariantCulture);
           value= new List<KeyValuePair<Position,Color>>();
          keyValuePair=new KeyValuePair<Position, Color>(p0,idseg[idsegmento]);
          value.Add(keyValuePair);
          dictionary.Add(ts,value);
          
        }
      }

      return dictionary;
    }
    private void buttonDisegna_Click(object sender, EventArgs e)
    {
      if (!isLoaded)
      {
        MessageBox.Show("First of all you must load an house!");
        return;
      }
      
      this.refreshSimulazione( );
    }
    private void zoomMenoButton_Click(object sender, EventArgs e)
    {

      zoomFactor = 1.5f;

      Bitmap bmpOriginale = new Bitmap(this.pictureBox1.Image);

      Size newSize = new Size((int) (bmpOriginale.Width /this.zoomFactor), (int) (bmpOriginale.Height /this.zoomFactor));
      Bitmap bmpNuovo = new Bitmap(bmpOriginale, newSize);
      this.pictureBox1.Image = (Image) bmpNuovo;

      
    }
    private void zoomButton_Click(object sender, EventArgs e)
    {

      zoomFactor = 1.5f;

        Bitmap bmpOriginale = new Bitmap(this.pictureBox1.Image);

        Size newSize = new Size((int) (bmpOriginale.Width * this.zoomFactor), (int) (bmpOriginale.Height *this.zoomFactor));
        Bitmap bmpNuovo = new Bitmap(bmpOriginale, newSize);
        this.pictureBox1.Image = (Image) bmpNuovo;

      
    }

    private void buttonSimula_Click(object sender, EventArgs e)
    {
      if (!isLoaded)
      {
        MessageBox.Show("First of all you must load an house!");
        return;
      }
      FormSimulation formSimulation=new FormSimulation();
      formSimulation.ShowDialog();
      if (formSimulation.Utenti.Count == 0 )
      {
        return;
      }
     int giorniSimulazione = formSimulation.NumeroGiorni;
         this.data.setPath();
      Task task = Task.Run( () =>    this.data.simulationHandler(formSimulation.Utenti,formSimulation.TipoTraietoria, giorniSimulazione));      
      if (task.IsCompleted)
      {
        task.Dispose();
        task = null;
      }


    }

    private void button1_Click_1(object sender, EventArgs e)
    {
      int num = (int) MessageBox.Show("Project info here.", "About");
    }
    private void cb1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    
  }
}
