using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;

namespace WiFiLoc_App
{
    public class DBAccess
    {
        static private DBAccess instance = null ;

        private SqlCeConnection connection = null;

        private DBAccess() {
            string dbfile = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\dbLocale.sdf";
            connection = new SqlCeConnection("datasource=" + "c:\\Users\\SEVEN\\Desktop\\Sniper\\WiFiLoc_App\\bin\\Debug\\dbLocale.sdf");
        }

        public static DBAccess getInstance() {
            if (instance == null)
                instance = new DBAccess();
            return instance;
        }

        public SqlCeConnection getConnection(){
            return connection;
        }

        
    
    }

}
