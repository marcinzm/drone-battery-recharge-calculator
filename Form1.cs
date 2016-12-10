using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using WMPLib;
using System.Globalization;

namespace DroneBatteryLoadCalculator
{
    public partial class Form1 : Form
    {

        public int czas_minuty=0;
        public int czas_sekundy=0;
        public int czas_skumulowany = 0;
        public int czas_skumulowany_update = 0;
        public Boolean koniec_uruchamiania = false;
        public double przelicznik = 0, przelicznik_minus_jeden=0, przelicznik1=0, przelicznik_minus_jeden_do_obliczen = 0;
        

        //System.Timers.Timer myTimer = new System.Timers.Timer(1000);
        //System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer(1000);
        //Timer myTimer = new Timer(1000);

        //private readonly SynchronizationContext synchronizationContext;
        //private DateTime previousTime = DateTime.Now;

        public Form1()
        {
            InitializeComponent();

            string p_comboBox2 = "";

            if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_lng.txt"))
            {
                string p_lng_ver = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_lng.txt");

                if (p_lng_ver == "Polski")
                {
                    p_comboBox2 = "Polski";
                }
                else
                {
                    p_comboBox2 = "English";
                }
            }
            else
            {
                System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_lng.txt", "Polski");
                p_comboBox2 = "Polski";
            }

            //synchronizationContext = SynchronizationContext.Current; //context from UI thread 
            label2.Text = "";
            label3.Text = "";
            label1.Text = DroneBatteryLoadCalculator.Properties.Resources.podaj_procent_do_naladowania;
            button1.Text = DroneBatteryLoadCalculator.Properties.Resources.button1_text;
            button2.Text= DroneBatteryLoadCalculator.Properties.Resources.button2_text;


            label5.Text = "min:"+ DroneBatteryLoadCalculator.Properties.Resources.sek;
            label2.Text = "";
            button2.Visible = false;
            button1.Enabled = false;
            czas_skumulowany = 0;
            czas_skumulowany_update = 0;
            czas_minuty = 0;
            czas_sekundy = 0;
            koniec_uruchamiania = false;
            textBox2.Enabled = false;
            textBox2.Text = 10.ToString();
            textBox3.Enabled = false;

            if (!System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters.txt"))
            {
                System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters.txt", 0.ToString());
            }


            /* var myTimer = new System.Timers.Timer(1000);

             myTimer.Elapsed += UpdateLabel;
             myTimer.Start();*/


            // To update the first time.
            //label4.Text = (DateTime.Today.AddDays(1) - DateTime.Now).ToString(@"hh\:mm\:ss");
            //var timer = new Timer { Interval = 1000 };
            //Timer timer1 = new System.Timers.Timer();

            //var timer1 = new System.Timers.Timer (1000);
            /*timer1.Tick += (o, args) =>
            {
                label4.Text = (DateTime.Today.AddDays(1) - DateTime.Now).ToString(@"hh\:mm\:ss");
            };
            timer1.Start();*/


            var MyTimer = new System.Windows.Forms.Timer();
            MyTimer.Interval = (1000);
            MyTimer.Tick += new EventHandler(MyTimer_Tick);
            MyTimer.Start();

            label4.Text = "00:00";

            //wartosci dla 10%

            comboBox2.Items.Add(new Item_lng("English", "en-EN"));
            comboBox2.Items.Add(new Item_lng("Polski", "pl-PL"));



            wypelnij_combo_box1();
            comboBox2.Text = p_comboBox2;

            if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_your_drone.txt"))
            {
                string p_your_drone = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_your_drone.txt");

                comboBox1.Text = p_your_drone;

            }
            else
            {
                System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_your_drone.txt", comboBox1.Text);
            }


        }

        private void wypelnij_combo_box1()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add(new Item("DJI Phantom 3 Pro, Adv, Std, 4K", 4.20));
            comboBox1.Items.Add(new Item("DJI Phantom 4, 4 Pro", 8.145)); //po przemianie
            comboBox1.Items.Add(new Item("DJI Phantom 2, Vision, Vision+ ", 8.528571)); //po przemianie wartosc 10.3667 jest mniej precyzyjna, a wartosc 8,528571 jest bardzo precyzyjna
            comboBox1.Items.Add(new Item("DJI Mavic Pro", 6.8731));  //po przemianie
            comboBox1.Items.Add(new Item("DJI Inspire 1", 11.1944)); //po przemianie
            comboBox1.Items.Add(new Item(DroneBatteryLoadCalculator.Properties.Resources.combo_box1_ustawienia_reczne, -1)); //po przemianie
        }

