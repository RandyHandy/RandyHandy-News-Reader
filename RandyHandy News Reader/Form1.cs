using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace RandyHandy_News_Reader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Request HTTP GET
            string resource_url = "http://randyhandy.net/feed";
            ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Proxy = null;
            request.Method = "GET";

            WebResponse response;
            string xml_feed = "";
            try
            {
                response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                xml_feed = sr.ReadToEnd();
                sr.Close();
                response.Close();
            }
            catch
            {
                MessageBox.Show("Somethimg went wrong, try again.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            List<Tuple<string, string>> lista = ParseFeed(xml_feed);

            foreach (Tuple<string, string> temp in lista)
            {
                string title = temp.Item1;
                string link = temp.Item2;
                ListViewItem element = new ListViewItem(title);
                element.SubItems.Add(link);
                listView1.Items.Add(element);
            }
        }

        public List<Tuple<string, string>> ParseFeed(string xml_code)
        {
            List<Tuple<string, string>> lista = new List<Tuple<string, string>>();
            Regex multisearch = new Regex("(?i)(?s)<title>(.+?)</title>.+?<link>(.+?)</link>");
            MatchCollection finds = multisearch.Matches(xml_code);
            if (finds.Count > 0)
            {
                foreach (Match info in finds)
                {
                    string title = WebUtility.HtmlDecode(info.Groups[1].Captures[0].Value);
                    string link = info.Groups[2].Captures[0].Value;
                    lista.Add(new Tuple<string, string>(title, link));
                }
                //Delete first element wich is site URL
                lista.RemoveAt(0);
            }
            return lista;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                string resource_url = listView1.SelectedItems[0].SubItems[1].Text;
                System.Diagnostics.Process.Start(resource_url);
            }
        }
    }
}
