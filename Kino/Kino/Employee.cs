using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Kino.Entity;
using NHibernate;
using System.Xml.Serialization;
using System.IO;

namespace Kino
{
    public partial class Employee : Form
    {
        private ISession mSession = null;
        private ISessionFactory mSessionFactory = null;
        private IList<Employees> employeesList;
        private BindingSource bs;
        /*  MySqlConnection con = new MySqlConnection();
          MySqlCommand cmd;*/
        private AccountInfo fAcInfo;
        private string currEmploye;
        private string currEmploye2;

        public Employee()
        {
            InitializeComponent();
            //dovolimo dodajanje in brisanje 
            dataEmployees.AllowUserToAddRows = true;
            dataEmployees.AllowUserToDeleteRows = true;
        }
        //nastavljanje nhiberate, da formo zaženemo preko druge forme
        public void SetNhib(ISessionFactory isf, ISession iss)
        {
            mSessionFactory = isf;
            mSession = iss;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //dataGrid ko odpremo
        private void Employee_Load(object sender, EventArgs e)
        {
            /*try
            {

                con.ConnectionString = "Server=localhost;Database=cinema;User ID=root;Password=;Port=3306";
                con.Open();
                MessageBox.Show("connected to database");
            }
            catch (Exception e1)
            {
                MessageBox.Show("connection falied " + e1.ToString());
            }*/

            employeesList = mSession.CreateCriteria(typeof(Employees)).List<Employees>();
            bs = new BindingSource();
            bs.DataSource = employeesList;
            bs.AllowNew = true;
            dataEmployees.DataSource = bs;
            bs.ListChanged += new System.ComponentModel.ListChangedEventHandler(bindingSource1_ListChanged);

        }

        //brisanje polj
        private void dataEmployees_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            using (ITransaction tx = mSession.BeginTransaction())
            {
                Employees newEmployee = (Employees)(e.Row.DataBoundItem);

                mSession.Delete(newEmployee);
                tx.Commit();
            }


        }

        private void bindingSource1_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    {
                        using (ITransaction tx = mSession.BeginTransaction())
                        {
                            Employees newEmployee = (Employees)(bs.List[e.NewIndex]);
                            if (newEmployee.email == null)
                                newEmployee.email = "";
                            if (newEmployee.pass == null)
                                newEmployee.pass = "";
                            mSession.Save(newEmployee);
                            tx.Commit();
                        }
                        break;
                    }//case
            }//switch
        }

        //brisanje polj


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void accountInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAcInfo = new AccountInfo();
            fAcInfo.SetNhib(mSessionFactory, mSession);
            fAcInfo.ShowDialog();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void addMovieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fMovieList = new Movies();
            fMovieList.SetNhib(mSessionFactory, mSession); // posreduje povezavo z bazo
            fMovieList.ShowDialog();
        }

        private void addMovieToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var fMovieList = new Movies();
            fMovieList.SetNhib(mSessionFactory, mSession); // posreduje povezavo z bazo
            fMovieList.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            List<string> employees = new List<string>();
            //polnjenje seznama
            foreach (DataGridViewRow row in dataEmployees.Rows)
            {
                currEmploye += row.Cells["email"].Value;
                currEmploye2 += row.Cells["pass"].Value;
                employees.Add(currEmploye);
                employees.Add(currEmploye2);
            }//foreach
            var ser = new XmlSerializer(typeof(List<string>));
            TextWriter writer = new StreamWriter(@"E:\Employees.xml");
            // o is List<place> here
            ser.Serialize(writer, employees);
        }
        // izbris zaposlenega
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataEmployees.SelectedRows.Count > 0)
            {
                int selectedIndex = dataEmployees.SelectedRows[0].Index;
                //
                int rowID = int.Parse(dataEmployees[0, selectedIndex].Value.ToString());
                dataEmployees.AllowUserToDeleteRows = true;
                string query = "DELETE FROM employees WHERE ID_employee =" + rowID;
                mSession.CreateSQLQuery(query).ExecuteUpdate();
                MessageBox.Show("Employee deleted");
            }//if
        }
    }
}
