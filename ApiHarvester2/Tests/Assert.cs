//using System;

//namespace ApiHarvester.Tests
//{
//    public static class Assert
//    {
//        public static void AreEqual<T>(T expected, T actual, string message = null)
//        {
//            if (!Equals(expected, actual))
//                throw new Exception(message ?? $"Assert.AreEqual failed. Expected: {expected}, Actual: {actual}");
//        }

//        public static void IsTrue(bool condition, string message = null)
//        {
//            if (!condition)
//                throw new Exception(message ?? "Assert.IsTrue failed.");
//        }
//    }
//}


using System;

namespace ApiHarvester.Tests
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual, string message = null) 
        { 
            if (!Equals(expected, actual)) 
                throw new Exception(message ?? $"Assert.AreEqual failed. Expected: {expected}, Actual: {actual}"); 
        }

        public static void IsTrue(bool condition, string message = null)
        {
            if (!condition)
                throw new Exception(message ?? "Assert.IsTrue failed.");
        }

        public static void IsNotNull(object obj, string message = null)
        {
            if (obj == null)
                throw new Exception(message ?? "Assert.IsNotNull failed.");
        }
    }
}