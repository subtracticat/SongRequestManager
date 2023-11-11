using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using SongRequestManager.Config;
using UnityEngine;
// Feature requests: Add Reason for being banned to banlist

namespace SongRequestManager
{
    public partial class RequestBot : MonoBehaviour
    {
        public static void EmptyDirectory(string directory, bool delete = true)
        {
            if (Directory.Exists(directory))
            {
                var directoryInfo = new DirectoryInfo(directory);
                foreach (System.IO.FileInfo file in directoryInfo.GetFiles()) file.Delete();
                foreach (System.IO.DirectoryInfo subDirectory in directoryInfo.GetDirectories()) subDirectory.Delete(true);

                if (delete) Directory.Delete(directory);
            }
        }

        public class StringNormalization
        {
            public static HashSet<string> BeatsaverBadWords = new HashSet<string>();

            public void ReplaceSymbols(StringBuilder text, char[] mask)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if (c < 128)
                    {
                        text[i] = mask[c];
                    }
                }
            }

            public string RemoveSymbols(ref string text, char[] mask)
            {
                var o = new StringBuilder(text.Length);

                foreach (var c in text)
                {
                    if (c > 127 || mask[c] != ' ')
                    {
                        o.Append(c);
                    }
                }
                return o.ToString();
            }

            public string RemoveDirectorySymbols(string text)
            {
                var mask = _SymbolsValidDirectory;
                var o = new StringBuilder(text.Length);

                foreach (var c in text)
                {
                    if (c > 127 || mask[c] != '\0')
                    {
                        o.Append(c);
                    }
                }
                return o.ToString();
            }

            // This function takes a user search string, and fixes it for beatsaber.
            public string NormalizeBeatSaverString(string text)
            {
                var words = Split(text);
                StringBuilder result = new StringBuilder();
                foreach (var word in words)
                {
                    if (word.Length < 3)
                    {
                        continue;
                    }

                    if (BeatsaverBadWords.Contains(word.ToLower()))
                    {
                        continue;
                    }

                    result.Append(word);
                    result.Append(' ');
                }

                //RequestBot.Instance.QueueChatMessage($"Search string: {result.ToString()}");

                if (result.Length == 0)
                {
                    return "qwesartysasasdsdaa";
                }

                return result.ToString().Trim();
            }

            public string[] Split(string text)
            {
                var sb = new StringBuilder(text);
                ReplaceSymbols(sb, _SymbolsMap);
                string[] result = sb.ToString().ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                return result;
            }

            public char[] _SymbolsMap = new char[128];
            public char[] _SymbolsNoDash = new char[128];
            public char[] _SymbolsValidDirectory = new char[128];

            public StringNormalization()
            {
                for (char i = (char)0; i < 128; i++)
                {
                    _SymbolsMap[i] = i;
                    _SymbolsNoDash[i] = i;
                    _SymbolsValidDirectory[i] = i;
                }

                foreach (var c in new char[] { '@', '*', '+', ':', '-', '<', '~', '>', '(', ')', '[', ']', '/', '\\', '.', ',' })
                {
                    if (c < 128)
                    {
                        _SymbolsMap[c] = ' ';
                    }
                }

                foreach (var c in new char[] { '@', '*', '+', ':', '<', '~', '>', '(', ')', '[', ']', '/', '\\', '.', ',' })
                {
                    if (c < 128)
                    {
                        _SymbolsNoDash[c] = ' ';
                    }
                }

                foreach (var c in Path.GetInvalidPathChars())
                {
                    if (c < 128)
                    {
                        _SymbolsValidDirectory[c] = '\0';
                    }
                }

                _SymbolsValidDirectory[':'] = '\0';
                _SymbolsValidDirectory['\\'] = '\0';
                _SymbolsValidDirectory['/'] = '\0';
                _SymbolsValidDirectory['+'] = '\0';
                _SymbolsValidDirectory['*'] = '\0';
                _SymbolsValidDirectory['?'] = '\0';
                _SymbolsValidDirectory[';'] = '\0';
                _SymbolsValidDirectory['$'] = '\0';
                _SymbolsValidDirectory['.'] = '\0';

                // Incomplete list of words that BeatSaver.com filters out for no good reason. No longer applies!
                foreach (var word in new string[] { "pp" })
                {
                    BeatsaverBadWords.Add(word.ToLower());
                }
            }
        }

        public static StringNormalization StringNormalizer = new StringNormalization();
    }
}