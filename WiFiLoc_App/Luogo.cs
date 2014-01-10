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
    public class Luogo
    {
        string _nomeLuogo;
        int _id;
        NetworkList netwlist;
        ActionList actionsList;


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

            LocalAppDBDataSet.LuogoRow lr = lds.Luogo.NewLuogoRow();

            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);

            sdaL.Fill(lds, "Luogo");
            sdaS.Fill(lds, "Segnale");

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
            LocalAppDBDataSet.LuogoRow lr = lds.Luogo.NewLuogoRow();
            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);
            SqlCeTransaction st = sc.BeginTransaction();

            sdaL.Fill(lds, "Luogo");
            sdaS.Fill(lds, "Segnale");

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

                try
                {
                    b = sdaS.Update(lds, "Segnale");
                    //commit changes from dataset to database
                    lds.Luogo.AcceptChanges();
                    lds.Segnale.AcceptChanges();
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
            LocalAppDBDataSet.LuogoRow lr = lds.Luogo.NewLuogoRow();
            SqlCeCommandBuilder builderL = new SqlCeCommandBuilder(sdaL);
            SqlCeCommandBuilder builderS = new SqlCeCommandBuilder(sdaS);

            sdaL.Fill(lds, "Luogo");
            sdaS.Fill(lds, "Segnale");
            
            foreach (DataRow dr in lds.Luogo.Rows)
            {

                LocalAppDBDataSet.LuogoRow ldr = (LocalAppDBDataSet.LuogoRow)dr;
                if(ldr.luogo == luogo){
               
                foreach (DataRow cr in dr.GetChildRows(lds.Relations["FK_Luogo_Segnale"]))
                {

                    LocalAppDBDataSet.SegnaleRow scr = (LocalAppDBDataSet.SegnaleRow)cr;
                    if (scr.id_luogo == ldr.id) {
                        cr.Delete();
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
            int b = sdaL.Update(lds, "Luogo");
            lds.Luogo.AcceptChanges();
            lds.Segnale.AcceptChanges();

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
