using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Camelot.Services.Windows.WinApi
{
    public static class Win32
    {
        public static string AssocQueryString(AssocF assocF, AssocStr association, string assocString)
        {
            var length = 0u;
            var queryResult = AssocQueryString(assocF, association, assocString, null, null, ref length);
            if (queryResult != 1)
            {
                return null;
            }

            var builder = new StringBuilder((int) length);
            queryResult = AssocQueryString(assocF, association, assocString, null, builder, ref length);

            return queryResult == 0 ? builder.ToString() : null;
        }

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string pszExtra,
            [Out] StringBuilder pszOut, ref uint pcchOut);

        public enum AssocStr
        {
            Command = 1,
            Executable = 2,
            FriendlyAppName = 4
        }

        [Flags]
        public enum AssocF
        {
            None = 0,
            OpenByExeName = 0x2
        }
    }
}