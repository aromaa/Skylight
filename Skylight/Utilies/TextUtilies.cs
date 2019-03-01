using SkylightEmulator.Core;
using SkylightEmulator.Extanssions;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class TextUtilies
    {
        public static List<Word> Wordfilter = new List<Word>();

        public static void LoadWordfilter(DatabaseClient dbClient)
        {
            Logging.Write("Loading wordfilter... ");
            List<Word> newWords = new List<Word>();

            DataTable words = dbClient.ReadDataTable("SELECT * FROM wordfilter");
            if (words != null && words.Rows.Count > 0)
            {
                foreach(DataRow dataRow in words.Rows)
                {
                    Word word = new Word();
                    word.Word_ = dataRow["word"].ToString();
                    word.Replacement_ = dataRow["replacement"].ToString();
                    word.Strict_ = int.Parse(dataRow["strict"].ToString());
                    newWords.Add(word);
                }
            }

            TextUtilies.Wordfilter = newWords;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public static bool ContainsLink(string url)
        {
            if (url.ContainsIgnoreCase("http://") || url.ContainsIgnoreCase("www.") || url.ContainsIgnoreCase("https://"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidURL(string url)
        {
            if (url.StartsWithIgnoreCase("http://") || url.StartsWithIgnoreCase("www.") || url.StartsWithIgnoreCase("https://"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool StringToBool(string s)
        {
            return s == "1";
        }

        public static string BoolToString(bool b)
        {
            if (b)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        public static string FilterString(string str)
        {
            return FilterString(str, true, false);
        }

        public static string FilterString(string Input, bool FilterBreak, bool FilterSlash)
        {
            Input = Input.Replace(Convert.ToChar(1), ' ');
            Input = Input.Replace(Convert.ToChar(2), ' ');
            Input = Input.Replace(Convert.ToChar(9), ' ');

            if (FilterBreak)
            {
                Input = Input.Replace(Convert.ToChar(13), ' ');
            }

            if (FilterSlash)
            {
                Input = Input.Replace('\'', ' ');
            }
            return Input;
        }

        public static string CheckBlacklistedWords(string str)
        {
            if (TextUtilies.Wordfilter != null && TextUtilies.Wordfilter.Count > 0)
            {
                foreach (Word word in TextUtilies.Wordfilter)
                {
                    if (word.Strict_ == 0)
                    {
                        if (str.ToLower().Contains(word.Word_.ToLower()))
                        {
                            str = Regex.Replace(str, word.Word_, word.Replacement_, RegexOptions.IgnoreCase);
                        }
                    }
                    else if (word.Strict_ == 1)
                    {
                        if (str.ToLower().Contains(" " + word.Word_.ToLower() + " "))
                        {
                            str = Regex.Replace(str, word.Word_, word.Replacement_, RegexOptions.IgnoreCase);
                        }
                    }
                    else if (word.Strict_ == 2)
                    {
                        string cheaters = @"\s*";
                        Regex re = new Regex(@"\b(" + string.Join(cheaters, word.Word_.ToCharArray()) + @")\b", RegexOptions.IgnoreCase);
                        str = re.Replace(str, match =>
                        {
                            //return new string('*', match.Length);
                            return word.Replacement_;
                        });
                    }
                    else if (word.Strict_ == 3)
                    {
                        byte[] bytes = Encoding.Default.GetBytes(str);
                        string noSpecialCheracters = Encoding.UTF8.GetString(bytes);

                        noSpecialCheracters = noSpecialCheracters.Replace("ä", "a");
                        noSpecialCheracters = noSpecialCheracters.Replace("ö", "o");
                        noSpecialCheracters = noSpecialCheracters.Replace("å", "a");
                        noSpecialCheracters = noSpecialCheracters.Replace("á", "a");
                        noSpecialCheracters = noSpecialCheracters.Replace("ó", "o");
                        noSpecialCheracters = noSpecialCheracters.Replace("â", "a");
                        noSpecialCheracters = noSpecialCheracters.Replace("ã", "a");
                        noSpecialCheracters = noSpecialCheracters.Replace("è", "e");
                        noSpecialCheracters = noSpecialCheracters.Replace("é", "e");
                        noSpecialCheracters = noSpecialCheracters.Replace("ê", "e");
                        noSpecialCheracters = noSpecialCheracters.Replace("ë", "e");
                        noSpecialCheracters = noSpecialCheracters.Replace("ì", "i");
                        noSpecialCheracters = noSpecialCheracters.Replace("à", "a");
                        noSpecialCheracters = noSpecialCheracters.Replace("í", "i");
                        noSpecialCheracters = noSpecialCheracters.Replace("î", "i");
                        noSpecialCheracters = noSpecialCheracters.Replace("ñ", "n");
                        noSpecialCheracters = noSpecialCheracters.Replace("ï", "i");
                        noSpecialCheracters = noSpecialCheracters.Replace("ò", "o");
                        noSpecialCheracters = noSpecialCheracters.Replace("ó", "o");
                        noSpecialCheracters = noSpecialCheracters.Replace("ô", "o");
                        noSpecialCheracters = noSpecialCheracters.Replace("õ", "o");
                        noSpecialCheracters = noSpecialCheracters.Replace("ù", "u");
                        noSpecialCheracters = noSpecialCheracters.Replace("ú", "u");
                        noSpecialCheracters = noSpecialCheracters.Replace("û", "u");
                        noSpecialCheracters = noSpecialCheracters.Replace("ü", "u");
                        noSpecialCheracters = noSpecialCheracters.Replace("ý", "y");
                        noSpecialCheracters = noSpecialCheracters.Replace("ÿ", "y");
                        noSpecialCheracters = noSpecialCheracters.Replace("ç", "c");
                        noSpecialCheracters = noSpecialCheracters.Replace("ø", "o");
                        noSpecialCheracters = noSpecialCheracters.Replace("š", "s");
                        noSpecialCheracters = noSpecialCheracters.Replace("ž", "z");
                        noSpecialCheracters = noSpecialCheracters.Replace("ß", "b");


                        noSpecialCheracters = noSpecialCheracters.Replace("Ä", "A");
                        noSpecialCheracters = noSpecialCheracters.Replace("Â", "A");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ö", "O");
                        noSpecialCheracters = noSpecialCheracters.Replace("Å", "A");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ó", "O");
                        noSpecialCheracters = noSpecialCheracters.Replace("À", "A");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ã", "A");
                        noSpecialCheracters = noSpecialCheracters.Replace("È", "E");
                        noSpecialCheracters = noSpecialCheracters.Replace("É", "E");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ê", "E");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ë", "E");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ì", "I");
                        noSpecialCheracters = noSpecialCheracters.Replace("Í", "I");
                        noSpecialCheracters = noSpecialCheracters.Replace("Î", "I");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ñ", "n");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ï", "i");
                        noSpecialCheracters = noSpecialCheracters.Replace("Á", "A");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ò", "O");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ó", "O");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ô", "O");
                        noSpecialCheracters = noSpecialCheracters.Replace("Õ", "O");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ù", "U");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ú", "U");
                        noSpecialCheracters = noSpecialCheracters.Replace("Û", "U");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ü", "U");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ý", "Y");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ÿ", "Y");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ç", "C");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ø", "O");
                        noSpecialCheracters = noSpecialCheracters.Replace("Š", "s");
                        noSpecialCheracters = noSpecialCheracters.Replace("Ž", "z");


                        noSpecialCheracters = noSpecialCheracters.Replace("@", "A");
                        noSpecialCheracters = noSpecialCheracters.Replace("º", "o");
                        noSpecialCheracters = noSpecialCheracters.Replace("ª", "a");


                        noSpecialCheracters = noSpecialCheracters.Replace(".", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace(",", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("-", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("*", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("/", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("(", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace(")", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("[", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("]", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("{", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("}", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("\\", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("<", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace(">", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("'", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace(":", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace(";", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("~", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("_", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("^", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("#", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("@", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("!", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("+", " ");
                        noSpecialCheracters = noSpecialCheracters.Replace("`", " ");

                        string cheaters = @"\s*";
                        Regex re = new Regex(@"\b(" + string.Join(cheaters, word.Word_.ToCharArray()) + @")\b", RegexOptions.IgnoreCase);
                        if (re.IsMatch(noSpecialCheracters))
                        {
                            return word.Replacement_;
                        }
                        else
                        {
                            if (str.ToLower().Contains(word.Word_.ToLower()) || str.ToLower().Contains(" " + word.Word_.ToLower() + " "))
                            {
                                return word.Replacement_;
                            }
                        }
                    }
                    else
                    {
                        if (str.ToLower().Contains(" " + word.Word_.ToLower() + " "))
                        {
                            str = Regex.Replace(str, word.Word_, word.Replacement_, RegexOptions.IgnoreCase);
                        }
                    }
                }
            }

            return str;
        }

        public static bool HaveBlacklistedWords(string str)
        {
            return TextUtilies.CheckBlacklistedWords(str) != str;
        }

        public static string DoubleWithDotDecimal(double d)
        {
            return d.ToString().Replace(',', '.');
        }

        public static string MergeArrayToString(string[] array, int start, bool space = true)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (i >= start)
                {
                    if (space && i > start)
                    {
                        stringBuilder.Append(" ");
                    }
                    stringBuilder.Append(array[i]);
                }
            }
            return stringBuilder.ToString();
        }

        public static string FormatString(string str, GameClient session)
        {
            if (session != null)
            {
                if (session.GetHabbo() != null)
                {
                    str = str.Replace("%username%", session.GetHabbo().Username);
                }
            }

            return str;
        }

        public static string GenerateRandomString(int size, string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", Random random = null)
        {
            if (random == null)
            {
                random = RandomUtilies.GetRandom();
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                char ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
