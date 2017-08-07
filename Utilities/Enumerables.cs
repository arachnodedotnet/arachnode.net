using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnode.Utilities
{
    public static class Enumerables
    {
        //public static IEnumerable<string> RemoveElementCharacters<string>(IEnumerable<string> enumerable, char[] charactersToRemove)
        //{
        //    IEnumerable<string> enumerableToReturn = new IEnumerable<string>();

        //    foreach(string @string in enumerable)
        //    {

        //    }

        //    return 
        //}

        public static List<string> RemoveListMemberCharacters(List<string> listToPrune, char[] charactersToRemove, bool replaceEmptyStringWithNull)
        {
            if(listToPrune == null | listToPrune.Count == 0)
            {
                return listToPrune;
            }

            List<string> listToReturn = new List<string>();

            foreach (string @string in listToPrune)
            {
                string @string2 = String.Join(" ", @string.Split(charactersToRemove));

                if (replaceEmptyStringWithNull)
                {
                    if (string.IsNullOrEmpty(@string))
                    {
                        @string2 = null;
                    }
                }

                listToReturn.Add(@string2);
            }

            return listToReturn;
        }
    }
}