        private class Item
        {
            public string Name;
            public double Value;
            public Item(string name, double value)
            {
                Name = name; Value = value;
            }
            public override string ToString()
            {
                // Generates the text shown in the combo box
                return Name;
            }
        }

        private class Item_lng
        {
            public string Name;
            public string Value;
            public Item_lng(string name, string value)
            {
                Name = name; Value = value;
            }
            public override string ToString()
            {
                // Generates the text shown in the combo box
                return Name;
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Display the Value property
                Item itm = (Item)comboBox1.SelectedItem;

                przelicznik = itm.Value;


                System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_your_drone.txt", comboBox1.Text);
                //MessageBox.Show("CCCC"+comboBox1.Text);


                //double p_double = double.Parse(System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters.txt"));

                /*if (przelicznik_minus_jeden>0 && przelicznik<0)
                {
                    przelicznik = przelicznik_minus_jeden;
                    textBox3.Enabled = true;
                }
                else
                {
                    przelicznik_minus_jeden = -1;
                    textBox3.Enabled = false;
                }*/

                if ((przelicznik > 0) || (przelicznik_minus_jeden > 0 && przelicznik < 0))
                {
                    button1.Enabled = true;
                    label2.Text = "";

                    if (przelicznik_minus_jeden > 0 && przelicznik < 0)
                    {
                        //MessageBox.Show("aaaa" + przelicznik_minus_jeden.ToString());
                        textBox3.Text = przelicznik_minus_jeden.ToString();
                        //przelicznik = przelicznik_minus_jeden;
                        textBox3.Enabled = true;
                        przelicznik_minus_jeden_do_obliczen = przelicznik_minus_jeden;
                        przelicznik_minus_jeden = -1;
                        
                    }
                    else
                    {
                        textBox3.Text = przelicznik.ToString();
                        textBox3.Enabled = false;
                    }

                    for (int i = 10; i < 21; i++)
                    {
                        label2.Text += i.ToString() + " %: " + oblicz(i, "min").ToString() + ":" + oblicz(i, "sek").ToString() + " min:"+ DroneBatteryLoadCalculator.Properties.Resources.sek+" " + Environment.NewLine;
                    }

                    
                    //przelicznik_minus_jeden = przelicznik;


                    if (!String.IsNullOrEmpty(textBox1.Text))
                    {
                        int number1;
                        if (int.TryParse(textBox1.Text, out number1))
                        {
                            if (int.Parse(textBox1.Text) > 0)
                            {
                                button2.Enabled = true;
                                button1.PerformClick();
                            }
                        }
                    }
                    else
                    {
                        label3.Text = "";
                        button2.Enabled = false;
                    }

                }
                else if (przelicznik==-1)
                {
                    
                    textBox3.Enabled = true;
                    przelicznik_minus_jeden = -1;

                    if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters.txt"))
                    {
                        textBox3.Text=System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters.txt");
                        //MessageBox.Show(textBox3.Text);
                        if (!String.IsNullOrEmpty(textBox3.Text))
                        {

                            double number1;
                            NumberStyles style = NumberStyles.AllowDecimalPoint;
                            CultureInfo culture = CultureInfo.CreateSpecificCulture("pl-PL");

                            if (Double.TryParse(textBox3.Text, style, culture, out number1))
                            {


                                przelicznik_minus_jeden = double.Parse(textBox3.Text);
                                przelicznik_minus_jeden_do_obliczen = przelicznik_minus_jeden;
                                //MessageBox.Show("zzz"+textBox3.Text);

                                //button2.Enabled = true;
                                comboBox1_SelectedIndexChanged(comboBox1, new EventArgs());
                            }
                        }
                    }
                    else
                    {
                        textBox3.Text = null;
                    }

                    

                }
                else
                {
                    button1.Enabled = false;
                }
                //Console.WriteLine("{0}, {1}", itm.Name, itm.Value);
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine("\nCant read property which is nulkl" + nre.Message);
            }
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            //label8.Text = getCPUCounter() + "%";
            //label9.Text = theMemCounter.NextValue().ToString();
            //label4.Text = (DateTime.Today.AddDays(1) - DateTime.Now).ToString(@"hh\:mm\:ss");
            //if (czas_skumulowany_update > 0)
            //label4.Text += "." + czas_skumulowany_update;
            //if (czas_minuty>0 || czas_sekundy>0)
            //{
                //label4.Text = czas_skumulowany_update.ToString();
                //label4.Text = czas_minuty + ":" + czas_sekundy;
                if (czas_skumulowany_update > 0)
                {
                    label4.Text = zamien_czas_skumulowany_na_czas_nieskumulowany(czas_skumulowany_update, "min").ToString() + ":" + zamien_czas_skumulowany_na_czas_nieskumulowany(czas_skumulowany_update, "sek").ToString();
                    //label4.Text = czas_skumulowany_update.ToString();
                }
                if (koniec_uruchamiania)
                {
                    czas_skumulowany_update = 0;
                    label4.Text = "0:0";
                    koniec_uruchamiania = false;


                    
                    WMPLib.WindowsMediaPlayer wplayerX1 = new WMPLib.WindowsMediaPlayer();
                    //wplayer.URL = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)+"\\345058__littlerainyseasons__sound-effect-magic.mp3";
                    wplayerX1.URL = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\DroneBatteryLoadCalculator_koniec.mp3";
                    wplayerX1.controls.play();
                    
                    
                    button1.Enabled = true;
                    button2.Enabled = true;
                }
        }

