using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Infrastructure.SMS
{
    public static class HeroSMSManager
    {
        public static IRestResponse send(string Destination, string message)
        {
            message= message.Replace(System.Environment.NewLine, "\\n");
            var client = new RestClient("http://188.0.240.110/api/select");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"op\" : \"send\"" +
                ",\"uname\" : \"tfshops\"" +
                ",\"pass\":  \"ad*4ddku\"" +
            ",\"message\" : \"" + message + "\"" +
                //",\"message\" : \"" + message.Body + "\"" +
                ",\"from\": \"3000505\"" +
                //",\"to\" : [\"09385060192\"]}"
                ",\"to\" : [\"" + Destination.Substring(1, Destination.Length - 1) + "\"]}"
                , ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
        public static IRestResponse sendMulti(List<string> Destination, string message)
        {
            IRestResponse response = null;
            message = message.Replace(System.Environment.NewLine, "\\n");
            var client = new RestClient("http://188.0.240.110/api/select");
            foreach (var item in Destination)
            {
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", "{\"op\" : \"send\"" +
                    ",\"uname\" : \"tfshops\"" +
                    ",\"pass\":  \"ad*4ddku\"" +
                ",\"message\" : \"" + message + "\"" +
                    //",\"message\" : \"" + message.Body + "\"" +
                    ",\"from\": \"3000505\"" +
                    //",\"to\" : [\"09385060192\"]}"
                    ",\"to\" : [\"" + item.Substring(1, item.Length - 1) + "\"]}"
                    , ParameterType.RequestBody);
                 response = client.Execute(request);
            }
            return response;
        }
    }
}
