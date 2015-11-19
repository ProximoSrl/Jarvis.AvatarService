using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jarvis.AvatarService.Support;
using NUnit.Framework;

namespace Jarvis.AvatarService.Tests
{
    [TestFixture]
    public class AvatarBuilderTests
    {
        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            AvatarBuilder.RootFolder = AppDomain.CurrentDomain.BaseDirectory;
        }

        [Test]
        [TestCase("andrea balducci", "AB")]
        [TestCase("andrea", "A")]
        [TestCase("andrea bal ducci", "AD")]
        [TestCase("andrea b a l d u", "AA")]
        public void avatar(string name, string expected)
        {
            var initials = AvatarBuilder.GetInitials(name);
            Assert.AreEqual(expected, initials);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void on_invalid_string_should_throw(string name)
        {
            Assert.Throws<Exception>(() => { AvatarBuilder.GetInitials(name); });
        }

        [Test]
        public void should_create_a_new_avatar_on_disk()
        {
            for (var i = 0; i <= 20; i++)
            {
                var pathTo = AvatarBuilder.CreateFor("user_"+i, 80, "andrea balducci");
                Debug.WriteLine("avatar file => {0}", (object)pathTo);
                Assert.NotNull(pathTo);
                Assert.IsTrue(File.Exists(pathTo), "File missing on disk");
            }
        }
    }
}
