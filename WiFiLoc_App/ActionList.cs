using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WiFiLoc_App
{
    public class ActionList
    {
       
        
        public class Action {
            public string Path { get; set; }
            public Action(string path) {
                Path = path;
            }
        }

        private ArrayList _listAction;

        public ActionList() {
            _listAction = new ArrayList();
        }

        public void SaveActions(ItemCollection ic) {
            
            foreach (ActionManager.itemApp i in ic) {
                _listAction.Add(new Action(i.applicazione));
            }
            
        }

        public void AddAction(Action act){
            _listAction.Add(act);
        }

        public void RemoveAction(Action act) {
            if(_listAction.Contains(act))
                _listAction.Remove(act);
        }

        public ArrayList GetAll() {
            return _listAction;
        }


    }
}
