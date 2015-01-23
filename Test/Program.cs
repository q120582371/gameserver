using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shiwch.util;
using System.Threading;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Text.RegularExpressions;
using System.Collections;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MyGameUser<TestObj> user = new MyGameUser<TestObj> { StatusCode = 1, Description = "swc", ReplyTo = 33, TestObj = new TestObj { StatusCode = 22, Description = "OK" } };

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(user));
        }

        [ProtoContract]
        class MyGameUser<T>
        {
            [ProtoMember(1)]
            public int StatusCode { get; set; }
            [ProtoMember(2)]
            public string Description { get; set; }
            [ProtoMember(3)]
            public int ReplyTo { get; set; }
            [ProtoMember(4)]
            [Newtonsoft.Json.JsonProperty]
            internal T TestObj { get; set; }
        }

        [ProtoContract]
        class MyGameUser2
        {
            [ProtoMember(1)]
            public int StatusCode { get; set; }
            [ProtoMember(2)]
            public string Description { get; set; }
            [ProtoMember(3)]
            public int ReplyTo { get; set; }
            [ProtoMember(4)]
            public TestObj TestObj { get; set; }
        }

        [ProtoContract]
        class TestObj
        {
            [ProtoMember(1)]
            public int StatusCode { get; set; }
            [ProtoMember(2)]
            public string Description { get; set; }
        }
    }
}
