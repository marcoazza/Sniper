using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections;
//using System.Windows;
using WiFiLoc_App;
using System.Data.SqlServerCe;



namespace WiFiLoc_Service
{
    public class Luogo
    {
        string _nomeLuogo;
        int _id;
        NetworkList netwlist;
        ActionList actionsList;
        static SqlCeConnection sc = DBAccess.getInstance().getConnection();



        public Luogo()
        {
            netwlist = new NetworkList();
            actionsList = new ActionList();
            Timestat = 0;

        }

        public Luogo(string luogo){
            netwlist = new NetworkList();
            actionsList = new ActionList();
            netwlist.acquireNetworkList();
            _nomeLuogo = luogo;
            Timestat = 0;
        }


        /// <summary>
        /// get or set place name
        /// </summary>
        public string NomeLuogo {
            get{ return _nomeLuogo;
                }
            set {
                _nomeLuogo = value;
            }
        }

        /// <summary>
        /// get or set place id
        /// </summary>
        public int Id {
        get {return _id; }
        set{_id = value; }
        }

        /// <summary>
        /// get or set times inside place
        /// </summary>
        public long Timestat { get; set; }

        /// <summary>
        /// get place network list
        /// </summary>
        public NetworkList NetwList {
            get { return netwlist; }
        }

        /// <summary>
        /// get place actions list
        /// </summary>
        public ActionList ActionsList { get { return actionsList; } }

        /// <summary>
        /// acquire and store scanned network
        /// </summary>
        public void saveNextList() { 
            netwlist = new NetworkList();
            netwlist.acquireNetworkList();
        }

        public override bool Equals(object obj)
        {
            Luogo l = (Luogo)obj;
            if (l == null)
                return false;
            return (this.Id == l.Id && this.NomeLuogo == l.NomeLuogo);
        }
        /// <summary>
        /// get from database the places already saved
        /// </summary>
        /// <returns> return ArrayList containing stored places, or an empty array if there are not places stored</returns>
        public static ArrayList getPossibiliLuoghi()
        {
            ArrayList luoghi= new ArrayList();
            sc.Open();

            LocalAppDBDataSet lds = new LocalAppDBDataSet();
            SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT * FROM Luogo", sc);
            SqlCeDataAdapter sdaS = new SqlCeDataAdapter("SELECT * FROM Segnale", sc);
            SqlCeDataAdapter sdaA = new SqlCeDataAdapter("SELECT * FROM Azione", sc);

            LocalAppDBDataSet.LuogoRow lr = lds.Luogo.NewLuogoRow();
            //command to generate basic queries (INSERT,UPDATE,DELETE)
            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);
            SqlCeCommandBuilder builderA = new SqlCeCommandBuilder(sdaA);

            sdaL.Fill(lds, "Luogo");
            sdaS.Fill(lds, "Segnale");
            sdaA.Fill(lds, "Azione");

            foreach (DataRow dr in lds.Luogo.Rows)
            {
                LocalAppDBDataSet.LuogoRow ldr = (LocalAppDBDataSet.LuogoRow) dr;
                Luogo possibile = new Luogo();
                possibile.Id = ldr.id;
                possibile.NomeLuogo = ldr.luogo;
                possibile.Timestat = ldr.timestat;

                //associate networks to Luogo
                foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Segnale"]))
                {

                    LocalAppDBDataSet.SegnaleRow scr = (LocalAppDBDataSet.SegnaleRow)cr;
                    Network n =new Network();
                    n.setNetwork(scr.mac,(int)scr.potenza);
                    possibile.netwlist.Hash.Add(n.Mac, n);
                }
                foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Azione"]))
                {

                    LocalAppDBDataSet.AzioneRow acr = (LocalAppDBDataSet.AzioneRow)cr;
                    
                    ActionList.Action a = new ActionList.Action(acr.azione);
                    possibile.actionsList.AddAction(a);
                }

                luoghi.Add(possibile);
            }
            sc.Close();

