using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Security;

namespace SecurityUT
{
    [TestClass]
    public class SecurityExtensionsUT
    {
        [TestMethod]
        public void Security_Can_Encrypt()
        {
            string expected = "XGi3zVBPQH1TuUBb78Jwcg==";
            string actual = ("a value").Encrypt();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Security_Can_Decrypt()
        {
            string expected = "a value";
            string actual = ("XGi3zVBPQH1TuUBb78Jwcg==").Decrypt();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Security_Can_Not_Decrypt()
        {
            bool throwedExeption = false;
            try
            {
                string value = ("a value").Decrypt();
            }
            catch (Exception)
            {
                throwedExeption = true;
            }

            Assert.IsTrue(throwedExeption);
        }
    }
}
