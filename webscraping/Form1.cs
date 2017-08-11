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
        DataTable table;
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
        }

        private async Task<List<Resume>> DetailsFromPage () {
            var doc = await Task.Factory.StartNew(() => client.Load("https://taft.coa.gov.tw/rsm/Code_cp.aspx?ID=1527616&EnTraceCode=10608110970"));
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

        private async void Form1_Load(object sender, EventArgs e) {
            var infos = await DetailsFromPage();

            foreach (var info in infos) {
                table.Rows.Add(info.Date, info.Type, info.Content, info.Ref);
            }
        }
    }
}
