using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Projekt_Gruppe_2
{
    public class Message
    {
        public int MessageID { get; set; }
        public long TimestampUnix { get; set; }
        public Byte[] Payload { get; set; }
        public string IPSender { get; set; }
        public string AliasSender { get; set; }
        public string DataFormat { get; set; }
        public string IPEmpfaenger { get; set; }
        public int Port { get; set; }

    }
}

/*
DateTime foo = DateTime.Now;
long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
Console.WriteLine(foo);
Console.WriteLine(unixTime);
Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
Console.WriteLine(unixTimestamp);*/