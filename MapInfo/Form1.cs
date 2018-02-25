using System;
using System.Windows.Forms;
using GMap.NET;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Globalization;

namespace MapInfo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Global {
            public static bool zmiana = true;
            public static double lat, lng, oldlat, oldlng;
            public static string APIkey = "75e536ee6d2c3b5995d274859c1ee495";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gMap.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance; //GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gMap.SetPositionByKeywords("Paris, France");
        }

        private void gMap_OnPositionChanged(PointLatLng point)
        {
            Global.zmiana = true;
            Global.lat = point.Lat;
            Global.lng = point.Lng;

            numericUpDown1.Value = Convert.ToDecimal(point.Lat);
            numericUpDown2.Value = Convert.ToDecimal(point.Lng);

            textBox1.Text = "https://www.google.com/maps/?q=" + point.Lat.ToString("#.0000000", CultureInfo.InvariantCulture) + "," + point.Lng.ToString("#.0000000", CultureInfo.InvariantCulture);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(Global.zmiana)
            {
                if((Global.oldlat == Global.lat) && (Global.oldlng == Global.lng))
                {
                    Global.zmiana = false;

                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.openweathermap.org/data/2.5/weather?lat="
                            + Global.lat.ToString() + "&lon=" + Global.lng.ToString() + "&units=metric&lang=pl&APPID=" + Global.APIkey);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        string JsonText = reader.ReadToEnd();

                        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                        dynamic json = jsonSerializer.Deserialize<dynamic>(JsonText);

                        label1.Text = "Miejsce: " + json["name"] + "\nKraj: " + json["sys"]["country"] + "\n\nPogoda: " + json["weather"][0]["description"]
                            + "\nTemperatura: " + json["main"]["temp"] + " C\nCiśnienie: " + json["main"]["pressure"] + " hPa\nWilgotność: " + json["main"]["humidity"]
                            + " %\nWiatr: " + json["wind"]["speed"] + " m/s";
                    }
                    catch //(WebException ex)
                    {
                        //string JsonText = "";
                    }

                } else
                {
                    Global.oldlat = Global.lat;
                    Global.oldlng = Global.lng;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gMap.SetPositionByKeywords(textBox2.Text);
            Global.zmiana = true;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                gMap.SetPositionByKeywords(textBox2.Text);
                Global.zmiana = true;
            }
        }
    }
}
