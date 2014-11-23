using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace JsonDataGridView
{
    public partial class Form1 : Form
    {
        private static JObject _cacheJObject;
        public Form1()
        {
            InitializeComponent();
            btnLoad.Enabled = false;
            chkCache.CheckedChanged += chkCache_click;
            dataGridView1.CellContentDoubleClick += dataGridView1_CellContentDoubleClick;
        }

        private IList<dynamic> GetDataSources(JObject jsonObj = null)
        {
            if (_cacheJObject != null)
            {
                var lista = NavigateObectoJson(txtListField.Text.Trim(), _cacheJObject);
                return lista.ToList<dynamic>();
            }
            else
            {
                var lista = NavigateObectoJson(txtListField.Text.Trim(), jsonObj);
                return lista.ToList<dynamic>();
            }
        }

        private async void btnProc_Click(object sender, EventArgs e)
        {
            var url = txtUrl.Text.Trim();
            var jsonString = await HttpGetAsync(url);
            var jsonObj = JObject.Parse(jsonString);
            if (chkCache.Checked)
            {
                _cacheJObject = jsonObj;
            }

            dataGridView1.DataSource = GetDataSources(jsonObj);
        }

        private void dataGridView1_CellContentDoubleClick(Object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            var cellText = senderGrid.Rows[e.RowIndex].DataBoundItem.ToString();
            var jsonObj = JObject.Parse(cellText);
            var dataSource = jsonObj.ToList<dynamic>();
            dataGridView1.DataSource = dataSource;
        }

        private static JArray NavigateObectoJson(string nodesName, JObject obj)
        {
            try
            {
                var parseNode = nodesName.Split('.');
                if (parseNode.Count() <= 1) return (JArray) obj[nodesName];
                var nObj = (JObject)obj[parseNode[0]];
                parseNode = parseNode.RemoveAt(0);
                return NavigateObectoJson(String.Join(".", parseNode), nObj);
            }
            catch (Exception)
            {
                MessageBox.Show("No se encuentran elementos de tipo lista.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public async Task<string> HttpGetAsync(string uri)
        {
            try
            {
                var headerAouth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("app.bbva.tulio1:f0cad0c2980da4317d614f002ff08ad7f65eec30"));
                var hc = new HttpClient();
                hc.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", headerAouth);
                var result = hc.GetStreamAsync(uri);

                var vs = await result;
                var am = new StreamReader(vs);

                return await am.ReadToEndAsync();
            }
            catch (WebException ex)
            {
                switch (ex.Status)
                {
                    case WebExceptionStatus.NameResolutionFailure:
                        MessageBox.Show("domain_not_found", "ERROR",MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    //Catch other exceptions here
                }
            }
            return null;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = GetDataSources();
        }

        private void chkCache_click(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            btnLoad.Enabled = chk.Checked;
        }
    }
}
