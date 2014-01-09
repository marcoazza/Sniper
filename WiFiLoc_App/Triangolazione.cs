using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiFiLoc_App;
using WiFiLoc_Service;


namespace WiFiLoc_App
{
    class Triangolazione
    {
        private Luogo luogoDaTriangolare;
        ArrayList possibiliLuoghi;
        ArrayList distanze;

        struct Distanza {
            public int id;
            public double distanza;
            public string nome;
        }
        class comparatoreDistanze : IComparer {

            int IComparer.Compare(Object x, Object y) { 
                Distanza d1 = (Distanza)x;
                Distanza d2 = (Distanza)y;

                return Comparer<double>.Default.Compare(d1.distanza, d2.distanza);
             
            }
        
        
        }

        public Triangolazione(Luogo l) {
            this.luogoDaTriangolare = l;
            possibiliLuoghi =Luogo.getPossibiliLuoghi();  
        }

        public Triangolazione() { 
        }
        /*
        public int triangola() {
            distanze = new ArrayList();
            
            foreach (Luogo l in possibiliLuoghi) {
                Distanza d;
                d.id = l.Id;
                d.distanza = 0;

                foreach (DictionaryEntry dn in luogoDaTriangolare.NetwList.Hash) {
                    Network n = (Network)dn.Value;
                    uint potenzaVicino;
                    
                    if(l.NetwList.Hash[n.Mac]==null)
                        potenzaVicino=0;
                    else
                        potenzaVicino=l.NetwList[n.Mac].Potenza;

                   // d.distanza =Math.Sqrt(System.Math.Pow(d.distanza,2)+System.Math.Pow(System.Math.Pow(10, (System.Math.Abs(potenzaVicino - n.Potenza))),2));
                    d.distanza += Math.Pow(((double)potenzaVicino - (double)n.Potenza), 2);
                    
                }
                d.distanza = Math.Sqrt(d.distanza);
                distanze.Add(d);
                
            }
            distanze.Sort(new comparatoreDistanze());
            int indiceLast = distanze.Count;

            Distanza bestMatch;
            //bestMatch = (Distanza)distanze[indiceLast];

            return 1;        
        }
        */
        public Luogo triangolaPosizione()
        {
            //creo un nuovo luogo per acquisire le reti circostanti
            luogoDaTriangolare = new Luogo();
            luogoDaTriangolare.saveNextList();
            distanze = new ArrayList();
            //acquisisco i luoghi gia conosciuti
            possibiliLuoghi = Luogo.getPossibiliLuoghi();
            
            //calcolo tutte le distanze tra il segnale del luogoDaTriangolare e i possibiliLuoghi
            foreach (Luogo l in possibiliLuoghi)
            {
                Distanza d;
                d.id = l.Id;
                d.nome = l.NomeLuogo;
                d.distanza = 0;

                foreach (DictionaryEntry dn in luogoDaTriangolare.NetwList.Hash)
                {
                    Network n = (Network)dn.Value;
                    double potenzaVicino;
                    double pAtt;

                    if (l.NetwList.Hash[n.Mac] == null)
                        potenzaVicino = 0;
                    else
                        potenzaVicino = (double) Math.Pow(10.0, (l.NetwList[n.Mac].Potenza - 44) / (-22.0));
                    pAtt = (double)Math.Pow(10.0, (n.Potenza - 44) / (-22.0));
                    
                    d.distanza += Math.Pow(((double)potenzaVicino - pAtt), 2);

                }
                d.distanza = Math.Sqrt(d.distanza);
                distanze.Add(d);

            }
            distanze.Sort(new comparatoreDistanze());
            int indiceLast = distanze.Count;

           
            //se trovo un luogo maggiore di una certa soglia, allora lo considero buono
            //altrimenti assumo di essere in una posizione sconusciuta
            if (indiceLast > 0)
            {
                Distanza bestMatch;
                bestMatch = (Distanza)distanze[0];
                luogoDaTriangolare.NomeLuogo = bestMatch.nome;
            }
            else {
                luogoDaTriangolare = null;
            }
                return luogoDaTriangolare;
        }

        private double potenzaToDistanza(double potenzaSegnale ){
           return (double)Math.Pow(10.0, (potenzaSegnale + 44.8) / (-22.0));
        }

    }
}
