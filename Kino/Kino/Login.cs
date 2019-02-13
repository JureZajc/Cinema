using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NHibernate.Cfg;
using NHibernate.Mapping.Attributes;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using Kino.Entity;
using System.IO;

namespace Kino
{
    public partial class Login : Form
    {
        // inicializacija seje
        private ISessionFactory mSessionFactory = null;
        private ISession mSession = null;
        //če rabmo novga uporabnika
        private Register fRegister;

        public Login()
        {
            InitializeComponent();
            ConfigureLog4Net();
            ConfigureNHibernate();

        }
        //nhibernate konfigruacija
        private void ConfigureNHibernate()
        {
            Configuration config = new Configuration();
            config.Configure();
            HbmSerializer.Default.Validate = true;
            config.AddInputStream(HbmSerializer.Default.Serialize(System.Reflection.Assembly.GetExecutingAssembly()));
            config.AddAssembly(typeof(Employees).Assembly);
            //schema update namesto export in bool za brisanje baze - ni potrebno zakomentirati ob naslednji uporabi
            new SchemaUpdate(config).Execute(true, true);
            mSessionFactory = config.BuildSessionFactory();
            mSession = mSessionFactory.OpenSession();
        }
        //log4net konfiguracija
        private void ConfigureLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        // zapremo aplikacijo če pritisnemo na tipko close
        private void button2_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        //preusmeritev na registracijo
        private void button3_Click(object sender, EventArgs e)
        {
            fRegister = new Register();
            fRegister.SetNhib(mSessionFactory, mSession);
            fRegister.ShowDialog();
        }
        //shranimo uporabnika v session.txt
        private void button1_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "session.txt";
            File.WriteAllText(path, textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
