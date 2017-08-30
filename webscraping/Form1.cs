using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace webscraping {
    public partial class Form1 : Form {
        DataTable table, table2;
        HtmlWeb client = new HtmlWeb();

        public Form1() {
            InitializeComponent();
            InitTable();
        }

        private void InitTable() {
            table = new DataTable("Record");
            table.Columns.Add("作業日期", typeof(string));
            table.Columns.Add("作業種類", typeof(string));
            table.Columns.Add("作業內容", typeof(string));
            table.Columns.Add("備註說明", typeof(string));
            dataGridView1.DataSource = table;

            table2 = new DataTable("Product");
            table2.Columns.Add("農產品經營業者", typeof(string));
            table2.Columns.Add("簡稱", typeof(string));
            table2.Columns.Add("生產者姓名", typeof(string));
            table2.Columns.Add("產品名稱", typeof(string));
            table2.Columns.Add("產地", typeof(string));
            table2.Columns.Add("包裝日期", typeof(string));
            table2.Columns.Add("驗證機構", typeof(string));
            dataGridView2.DataSource = table2;
        }

        private async Task<List<Resume>> ProductionRecord ( string url ) {
            var doc = await Task.Factory.StartNew(() => client.Load(url));
 
            var dateNodes = doc.DocumentNode.SelectNodes("//*[@id=\"tableSort\"]//tr/td[1]");
            var typeNodes = doc.DocumentNode.SelectNodes("//*[@id=\"tableSort\"]//tr/td[2]");
            var contentNodes = doc.DocumentNode.SelectNodes("//*[@id=\"tableSort\"]//tr/td[3]");
            var refNodes = doc.DocumentNode.SelectNodes("//*[@id=\"tableSort\"]//tr//td[4]");

            if (dateNodes == null || typeNodes == null || contentNodes == null) {
                return new List<Resume>();
            }

            var innerDate = dateNodes.Select(node => node.InnerText).ToList();
            var innerTypes = typeNodes.Select(node => node.InnerText).ToList();
            var innerContent = contentNodes.Select(node => node.InnerText).ToList();
            var innerRef = refNodes.Select(node => node.InnerText).ToList();

            List<Resume> toReturn = new List<Resume>();

            for (int i = 0; i < innerDate.Count(); ++i) {
                toReturn.Add(new Resume() { Date = innerDate[i], Type = innerTypes[i], Content = innerContent[i], Ref = innerRef[i] });
            }

            return toReturn;
        }

        private async Task<List<Product>> FarmRecord (string url)
        {
            var doc = await Task.Factory.StartNew(() => client.Load(url));
            Console.WriteLine("DOC Name");
            Console.WriteLine(doc);

            var companyName = doc.GetElementbyId("ctl00_ContentPlaceHolder1_Producer").InnerText;
            var companyShort = doc.GetElementbyId("ctl00_ContentPlaceHolder1_Producer").InnerText;
            var Farmer = doc.GetElementbyId("ctl00_ContentPlaceHolder1_FarmerName").InnerText;
            var productName = doc.GetElementbyId("ctl00_ContentPlaceHolder1_ProductName").InnerText;
            var origin = doc.GetElementbyId("ctl00_ContentPlaceHolder1_Place").InnerText;
            var packedDate = doc.GetElementbyId("ctl00_ContentPlaceHolder1_PackDate").InnerText;
            var varifiedCompany = doc.GetElementbyId("ctl00_ContentPlaceHolder1_ao_name").InnerText;

            if (companyName == null) {
                return new List<Product>();
            }

            List<Product> toReturn = new List<Product>();
   
            toReturn.Add(new Product() { Company = companyName, CompanyShort = companyShort, Farmer = Farmer, ProductName = productName, Origin = origin, PackedDate = packedDate, VarifiedCompany = varifiedCompany });

            return toReturn;
        }

        private async Task<String> getFurtherInfo (string url, Recipe[] re)
        {
            var doc = await Task.Factory.StartNew(() => client.Load(url));
            string toReturn = doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_ProductName\"]/a").Attributes["href"].Value;

            re[0].dishName = doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[1]/p/a").InnerText.ToString();
            re[0].dishPhoto = doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[1]/div/a/img").Attributes["src"].Value;
            re[0].dishUrl = "https://taft.coa.gov.tw" +  doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[1]/p/a").Attributes["href"].Value;

            re[1].dishName = doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[2]/p/a").InnerText.ToString();
            re[1].dishPhoto = doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[2]/div/a/img").Attributes["src"].Value;
            re[1].dishUrl = "https://taft.coa.gov.tw" + doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[2]/p/a").Attributes["href"].Value;

            re[2].dishName = doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[3]/p/a").InnerText.ToString();
            re[2].dishPhoto = doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[3]/div/a/img").Attributes["src"].Value;
            re[2].dishUrl = "https://taft.coa.gov.tw" + doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[3]/p/a").Attributes["href"].Value;

            re[3].dishName = doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[4]/p/a").InnerText.ToString();
            re[3].dishPhoto = doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[4]/div/a/img").Attributes["src"].Value;
            re[3].dishUrl = "https://taft.coa.gov.tw" + doc.DocumentNode.SelectSingleNode("//*[@id=\"ctl00_ContentPlaceHolder1_RecommandDIV\"]/div/ul/li[4]/p/a").Attributes["href"].Value;

            return toReturn;
        }

        private async void Form1_Load(object sender, EventArgs e) {
            string url = "https://taft.coa.gov.tw/rsm/Code_cp.aspx?ID=1527616&EnTraceCode=10608110970";

            Recipe[] topRecipe = new Recipe[4];

            var productioninfos = await ProductionRecord(url);

            foreach (var info in productioninfos) {
                table.Rows.Add(info.Date, info.Type, info.Content, info.Ref);
            }

            var farmerinfos = await FarmRecord(url);

            foreach (var info in farmerinfos) {
                table2.Rows.Add(info.Company, info.CompanyShort, info.Farmer, info.ProductName, info.Origin, info.PackedDate, info.VarifiedCompany); 
            }

            var pedia = await getFurtherInfo(url, topRecipe);

            for (int i = 0; i < 4; ++i) {
                Console.WriteLine(topRecipe[i].dishName);
                Console.WriteLine(topRecipe[i].dishPhoto);
                Console.WriteLine(topRecipe[i].dishUrl);
                Console.WriteLine("------------------");
            }
            
        }
    }
}