        /*        private static void UpdateLabel(object sender, ElapsedEventArgs e)
                {
                    //Update label here
                    label4.Text = czas_skumulowany.ToString();
                }*/

        private int oblicz(int procent, string minuty_sekundy = "")
        {
            int wartosc = 0;
            if (przelicznik_minus_jeden_do_obliczen > 0 && przelicznik < 0)
            {
                przelicznik1 = przelicznik_minus_jeden_do_obliczen;
                //MessageBox.Show(przelicznik1.ToString());
            }
            else
            {
                przelicznik1 = przelicznik;
            }
            if (String.IsNullOrEmpty(minuty_sekundy))
            {
                //float wartosc1 = (float)(procent * 4.2) / 10;
                int czas_min = oblicz(procent, "min");
                int czas_sek = oblicz(procent, "sek");

                wartosc = czas_min * 60 + czas_sek;
                //wartosc =zamien_na_sekundy(wartosc1);
            }
            else if (minuty_sekundy=="min")
            {
                //int wartosc1 = (int)(procent * 4.2) / 10;

                //int wartosc1 = (int)((procent * 4.2*60) / 10);
                
                int wartosc1 = (int)((procent * przelicznik1 * 60) / 10);
                //10 proc - 4 minuty
                //procent = x

                //zamien_dziesietne_na_sekundy
                //wartosc = wartosc1;
                //label5.Text = wartosc12.ToString();

                wartosc = zamien_czas_skumulowany_na_czas_nieskumulowany(wartosc1,"min");
            }
            else if (minuty_sekundy=="sek")
            {
                /*int wartosc1 = (int)(procent * 4.2*60) / 10;
                float wartosc2 = (float)(procent * 4.2*60) / 10;
                int calkowite_sekundy_decimal = (int)((wartosc2 - wartosc1) * 100);*/


                int wartosc1 = (int)((procent * przelicznik1 * 60) / 10);

                //label5.Text = wartosc1.ToString();

                wartosc = zamien_czas_skumulowany_na_czas_nieskumulowany(wartosc1, "sek");

                //label5.Text = wartosc.ToString();

                //wartosc = calkowite_sekundy_decimal;
            }

            return wartosc;
        }

