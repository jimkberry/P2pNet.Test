using System;
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

        public IP2pNet p2p;
        public string name;
        public string town;
        public string p2pId;

        public TestClient(string _name, string _town)
        {
            town = _town;
            name = _name;
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

        }
        public void OnPeerLeft(string p2pId)
        {

        }
        public void OnP2pMsg(string from, string to, object msgData)
        {

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
        public void ClientShouldJoin()
        {
            //same as above, really
            TestClient tc = new TestClient("jim","meredith");
            tc.Connect(redisConnectionStr);
            string returnedId = tc.Join(testGameChannel);
            Assert.That(returnedId, Is.Not.Null);
            Assert.That(tc.p2p.GetId(), Is.EqualTo(returnedId));
            Assert.That(tc.p2pId, Is.EqualTo(returnedId));
        }
    }

}
