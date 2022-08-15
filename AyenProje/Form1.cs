using FileHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Documents;
using System.Windows.Forms;

namespace AyenProje
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        JArray response = new JArray();
        private void btnIndir_Click(object sender, EventArgs e)
        {
            using (var client = new System.Net.WebClient())
            {
                
                string json="";
                try
                {
                     json = client.DownloadString(txtLink.Text.ToString());
                     response = JArray.Parse(json);
                    int i = 0;
                    List<string> list = new List<string>();
                    foreach (var item in response)
                    {
                        if (i == 0)
                        {
                            TableRow thead = new TableRow();
                            foreach (var ss in item)
                            {
                                list.Add(ss.Path.ToString());
                                string y = list.Last();
                                CheckBox box;
                                box = new CheckBox();
                                box.Text = y;
                                box.Name = "chk"+i;
                                box.AutoSize = true;
                                box.Location = new Point(540, list.Count() * 30);
                                this.Controls.Add(box);
                                i++;
                            }
                        }

                    }
                }
                catch (Exception ex)
                { MessageBox.Show(ex.Message); }
            }
        }
        private void btnGrup_Click(object sender, EventArgs e)
        {
            
            List<string> list = new List<string>();
            var cbs = this.Controls.OfType<CheckBox>().Where(cb => cb.Checked);
            foreach (CheckBox cb in cbs)
            {
                var afterJumps = cb.Text.Split(new[] { "[0]." }, StringSplitOptions.None)[1];
                list.Add(afterJumps);
            }

            /*var data = response.Where(x => x["Brand"].ToString() != "Adidas" && x["Brand"].ToString() != "adidas").GroupBy(
                    p => p[list[1]],
                    p => p[list[0]],
                    (key, g) => new { Urun =g, Renk = key });*/
            var data = from urun in response where urun["Brand"].ToString()!="adidas" where urun["Brand"].ToString()!="Adidas"
                       group urun by urun[list[1]] into Grup
                        select new
                        {
                            Urun = Grup,
                            Renk = Grup.Key
                        };

            foreach (var urunler in data)
            {
                foreach (var item in urunler.Urun)
                {
                    var Renk = item["Color"].ToString();
                    var Size = item["Size"].ToString();
                    var SKU = item["SKU"].ToString();

                    using (var csvRead = new StreamReader(@"C:\Users\Eray DUMAN\Desktop\stock_prices.csv"))
                    {
                        int i = 0;
                        List<string> SKUList = new List<string>();
                        List<string> StokList = new List<string>();
                        List<string> FiyatList = new List<string>();
                        while (!csvRead.EndOfStream)
                        {
                            var line = csvRead.ReadLine();
                            var values = line.Split(';');
                            if(i>=1)
                            {
                                SKUList.Add(values[0]);
                                StokList.Add(values[1]);
                                FiyatList.Add(values[2]);
                            }
                            i++;
                        }
                        AyenUrunler ayenUrun = new AyenUrunler();
                        ayenUrun.Urunler = new List<Urun>();
                        Urun urun = new Urun();
                        urun.Varyantlar = new List<Varyant>();
                        Varyant varyant = new Varyant();
                        varyant.Ozellikler = new List<Ozellik>();
                        int index = SKUList.FindIndex(a => a == SKU);
                        if (index!=-1)
                        {
                            ayenUrun.Urunler.Add(new Urun { UrunKodu =item["ProductCode"].ToString(),UrunAdi=item["Name"].ToString(),Marka=item["Brand"].ToString(),Stok=item.Count().ToString() });
                            urun.Varyantlar.Add(new Varyant { Sku = item["SKU"].ToString(), Fiyat = FiyatList[index], Stok = StokList[index] });
                        }
                        varyant.Ozellikler.Add(new Ozellik { Ad = "Size", Deger = Size });
                        varyant.Ozellikler.Add(new Ozellik { Ad = "Color", Deger = Renk });
                        var jsonSerializerSettings = new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.All
                        };
                        var json = JsonConvert.SerializeObject(ayenUrun.Urunler[0], jsonSerializerSettings);
                        var json1 = JsonConvert.SerializeObject(urun.Varyantlar[0], jsonSerializerSettings);
                        var json2 = JsonConvert.SerializeObject(varyant.Ozellikler[0], jsonSerializerSettings);
                        var jObject1 = JObject.Parse(json);
                        var jObject2 = JObject.Parse(json1);
                        var jObject3 = JObject.Parse(json2);
                        var result = new JObject();

                        result.Merge(jObject1);
                        result.Merge(jObject2);
                        result.Merge(jObject3);
                        File.AppendAllText(@"C:\Users\Eray DUMAN\Desktop\AyenUrunler.json", result.ToString());
                    }
                }

            } 
        }

    }
}
