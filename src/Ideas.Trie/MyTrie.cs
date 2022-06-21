using System.Diagnostics;
using System.Text;

namespace Ideas.Trie;

public class MyTrie
{
    [DebuggerDisplay($"{{{nameof(Value)},nq}}, {{{nameof(EndOfWord)}, nq}}")]
    struct Entry
    {
        public Entry[] Entries;
        public readonly char Value;
        public bool EndOfWord = false;

        public Entry(char value, bool endOfWord)
        {
            EndOfWord = endOfWord;
            Entries = Array.Empty<Entry>();
            Value = value;
        }
    }

    Entry[] _root = Array.Empty<Entry>();
    readonly bool _caseInsensitive;
    
    public MyTrie(bool caseInsensitive = false)
    {
        _caseInsensitive = caseInsensitive;
    }
    
    public void AddWord(string value)
    {
        // ReSharper disable once SuggestVarOrType_Elsewhere
        ref Entry[] entries = ref _root;

        var lastIndex = value.Length - 1;
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];

            if (_caseInsensitive && char.IsUpper(ch))
                ch = char.ToLowerInvariant(ch);
            
            var found = false;
            for (var j = 0; j < entries.Length; j++)
            {
                if (entries[j].Value == ch)
                {
                    if (i == lastIndex)
                    {
                        entries[j].EndOfWord = true;
                        return;
                    }
                    
                    entries = ref entries[j].Entries;
                    found = true;
                    break;
                }
            }
            
            if (!found)
            {
                var newEntries = new Entry[entries.Length + 1];
                entries.CopyTo(newEntries, 0);
                newEntries[^1] = new Entry(ch, i == lastIndex);
                
                entries = newEntries;
                entries = ref entries[^1].Entries;
            }
        }
    }
    
    public bool HasWord(ReadOnlySpan<char> value)
    {
        ReadOnlySpan<Entry> entries = _root;

        var lastIndex = value.Length - 1;
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];

            if (_caseInsensitive && char.IsUpper(ch))
                ch = char.ToLowerInvariant(ch);
            
            var found = false;
            for (var j = 0; j < entries.Length; j++)
            {
                if (entries[j].Value == ch)
                {
                    if (i == lastIndex)
                        return entries[j].EndOfWord;
                    
                    entries = entries[j].Entries;
                    found = true;
                    break;
                }
            }
            
            if (!found)
                return false;
        }

        return false;
    }
    
    public bool HasPrefix(ReadOnlySpan<char> value)
    {
        ReadOnlySpan<Entry> entries = _root;
        
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];

            if (_caseInsensitive && char.IsUpper(ch))
                ch = char.ToLowerInvariant(ch);
            
            var found = false;
            for (var j = 0; j < entries.Length; j++)
            {
                if (entries[j].Value == ch)
                {
                    entries = entries[j].Entries;
                    found = true;
                    break;
                }
            }
            
            if (!found)
                return false;
        }

        return true;
    }

    public IEnumerable<string> GetWords()
    {
        return GetWords(ReadOnlySpan<char>.Empty);
    }
    
    public IEnumerable<string> GetWords(ReadOnlySpan<char> prefix)
    {
        var stringBuilder = new StringBuilder();
        
        var entries = _root;
        
        for (var i = 0; i < prefix.Length; i++)
        {
            var ch = prefix[i];

            if (_caseInsensitive && char.IsUpper(ch))
                ch = char.ToLowerInvariant(ch);
            
            var found = false;
            for (var j = 0; j < entries.Length; j++)
            {
                if (entries[j].Value == ch)
                {
                    stringBuilder.Append(entries[j].Value);
                    entries = entries[j].Entries;
                    found = true;
                    break;
                }
            }
            
            if (!found)
                return Array.Empty<string>();
        }
        
        return GetWordsLoop(entries, stringBuilder);
     
        static IEnumerable<string> GetWordsLoop(Entry[] entries, StringBuilder stringBuilder)
        {
            foreach (var entry in entries)
            {
                stringBuilder.Append(entry.Value);

                if (entry.EndOfWord)
                {
                    yield return stringBuilder.ToString();
                }
                
                foreach (var word in GetWordsLoop(entry.Entries, stringBuilder))
                {
                    yield return word;
                }

                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }    
        }
    }
}