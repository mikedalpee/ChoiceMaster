using System;
using PublishSubscribe.IntraProcessPublishSubscribe;
using PublishSubscribe.IPublishSubscribe;

namespace ChoiceMaster
{
    public class MyBroker
    {
        private static readonly Broker<string> s_instance =
            new Lazy<Singleton<Broker<string>>>(
                () => new Singleton<Broker<string>>(
                        MyBroker.Name,
                        x => new Broker<string>(x))).Value.Instance(MyBroker.Name);

        public static string Name
        {
            get
            {
                return "ChoiceMaster";
            }
        }

        public static Broker<string> Instance
        {
            get
            {
                return s_instance;
            }
        }

        public static void Reset()
        {
            s_instance.Unsubscribe();
            s_instance.Unregister();
        }
    }
}
