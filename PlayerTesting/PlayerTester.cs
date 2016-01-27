using System;
using System.Collections;
using System.Collections.Generic;

using Player;
using SharedObjects;

using NUnit.Framework;

namespace PlayerTesting
{
    [TestFixture]
    public class PlayerTester
    {
        [Test]
        public void PlayerConstructor()
        {
            IdentityInfo info = new IdentityInfo()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast"
            };
            Player.Player player = new Player.Player("127.0.0.1:12000", info);

            Assert.That(player.Game, Is.Null);
            Assert.That(player.Identity, Is.EqualTo(info));
            Assert.That(player.Observers, Is.EquivalentTo(new List<IObserver>()));
            Assert.That(player.Process, Is.Null);
            Assert.That(player.RegistryEndPoint.HostAndPort, Is.EqualTo("127.0.0.1:12000"));
        }

        [Test]
        public void PlayerSubscribe()
        {
            IdentityInfo info = new IdentityInfo()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast"
            };
            Player.Player player = new Player.Player("127.0.0.1:12000", info);

            IObserver form = new PlayerForm();

            player.Subscribe(form);

            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));

            player.Subscribe(form);
            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));

            player.Subscribe(null);
            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));
        }

        [Test]
        public void PlayerUnsubscribe()
        {
            IdentityInfo info = new IdentityInfo()
            {
                Alias = "TestAlias",
                ANumber = "A000",
                FirstName = "TestFirst",
                LastName = "TestLast"
            };
            Player.Player player = new Player.Player("127.0.0.1:12000", info);

            IObserver form = new PlayerForm();
            IObserver form2 = new PlayerForm();

            player.Unsubscribe(form);
            Assert.That(player.Observers.Count, Is.EqualTo(0));

            player.Subscribe(form);

            player.Unsubscribe(null);
            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));

            player.Unsubscribe(form2);
            Assert.That(player.Observers[0], Is.EqualTo(form));
            Assert.That(player.Observers.Count, Is.EqualTo(1));

            player.Unsubscribe(form);
            Assert.That(player.Observers.Count, Is.EqualTo(0));
        }
    }
}
