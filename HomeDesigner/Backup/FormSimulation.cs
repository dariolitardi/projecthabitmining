
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
    
    public class FormSimulation:Form
    {
        private ComboBox numeroPersoneBox;
        private ComboBox tipoTraiettorieBox;

        private List<KeyValuePair<Label,KeyValuePair<ComboBox,ComboBox>>> listaComboBoxs;
        private Button simula;
        private List<KeyValuePair<int, string>> utenti;
        private string tipoTraietoria;
        public FormSimulation()
        {
            this.InitializeComponent();
            this.Text = "Simulation Properties";
            this.utenti= new List<KeyValuePair<int, string>>();
        }

        public string TipoTraietoria => tipoTraietoria;

        public List<KeyValuePair<int, string>> Utenti
        {
            get => utenti;
            set => utenti = value;
        }

        private void InitializeComponent()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            listaComboBoxs= new  List<KeyValuePair<Label,KeyValuePair<ComboBox,ComboBox>>>();

            this.numeroPersoneBox = new ComboBox();
            this.tipoTraiettorieBox = new ComboBox();

            this.simula= new Button();    
            this.numeroPersoneBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.numeroPersoneBox.FormattingEnabled = true;
            this.numeroPersoneBox.Location = new Point(50, 20);
            this.numeroPersoneBox.Name = "numeroPersoneBox";
            this.numeroPersoneBox.Size = new Size(121, 21);
            this.numeroPersoneBox.TabIndex = 30;
            this.numeroPersoneBox.SelectedIndexChanged += new EventHandler(this.cb1_SelectedIndexChanged);
            this.Controls.Add((Control) this.numeroPersoneBox);
            this.numeroPersoneBox.Items.Add((object) "[number_of_persons]");
          
            this.numeroPersoneBox.Items.Add((object) "1");
            this.numeroPersoneBox.Items.Add((object) "2");
            this.numeroPersoneBox.Items.Add((object) "3");
            this.numeroPersoneBox.Items.Add((object) "4");
            this.numeroPersoneBox.SelectedItem = (object) "[number_of_persons]";
            
            this.tipoTraiettorieBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.tipoTraiettorieBox.FormattingEnabled = true;
            this.tipoTraiettorieBox.Location = new Point(50, 50);
            this.tipoTraiettorieBox.Name = "tipoTraiettorieBox";
            this.tipoTraiettorieBox.Size = new Size(130, 21);
            this.tipoTraiettorieBox.TabIndex = 30;
            this.tipoTraiettorieBox.SelectedIndexChanged += new EventHandler(this.cb1_SelectedIndexChanged);
            this.tipoTraiettorieBox.Items.Add((object) "[type_of_trajectories]");
          
            this.tipoTraiettorieBox.Items.Add((object) "Continuous trajectories");
            this.tipoTraiettorieBox.Items.Add((object) "Discrete trajectories");
  
            this.tipoTraiettorieBox.SelectedItem = (object) "[type_of_trajectories]";
            this.Controls.Add((Control) this.tipoTraiettorieBox);

            this.simula.Location = new Point(100, 210);
            this.simula.Name = "simula";
            this.simula.Size = new Size(70, 31);
            this.simula.TabIndex = 5;
            this.simula.Text = "Simulate";
            this.simula.UseVisualStyleBackColor = true;
            this.simula.Click += new EventHandler(this.buttonSimula_Click);
            this.Controls.Add((Control) this.simula);

        }

        private void cb2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        private void cb3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        private void buttonSimula_Click(object sender, EventArgs e)
        {
            if (this.numeroPersoneBox.Text.Equals("[number_of_persons]")
                )
            {
                MessageBox.Show("Choose the number of people for the simulation!");

                return;
            }

            if (this.tipoTraiettorieBox.Text.Equals("[type_of_trajectories]"))
            {
                MessageBox.Show("Choose the type of the trajectories!");

                return;  
            }
            else
            {
                if(tipoTraiettorieBox.Text.Equals("Continuous trajectories"))
                tipoTraietoria = "Continuous";
                else
                {
                    tipoTraietoria = "Discrete";

                }
                foreach (KeyValuePair<Label, KeyValuePair<ComboBox,ComboBox> > keyValuePair in listaComboBoxs)
                {
                    if (keyValuePair.Value.Key.Text.Equals("variable") &&
                        (!keyValuePair.Value.Value.Text.Equals("[speed (m/s) ]")))
                    {
                        MessageBox.Show("Since the speed is variable, don't select any speed value!");

                        return;
                    }
                    int utente = Convert.ToInt32(keyValuePair.Key.Text.Split(' ')[1]);
                    float velocita;
                    if (keyValuePair.Value.Key.Text.Equals("variable"))
                    {
                        velocita = 1;
                        this.utenti.Add(new KeyValuePair<int, string>(utente,velocita+" "+Convert.ToString(true)));

                    }
                    else
                    {
                        velocita = (float) Convert.ToDouble(keyValuePair.Value.Value.Text);
                        this.utenti.Add(new KeyValuePair<int, string>(utente,velocita+" "+Convert.ToString(false)));

                    }

                }

                this.Close();
            }
        }

        private void cb1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.numeroPersoneBox.Text.Equals("[number_of_persons]"))
            {
                return;
            }
            else
            {
                int numPersone = Convert.ToInt32(this.numeroPersoneBox.Text);
                int x = 50;
                int y = 60;

                foreach (KeyValuePair<Label,KeyValuePair<ComboBox,ComboBox>> c in listaComboBoxs)
                {
                    
                
               
                    this.Controls.Remove(c.Key);
                    this.Controls.Remove(c.Value.Key);
                    this.Controls.Remove(c.Value.Value);

                }
                listaComboBoxs.Clear();
               
                for (int i = 1; i <= numPersone; i++)
                {
                    ComboBox velocita= new ComboBox();
                    Label label= new Label();
                    ComboBox flag= new ComboBox();
                    KeyValuePair<ComboBox,ComboBox> k= new KeyValuePair<ComboBox, ComboBox>(flag,velocita);
                    listaComboBoxs.Add(new KeyValuePair<Label,  KeyValuePair<ComboBox,ComboBox> >(label,k));

                }
               
                for (int i = 1; i <= numPersone; i++)
                {
                    
                    ComboBox velocita = listaComboBoxs[i-1].Value.Value;
                    ComboBox flagVelocitaVar = listaComboBoxs[i-1].Value.Key;

                    Label label = listaComboBoxs[i - 1].Key;
                    
                    velocita.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    velocita.FormattingEnabled = true;
                   
                    velocita.Location = new Point(x+110, i*30+y);
                    velocita.Name = "numeroPersoneBox";
                    velocita.Size = new Size(100, 21);
                    velocita.TabIndex = 30;
                    velocita.SelectedIndexChanged += new EventHandler(this.cb2_SelectedIndexChanged);
                   
                    velocita.Items.Add((object) "[speed (m/s) ]");

                    velocita.Items.Add((object) "0,25");
                    velocita.Items.Add((object) "0,5");
                    velocita.Items.Add((object) "1");
                    velocita.SelectedItem = (object)"[speed (m/s) ]";
                    this.Controls.Add((Control)velocita);
                    label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    label.AutoSize = true;
                    label.Location = new Point(10, i*30+y);
                    label.Name = "label"+i;
                    label.Size = new Size(72, 13);
                    label.TabIndex = 2;
                    label.Text = "User "+i;
                    this.Controls.Add((Control)label);
                    
                    flagVelocitaVar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    flagVelocitaVar.FormattingEnabled = true;
                   
                    flagVelocitaVar.Location = new Point(x, i*30+y);
                    flagVelocitaVar.Name = "flagVelocita"+i;
                    flagVelocitaVar.Size = new Size(100, 21);
                    flagVelocitaVar.TabIndex = 30;
                    flagVelocitaVar.SelectedIndexChanged += new EventHandler(this.cb3_SelectedIndexChanged);
                   
                    flagVelocitaVar.Items.Add((object) "[type of speed]");

                    flagVelocitaVar.Items.Add((object) "continuous");
                    flagVelocitaVar.Items.Add((object) "variable");
                    
                    flagVelocitaVar.SelectedItem = (object)"[type of speed]";
                    this.Controls.Add((Control)flagVelocitaVar);

                }
                
            }
        }
    }
}