        private int zamien_czas_skumulowany_na_czas_nieskumulowany(int p_czas_skumulowany,string typ="min")
        {
            int wartosc = 0;

            if (typ == "min")
            {
                wartosc=(int)(p_czas_skumulowany / 60);
            }
            else if (typ=="sek")
            {
                if (p_czas_skumulowany > 59)
                {
                    //float p_sekundy = (float)(p_czas_skumulowany % 60);
                    int p_sekundy = (int)(p_czas_skumulowany % 60);

                    //label5.Text = p_sekundy.ToString()+"aaa";
                    //wartosc = (int)(p_czas_skumulowany / 60) - (int)(p_sekundy * 100);
                    //int reszta_sekund = ((int)(p_sekundy) - (int)(p_czas_skumulowany / 60))*100;
                    //wartosc = (60 * reszta_sekund) / 100;

                    //100 - 60
                    //  reszta_sekund - x
                    wartosc = p_sekundy;
                }
                else
                {
                    wartosc = p_czas_skumulowany;
                }
            }
            else
            {
                wartosc = 0;
            }


            return wartosc;
        }

        private int zamien_na_sekundy(float czas)
        {
            int calkowite_minuty = (int)czas;
            int calkowite_sekundy_decimal = (int)((czas - (int)calkowite_minuty)*100);

            //100 - 60
            //calkowite_sekundy_decimal - x

            czas_skumulowany = (czas_minuty * 60) + zamien_dziesietne_na_sekundy(calkowite_sekundy_decimal);


            return czas_skumulowany;
        }

        private int zamien_dziesietne_na_sekundy(int sekundy_decimal)
        {
            return (60 * sekundy_decimal) / 100;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                    button2.Enabled = false;
            }


        }

        /*public string ReadResourceValue(string file, string key)

        {


            

            string resourceValue = string.Empty;
            try
            {

                string resourceFile = file;

                string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();

                System.Resources.ResourceManager resourceManager = System.Resources.ResourceManager.CreateFileBasedResourceManager(resourceFile, filePath, null);
                // retrieve the value of the specified key
                resourceValue = resourceManager.GetString(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                resourceValue = string.Empty;
            }
            return resourceValue;
        }*/

