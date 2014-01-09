using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFiLoc_App
{
    class DBAccess
    {
        public static void addNewLuogo(string nome) {
            WiFiLoc_Service.Luogo l = new WiFiLoc_Service.Luogo(nome);
            l.luogoToDB();
        }

        public static ArrayList getLuoghi() {
            return WiFiLoc_Service.Luogo.getPossibiliLuoghi();
        }
    }
}
