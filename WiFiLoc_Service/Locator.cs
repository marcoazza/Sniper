using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiFiLoc_Service;



namespace WiFiLoc_Service
{
    public static class Locator
    {
        const double COMMON_AP_SIMILARITY_WEIGHT = 0.4;
        const double COMMON_AP_DISTANCE_WEIGHT = 0.3;
        const double AP_MISSING_WEIGHT = 0.2;
        const double AP_NEW_WEIGHT = 0.1;

        const double SIMILARITY_THRESHOLD = 20;

        struct Confidence
        {
            public double value;
            public Luogo place;
        }

        class compareConfidence : IComparer
        {
            int IComparer.Compare(Object x, Object y)
            {
                Confidence c1 = (Confidence)x;
                Confidence c2 = (Confidence)y;
                return Comparer<double>.Default.Compare(c1.value, c2.value);
            }
        }


        public static Luogo locate()
        {

            Confidence bestConfidence;
            bestConfidence.value = 0;
            bestConfidence.place = null;

            //creo un nuovo luogo per acquisire le reti circostanti
            Luogo placeToBeLocated = new Luogo();
            placeToBeLocated.saveNextList();
            ArrayList confidences = new ArrayList();

            //acquisisco i luoghi gia conosciuti
            ArrayList savedPlaces = Luogo.getPossibiliLuoghi();

            foreach (Luogo l in savedPlaces)
            {
                Confidence confidence;
                confidence = getconfidence(placeToBeLocated, l);
                confidences.Add(confidence);
            }

            confidences.Sort(new compareConfidence());
            if (confidences.Count != 0)
                bestConfidence = (Confidence)confidences[confidences.Count - 1];

            if (confidences.Count == 0 || bestConfidence.value < 0.5)
            {
                return null;
            }
            else
            {
                return bestConfidence.place;
            }
        }


        private static Confidence getconfidence(Luogo placeToBeLocated, Luogo savedPlace)
        {
            Confidence c;
            double similarityConf = COMMON_AP_SIMILARITY_WEIGHT * getAPSimilarityConfidence(placeToBeLocated, savedPlace);
            double distanceConf = COMMON_AP_DISTANCE_WEIGHT * getAPDistanceConfidence(placeToBeLocated, savedPlace);
            double missConf = AP_MISSING_WEIGHT * getAPMissingConfidence(placeToBeLocated, savedPlace);
            double newConf = AP_NEW_WEIGHT * getAPNewConfidence(placeToBeLocated, savedPlace);
            c.value = similarityConf + distanceConf + missConf + newConf;
            c.place = savedPlace;
            return c;
        }

        /// <summary>
        /// Given the common APs calculate a confidence based on the number of APs whose signals differs of an ammount smaller of a defined threshold. 
        /// </summary>
        /// <param name="placeToBeLocated"></param>
        /// <param name="savedPlaces"></param>
        /// <returns></returns>
        private static double getAPSimilarityConfidence(Luogo placeToBeLocated, Luogo savedPlace)
        {
            ArrayList commonAPs = getCommonAPs(placeToBeLocated, savedPlace);
            double APDiffUnderTs = 0;
            double confidence;
            foreach (string ap in commonAPs)
            {
                double APPowerEnv = (double)placeToBeLocated.NetwList[ap].Potenza;
                double APPowerSaved = (double)savedPlace.NetwList[ap].Potenza;

                if (APPowerEnv - APPowerSaved < SIMILARITY_THRESHOLD)
                    APDiffUnderTs++;
            }
            if (commonAPs.Count == 0) {
                return 0;
            }
            confidence = APDiffUnderTs / commonAPs.Count;
            return confidence;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="placeToBeLocated"></param>
        /// <param name="savedPlaces"></param>
        /// <returns></returns>
        private static double getAPDistanceConfidence(Luogo placeToBeLocated, Luogo savedPlace)
        {
            ArrayList commonAPs = getCommonAPs(placeToBeLocated, savedPlace);
            double distanceSum = 0;
            double distance;
            foreach (string ap in commonAPs)
            {
                double APPowerEnv = (double)placeToBeLocated.NetwList[ap].Potenza;
                double APPowerSaved = (double)savedPlace.NetwList[ap].Potenza;
                double APDistanceEnv = powerToDistance(APPowerEnv) / 100;
                double APDistanceSaved = powerToDistance(APPowerSaved) / 100;
                distanceSum += Math.Pow(Math.Abs(APDistanceEnv - APDistanceSaved), commonAPs.Count);
            }

            if (commonAPs.Count == 0)
                distance = 1;
            else
                distance = Math.Pow(distanceSum, 1.0 / commonAPs.Count);

            return 1 - distance;
        }

        private static double getAPMissingConfidence(Luogo placeToBeLocated, Luogo savedPlace)
        {
            ArrayList commonAPs = getCommonAPs(placeToBeLocated, savedPlace);
            double savedPlaceAPNum = savedPlace.NetwList.Hash.Count;
            double commonAPNum = commonAPs.Count;
            if(savedPlaceAPNum != 0)
                return (savedPlaceAPNum - (savedPlaceAPNum - commonAPNum)) / savedPlaceAPNum;
            return 0;
        }

        private static double getAPNewConfidence(Luogo placeToBeLocated, Luogo savedPlace)
        {
            ArrayList commonAPs = getCommonAPs(placeToBeLocated, savedPlace);
            double placeToBeLocatedAPNum = placeToBeLocated.NetwList.Hash.Count;
            double commonAPNum = commonAPs.Count;
            if (placeToBeLocatedAPNum == 0)
            {
                return 0;
            }
            return (placeToBeLocatedAPNum - (placeToBeLocatedAPNum - commonAPNum)) / placeToBeLocatedAPNum;

        }

        /// <summary>
        /// Convert signal power to estimated distance
        /// </summary>
        /// <param name="signalPower">signal power</param>
        /// <returns>estimated distance given signal power</returns>
        private static double powerToDistance(double signalPower)
        {
            return (double)Math.Pow(10.0, (signalPower + 44.8) / (-22.0));
        }

        /// <summary>
        /// Find common APs in different places
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        private static ArrayList getCommonAPs(Luogo l1, Luogo l2)
        {
            ArrayList l1APs = new ArrayList(l1.NetwList.Hash.Keys);
            ArrayList l2APs = new ArrayList(l2.NetwList.Hash.Keys);
            ArrayList commonAPs = new ArrayList();

            foreach (string mac in l1APs)
            {
                foreach (string savedMac in l2APs)
                {
                    if (mac == savedMac)
                        commonAPs.Add(mac);
                }
            }
            return commonAPs;
        }

    }
}