using System;
using System.Collections.Generic;
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
        [Test]
        [TestCase("andrea balducci","AB")]
        [TestCase("andrea","A")]
        [TestCase("andrea bal ducci","AD")]
        [TestCase("andrea b a l d u","AA")]
        public void avatar(string name, string expected)
        {
            var initials = AvatarBuilder.GetInitials(name);
            Assert.AreEqual(expected, initials);
        }
    }
}
