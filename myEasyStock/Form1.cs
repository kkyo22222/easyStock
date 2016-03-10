using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using System.Management;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace myEasyStock
{
   


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<string> myStringLists = new List<string>();
        List<string> nowStringLists = new List<string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            
            if (File.Exists("List.txt"))
            {
                StreamReader sr = new StreamReader("List.txt");
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    myStringLists.Add(line);
                }
                sr.Close();
            }
            foreach (string StringList in myStringLists)
            {
                dataGridView2.Rows.Add(new object[] { StringList});              
            }
            


            
        }
       
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            WebClient url = new WebClient();
            //將網頁來源資料暫存到記憶體內
            dataGridView1.Rows.Clear();

            for (int i = 0; i < nowStringLists.Count; i++)
            {
                string requestHtml = "http://tw.stock.yahoo.com/q/q?s=" +nowStringLists[i];
                MemoryStream ms = new MemoryStream(url.DownloadData(requestHtml));
                // 使用預設編碼讀入 HTML 
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(ms, Encoding.Default);
                // 裝載第一層查詢結果 
                HtmlAgilityPack.HtmlDocument hdc = new HtmlAgilityPack.HtmlDocument();
                //XPath 來解讀它 /html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1] 
                hdc.LoadHtml(doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1]").InnerHtml);

                // 取得個股標頭 
                HtmlNodeCollection htnode = hdc.DocumentNode.SelectNodes("./tr[1]/th");
                // 取得個股數值 
                string[] txt = hdc.DocumentNode.SelectSingleNode("./tr[2]").InnerText.Trim().Split('\n');
                int j = 0;
                foreach (HtmlNode nodeHeader in htnode)
                {
                    txt[j] = txt[j].Trim().Replace("加到投資組合", "");
                    j++;
                }

                // 輸出資料 

                dataGridView1.Rows.Add(new object[] { txt[0], txt[2], txt[1] });


                //清除資料
                doc = null;
                hdc = null;
                //url = null;
                ms.Close();
            }
            
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1_Tick(null, null);
        }

        private void saveListButton_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("List.txt");
            foreach (string StringList in nowStringLists)
            {
                sw.WriteLine(StringList);
            }
            sw.Close();
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            nowStringLists.Clear();
            for(int i =0 ;i<dataGridView2.RowCount-1;i++){
                nowStringLists.Add(dataGridView2[0, i].Value.ToString());
            }
            
        }  
       

        
    }
}
