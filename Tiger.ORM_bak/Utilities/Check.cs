using System;
using System.Collections.Generic;
using System.Text;

namespace Tiger.ORM.Utilities
{
    internal static class Check
    {
        public static void NotNull(object checkObj,string marking)
        {
            if (checkObj == null)
                throw new Exceptions.TigerORMException($"{marking} is null.");
        }

        public static void NotEmpty(string checkStr,string marking)
        {
            if (string.IsNullOrEmpty(checkStr))
                throw new Exceptions.TigerORMException($"{marking} is empty.");
        }

    }
}
