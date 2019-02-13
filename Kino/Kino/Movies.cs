using Kino.Entity;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.Attributes;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

using System.Xml;
using System.Xml.Serialization;

namespace Kino
{
    public partial class Movies : Form
    {
        //inicializacije seje
        private ISessionFactory mSessionFactory = null;
        private ISession mSession = null;
        private IList<Movie> movieList;
        private BindingSource bs;
        //binding source in list za prikaz filmov v listu    
        public List<Movie> movieData = new List<Movie>();
        public BindingSource movieBinding = new BindingSource();
        //inicializacija oken
        private AccountInfo fAcInfo;
        private string currMovie;
        private object o;
        private string currMovie2;


        //public Login fLoginCount;

        public Movies()
        {
            InitializeComponent();
            ConfigureLog4Net();
            ConfigureNHibernate();
            LoginUser();
            // InitItems();
            //prikaz filmov
            movieBinding.DataSource = movieList; 
            dataMovies.AllowUserToAddRows = true;
            dataMovies.AllowUserToDeleteRows = true;

        }

        //prijava
        private void LoginUser()
        {
            //dovolimo 2 napačni prijavi textBox1 = username, textBox2 = password
            int count = 2;
            var fLoginCount = new Login();
            if (fLoginCount.IsDisposed)
            {
                fLoginCount = new Login();
            }//if
            while (count > 0)
            {
                if (fLoginCount.ShowDialog() == DialogResult.OK)
                {
                    if (CheckUser(fLoginCount.Controls["textBox1"].Text, (fLoginCount.Controls["textBox2"].Text)))
                    {
                        count = -1;
                        this.Text = "user:" + fLoginCount.Controls["textBox1"].Text;
                    }//if
                    else
                    {
                        MessageBox.Show("Wrong entry");
                        count--;
                    }//else
                }//if
            }//while
            fLoginCount.Dispose();
            if (count == 0)
            {
                System.Environment.Exit(0);
            }//if
        }
        //preverimo če so podatki pravilni
        private bool CheckUser(string email, string pass)
        {
            bool result = false;
            try
            {
                ITransaction tx = mSession.BeginTransaction();
                IQuery query = mSession.CreateQuery("from Employees d where d.email = ?");
                Employees res = query.SetString(0, email).UniqueResult<Employees>();
                tx.Commit();
                if (res != null && res.pass.Equals(pass))
                {
                    result = true;
                }//if
            }//try
            catch (Exception e1)
            {
                ResetSession();
            }//catch
            return result;
        }

        private void ResetSession()
        {
            mSession.Close();
            mSession.Dispose();
            mSession = mSessionFactory.OpenSession();
        }


        private void ConfigureNHibernate()
        {
            Configuration config = new Configuration();
            config.Configure();
            HbmSerializer.Default.Validate = true;
            config.AddInputStream(HbmSerializer.Default.Serialize(System.Reflection.Assembly.GetExecutingAssembly()));
            config.AddAssembly(typeof(Movie).Assembly);
            //posodobi namesto ustvari, da ne potrebujemo zakomentirati kode -- v nasprotnem primeru bi izgubili celotno bazo
            //torej SchemaUpdate namesto bool bDrop in SchemaExport
            new SchemaUpdate(config).Execute(true, true);
            mSessionFactory = config.BuildSessionFactory();
            mSession = mSessionFactory.OpenSession();
        }

        private void ConfigureLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        public void SetNhib(ISessionFactory isf, ISession iss)
        {
            mSessionFactory = isf;
            mSession = iss;
        }

        private void dataMovies2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        
        //odjava
        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //spremeni podatke
        private void accountChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAcInfo = new AccountInfo();
            fAcInfo.SetNhib(mSessionFactory, mSession);
            fAcInfo.ShowDialog();
        }
        //seznam filmov
        private void Movies_Load(object sender, EventArgs e)
        {
            movieList = mSession.CreateCriteria(typeof(Movie)).List<Movie>();
            dataMovies.ReadOnly = false;
            bs = new BindingSource();
            bs.DataSource = movieList;

            bs.AllowNew = true;
            //   bs.AllowRemove = true;
            dataMovies.DataSource = bs;
            bs.ListChanged += new System.ComponentModel.ListChangedEventHandler(bindingSource1_ListChanged);
        }

        private void bindingSource1_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    {
                        using (ITransaction tx = mSession.BeginTransaction())
                        {
                            Movie newMovie = (Movie)(bs.List[e.NewIndex]);
                            if (newMovie.movie_name == null)
                                newMovie.movie_name = "";
                            if (newMovie.visitors == null)
                                newMovie.visitors = "";
                            mSession.Save(newMovie);
                            tx.Commit();
                        }
                        break;
                    }//case
            }//switch
        }

        // prikaz okna s seznamom zaposlenih
        private void employeesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fEmployeeList = new Employee();
            fEmployeeList.SetNhib(mSessionFactory, mSession); // posreduje povezavo z bazo
            fEmployeeList.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Movies_Load_1(object sender, EventArgs e)
        {

        }

        private void employeesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var fEmployeeList = new Employee();
            fEmployeeList.SetNhib(mSessionFactory, mSession); // posreduje povezavo z bazo
            fEmployeeList.ShowDialog();
        }
        //xml 
        private void button2_Click_1(object sender, EventArgs e)
        {
            List<string> movies = new List<string>();
            //polnjenje seznama
            foreach (DataGridViewRow row in dataMovies.Rows)
            {
                currMovie += row.Cells["movie_name"].Value;
                currMovie2 += row.Cells["visitors"].Value;
                movies.Add(currMovie);
                movies.Add(currMovie2);
            }//foreach
            var ser = new XmlSerializer(typeof(List<string>));
            TextWriter writer = new StreamWriter(@"E:\Movie.xml");
            // o is List<place> here
            ser.Serialize(writer, movies);

            /*try           {
                XmlReader xmlFile;
                xmlFile = XmlReader.Create("Movie.xml", new XmlReaderSettings());
                DataSet ds = new DataSet();
                ds.ReadXml(xmlFile);
                dataMovies.DataSource = movies;
            }//try
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }//catch*/
        }
        //delete
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataMovies.SelectedRows.Count > 0)
            {
                int selectedIndex = dataMovies.SelectedRows[0].Index;
                // dobimo rowID iz prve vrstice označene vrstice
                int rowID = int.Parse(dataMovies[0, selectedIndex].Value.ToString());
                dataMovies.AllowUserToDeleteRows = true;
                string query = "DELETE FROM movie WHERE ID_movie =" + rowID;
                mSession.CreateSQLQuery(query).ExecuteUpdate();
                MessageBox.Show("Movie deleted");                
            }//if
        }
    }

}