        private void button1_Click(object sender, EventArgs e)
        {

            int Number;
            bool isNumber;
            isNumber = Int32.TryParse(textBox1.Text, out Number);

            if (!isNumber)
            {
                czas_minuty = 0;
                czas_sekundy = 0;
                button2.Visible = false;
                label4.Text = "00:00";
                label3.Text = "";



                MessageBox.Show(DroneBatteryLoadCalculator.Properties.Resources.wprowadz_tekst1);
                //MessageBox.Show(ReadResourceValue("Resources.resx", "wprowadz_tekst1"));
            }
            else
            {
                int p_procent = int.Parse(textBox1.Text);

                czas_skumulowany=oblicz(p_procent,"");
                czas_minuty = oblicz(p_procent, "min");
                czas_sekundy = oblicz(p_procent, "sek");
                

                label3.Text = p_procent+ DroneBatteryLoadCalculator.Properties.Resources.musisz_ladowac1 +oblicz(p_procent,"min").ToString()+":" + oblicz(p_procent, "sek").ToString()+" min:"+ DroneBatteryLoadCalculator.Properties.Resources.sek;

                label4.Text = czas_minuty + ":" + czas_sekundy;
                button2.Visible = true;
                button2.Enabled = true;
            }


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {


            Item_lng itm = (Item_lng)comboBox2.SelectedItem;

            //string zapamietaj_combobox1_2 = comboBox1.Text;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(itm.Value);
            ChangeLanguage(itm.Value);

            label1.Text = DroneBatteryLoadCalculator.Properties.Resources.podaj_procent_do_naladowania;
            label6.Text = DroneBatteryLoadCalculator.Properties.Resources.dron;
            label3.Text = DroneBatteryLoadCalculator.Properties.Resources.musisz_ladowac1;

            string p_your_drone = "";

            if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_your_drone.txt"))
            {
                p_your_drone = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_your_drone.txt");
            }


            //string zapamietaj_combobox1_3 = comboBox1.Text;
            //comboBox1.Text = DroneBatteryLoadCalculator.Properties.Resources.combo_box1_ustawienia_reczne;
            wypelnij_combo_box1();





            if (p_your_drone== DroneBatteryLoadCalculator.Properties.Resources.combo_box1_ustawienia_reczne)
            {
                //comboBox1.Text = zapamietaj_combobox1_2;
                comboBox1.Text = DroneBatteryLoadCalculator.Properties.Resources.combo_box1_ustawienia_reczne;
            }
            /*else if (p_your_drone == DroneBatteryLoadCalculator.Properties.Resources.combo_box1_ustawienia_reczne)
            {
                comboBox1.Text = zapamietaj_combobox1_3;
            }*/
            else
            {
                comboBox1.Text = p_your_drone;
            }

            if (comboBox2.Text == "Polski")
            {
                label2.Text = label2.Text.Replace("sec", "sek");
                label5.Text = label5.Text.Replace("sec", "sek");
            }
            else
            {
                label2.Text = label2.Text.Replace("sek", "sec");
                label5.Text = label5.Text.Replace("sek", "sec");
            }
            button1.Text = DroneBatteryLoadCalculator.Properties.Resources.button1_text;
            button2.Text = DroneBatteryLoadCalculator.Properties.Resources.button2_text;
            //aaaaaaaaaaaaaaaaaaaa



            System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters_lng.txt", comboBox2.Text);



        }

        private void ChangeLanguage(string lang)
        {
            foreach (Control c in this.Controls)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
                resources.ApplyResources(c, c.Name, new CultureInfo(lang));
                //MessageBox.Show(c.ToString());
                //if (c.ToString().StartsWith("System.Windows.Forms.GroupBox"))
                if (c.ToString().StartsWith("System.Windows.Forms"))
                {
                    //MessageBox.Show("wlazl");
                    foreach (Control child in c.Controls)
                    {
                        //MessageBox.Show(child.Name.ToString());
                        ComponentResourceManager resources_child = new ComponentResourceManager(typeof(Form1));
                        resources_child.ApplyResources(child, child.Name, new CultureInfo(lang));
                    }
                }
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void thread_function()
        {
            for (int i = 1; i <= czas_skumulowany; i++)
            {
                czas_skumulowany_update = czas_skumulowany - i;
                Thread.Sleep(1000);
                //Console.WriteLine("{0} says hello", Thread.CurrentThread.Name);
                //Thread.Sleep(1000);
                
            }
            koniec_uruchamiania = true;
        }

        private  void button2_Click(object sender, EventArgs e)
        {
            //label5.Text = czas_skumulowany.ToString();
            //label5.Text = czas_sekundy.ToString();
            if (czas_minuty > 0 || czas_sekundy >0)
            {

                WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
                //wplayer.URL = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)+"\\316552__littlerobotsoundfactory__splat-15.wav";
                wplayer.URL = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\DroneBatteryLoadCalculator_start.wav";
                wplayer.controls.play();
                button1.Enabled = false;
                button2.Enabled = false;

                Thread worker_thread = new Thread(new ThreadStart(thread_function));
                worker_thread.Name = "worker thread";
                worker_thread.Start();

            }

         }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //var sb = new StringBuilder();

            //sb.text = TwojTextBox.Text;

            if (przelicznik == -1)
            {

                System.IO.File.WriteAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Parameters.txt", textBox3.Text);
                
                //MessageBox.Show("gggg" + textBox3.Text);

                double number1;
                int number2;
                NumberStyles style = NumberStyles.AllowDecimalPoint;
                CultureInfo culture = CultureInfo.CreateSpecificCulture("fr-FR");

                if (Double.TryParse(textBox3.Text, style, culture, out number1))
                {
                    button1.Enabled = true;
                    przelicznik_minus_jeden_do_obliczen = double.Parse(textBox3.Text);
                    label2.Text = "";

                    for (int i = 10; i < 21; i++)
                    {
                        label2.Text += i.ToString() + " %: " + oblicz(i, "min").ToString() + ":" + oblicz(i, "sek").ToString() + " min:"+ DroneBatteryLoadCalculator.Properties.Resources.sek+" " + Environment.NewLine;
                    }

                    if (!String.IsNullOrEmpty(textBox1.Text))
                    { 
                        if (int.TryParse(textBox1.Text, out number2))
                        {
                            button2.Enabled = true;
                        }
                    }

                }
                else
                {
                    MessageBox.Show(DroneBatteryLoadCalculator.Properties.Resources.wprowadz_liczbe1);
                    label3.Text = "";
                    label2.Text = "";
                    label4.Text = "00:00";
                    button1.Enabled = false;
                    button2.Enabled = false;
                }
            }

        }
    }
}
