using System;
using PublishSubscribe.IntraProcessPublishSubscribe;
using PublishSubscribe.IPublishSubscribe;

namespace ChoiceMaster
{
    public class MyBroker
    {
        private static readonly Broker<string> s_instance =
           SingletonManager<Broker<string>>.Register(
               MyBroker.Name,
               new Broker<string>(MyBroker.Name));

        public static string Name
        {
            get
            {
                return "MyBroker";
            }
        }
        public static Broker<string> Instance
        {
            get
            {
                return s_instance;
            }
        }
    }
}
