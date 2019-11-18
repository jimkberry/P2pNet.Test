﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using P2pNet;

namespace Tests
{

    public class TestClient : IP2pNetClient
    {
        public class PeerData
        {
            string town;
            string name;
            public PeerData(string _name, string _town)
            {
                town = _town;
                name = _name;
            }

        }

        public class MsgRecord
        {
            public string from;
            public string to;
            public object msgData;

            public MsgRecord(string _from, string _to, object _msgData)
            {
                from = _from;
                to = _to;
                msgData = _msgData;
            }
        }
        public IP2pNet p2p;
        public string name;
        public string town;
        public string p2pId;
        public Dictionary<string, PeerData> localPeers;
        public List<MsgRecord> msgList;


        public TestClient(string _name, string _town)
        {
            town = _town;
            name = _name;
            localPeers = new Dictionary<string, PeerData>();
            msgList = new List<MsgRecord>();
        }

        public bool Connect(string connectionStr)
        {
            p2p = new P2pRedis(this, connectionStr);
            return p2p != null;
        }

        public string Join(string gameChannel)
        {
            return p2pId = p2p.Join(gameChannel);
        }


        public object P2pHelloData()
        {
            return new PeerData(name, town);
        }
        public void OnPeerJoined(string p2pId, object helloData)
        {
            localPeers[p2pId] = ( helloData as PeerData);
        }
        public void OnPeerLeft(string p2pId)
        {
            localPeers.Remove(p2pId);
        }
        public void OnP2pMsg(string from, string to, object msgData)
        {
            msgList.Add( new MsgRecord(from, to, msgData));
        }
    }

    [TestFixture]
    public class P2pRedisTests
    {
        public string redisConnectionStr = "sparkyx,password=sparky-redis79";
        public string testGameChannel = "testGameChannel";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldCreateP2pRedis()
        {
            TestClient tc = new TestClient("jim","meredith");
            IP2pNet p2p = new P2pRedis(tc, redisConnectionStr);
            Assert.That(p2p, Is.Not.Null);
            Assert.That(p2p.GetId(), Is.Null);
        }
        [Test]
        public void P2pShouldConnect()
        {
            TestClient tc = new TestClient("jim","meredith");
            IP2pNet p2p = new P2pRedis(tc, redisConnectionStr);
            Assert.That(p2p, Is.Not.Null);
            Assert.That(p2p.GetId(), Is.Null);
        }
        [Test]
        public void ClientShouldConnect()
        {
            //same as above, relly
            TestClient tc = new TestClient("jim","meredith");
            tc.Connect(redisConnectionStr);
            Assert.That(tc.p2p, Is.Not.Null);
            Assert.That(tc.p2p.GetId(), Is.Null);
        }

        [Test]
        public async Task ClientShouldJoin()
        {
            //same as above, really
            TestClient tc = new TestClient("jim","meredith");
            tc.Connect(redisConnectionStr);
            string returnedId = tc.Join(testGameChannel);
            Assert.That(returnedId, Is.Not.Null);
            Assert.That(tc.p2p.GetId(), Is.EqualTo(returnedId));
            Assert.That(tc.p2pId, Is.EqualTo(returnedId));
            await Task.Delay(250); // &&& SUPER LAME!!!!
            Assert.That(tc.p2p.GetPeerIds().Count, Is.EqualTo(1));
        }

        [Test]
        public async Task TwoClientsShouldTalk()
        {
            TestClient tcJim = new TestClient("jim","meredith");
            tcJim.Connect(redisConnectionStr);

            await Task.Delay(250);
            TestClient tcEllen = new TestClient("ellen","raymond");
            tcEllen.Connect(redisConnectionStr);

            await Task.Delay(100);
            tcJim.Join(testGameChannel);

            await Task.Delay(300);
            tcEllen.Join(testGameChannel);

            await Task.Delay(300);
            Assert.That(tcJim.localPeers.Values.Count, Is.EqualTo(2));
            Assert.That(tcEllen.msgList.Count, Is.EqualTo(0));
            Assert.That(tcJim.msgList.Count, Is.EqualTo(0));
            tcJim.p2p.Send(testGameChannel, (object)"Hello game channel");
            tcJim.p2p.Send(tcEllen.p2pId, (object)"Hello Ellen");

            await Task.Delay(200);
            Assert.That(tcEllen.msgList.Count, Is.EqualTo(2));
            Assert.That(tcJim.msgList.Count, Is.EqualTo(1));
        }
    }

}