            return luoghi;

        }

        /// <summary>
        /// get from database specific place
        /// </summary>
        /// <param name="name">name of place to be retrived</param>
        /// <returns> return place</returns>

        public static Luogo getLuogo(string name)
        {
            ArrayList luoghi = new ArrayList();
            sc.Open();

            LocalAppDBDataSet lds = new LocalAppDBDataSet();
            SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT * FROM Luogo WHERE luogo='"+name+"'", sc);
            SqlCeDataAdapter sdaS = new SqlCeDataAdapter("SELECT * FROM Segnale", sc);
            SqlCeDataAdapter sdaA = new SqlCeDataAdapter("SELECT * FROM Azione", sc);

            LocalAppDBDataSet.LuogoRow lr = lds.Luogo.NewLuogoRow();
            //command to generate basic queries (INSERT,UPDATE,DELETE)
            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);
            SqlCeCommandBuilder builderA = new SqlCeCommandBuilder(sdaA);

            sdaL.Fill(lds, "Luogo");
            sdaS.Fill(lds, "Segnale");
            sdaA.Fill(lds, "Azione");
            if (lds.Luogo.Rows.Count == 0) {
                return null;
            } 
            DataRow dr = lds.Luogo.Rows[0];
           
                LocalAppDBDataSet.LuogoRow ldr = (LocalAppDBDataSet.LuogoRow)dr;
                
                Luogo possibile = new Luogo();
                possibile.Id = ldr.id;
                possibile.NomeLuogo = ldr.luogo;
                possibile.Timestat = ldr.timestat;

                //associate networks to Luogo
                foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Segnale"]))
                {

                    LocalAppDBDataSet.SegnaleRow scr = (LocalAppDBDataSet.SegnaleRow)cr;
                    Network n = new Network();
                    n.setNetwork(scr.mac, (int)scr.potenza);
                    possibile.netwlist.Hash.Add(n.Mac, n);
                }
                foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Azione"]))
                {

                    LocalAppDBDataSet.AzioneRow acr = (LocalAppDBDataSet.AzioneRow)cr;

                    ActionList.Action a = new ActionList.Action(acr.azione);
                    possibile.actionsList.AddAction(a);
                }


