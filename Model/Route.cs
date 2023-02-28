using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class Route<TKey,TValue> : SmartList<Route<TKey,TValue>>
    {
        public Route<TKey, TValue> Parent { get; set; } = null;
        public TKey Key { get; set; }
        public IEnumerable<TKey> Keys => from v in this select v.Key;
        public IEnumerable<TKey> KeysCollection => from v in Collection select v.Key;
        public TValue Value { get; set; }
        public IEnumerable<TValue> Values => from v in this select v.Value;
        public IEnumerable<TValue> ValuesCollection => from v in Collection select v.Value;
        public virtual IEnumerable<Route<TKey, TValue>> Collection
        {
            get
            {
                yield return this;
                foreach (var item in this)
                    foreach (var h in item.Collection)
                        yield return h;
            }
        }

        public Route(Route<TKey, TValue> cluster) : this(cluster.Key, cluster.Value, from v in cluster select new Route<TKey,TValue>(v))
        {
        }
        public Route(TKey key, TValue value, params Route<TKey, TValue>[] clusters) : this(key,value,clusters.AsEnumerable())
        {
        }
        public Route(TKey key, TValue value, IEnumerable<Route<TKey, TValue>> clusters) : this(key, value)
        {
            if (clusters != null) AddRange(clusters);
        }
        public Route(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public override Route<TKey, TValue>[] AddArray(params Route<TKey, TValue>[] array)
        {
            return base.AddArray((from v in array let p = v.Parent = this select v).ToArray());
        }





        public string IndentSign { get; set; } = "\t";
        public string StringFormat { get; set; } = "{0}{1}({3}):{4}{5}";
        public char[] TrimChars { get; set; } = { ' ', '\t','\r','\n' ,':'};
        public override string ToString() => ToString(0, StringFormat);
        public string ToString(int indent) => ToString(0, StringFormat);
        public string ToString(int indent,string format)
        {
            var newIndent = indent + 1;
            return string.Format(format,
                Statement.Recursive(indent, v => IndentSign + v, ""),
                Key,
                Value,
                Count,
                Environment.NewLine,
                string.Join(Environment.NewLine,from v in this select v.ToString(newIndent, format).TrimEnd(TrimChars)));
        }
    }
}
