using System;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Elastacloud.SimpleScaler
{
    /// <summary>
    /// Go here to see the current role instances: http://netmfmvc.cloudapp.net/Management/
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            var currentState = GetCurrentState();
            Debug.Print(currentState);

            AnalogInput analogInput = new AnalogInput(AnalogChannels.ANALOG_PIN_A0);
            var data = analogInput.Read()*10D;

            try
            {
                var setStateResult = SetCurrentState((int) data);
                Debug.Print(setStateResult);
            }
            catch(WebException wex)
            {
                //I expect this to time out - the gateway blocks the call and my netduino is going to 
                //give up before completing. 
                Debug.Print("Call did not return, check state manually");
            }

            while (true)
            {
                currentState = GetCurrentState();
                Debug.Print(currentState);

                Thread.Sleep(5000);
            }
        }

        private static string SetCurrentState(int dataParam)
        {
            HttpWebRequest setState = (HttpWebRequest)WebRequest.Create("http://fluentmf.cloudapp.net/api/values?number=" + dataParam);
            setState.Method = "POST";
            setState.ContentLength = 0;
            
            var response = setState.GetResponse();
            var stream = response.GetResponseStream();

            using (StreamReader sr = new StreamReader(stream))
            {
                var data = sr.ReadToEnd();
                return data;
            }

        }

        private static string GetCurrentState()
        {
            HttpWebRequest getState = (HttpWebRequest)WebRequest.Create("http://fluentmf.cloudapp.net/api/values");
            var response = getState.GetResponse();
            var stream = response.GetResponseStream();

            using (StreamReader sr = new StreamReader(stream))
            {
                var data = sr.ReadToEnd();
                return data;
            }
        }
    }
}
