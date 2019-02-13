using Kino.Entity;
using NHibernate.Mapping.Attributes;
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
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate;

namespace Kino
{
    public partial class AccountInfo : Form
    {
        //inicializacije seje
        private ISessionFactory mSessionFactory = null;
        private ISession mSession = null;


        public AccountInfo()
        {
            InitializeComponent();
            ConfigureLog4Net();
            ConfigureNHibernate();
            //iskanje emaila trenutno prijavljene osebe
            string path = AppDomain.CurrentDomain.BaseDirectory + "session.txt";
            string ssName = File.ReadAllText(path);
            //query s pogojem za zaposlenega
            IQuery query = mSession.CreateQuery("from Employees d where d.email = ?");
            Employees employee = query.SetString(0, ssName).UniqueResult<Employees>();

            //napolnimo polja preko tabele iz baze
            textBox1.Text = employee.email;
            //textBox2.Text = employee.pass;
            //flush session, zaradi kasnejse uporabe sessiona in transactiona
            mSession.Flush();
        }

        private void ConfigureNHibernate()
        {
            Configuration config = new Configuration();
            config.Configure();
            HbmSerializer.Default.Validate = true;
            config.AddInputStream(HbmSerializer.Default.Serialize(System.Reflection.Assembly.GetExecutingAssembly()));
            config.AddAssembly(typeof(Employees).Assembly);
            //schema update namesto schema export in bool za brisanje -- da ne izgubimo celotne baze
            new SchemaUpdate(config).Execute(true, true);
            mSessionFactory = config.BuildSessionFactory();
            mSession = mSessionFactory.OpenSession();

        }

        private void ConfigureLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }



        //metoda za nastavljanje nhibernate - da lahko odpremo formo preko druge forme
        public void SetNhib(ISessionFactory isf, ISession iss)
        {
            mSessionFactory = isf;
            mSession = iss;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // polja ne smejo biti prazna
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please enter email");
            }//if
            else if (textBox2.Text == "")
            {
                MessageBox.Show("Please enter new password");
            }//else if
            // če so polja izpolnjena
            else
            {
                //iskanje trenutnega uporabnika
                string path = AppDomain.CurrentDomain.BaseDirectory + "session.txt";
                string usName = File.ReadAllText(path);
                //query s pogojem
                IQuery query = mSession.CreateQuery("from Employees d where d.email = ?");
                Employees employee = query.SetString(0, usName).UniqueResult<Employees>();
                //password v bazi
                string password = employee.pass;
                if (textBox2.Text == textBox3.Text)
                {
                    if (textBox4.Text == password)
                    {
                        //update
                        ITransaction tx = mSession.BeginTransaction();
                        string updateQuery = "UPDATE Employees e SET e.email ='" + textBox1.Text + "',e.pass ='" + textBox2.Text + "' WHERE e.email ='" + usName + "'";
                        mSession.CreateSQLQuery(updateQuery).ExecuteUpdate();
                        tx.Commit();
                        mSession.Close();
                        if (MessageBox.Show("User info changed sucessfully!. Application will now exit, you can login with new login details", "Success message", MessageBoxButtons.OK) == DialogResult.OK)
                        {
                            this.Close();
                            Application.Exit();
                        }//if
                    }//if preverjanje s bazo
                    //če geslo za preverjanje ni isto 
                    else if (textBox4.Text != password)
                    {
                        MessageBox.Show("Your old password is incorect");
                    }//else if
                    //prikaz message da smo uspešno spremenili polja, ko kliknemo ok se aplikacija zapre
                    /*if(MessageBox.Show("User info changed sucessfully!. Application will now exit, you can login with new login details", "Success message", MessageBoxButtons.OK) == DialogResult.OK)
                    {
                        this.Close();
                    }//if*/
                    //Application.Exit();
                }//preverjanje passa če sta ista
            }//else
        }
    }
}
