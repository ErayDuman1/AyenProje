using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyenProje
{
    public class AyenUrunler
    {
        public List<Urun> Urunler { get; set; }
    }
    public class Urun
    {
        public string UrunKodu { get; set; }

        public string UrunAdi { get; set; }

        public string Marka { get; set; }
        public string Stok { get; set; }
        public List<Varyant> Varyantlar { get; set; }
    }

    public class Varyant
    {
        public string Sku { get; set; }
        public string Stok { get; set; }
        public string Fiyat { get; set; }
        public List<Ozellik> Ozellikler { get; set; }
    }
    public class Ozellik
    {
        public string Ad { get; set; }

        public string Deger { get; set; }
    }
}
