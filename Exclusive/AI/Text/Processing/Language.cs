using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Text.Processing
{
    public class Language
    {
        public virtual string Separator { get; set; } = "¶";
        public virtual string RootDirectory { get; set; } = @"Exclusive\AI\Text\Processing\";
        public virtual string CorpusDirectory => RootDirectory + GetType().Name + @"\";
        public virtual string IncludePattern { get; set; } = "\\.pt$";
        public virtual string ExcludePattern { get; set; } = null;

        public RegexOptions ExpressionOptions { get; set; } = RegexOptions.IgnoreCase;
        public SmartDictionary<string, string> PreExpressions = new SmartDictionary<string, string>();
        public SmartDictionary<string,SmartDictionary<string, string>> CorpusExpressions = new SmartDictionary<string, SmartDictionary<string, string>>();
        public SmartDictionary<string, string> PostExpressions = new SmartDictionary<string, string>();

        public Language(string separator="¶", string includePattern = "\\.pt$", string excludePattern = null)
        {
            Separator = separator;
            IncludePattern = includePattern;
            ExcludePattern = excludePattern;
            Initialize();
        }
        public virtual void Initialize()
        {
            try { PathService.CreateAllDirectories(CorpusDirectory); } catch { }
            var paths = Directory.GetDirectories(CorpusDirectory).OrderBy(v => v).ToArray();
            PreExpressions = GetPatternsValues(paths.FirstOrDefault());
            foreach (var item in paths.Skip(1).Take(paths.Length - 2))
                CorpusExpressions.AddOrSet(Path.GetFileNameWithoutExtension(Regex.Replace(item, "^\\w\\s*-\\s*", "")).ToUpper().Trim(), GetPatternsValues(item));
            PostExpressions = GetPatternsValues(paths.LastOrDefault());
        }
        public virtual void Finalize()
        {
            PreExpressions.Clear();
            CorpusExpressions.Clear();
            PostExpressions.Clear();
        }

        /// <summary>
        /// To Normalize text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual string Normalization(string text, string level = null)
        {
            text = PreProcess(text);
            var ks = CorpusExpressions.Keys.ToArray();
            for (int i = 0;i < ks.Length && ks[i] != level; i++)
                text = CorpusProcess(text, CorpusExpressions[ks[i]]);
            return PostProcess(text);
        }
        /// <summary>
        /// To Normalize text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual string Normalization(string text, int level)
        {
            return Normalization(text, CorpusExpressions.Keys.ElementAtOrDefault(level));
        }
        /// <summary>
        /// To break text into chunks after Normalization
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> Tokenization(string text, string level = null)=> Segmentation(Normalization(text, level));
        /// <summary>
        /// To break text into chunks after Normalization
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> Tokenization(string text, int level)=> Segmentation(Normalization(text, level));
        public virtual IEnumerable<string> Segmentation(string text)
        {
            try { return (from kw in text.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries) where !string.IsNullOrWhiteSpace(kw) select kw.Trim()).Distinct(); }
            catch { return new string[] { }; }
        }


        public virtual string PreProcess(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            text = Separator + text.ToLower() + Separator;
            foreach (var item in PreExpressions)
                text = Regex.Replace(text, item.Key, item.Value, ExpressionOptions);
            return text;
        }
        public virtual string CorpusProcess(string text, SmartDictionary<string, string> expressions)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            foreach (var item in expressions)
                text = Regex.Replace(text, item.Key, item.Value, ExpressionOptions);
            return text;
        }
        public virtual string PostProcess(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            foreach (var item in PostExpressions)
                text = Regex.Replace(text, item.Key, item.Value, ExpressionOptions);
            return text.Trim(Separator.ToCharArray());
        }

        public virtual bool AreSame(string term1, string term2)
        {
            return AreSame(term1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                term2.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
        }
        public virtual bool AreSame(string[] termwords1, string[] termwords2)
        {
            if (termwords1.Length != termwords2.Length) return false;
            var ls = termwords1.ToList();
            return (from ws2 in termwords2 where ls.Exists((ws1) => ws1 == ws2) select ws2).Count() == termwords1.Length;
        }
        public virtual bool AreLike(string term1, string term2)
        {
            return AreLike(term1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                term2.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
        }
        public virtual bool AreLike(string[] termwords1, string[] termwords2)
        {
            if (termwords1.Length != termwords2.Length) return false;
            var ls = termwords1.ToList();
            return (from ws2 in termwords2 where ls.Exists((ws1) => ws1.Contains(ws2) || ws2.Contains(ws1)) select ws2).Count() == termwords1.Length;
        }

        public virtual int ComputeMaxSubTerms(int len)
        {
            if (len < 2) return 0;
            return len + ComputeMaxSubTerms(len - 1);
        }

        public SmartDictionary<string, string> GetPatternsValues(string dir)
        {
            SmartDictionary<string, string> pt = new SmartDictionary<string, string>();
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir)) return pt;
            foreach (var path in PathService.GetAllFilesInAllDirectoriesInPath(dir).OrderBy(v => v))
                if ((string.IsNullOrEmpty(IncludePattern) || Regex.IsMatch(path, IncludePattern, RegexOptions.IgnoreCase)) && (string.IsNullOrEmpty(ExcludePattern) || !Regex.IsMatch(path, ExcludePattern, RegexOptions.IgnoreCase)))
                foreach (var item in File.ReadLines(path))
                    {
                        string[] sta = item.Split(new string[] { "→" }, StringSplitOptions.None);
                        if (sta.First().Length < 1) continue;
                        pt.AddOrSet(
                            ConvertService.FromRegexPattern(sta.First()).Replace("¶", Separator),
                            sta.Length > 1 ? ConvertService.FromRegexPattern(sta.Last()).Replace("¶", Separator) : ""
                        );
                    }
            string[] keys = pt.Keys.ToArray();
            List<string> nkeys = new List<string>();
            foreach (var item in keys)
                if(string.IsNullOrEmpty(pt[item]))
                {
                    nkeys.Add(item);
                    pt.Remove(item);
                }
            if (nkeys.Count > 0) pt.AddOrSet("("+string.Join(")|(",nkeys)+")", "");
            return pt;
        }
    }
}
