﻿using System;
using PublishSubscribe.IntraProcessPublishSubscribe;

namespace ChoiceMaster
{
    public class MyBroker
    {
        private static readonly Lazy<BrokerSingleton<string>> s_instance = new Lazy<BrokerSingleton<string>>(() => new BrokerSingleton<string>(MyBroker.Name));

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
                return s_instance.Value.Instance(MyBroker.Name);
            }
        }

        public static void Reset()
        {
            s_instance.Value.Instance(MyBroker.Name).Unsubscribe();
            s_instance.Value.Instance(MyBroker.Name).Unregister();
        }
    }
}
