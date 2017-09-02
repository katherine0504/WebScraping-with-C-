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
        }

        private async Task<int> ProductionRecord (string url, Resume[] resume) {
            var doc = await Task.Factory.StartNew(() => client.Load(url));
 
            var dateNodes = doc.DocumentNode.SelectNodes("//*[@id=\"tableSort\"]//tr/td[1]");
            var typeNodes = doc.DocumentNode.SelectNodes("//*[@id=\"tableSort\"]//tr/td[2]");
            var contentNodes = doc.DocumentNode.SelectNodes("//*[@id=\"tableSort\"]//tr/td[3]");
            var refNodes = doc.DocumentNode.SelectNodes("//*[@id=\"tableSort\"]//tr//td[4]");

            if (dateNodes == null || typeNodes == null || contentNodes == null) {
                return -1;
            }

            var innerDate = dateNodes.Select(node => node.InnerText).ToList();
            var innerTypes = typeNodes.Select(node => node.InnerText).ToList();
            var innerContent = contentNodes.Select(node => node.InnerText).ToList();
            var innerRef = refNodes.Select(node => node.InnerText).ToList();

            int cnt = innerDate.Count();

            for (int i = 0; i < innerDate.Count(); ++i) {
                resume[i].Date = innerDate[i];
                resume[i].Type = innerTypes[i];
                resume[i].Content = innerContent[i];
                resume[i].Ref = innerRef[i];
            }

            return cnt;
        }

        private async Task<Boolean> FarmRecord (string url, Product[] pro)
        {
            var doc = await Task.Factory.StartNew(() => client.Load(url));

            var companyName = doc.GetElementbyId("ctl00_ContentPlaceHolder1_Producer").InnerText;
            var companyShort = doc.GetElementbyId("ctl00_ContentPlaceHolder1_Producer").InnerText;
            var Farmer = doc.GetElementbyId("ctl00_ContentPlaceHolder1_FarmerName").InnerText;
            var productName = doc.GetElementbyId("ctl00_ContentPlaceHolder1_ProductName").InnerText;
            var origin = doc.GetElementbyId("ctl00_ContentPlaceHolder1_Place").InnerText;
            var packedDate = doc.GetElementbyId("ctl00_ContentPlaceHolder1_PackDate").InnerText;
            var varifiedCompany = doc.GetElementbyId("ctl00_ContentPlaceHolder1_ao_name").InnerText;

            if (companyName == null) {
                return false;
            }

            pro[0].Company = companyName;
            pro[0].CompanyShort = companyShort;
            pro[0].Farmer = Farmer;
            pro[0].ProductName = productName;
            pro[0].Origin = origin;
            pro[0].PackedDate = packedDate;
            pro[0].VarifiedCompany = varifiedCompany;

            return true;
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
            Product[] ProductInfo = new Product[1];
            Resume[] ProductResume = new Resume[50];

            var resumecnt = await ProductionRecord(url, ProductResume);
            
            if (resumecnt != -1) {
                for (int i = 0; i < resumecnt; ++i) {
                    Console.WriteLine(ProductResume[i].Date);
                    Console.WriteLine(ProductResume[i].Type);
                    Console.WriteLine(ProductResume[i].Content);
                    Console.WriteLine(ProductResume[i].Ref);
                    Console.WriteLine("****************");
                }
            } else {
                Console.WriteLine("Errors in parsing records");
            }

            var farmresults = await FarmRecord(url, ProductInfo);

            if (farmresults) {
                Console.WriteLine(ProductInfo[0].Company);
                Console.WriteLine(ProductInfo[0].CompanyShort);
                Console.WriteLine(ProductInfo[0].Farmer);
                Console.WriteLine(ProductInfo[0].Origin);
                Console.WriteLine(ProductInfo[0].PackedDate);
                Console.WriteLine(ProductInfo[0].ProductName);
                Console.WriteLine(ProductInfo[0].VarifiedCompany);
            } else {
                Console.WriteLine("Errors in parsing farmresults");
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
