﻿using System;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using WiFiLoc_App;
using System.Management;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Collections;



namespace WiFiLoc_App
{
    /// <summary>
    /// Class to manage interaction with actions
    /// </summary>
    class ActionManager
    {

        public class itemApp {

            public string applicazione { get; set; }
            public String icon { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }


        public static itemApp getAssociatedItem(string path) {
            string envPath = Environment.CurrentDirectory.ToString();
            string n = System.IO.Path.GetFileNameWithoutExtension(path);
            String ico = envPath + "\\images\\" + n + ".ico";
            if(!System.IO.File.Exists(ico)){
                ico =  "\\images\\action_icon.png";
            }
            return new itemApp {applicazione=path,icon=ico, name=n }; 
        }


        public static void startProcess(string stringProc) {
            Process p = new Process();
            p.StartInfo.FileName = stringProc;
            p.Start();
        
        }
    
        public delegate void updateAppList (System.Windows.Controls.ListBox l,string s,string ico,string n);


        /// <summary>
        /// get installed software reading local registry
        /// </summary>
        /// <param name="li"> list where item found will stored </param>
        public static void getInstalledSoftware(Object li)
        {
            List<string> items= new List <string>();
            System.Windows.Controls.ListBox l = (System.Windows.Controls.ListBox)li;
            string rp = Environment.CurrentDirectory.ToString();
            System.IO.Directory.CreateDirectory(rp + "\\images\\");

            //The registry key:
            string SoftwareKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(SoftwareKey))
           
            //Let's go through the registry keys and get the info we need
            foreach (string skName in rk.GetSubKeyNames())
            {
                using (RegistryKey sk = rk.OpenSubKey(skName))
                {
                    try
                    {
                        //If the key has value, continue, if not, skip it:
                        if (!(sk.GetValue("") == null))
                        {
                            Icon ico = Icon.ExtractAssociatedIcon((string)sk.GetValue("")); 
                            Bitmap b = ico.ToBitmap();
                            object[] myArray = new object[4];
                            myArray[0] = l;
                            myArray[1] = sk.GetValue("");
                            string completePath = rp + "\\images\\" + System.IO.Path.GetFileNameWithoutExtension((string)sk.GetValue("")) + ".ico";

                            if(!System.IO.File.Exists(completePath)){
                                try
                                {
                                    b.Save(completePath);
                                }
                                catch (Exception e)
                                {
                                }
                            }

                            myArray[2] = rp + "\\images\\" + System.IO.Path.GetFileNameWithoutExtension((string)sk.GetValue("")) + ".ico";
                            myArray[3] = System.IO.Path.GetFileNameWithoutExtension((string)sk.GetValue(""));

                            if (!items.Contains(myArray[1])) {
                                l.Dispatcher.BeginInvoke(new updateAppList(addItem), myArray);
                                items.Add((string)myArray[1]);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        //No, that exception is not getting away
                    }
                }
            }
        }


        /// <summary>
        /// add item to list
        /// </summary>
        /// <param name="myL"> list where action will be added </param>
        /// <param name="s"> action path </param>
        /// <param name="ico"> icon action path </param>
        /// <param name="n"> action name </param>
        public static void addItem(System.Windows.Controls.ListBox myL,string s,String ico,string n) {

            myL.Items.Add(new itemApp { applicazione=s,icon=ico, name=n });

        }



        /// <summary>
        /// extract Actions from ListBox Control
        /// </summary>
        /// <param name="itemCollection"> list where action will extracted </param>
        /// <returns> Array containing Actions <returns>
        public static ArrayList SaveActions(System.Windows.Controls.ItemCollection itemCollection)
        {
            ArrayList apps = new ArrayList();
            foreach (ActionManager.itemApp i in itemCollection)
            {
               apps.Add(new ActionList.Action(i.applicazione) );
            }
            return apps;
        }
    }
}
