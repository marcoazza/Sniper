using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SqlServerCe;


namespace WiFiLoc_App
{
    class DBconnection
    {
        public static SqlCeConnection getDBConnection(){
                    string dbfile = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\dbLocale.sdf";
                    SqlCeConnection connection = new SqlCeConnection("datasource=" + dbfile);
                   // SqlConnection myConnection = new SqlConnection("server=PAOLOPC;Integrated Security=SSPI;database=LocalAppDB");
                    return connection;
        }

    }
}
