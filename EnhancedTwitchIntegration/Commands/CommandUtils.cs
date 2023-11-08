using System.Text.RegularExpressions;

namespace SongRequestManager.Commands
{
    public static class CommandUtils
    {
        private static Regex HexRegex = new Regex("^[0-9a-fA-F]+$", RegexOptions.Compiled);

        public static bool IsBeatSaverId(string arg)
        {
            return HexRegex.Match(arg).Success;
        }
    }
}
