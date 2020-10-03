using System.Collections.Generic;

namespace Helpers
{
    static class IsNumber
    {
        public static bool isNumber(this string str)
        {
            Dictionary<char, char> NumbersDictionary = new Dictionary<char, char>
            {
                ['0'] = '0',
                ['1'] = '1',
                ['2'] = '2',
                ['3'] = '3',
                ['4'] = '4',
                ['5'] = '5',
                ['6'] = '6',
                ['7'] = '7',
                ['8'] = '8',
                ['9'] = '9'
            };
            bool result = true;
            foreach (var item in str)
            {
                if (!NumbersDictionary.ContainsKey(item))
                {
                    result = false;
                }
            }
            return result;
        }
    }
}