
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
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Devart.Data.SQLite;
using Devart.Data;

namespace HomeDesigner
{
    public class FormInfo : Form
    {
        private ComboBox startTextBoxH;
        private ComboBox startTextBoxM;
        private ComboBox startTextBoxS;
        private Label labelStart;
        private Label labelEnd;
    

        private ComboBox endTextBoxH;
        private ComboBox endTextBoxM;
        private ComboBox endTextBoxS;

        private Button buttonOk;

        private String selectedInterval;
        public FormInfo()
        {
            this.InitializeComponent();
            this.Text = "Simulation Info";
        }

        public string SelectedInterval => selectedInterval;
        private void cb3_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedInterval = startTextBoxH.Text.ToString()+":"+startTextBoxM.Text.ToString()+":"+startTextBoxS.Text.ToString()
                               +"-"+endTextBoxH.Text.ToString()+":"+endTextBoxM.Text.ToString()+":"+endTextBoxS.Text.ToString();
    
        }
        private void InitializeComponent()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
           

            this.labelStart=new Label();
            this.labelStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.labelStart.AutoSize = true;
            this.labelStart.Location = new Point(0, 20);
            this.labelStart.Name = "label1";
            this.labelStart.Size = new Size(72, 13);
            this.labelStart.TabIndex = 2;
            this.labelStart.Text = "Start time";
            this.startTextBoxH = new ComboBox();
            this.startTextBoxH.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.startTextBoxH.Location = new Point(60, 20);
            this.startTextBoxH.Name = "startBox";
            this.startTextBoxH.Size = new Size(50, 21);
            this.startTextBoxH.TabIndex = 30;
            this.startTextBoxH.SelectedIndexChanged += new EventHandler(this.cb3_SelectedIndexChanged);

            this.startTextBoxM = new ComboBox();
            this.startTextBoxM.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.startTextBoxM.Location = new Point(120, 20);
            this.startTextBoxM.Name = "startBox";
            this.startTextBoxM.Size = new Size(50, 21);
            this.startTextBoxM.TabIndex = 30;
            this.startTextBoxM.SelectedIndexChanged += new EventHandler(this.cb3_SelectedIndexChanged);

            this.startTextBoxS = new ComboBox();
            this.startTextBoxS.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.startTextBoxS.Location = new Point(190, 20);
            this.startTextBoxS.Name = "startBox";
            this.startTextBoxS.Size = new Size(50, 21);
            this.startTextBoxS.TabIndex = 30;
            this.startTextBoxS.SelectedIndexChanged += new EventHandler(this.cb3_SelectedIndexChanged);
            this.labelEnd=new Label();

            this.labelEnd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.labelEnd.AutoSize = true;
            this.labelEnd.Location = new Point(0, 80);
            this.labelEnd.Name = "label2";
            this.labelEnd.Size = new Size(72, 13);
            this.labelEnd.TabIndex = 2;
            this.labelEnd.Text = "End time";
            this.endTextBoxH = new ComboBox();
            this.endTextBoxH.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.endTextBoxH.Location = new Point(60, 80);
            this.endTextBoxH.Name = "endBox";
            this.endTextBoxH.Size = new Size(50, 21);
            this.endTextBoxH.TabIndex = 30;
            this.endTextBoxH.SelectedIndexChanged += new EventHandler(this.cb3_SelectedIndexChanged);

            this.endTextBoxM = new ComboBox();
            this.endTextBoxM.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.endTextBoxM.Location = new Point(120, 80);
            this.endTextBoxM.Name = "endBox";
            this.endTextBoxM.Size = new Size(50, 21);
            this.endTextBoxM.TabIndex = 30;
            this.endTextBoxM.SelectedIndexChanged += new EventHandler(this.cb3_SelectedIndexChanged);

            this.endTextBoxS = new ComboBox();
            this.endTextBoxS.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.endTextBoxS.Location = new Point(190, 80);
            this.endTextBoxS.Name = "endBox";
            this.endTextBoxS.Size = new Size(50, 21);
            this.endTextBoxS.TabIndex = 30;
            this.endTextBoxS.SelectedIndexChanged += new EventHandler(this.cb3_SelectedIndexChanged);

            this.buttonOk=new Button();
            this.buttonOk.Location = new Point(100, 200);
            this.buttonOk.Name = "OkButton";
            this.buttonOk.Size = new Size(40, 40);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new EventHandler(this.buttonOk_Click);
           


  
          
          for(int i=0; i<=23; i++){
              if (i < 10)
              {
                  this.startTextBoxH.Items.Add((object) "0"+i.ToString());
               
                  this.endTextBoxH.Items.Add((object)  "0"+i.ToString());
              }
              else
              {
                  this.startTextBoxH.Items.Add((object) i.ToString());
               
                  this.endTextBoxH.Items.Add((object)  i.ToString());
              }
              }
            
            for(int i=0; i<=59; i++){
                if (i < 10)
                {
                    this.startTextBoxM.Items.Add((object) "0"+i.ToString());
               
                    this.endTextBoxM.Items.Add((object)  "0"+i.ToString());
                    this.startTextBoxS.Items.Add((object) "0"+i.ToString());
                    this.endTextBoxS.Items.Add((object)  "0"+i.ToString());

                }
                else
                {
                    this.startTextBoxM.Items.Add((object) i.ToString());
               
                    this.endTextBoxS.Items.Add((object)  i.ToString());
                    this.startTextBoxS.Items.Add((object) i.ToString());
                    this.endTextBoxS.Items.Add((object)  i.ToString());
                }
            }


            this.startTextBoxH.SelectedItem = (object) "00";
            this.startTextBoxM.SelectedItem = (object) "00";
            this.startTextBoxS.SelectedItem = (object) "00";
            this.endTextBoxH.SelectedItem = (object) "00";
            this.endTextBoxM.SelectedItem = (object) "00";
            this.endTextBoxS.SelectedItem = (object) "00";

               
           
            this.Controls.Add((Control) this.labelStart);
            this.Controls.Add((Control) this.labelEnd);
          

            
            this.Controls.Add((Control) this.startTextBoxH);
            this.Controls.Add((Control) this.startTextBoxM);
            this.Controls.Add((Control) this.startTextBoxS);

            this.Controls.Add((Control) this.endTextBoxH);
            this.Controls.Add((Control) this.endTextBoxM);
            this.Controls.Add((Control) this.endTextBoxS);

            this.Controls.Add((Control) this.buttonOk);

        }
      

        private void buttonOk_Click(object sender, EventArgs e)
        {
          
            if ( selectedInterval.Split('-')[0].Equals("00:00:00") &&  selectedInterval.Split('-')[1].Equals("00:00:00"))
            {
                MessageBox.Show("You must choose an interval of time to draw! ");
                return;
            }
            if (DateTime.Parse(selectedInterval.Split('-')[0]) > DateTime.Parse(selectedInterval.Split('-')[1]))
            {
                MessageBox.Show("You must choose a correct interval! ");
                return;
            }
            selectedInterval = startTextBoxH.Text.ToString()+":"+startTextBoxM.Text.ToString()+":"+startTextBoxS.Text.ToString()
                               +"-"+endTextBoxH.Text.ToString()+":"+endTextBoxM.Text.ToString()+":"+endTextBoxS.Text.ToString();


                this.Close();
            
            
        }

        
    }
}

   