                sc.Close();
            return possibile;

        }


        /// <summary>
        /// store place to database
        /// </summary>
        public void luogoToDB()
        {

            sc.Open();

            LocalAppDBDataSet lds = new LocalAppDBDataSet();
            SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT id,luogo,timestat FROM Luogo", sc);
            SqlCeDataAdapter sdaS = new SqlCeDataAdapter("SELECT * FROM Segnale", sc);
            SqlCeDataAdapter sdaA = new SqlCeDataAdapter("SELECT * FROM Azione", sc);
            LocalAppDBDataSet.LuogoRow lr = lds.Luogo.NewLuogoRow();
            //command to generate basic queries (INSERT,UPDATE,DELETE)
            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);
            SqlCeCommandBuilder builderA = new SqlCeCommandBuilder(sdaA);
            SqlCeTransaction st = sc.BeginTransaction();
            try
            {
                sdaL.Fill(lds, "Luogo");
                sdaS.Fill(lds, "Segnale");
                sdaA.Fill(lds, "Azione");
                lr.luogo = _nomeLuogo;
                lr.timestat = Timestat ;
                lds.Luogo.Rows.Add(lr);

                int b = sdaL.Update(lds, "Luogo");
                int id3 = Convert.ToInt32(lds.Luogo.Compute("MAX(id)", String.Empty));
                SqlCeCommand sqlcom = new SqlCeCommand("SELECT id FROM Luogo WHERE luogo='" + this._nomeLuogo + "'", sc);
                sqlcom.Transaction = st;

                SqlCeDataReader sdr = sqlcom.ExecuteReader();
                sdr.Read();
                int id = sdr.GetInt32(0);
                sdr.Close();

                //add networks to table Segnale
                foreach (DictionaryEntry de in netwlist.Hash)
                {
                    Network n = new Network();
                    n = de.Value as Network;
                    if (n == null)
                    {
                        System.ApplicationException ex = new System.ApplicationException("non è un Network");
                        throw ex;
                    }
                    else
                    {
                        LocalAppDBDataSet.SegnaleRow sr = lds.Segnale.NewSegnaleRow();
                        sr.mac = n.Mac;
                        sr.id_luogo = id;
                        sr.potenza = (int) n.Potenza;
                        lds.Segnale.Rows.Add(sr);
                    }

                }

                //add actions to table Azione
                foreach(ActionList.Action a in ActionsList.GetAll()){
                    if (a.Path != null || a.Path != "") {
                        LocalAppDBDataSet.AzioneRow ar = lds.Azione.NewAzioneRow();
                        ar.id_l = id;
                        ar.azione = a.Path;
                        lds.Azione.Rows.Add(ar);
                    }
                }
                b = sdaS.Update(lds, "Segnale");
                b = sdaA.Update(lds, "Azione");
                //commit changes from dataset to database
                lds.Luogo.AcceptChanges();
                lds.Segnale.AcceptChanges();
                lds.Azione.AcceptChanges();
                st.Commit(); 
            }

            catch (Exception e) {
                st.Rollback();
                throw e;
            }
            sc.Close();

        }

        /// <summary>
        /// remove place and related networks
        /// </summary>
        /// <param name="luogo"> place to remove </param>
        public static void removeLuogoFromDB(string luogo)
        {
            ArrayList luoghi = new ArrayList();
            sc.Open();
            
            LocalAppDBDataSet lds = new LocalAppDBDataSet();
            SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT * FROM Luogo", sc);
            SqlCeDataAdapter sdaS = new SqlCeDataAdapter("SELECT * FROM Segnale", sc);
            SqlCeDataAdapter sdaA = new SqlCeDataAdapter("SELECT * FROM Azione", sc);
            LocalAppDBDataSet.LuogoRow lr = lds.Luogo.NewLuogoRow();
            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);
            SqlCeCommandBuilder builderA = new SqlCeCommandBuilder(sdaA);
            SqlCeTransaction st = sc.BeginTransaction();
            try
            {
                sdaL.Fill(lds, "Luogo");
                sdaS.Fill(lds, "Segnale");
                sdaA.Fill(lds, "Azione");

                foreach (DataRow dr in lds.Luogo.Rows)
                {

                    LocalAppDBDataSet.LuogoRow ldr = (LocalAppDBDataSet.LuogoRow)dr;
                    if (ldr.luogo == luogo)
                    {

                        // mark as removed signals
                        foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Segnale"]))
                        {

                            LocalAppDBDataSet.SegnaleRow scr = (LocalAppDBDataSet.SegnaleRow)cr;
                            if (scr.id_luogo == ldr.id)
                            {
                                cr.Delete();
                            }

                        }
                        // mark as removed actions
                        foreach (DataRow currentRow in dr.GetChildRows(lds.Relations["FK_Luogo_Azione"]))
                        {

                            LocalAppDBDataSet.AzioneRow acr = (LocalAppDBDataSet.AzioneRow)currentRow;
                            if (acr.id_l == ldr.id)
                            {
                                currentRow.Delete();
                            }

                        }
                        //segno come cancellate la rispettiva riga di Luogo
                        dr.Delete();
                    }

                }

                //apply changes to DB
                //attenzione all'ordine con cui eseguo i comandi!, prima Segnale poi Luogo
                //altrimenti incasino il DB x i vincoli tra le chiavi
                int c = sdaS.Update(lds, "Segnale");
                c = sdaA.Update(lds, "Azione");
                int b = sdaL.Update(lds, "Luogo");

                lds.Luogo.AcceptChanges();
                lds.Segnale.AcceptChanges();
                lds.Azione.AcceptChanges();
                st.Commit();
                sc.Close();

            }
            catch (Exception e) {
                st.Rollback();
                throw e;
            }
            sc.Close();
            return;
        }

        /// <summary>
        /// check if a place is already present into database
        /// </summary>
        /// <returns> return true  if present, false otherwise </returns>
        public bool checkIfNameExist(){
            sc.Open();
            LocalAppDBDataSet lds = new LocalAppDBDataSet();
            SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT id,luogo FROM Luogo", sc);
            SqlCeDataAdapter sdaS = new SqlCeDataAdapter("SELECT * FROM Segnale", sc);

            LocalAppDBDataSet.LuogoRow lr = lds.Luogo.NewLuogoRow();


            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);

            SqlCeCommand sqlcom = new SqlCeCommand("SELECT id FROM Luogo WHERE luogo='" + this._nomeLuogo + "'", sc);

            SqlCeDataReader sdr = sqlcom.ExecuteReader();
            
            //sdr.Read();
            int i = 0;
            if (sdr.Read()) {
                i = sdr.GetInt32(0);
            }
            sc.Close();
        return i!=0;
        }

        /// <summary>
        /// update place s networks
        /// </summary>
        public static void UpdatePlacePosition(string placeName) {
            Luogo placeToBeUpdated = new Luogo();
            placeToBeUpdated.NomeLuogo = placeName;
            placeToBeUpdated.NetwList.acquireNetworkList();
            if (placeToBeUpdated.NomeLuogo != null && placeToBeUpdated.NomeLuogo != "" && placeToBeUpdated.checkIfNameExist())
            {
                ArrayList luoghi = new ArrayList();
                sc.Open();

                LocalAppDBDataSet lds = new LocalAppDBDataSet();
                SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT * FROM Luogo", sc);
                SqlCeDataAdapter sdaS = new SqlCeDataAdapter("SELECT * FROM Segnale", sc);

                SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
                SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);
                SqlCeTransaction st = sc.BeginTransaction();
                try{

                    sdaL.Fill(lds, "Luogo");
                    sdaS.Fill(lds, "Segnale");
                
                    SqlCeCommand sqlcom = new SqlCeCommand("SELECT id FROM Luogo WHERE luogo='" + placeToBeUpdated.NomeLuogo + "'", sc);
                    SqlCeDataReader sdr = sqlcom.ExecuteReader();
                    sdr.Read();
                    placeToBeUpdated.Id = sdr.GetInt32(0);

                    sdr.Close();
               
                    foreach (DataRow dr in lds.Luogo.Rows)
                    {

                        LocalAppDBDataSet.LuogoRow ldr = (LocalAppDBDataSet.LuogoRow)dr;
                        if (ldr.id == placeToBeUpdated.Id)
                        {

                            // mark as removed signals
                            foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Segnale"]))
                            {

                                LocalAppDBDataSet.SegnaleRow scr = (LocalAppDBDataSet.SegnaleRow)cr;
                                if (scr.id_luogo == ldr.id)
                                {
                                    cr.Delete();
                                }

                            }
                            //add networks to table Segnale
                            foreach (DictionaryEntry de in placeToBeUpdated.NetwList.Hash)
                            {
                                Network n = new Network();
                                n = de.Value as Network;
                                if (n == null)
                                {
                                    System.ApplicationException ex = new System.ApplicationException("non è un Network");
                                    throw ex;
                                }
                                else
                                {
                                    LocalAppDBDataSet.SegnaleRow sr = lds.Segnale.NewSegnaleRow();
                                    sr.mac = n.Mac;
                                    sr.id_luogo = placeToBeUpdated.Id;
                                    sr.potenza = (int)n.Potenza;
                                    lds.Segnale.Rows.Add(sr);
                                }

                            }
                        }

                    }

                    //apply changes to DB
                    //attenzione all'ordine con cui eseguo i comandi!, prima Segnale poi Luogo
                    //altrimenti incasino il DB x i vincoli tra le chiavi
                    int c = sdaS.Update(lds, "Segnale");
                    int b = sdaL.Update(lds, "Luogo");

                    lds.Luogo.AcceptChanges();
                    lds.Segnale.AcceptChanges();
                    st.Commit();
                    sc.Close();

                }
                catch (Exception e) {
                st.Rollback();
                throw e;
            }
            }

        return;
        }

        /// <summary>
        /// update place name or place actions
        /// </summary>
        public static void updatePlace(Luogo placeToBeUpdated,string oldName)
        {

            if (placeToBeUpdated.NomeLuogo == oldName)
            {
                ChangeActions(placeToBeUpdated, oldName);
            }
            else {
                ChangePlaceAndName(placeToBeUpdated, oldName);
            }

            return;
        }
        /// <summary>
        /// update place actions
        /// </summary>
        /// <param name="placeToBeUpdated">place to be updated</param>
        /// <param name="oldName">old place name</param>
        private static void ChangeActions(Luogo placeToBeUpdated, string oldName)
        {

            if (placeToBeUpdated.NomeLuogo != null && placeToBeUpdated.NomeLuogo != "")
            {

                sc.Open();

                LocalAppDBDataSet lds = new LocalAppDBDataSet();
                SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT * FROM Luogo", sc);
                SqlCeDataAdapter sdaA = new SqlCeDataAdapter("SELECT * FROM Azione", sc);

                SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
                SqlCeCommandBuilder builderA = new SqlCeCommandBuilder(sdaA);
                SqlCeTransaction st = sc.BeginTransaction();
                try{
                    sdaL.Fill(lds, "Luogo");
                    sdaA.Fill(lds, "Azione");

                    SqlCeCommand sqlcom = new SqlCeCommand("SELECT id FROM Luogo WHERE luogo='" + oldName + "'", sc);
                    SqlCeDataReader sdr = sqlcom.ExecuteReader();
                    sdr.Read();
                    placeToBeUpdated.Id = sdr.GetInt32(0);

                    sdr.Close();

                    foreach (DataRow dr in lds.Luogo.Rows)
                    {

                        LocalAppDBDataSet.LuogoRow ldr = (LocalAppDBDataSet.LuogoRow)dr;
                        if (ldr.id == placeToBeUpdated.Id)
                        {
                            ldr.luogo = placeToBeUpdated.NomeLuogo;
                            // mark as removed actions
                            foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Azione"]))
                            {

                                LocalAppDBDataSet.AzioneRow acr = (LocalAppDBDataSet.AzioneRow)cr;
                                if (acr.id_l == ldr.id)
                                {
                                    cr.Delete();
                                }

                            }
                            //add action to dataset
                            foreach (ActionList.Action action in placeToBeUpdated.ActionsList.GetAll())
                            {
                                LocalAppDBDataSet.AzioneRow ar = lds.Azione.NewAzioneRow();
                                ar.id_l = placeToBeUpdated.Id;
                                ar.azione = action.Path;
                                lds.Azione.Rows.Add(ar);
                            }
                        }

                    }

                    //apply changes to DB
                    //attenzione all'ordine con cui eseguo i comandi!, prima Segnale poi Luogo
                    //altrimenti incasino il DB x i vincoli tra le chiavi
                    int c = sdaA.Update(lds, "Azione");
                    int b = sdaL.Update(lds, "Luogo");

                    lds.Luogo.AcceptChanges();
                    lds.Segnale.AcceptChanges();
                    st.Commit();
                    sc.Close(); 
                }
                catch (Exception e) {
                st.Rollback();
                throw e;
            }
                
                }
            
            sc.Close();
        return;
        }

        /// <summary>
        /// update place actions and place name
        /// </summary>
        /// <param name="placeToBeUpdated">place to be updated</param>
        /// <param name="oldName">old place name</param>
        private static void ChangePlaceAndName(Luogo placeToBeUpdated, string oldName)
        {

            if (placeToBeUpdated.NomeLuogo != null && placeToBeUpdated.NomeLuogo != "" && !placeToBeUpdated.checkIfNameExist())
            {
                ArrayList luoghi = new ArrayList();
                sc.Open();

                LocalAppDBDataSet lds = new LocalAppDBDataSet();
                SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT * FROM Luogo", sc);
                SqlCeDataAdapter sdaS = new SqlCeDataAdapter("SELECT * FROM Segnale", sc);
                SqlCeDataAdapter sdaA = new SqlCeDataAdapter("SELECT * FROM Azione", sc);

                SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
                SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);
                SqlCeCommandBuilder builderA = new SqlCeCommandBuilder(sdaA);
                SqlCeTransaction st = sc.BeginTransaction();
                try{
                    sdaL.Fill(lds, "Luogo");
                    sdaS.Fill(lds, "Segnale");
                    sdaA.Fill(lds, "Azione");

                    SqlCeCommand sqlcom = new SqlCeCommand("SELECT id FROM Luogo WHERE luogo='" + oldName + "'", sc);
                    SqlCeDataReader sdr = sqlcom.ExecuteReader();
                    sdr.Read();
                    placeToBeUpdated.Id = sdr.GetInt32(0);

                    sdr.Close();

                    foreach (DataRow dr in lds.Luogo.Rows)
                    {

                        LocalAppDBDataSet.LuogoRow ldr = (LocalAppDBDataSet.LuogoRow)dr;
                        if (ldr.id == placeToBeUpdated.Id)
                        {
                            //update place name
                            ldr.luogo = placeToBeUpdated.NomeLuogo;

                            // mark as removed action
                            foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Azione"]))
                            {

                                LocalAppDBDataSet.AzioneRow acr = (LocalAppDBDataSet.AzioneRow)cr;
                                if (acr.id_l == ldr.id)
                                {
                                    cr.Delete();
                                }

                            }
                            //add action to dataset
                            foreach (ActionList.Action action in placeToBeUpdated.ActionsList.GetAll())
                            {
                                LocalAppDBDataSet.AzioneRow ar = lds.Azione.NewAzioneRow();
                                ar.id_l = placeToBeUpdated.Id;
                                ar.azione = action.Path;
                                lds.Azione.Rows.Add(ar);
                            }

                        }

                    }

                    //apply changes to DB
                    //attenzione all'ordine con cui eseguo i comandi!, prima Segnale poi Luogo
                    //altrimenti incasino il DB x i vincoli tra le chiavi
                    int c = sdaA.Update(lds, "Azione");
                    int b = sdaL.Update(lds, "Luogo");

                    lds.Luogo.AcceptChanges();
                    lds.Azione.AcceptChanges();
                    st.Commit();
                    sc.Close();
                  }
                catch (Exception e) {
                st.Rollback();
                throw e;
            }
            }
            sc.Close();
            return;
        }

        /// <summary>
        /// add permance time into place
        /// </summary>
        /// <param name="newStat">last interval time in place</param>
        /// <returns>total time in place</returns>
        public long UpdateStats(int newStat)
        {
            sc.Open();
            LocalAppDBDataSet lds = new LocalAppDBDataSet();
            SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT * FROM Luogo WHERE luogo='" + this._nomeLuogo + "'", sc);
            long totTime = -1;

            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeTransaction st = sc.BeginTransaction();
            try{
                sdaL.Fill(lds,"Luogo");
                if (lds.Luogo.Rows.Count == 0)
                {
                    return totTime;
                }
                DataRow dr = lds.Luogo.Rows[0];
                LocalAppDBDataSet.LuogoRow ldr = (LocalAppDBDataSet.LuogoRow)dr;
                totTime = ldr.timestat;
                ldr.timestat += newStat;
                totTime += newStat;
                int rowUpdated = sdaL.Update(lds,"Luogo");
                lds.Luogo.AcceptChanges();
                st.Commit();
                sc.Close();
                
            }
                catch (Exception e) {
                st.Rollback();
                throw e;
            }
            sc.Close();
            return totTime;
        }
    }



    
}
