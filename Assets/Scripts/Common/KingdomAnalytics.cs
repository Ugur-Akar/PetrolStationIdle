using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
//using ElephantSDK;
#else
using GameAnalyticsSDK;
#endif


namespace SoftwareKingdom
{
    public class KingdomAnalytics
    {
        public static void LevelCompleted(int levelIndex, params string[] parameters)
        {
            Debug.Log("Sending level completed event: " + levelIndex);
            PrintParams(parameters);
#if UNITY_IOS
            //  Params analyticParams = GetParams(parameters);
            //  Elephant.LevelCompleted(levelIndex + 1, analyticParams);
#else

#endif

        }

        public static void LevelFailed(int levelIndex, params string[] parameters)
        {

            Debug.Log("Sending level failed event: " + levelIndex);
            PrintParams(parameters);
#if UNITY_IOS
            //Params analyticParams = GetParams(parameters);
            //Elephant.LevelFailed(levelIndex  + 1, analyticParams);     
#endif
        }

       

        public static void LevelStarted(int levelIndex, params string[] parameters)
        {
            Debug.Log("Sending level started event: " + levelIndex);
            PrintParams(parameters);
#if UNITY_IOS
            //Params analyticParams = GetParams(parameters);
            //Elephant.LevelStarted(levelIndex + 1, analyticParams);
#endif
        }

        public static void CustomEvent(string eventType, int levelIndex, params string[] parameters)
        {
            Debug.Log("Sending level custom event: " + levelIndex);
            PrintParams(parameters);
#if UNITY_IOS
            //Params analyticParams = GetParams(parameters);
            //Elephant.Event(eventType,levelIndex + 1, analyticParams);
#endif
        }


        /*private static Params GetParams(string[] parameters)
        {
            if (parameters.Length == 0) return null;
            Params analyticParams = Params.New();
            Debug.Log("Parameters:");
            for (int i = 0; i < parameters.Length / 2; i++)
            {
                string key = parameters[2 * i];
                double value = System.Convert.ToDouble(parameters[2 * i + 1]);
                analyticParams.Set(key, value);
            }
            return analyticParams;
        }*/

        private static void PrintParams(string[] parameters)
       {
           if (parameters.Length == 0) return ;
         
           Debug.Log("Parameters:");
           for (int i = 0; i < parameters.Length / 2; i++)
           {
               string key = parameters[2 * i];
               double value = System.Convert.ToDouble(parameters[2 * i + 1]);
               Debug.Log(key + "=" + value);
           }
   
       }


    }
}


