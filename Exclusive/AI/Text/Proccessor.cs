﻿using MiMFa.General;
using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Text
{
    public class Proccessor
    {
        public Regex WordsSplitter = new Regex("\\W+");
        public Percent NameSameness(string str1, string str2)
        {
            Percent p1 = Sameness(str1, str2);
            if (p1.Positive > 70) return p1;
            str1 = ConvertService.ToSeparatedWords(str1).ToLower();
            str2 = ConvertService.ToSeparatedWords(str2).ToLower();
            var lst1 = GetWords(str1);
            var lst2 = GetWords(str2);
            var maxc = Math.Max(lst1.Count, lst2.Count);
            if (maxc == 0) return p1; 
            double f = 100 / maxc;
            Percent p = new Percent(-f * (Math.Abs(lst1.Count - lst2.Count) + 1) / 2, 0,0);
            for (int i = 0; i < lst1.Count; i++)
            {
                bool find = false;
                for (int j = 0; j < lst2.Count; j++)
                    if (lst1[i].Length > lst2[j].Length)
                    { if (find = lst1[i].StartsWith(lst2[j])) { p.AddValue((lst2[j].Length * 1d/ lst1[i].Length) * f); break; } }
                    else
                    { if (find = lst2[j].StartsWith(lst1[i])) { p.AddValue((lst1[i].Length * 1d / lst2[j].Length) * f); break; } }
                if (!find) p.AddValue(-f);
            }
            return p1>p?p1: p;
        }

        public Percent Sameness(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, 0, 100);
            if (string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2)) return new Percent(-100, 0, 0);
            if (!string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, -100, 0);
            Percent percent;
            if ((percent = Comparsion(str1, str2)).Both > 70) return percent;
            var lst1 = GetWords(str1);
            var lst2 = GetWords(str2);
            if (Math.Max(lst1.Count, lst1.Count) == 0) return percent;
            if (lst1.Count < 2 && lst2.Count < 2) return percent;
            if ((lst1.Count >= 2 * lst2.Count) || (lst2.Count >= 2 * lst1.Count))
                return new Percent(-100, 0, 0);
            double unit = Convert.ToDouble(93) / ((lst2.Count + lst1.Count) / 2);
            percent = new Percent(-unit * (Math.Abs(lst1.Count - lst2.Count) + 1) / 2, 0, 0);
            Percent newf = new Percent(0, 0, 0);
            Percent maxf = new Percent(0, 0, 0);
            int maxindex = -1;
            for (int i = 0; i < lst1.Count; i++)
            {
                maxf = new Percent(0, 0, 0);
                for (int j = 0; j < lst2.Count; j++)
                {
                    newf = Comparsion(lst1[i], lst2[j]);
                    if (newf > maxf)
                    {
                        maxf.SetValue(newf);
                        maxindex = j;
                    }
                }
                if (maxf > 50)
                {
                    if (maxindex > -1) lst2.RemoveAt(maxindex);
                    percent.AddValue((maxf / 100) * unit);
                }
                else percent.AddValue(-unit);
            }
            return percent;
        }
        public Percent Comparsion(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, 0, 100);
            if (string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2)) return new Percent(-100, 0, 0);
            if (!string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, -100, 0);
            if (str1 == str2) return new Percent(0, 0, 100);
            str1 = str1.Trim().Replace(Environment.NewLine, " ").Replace("   ", " ").Replace("  ", " ");
            str2 = str2.Trim().Replace(Environment.NewLine, " ").Replace("   ", " ").Replace("  ", " ");
            if (str1 == str2) return new Percent(-1, 0, 99);
            int ct = str1.CompareTo(str2);
            if (ct == 0) return new Percent(-5, 0, 95);
            string str1l = str1.ToLower();
            string str2l = str2.ToLower();
            if (str1l == str2l)
            {
                Percent mm = new Percent(0, 0, 90);
                double fu = 10D / str1.Length;
                for (int i = 0; i < str1.Length; i++)
                    mm.AddValue((str1[i] == str2[i]) ? fu : -fu);
                return mm;
            }
            ct = str1l.CompareTo(str2l);
            if (ct == 0) return new Percent(-10, 0, 90);
            double unit = 100D / ((str2.Length + str1.Length) / 2D);
            int toler = Math.Abs(str1.Length - str2.Length);
            double tu = toler * unit/3F;
            if (str1.Contains(str2l) || str2l.Contains(str1))
                return (new Percent(-tu, 0, 100 - tu));
            if (str1l.Contains(str2l) || str2l.Contains(str1l))
                return (new Percent(-10 - tu, 0, 90 - tu));
            if (str1.Length < 2 || str2.Length < 2) return new Percent(-100, 0, 0);
            int m1 = str1.Length / 2;
            int m2 = str2.Length / 2;
            Percent percent1 = Comparsion(str1.Substring(0, m1), str2.Substring(0, m2)) / 2;
            Percent percent2 = Comparsion(str1.Substring(m1), str2.Substring(m2)) / 2;
            Percent percent = percent1 + percent2;
            if (percent.Positive > 60) return percent;
            string s1 = str1, s2 = str2;
            if (str1.Length < str2.Length)
            {
                s1 = str2;
                s2 = str1;
            }
            int j = 0;
            percent = new Percent(-unit * (Math.Abs(str1.Length - str2.Length) + 1) / 2, 0, 0);
            for (int i = 0; i < s2.Length; i++)
                if (s1[i] != s2[j++] && s1[i] != s2[(j>0)?j - 1: 0])
                    percent.AddValue(-unit);
                else
                    percent.AddValue(unit);
            percent.AddValue(-tu);
            return percent;
        }

        public Percent ContentSameness(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, 0, 100);
            if (string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2)) return new Percent(-100, 0, 0);
            if (!string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, -100, 0);
            Percent percent;
            if (((percent = ContentComparsion(str1, str2))) > 70) return percent;
            List<string> lst1 = GetKeywords(str1).ValueList;
            List<string> lst2 = GetKeywords(str2).ValueList;
            var nlst1 = GetWords(str1);
            var nlst2 = GetWords(str2);
            if (Math.Max(nlst1.Count, nlst1.Count) == 0) return percent;
            if (lst1.Count < 2 * nlst1.Count / 3 || lst2.Count < 2 * nlst2.Count / 3)
            {
                lst1 = nlst1;
                lst2 = nlst2;
            }
            else return ContentComparsion(CollectionService.GetAllItems(lst1, " "), CollectionService.GetAllItems(lst2, " "));
            if (lst1.Count < 2 && lst2.Count < 2) return percent;
            if ((lst1.Count >= 2 * lst2.Count) || (lst2.Count >= 2 * lst1.Count))
                return new Percent(-100, 0, 0);
            double unit = Convert.ToDouble(93) / ((lst2.Count + lst1.Count) / 2);
            percent = new Percent(-unit, 0, 0);
            Percent newf = new Percent(0, 0, 0);
            Percent maxf = new Percent(0, 0, 0);
            int maxindex = -1;
            for (int i = 0; i < lst1.Count; i++)
            {
                maxf = new Percent(0, 0, 0);
                for (int j = 0; j < lst2.Count; j++)
                {
                    newf = ContentComparsion(lst1[i], lst2[j]);
                    if (newf > maxf)
                    {
                        maxf.SetValue(newf);
                        maxindex = j;
                    }
                }
                if (maxf > 50)
                {
                    if (maxindex > -1) lst2.RemoveAt(maxindex);
                    percent.AddValue((maxf / 100) * unit);
                }
                else percent.AddValue(-unit);
            }
            return percent;
        }
        public Percent ContentComparsion(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, 0, 100);
            if (string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2)) return new Percent(-100, 0, 0);
            if (!string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, -100, 0);
            if (str1 == str2) return new Percent(0, 0, 100);
            str1 = str1.Trim().Replace(Environment.NewLine, " ").Replace("   ", " ").Replace("  ", " ");
            str2 = str2.Trim().Replace(Environment.NewLine, " ").Replace("   ", " ").Replace("  ", " ");
            if (str1 == str2) return new Percent(-1, 0, 99);
            int ct = str1.CompareTo(str2);
            if (ct == 0) return new Percent(-5, 0, 95);
            string str1l = str1.ToLower();
            string str2l = str2.ToLower();
            if (str1l == str2l)
            {
                Percent mm = new Percent(0, 0, 90);
                double fu = Convert.ToDouble(10) / str1.Length;
                for (int i = 0; i < str1.Length; i++)
                    mm.AddValue((str1[i] == str2[i]) ? fu : -fu);
                return mm;
            }
            ct = str1l.CompareTo(str2l);
            if (ct == 0) return new Percent(-10, 0, 90);
            Percent percent = ContentSameWordPercent(str1, str2);
            return percent;
        }
        public Percent ContentSameWordPercent(string str1, string str2)
        {
            Percent mp = new Percent(0, 0, 0);
            Percent nnmp = new Percent(0, 0, 0);
            str1 = (str1 + "").ToUpper();
            str2 = (str2 + "").ToUpper();
            var sa1 = GetWords(str1);
            var sa2 = GetWords(str2); 
            int l = Math.Abs(sa1.Count - sa2.Count);
            List<string> lmin = new List<string>();
            List<string> lmax = new List<string>();
            if (sa1.Count > sa2.Count) { lmin = sa2.ToList(); lmax = sa1.ToList(); }
            else { lmin = sa1.ToList(); lmax = sa2.ToList(); }
            if (l == 0) mp.AddValue(20, true);
            else if (l == 1) mp.AddValue(-50);
            else if ((lmin.Count - l) < 0) mp.AddValue(-100);
            else mp.AddValue(-50);
            double unit = lmin.Count ==0? 0 : 100 / lmin.Count;
            mp.AddValue(-unit * l);
            for (int i = 0; i < lmin.Count; i++)
            {
                for (int j = 0; j < Math.Min(lmax.Count, lmin.Count); j++)
                    if (lmin[i].Contains(lmax[j]) || lmax[j].Contains(lmin[i]))
                        if (lmin[i] == lmax[j])
                        {
                            mp.AddValue(unit);
                            lmax.RemoveAt(j);
                            break;
                        }
                        else
                        {
                            if (i == 0 || i >= lmin.Count - 1) mp.AddValue(-100);
                            else mp.AddValue(-unit);
                            lmax.RemoveAt(j);
                            break;
                        }
                    else if ((nnmp = Comparsion(lmin[i], lmax[j])).Positive > 60)
                    {
                        mp.AddValue(nnmp.Positive*unit/100);
                    }
                    else if (i >= lmin.Count - 1)
                    {
                        mp.AddValue(-1 * unit);
                    }
            }
            if (mp.Negative < -1 * unit && mp.Positive >= 100) mp.AddValue(-100);
            return mp;
        }

        public SmartKeyValueList<int, string> GetKeywords(string text)
        {
            SmartKeyValueList<int, string> kws = new SmartKeyValueList<int, string>();
            List<string> ls = 
                CollectionService.Distinct(
                CollectionService.ExecuteInAllItems(
                CollectionService.Concat(
                GetKeywordFromStruct(text),
                GetKeywordFromAnd(text),
                GetKeywordFromComma(text),
                GetKeywordFromNumber(text),
                GetKeywordFromEqual(text),
                GetKeywordFromParenthesis(text),
                GetKeywordFromBrackets(text),
                GetKeywordFromBraces(text),
                GetKeywordFromQuotation(text),
                GetKeywordFromDoubleQuotation(text)
                ),(t)=> t
                .Replace("."," ")
                .Replace(",", " ")
                .Replace("،", " ")
                .Replace("؛", " ")
                .Replace(";", " ")
                .Replace("?", " ")
                .Replace("؟", " ")
                .Replace("!", " ")
                .Replace("'", " ")
                .Replace("`", " ")
                .Replace("\"", " ")
                .Replace("(", " ")
                .Replace(")", " ")
                .Replace("[", " ")
                .Replace("]", " ")
                .Replace("{", " ")
                .Replace("}", " ")
                .Replace("&", " ")
                .Trim()));
            foreach (var item in ls)
                kws.Add(StringService.WordsNumber(text,item), item);
            return kws;
        }
        public List<string> GetKeywordFromAnd(string text)
        {
            List<string> ls = new List<string>();
            try
            {
                string[] sta = text.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sta.Length; i++)
                {
                    var st = sta[i].Split(new string[] { " ", "'", "`" }, StringSplitOptions.RemoveEmptyEntries);
                    if (i > 0) ls.Add(st.First());
                    if (i < sta.Length - 1) ls.Add(st.Last());
                }
            }
            catch { }

            return ls;
        }
        public List<string> GetKeywordFromComma(string text)
        {
            List<string> ls = new List<string>();
            try
            {
                string[] sta = text.Split(new string[] { ",", "،" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sta.Length; i++)
                {
                    var st = sta[i].Split(new string[] { " ", "'", "`" }, StringSplitOptions.RemoveEmptyEntries);
                    if (i > 0) ls.Add(st.First());
                    if (i < sta.Length - 1) ls.Add(st.Last());
                }
            }
            catch { }

            return ls;
        }
        public List<string> GetKeywordFromNumber(string text)
        {
            List<string> ls = new List<string>();
            try
            {
                string[] sta = text.Split(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sta.Length; i++)
                {
                    var st = sta[i].Split(new string[] { " ", "'", "`" }, StringSplitOptions.RemoveEmptyEntries);
                    if (i > 0) ls.Add(st.First());
                    if (i < sta.Length - 1) ls.Add(st.Last());
                }
            }
            catch { }
            return ls;
        }
        public List<string> GetKeywordFromEqual(string text)
        {
            List<string> ls = new List<string>();
            try
            {
                string[] sta = text.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sta.Length; i++)
                {
                    var st = sta[i].Split(new string[] { "=", "\n", ";", "؛" }, StringSplitOptions.RemoveEmptyEntries);
                    if (i > 0) ls.Add(st.First());
                    if (i < sta.Length - 1) ls.Add(st.Last());
                }
            }
            catch { }
            return ls;
        }
        public List<string> GetKeywordFromParenthesis(string text)
        {
            List<string> ls = StringService.WordsBetween(text, "(", ")", false);
            return ls;
        }


        public List<string> GetKeywordFromBrackets(string text)
        {
            List<string> ls = StringService.WordsBetween(text, "[", "]", false);
            return ls;
        }
        public List<string> GetKeywordFromBraces(string text)
        {
            List<string> ls = new List<string>();
            try
            {
                var sta = StringService.WordsBetween(text, "{", "}", false);
                for (int i = 0; i < sta.Count; i++)
                {
                    var st = sta[i].Split(new string[] { ",", "،", "\n", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (i > 0) ls.Add(st.First());
                    if (i < sta.Count - 1) ls.Add(st.Last());
                }
            }
            catch { }

            return ls;
        }
        public List<string> GetKeywordFromQuotation(string text)
        {
            List<string> ls = StringService.WordsBetween(text, "'", "'", false);
            return ls;
        }
        public List<string> GetKeywordFromDoubleQuotation(string text)
        {
            List<string> ls = StringService.WordsBetween(text, "\"", "\"", false);
            return ls;
        }
        public List<string> GetKeywordFromStruct(string text)
        {
            List<string> ls = new List<string>();
            List<string> sen = GetSentences(text);
            try
            {
                foreach (var item in sen)
                {
                    string[] sta = item.Split(new string[] { "  ", " ", "'", "`"}, StringSplitOptions.RemoveEmptyEntries);
                    if (sta.Length > 0 && sta.Length < 5) ls.AddRange(sta);
                    else
                        for (int i = 1; i < sta.Length; i++)
                            if (sta[i - 1].Length <= sta[i].Length || sta[i].Length == 1) ls.Add(sta[i]);
                }
            }
            catch { }

            return ls;
        }

        public List<string> GetSentences(string text)
        {
            return text.Split(new string[] { ".", ";", "؛", "\n", "!", "؟", "?" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public List<string> NamesListCompletion(List<string> namesList)
        {
            List<string> result = new List<string>();
            List<List<string>> com = new List<List<string>>();
            string[] splittor = new string[] { " ", "." };
            try
            {
                com = (from name in namesList
                       where Statement.And((from part in name.Split(splittor, StringSplitOptions.RemoveEmptyEntries)
                                            select part.Length > 1).ToArray())
                       select name.Split(splittor, StringSplitOptions.RemoveEmptyEntries).ToList()).ToList();
            }
            catch { }
            foreach (var item in namesList)
            {
                string[] arr = item.Split(splittor, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    result.Add(CollectionService.GetAllItems(com.Find((name)=>
                    {
                        if (arr.Length != name.Count) return false;
                        for (int i = 0; i < name.Count; i++)
                            if (!name[i].StartsWith(arr[i])) return false;
                        return true;
                    })," "));
                } catch { result.Add(item); }
            }

            return result;
        }

        public List<string> GetWords(string text)
        {
            return (from v in WordsSplitter.Split(text) where !string.IsNullOrWhiteSpace(v) select v).ToList();
        }
    }
    public class LikeComparer : IEqualityComparer<string>
    {
        Proccessor Comparer = new Proccessor();
        public bool Equals(string x, string y)=> Comparer.NameSameness(x, y).Both > 60;

        public int GetHashCode(string obj)=>obj.GetHashCode();
    }
    public class SameComparer : IEqualityComparer<string>
    {
        Proccessor Comparer = new Proccessor();
        public bool Equals(string x, string y)=> Comparer.NameSameness(x, y).Both > 80;

        public int GetHashCode(string obj)=>obj.GetHashCode();
    }
}
