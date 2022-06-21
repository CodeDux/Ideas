using BenchmarkDotNet.Attributes;
using RmTrie = rm.Trie.Trie;

namespace Ideas.Trie;

[MemoryDiagnoser]
public class TrieBench
{
    RmTrie _rmTrie = null!;
    MyTrie _myTrie = null!;
    static readonly string[] Words = { "test", "testing", "this", "thisAndThat" };
    static readonly string[] Prefixes = { "tes", "th" };
    static readonly string[] UnknownWords = { "tesco", "testix", "thats" };

    [GlobalSetup]
    public void Setup()
    {
        _rmTrie = new RmTrie();
        _myTrie = new MyTrie();

        foreach (var word in Words)
        {
            _rmTrie.AddWord(word);
            _myTrie.AddWord(word);
        }
    }
    
    [Benchmark]
    public void MyTrie_Build()
    {
        var myTrie = new MyTrie();
        foreach (var word in Words)
        {
            myTrie.AddWord(word);
        }
    }
    
    [Benchmark]
    public void RmTrie_Build()
    {
        var rmTrie = new rm.Trie.Trie();
        foreach (var word in Words)
        {
            rmTrie.AddWord(word);
        }
    }
    
    [Benchmark]
    public void MyTrie_HasWord()
    {
        foreach (var word in Words)
        {
            if (!_myTrie.HasWord(word))
                throw new Exception();
        }
        
        foreach (var word in UnknownWords)
        {
            if (_myTrie.HasWord(word))
                throw new Exception();
        }
    }
    
    [Benchmark]
    public void RmTrie_HasWord()
    {
        foreach (var word in Words)
        {
            if (!_rmTrie.HasWord(word))
                throw new Exception();
        }
        
        foreach (var word in UnknownWords)
        {
            if (_rmTrie.HasWord(word))
                throw new Exception();
        }
    }
    
    [Benchmark]
    public void MyTrie_HasPrefix()
    {
        foreach (var prefix in Prefixes)
        {
            if (!_myTrie.HasPrefix(prefix))
                throw new Exception();
        }
    }
    
    [Benchmark]
    public void RmTrie_HasPrefix()
    {
        foreach (var prefix in Prefixes)
        {
            if (!_rmTrie.HasPrefix(prefix))
                throw new Exception();
        }
    }
    
    [Benchmark]
    public void MyTrie_GetWords_With_Prefix()
    {
        foreach (var prefix in Prefixes)
        {
            var result = _myTrie.GetWords(prefix).ToArray();
            foreach (var word in result)
            {
                if (!Words.Contains(word))
                    throw new Exception();
            }
        }
    }
    
    [Benchmark]
    public void RmTrie_GetWords_With_Prefix()
    {
        foreach (var prefix in Prefixes)
        {
            var result = _rmTrie.GetWords(prefix).ToArray();
            foreach (var word in result)
            {
                if (!Words.Contains(word))
                    throw new Exception();
            }
        }
    }
    
    [Benchmark]
    public void MyTrie_GetWords()
    {
        var result = _myTrie.GetWords().ToArray();
        foreach (var word in Words)
        {
            if (!result.Contains(word))
                throw new Exception();
        }
    }
    
    [Benchmark]
    public void RmTrie_GetWords()
    {
        var result = _rmTrie.GetWords().ToArray();
        foreach (var word in Words)
        {
            if (!result.Contains(word))
                throw new Exception();
        }
    }
}