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
using Kino.Entity;
using NHibernate;

namespace Kino
{
    public partial class Register : Form
    {
        //inicializacije seje
        private ISessionFactory mSessionFactory = null;
        private ISession mSession = null;
        public Register()
        {
            InitializeComponent();
            ConfigureLog4Net();
            ConfigureNHibernate();
        }

        private void ConfigureNHibernate()
        {
            Configuration config = new Configuration();
            config.Configure();
            HbmSerializer.Default.Validate = true;
            config.AddInputStream(HbmSerializer.Default.Serialize(System.Reflection.Assembly.GetExecutingAssembly()));
            config.AddAssembly(typeof(Employees).Assembly);
            //posodobi namesto ustvari, da ne potrebujemo zakomentirati kode -- v nasprotnem primeru bi izgubili celotno bazo
            //torej SchemaUpdate namesto bool bDrop in SchemaExport
            new SchemaUpdate(config).Execute(true, true);
            mSessionFactory = config.BuildSessionFactory();
            mSession = mSessionFactory.OpenSession();
        }
        public void SetNhib(ISessionFactory isf, ISession iss)
        {
            mSessionFactory = isf;
            mSession = iss;
        }

        private void ConfigureLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //mormo izpolnat vsa polja ker imam not null vključen
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please enter your email", "Error message");
            }//if
            else if (textBox2.Text == "")
            {
                MessageBox.Show("Please enter your password", "Error message");
            }//if
            else if (textBox2.Text != textBox3.Text)
            {
                MessageBox.Show("Passwords don't match!", "Error message");
            }//if
            //če dela
            else
            {
                using (var mSession = mSessionFactory.OpenSession())
                {
                    using (var tx = mSession.BeginTransaction())
                    {
                        //dodajanje zaposlenega
                        Employees employee = new Employees();
                        employee.email = textBox1.Text;
                        employee.pass = textBox2.Text;
                        try
                        {
                            //shranimo v bazo
                            mSession.Save(employee);
                            tx.Commit();
                        }//try
                        catch (Exception e1)
                        {
                            tx.Rollback();
                            MessageBox.Show(e1.Message, "Exception message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }//catch
                    }//using
                }//using
                if (MessageBox.Show("User created sucessfully!", "Success message", MessageBoxButtons.OK) == DialogResult.OK) ;

            }//else
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
