using System.Collections.Generic;

namespace RoslynMacros.Common.Data
{
    public class CollectionType<T>
    {
        protected readonly Dictionary<string, T> Data = new Dictionary<string, T>();

        public virtual T this[string index]
        {
            get
            {
                if (Data.TryGetValue(index, out var v)) return v;
                Data[index] = default;
                return default;
            }
            set => Data[index] = value;
        }

        public void AddBulk(IEnumerable<(string, T)> kvs)
        {
            foreach (var (k, v) in kvs) this[k] = v;
        }
    }


    public class FlagVar : CollectionType<bool>
    {
        public void AddBulk(IEnumerable<string> flags)
        {
            foreach (var flag in flags) this[flag.StripArgument()] = true;
        }

        public void Set(string s) => this[s] = true;
        public void UnSet(string s) => this[s] = false;
    }

    public class CollectionStr : CollectionType<string>
    {
        public override string this[string index]
        {
            get
            {
                if (Data.TryGetValue(index, out var v)) return v;
                Data[index] = "";
                return "";
            }
            set => Data[index] = value ?? "";
        }

        public void Set(string var, string value) => this[var] = value;
    }

    public class CollectionInt : CollectionType<int>
    {
        public void Set(string vari, int i) => this[vari] = i;
    }

    public class CollectionVar : HashSet<string>
    {
        public void AddBulk(IEnumerable<string> items)
        {
            foreach (var i in items) Add(i);
        }
    }
}