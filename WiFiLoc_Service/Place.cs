﻿using System;
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
    public class Place
    {
        string _nomeLuogo;
        int _id;
        NetworkList netwlist;
        ActionList actionsList;


        public Place()
        {
            netwlist = new NetworkList();
            actionsList = new ActionList();
            Timestat = 0;

        }

        public Place(string luogo){
            netwlist = new NetworkList();
            actionsList = new ActionList();
            netwlist.acquireNetworkList();
            _nomeLuogo = luogo;
            Timestat = 0;
        }

        public void saveNextList() { 
            netwlist = new NetworkList();
            netwlist.acquireNetworkList();
        }


        public string NomeLuogo {
            get{ return _nomeLuogo;
                }
            set {
                _nomeLuogo = value;
            }
        }

        public int Id {
        get {return _id; }
        set{_id = value; }
        }
        public long Timestat { get; set; }

        public NetworkList NetwList {
            get { return netwlist; }
        }
        public ActionList ActionsList { get { return actionsList; } }

        /// <summary>
        /// get from database the places already saved
        /// </summary>
        /// <returns> return ArrayList containing stored places </returns>
        public static ArrayList getPossibiliLuoghi()
        {
            ArrayList luoghi= new ArrayList();
            SqlCeConnection sc = DBconnection.getDBConnection();
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
                Place possibile = new Place();
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

            return luoghi;

        }

        /// <summary>
        /// store place to database
        /// </summary>
        public void luogoToDB()
        {

            SqlCeConnection sc = DBconnection.getDBConnection();
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

            sdaL.Fill(lds, "Luogo");
            sdaS.Fill(lds, "Segnale");
            sdaA.Fill(lds, "Azione");

            try
            {
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

                try
                {
                    b = sdaS.Update(lds, "Segnale");
                    b = sdaA.Update(lds, "Azione");
                    //commit changes from dataset to database
                    lds.Luogo.AcceptChanges();
                    lds.Segnale.AcceptChanges();
                    lds.Azione.AcceptChanges();
                    st.Commit();

                }
                catch (Exception e)
                {
                    st.Rollback();
                } 
            }

            catch (ConstraintException e) {

                throw e;
            }

        }

        /// <summary>
        /// remove place and related networks
        /// </summary>
        /// <param name="luogo"> place to remove </param>
        public static void removeLuogoFromDB(string luogo)
        {
            ArrayList luoghi = new ArrayList();
            SqlCeConnection sc = DBconnection.getDBConnection();
            sc.Open();
            
            LocalAppDBDataSet lds = new LocalAppDBDataSet();
            SqlCeDataAdapter sdaL = new SqlCeDataAdapter("SELECT * FROM Luogo", sc);
            SqlCeDataAdapter sdaS = new SqlCeDataAdapter("SELECT * FROM Segnale", sc);
            SqlCeDataAdapter sdaA = new SqlCeDataAdapter("SELECT * FROM Azione", sc);
            LocalAppDBDataSet.LuogoRow lr = lds.Luogo.NewLuogoRow();
            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);
            SqlCeCommandBuilder builderA = new SqlCeCommandBuilder(sdaA);

            sdaL.Fill(lds, "Luogo");
            sdaS.Fill(lds, "Segnale");
            sdaA.Fill(lds, "Azione");

            foreach (DataRow dr in lds.Luogo.Rows)
            {

                LocalAppDBDataSet.LuogoRow ldr = (LocalAppDBDataSet.LuogoRow)dr;
                if(ldr.luogo == luogo){
               
                // mark as removed signals
                foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Segnale"]))
                {

                    LocalAppDBDataSet.SegnaleRow scr = (LocalAppDBDataSet.SegnaleRow)cr;
                    if (scr.id_luogo == ldr.id) {
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

            sc.Close();
            return;
        }

        /// <summary>
        /// check if a place is already present into database
        /// </summary>
        /// <returns> return true  if present, false otherwise </returns>
        public bool checkIfNameExist(){
            SqlCeConnection sc = DBconnection.getDBConnection();
            sc.Open();
            //SqlConnection scS = DBconnection.getDBConnection();
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

        return i!=0;
        }
    }


    
}
