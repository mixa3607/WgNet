using System;
using FluentAssertions;
using Xunit;

namespace ArkProjects.Wireguard.Tools.Tests
{
    public class WgToolsTests
    {
        [Fact]
        public void GenPrivateKey()
        {
            var key = WgTools.GenPrivateKey();
            Assert.Equal(32, key.Length);

            WgTools.GenPublicKey(key);
        }

        [Fact]
        public void GenPreSharedKey()
        {
            var key = WgTools.GenPreSharedKey();
            Assert.Equal(32, key.Length);
        }

        [Fact]
        public void GenPublicKey()
        {
            var privKey1 = Convert.FromBase64String("aADHU5A4mJgrcLT5LOqBfr65a58lgIBcRyeayGicKHs=");
            var pubKey1 = Convert.FromBase64String("h+EedKxu8s0Gbbws65kW8790g3c93LOs4wonKN5rWgE=");
            var privKey2 = Convert.FromBase64String("+MghLjIzDKzfn6anRnoOHs4b5cSwfeBd6yZq7s1SDWA=");
            var pubKey2 = Convert.FromBase64String("K95EAeBS2wLJjoFvnqDNb/jN0WMGeawYU4r9VS7/CUA=");
            WgTools.GenPublicKey(privKey1).Should().Equal(pubKey1);
            WgTools.GenPublicKey(privKey2).Should().Equal(pubKey2);

            Assert.Throws<ArgumentNullException>(() => WgTools.GenPublicKey(null));
            Assert.Throws<FormatException>(() => WgTools.GenPublicKey(Array.Empty<byte>()));
            Assert.Throws<FormatException>(() => WgTools.GenPublicKey(new byte[] { 0, 0 }));
        }
    }
}