using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieBuddy
{
  public interface ICacheDictionaryRemovalStrategy<TKey>
  {
    /// <summary>
    /// Initialize the strategy and pass the maximum number of allowed items
    /// </summary>
    /// <param name="maxSize">The maximum number of allowed items</param>
    void Initialize(int maxSize);

    /// <summary>
    /// Notify the strategy that a key was added to the base collection
    /// </summary>
    /// <param name="key">The key that was added</param>
    void KeyAdded(TKey key);

    /// <summary>
    /// Notify the strategy that a key was removed from the base collection
    /// </summary>
    /// <param name="key">The key that was removed</param>
    void KeyRemoved(TKey key);

    /// <summary>
    /// Notify the strategy that a key was accessed (retrieved by the user) in the base collection
    /// </summary>
    /// <param name="key">The key that was retrieved</param>
    void KeyAccessed(TKey key);

    /// <summary>
    /// Notify the strategy that the base collection was cleared
    /// </summary>
    void Clear();

    /// <summary>
    /// Get the most appropriate key to remove, this is called when the base collection runs out of space
    /// </summary>
    /// <returns>The key that the base collection will remove</returns>
    TKey GetKeyToRemove();
  }

  public class CacheDictionary<TKey, TValue> : IDictionary<TKey, TValue>
  {
    private Dictionary<TKey, TValue> _data;
    private int _maxSize;
    private ICacheDictionaryRemovalStrategy<TKey> _removalStrategy;

    public CacheDictionary(int maxSize, ICacheDictionaryRemovalStrategy<TKey> removalStrategy)
    {
      if (removalStrategy == null)
        throw new ArgumentNullException("removalStrategy");
      if (maxSize == 0)
        throw new ArgumentException("maxSize must be a positive integer value");
      _maxSize = maxSize;
      _removalStrategy = removalStrategy;
      _data = new Dictionary<TKey, TValue>();

      _removalStrategy.Initialize(maxSize);
    }

    #region IDictionaty Implementation

    public void Add(TKey key, TValue value)
    {
      if (_data.ContainsKey(key))
        _data.Add(key, value); //I want to throw the same exception as the internal dictionary for this case.

      if (_data.Count == _maxSize)
      {
        TKey keyToRemove = _removalStrategy.GetKeyToRemove();
        if (_data.ContainsKey(keyToRemove))
          _data.Remove(keyToRemove);
        else
          throw new Exception(String.Format("Could not find a valid key to remove from cache, key = {0}", key == null ? "null" : key.ToString()));
      }
      _data.Add(key, value);
      _removalStrategy.KeyAdded(key);
    }

    public bool ContainsKey(TKey key)
    {
      return _data.ContainsKey(key);
    }

    public ICollection<TKey> Keys
    {
      get {return _data.Keys;}
    }

    public bool Remove(TKey key)
    {
      bool result = _data.Remove(key);
      if (result)
        _removalStrategy.KeyRemoved(key);
      return result;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      bool result = _data.TryGetValue(key, out value);
      if (result)
        _removalStrategy.KeyAccessed(key);
      return result;
    }

    public ICollection<TValue> Values
    {
      get { return _data.Values; }
    }

    public TValue this[TKey key]
    {
      get
      {
        TValue result = _data[key];
        _removalStrategy.KeyAccessed(key);
        return result;
      }
      set
      {
        _data[key] = value;
        _removalStrategy.KeyAccessed(key);
      }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      Add(item.Key, item.Value);
    }

    public void Clear()
    {
      _data.Clear();
      _removalStrategy.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return _data.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      ((IDictionary<TKey, TValue>)_data).CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _data.Count; }
    }

    public bool IsReadOnly
    {
      get { return ((IDictionary<TKey, TValue>)_data).IsReadOnly; }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      bool result = ((IDictionary<TKey, TValue>)_data).Remove(item);
      if (result)
        _removalStrategy.KeyRemoved(item.Key);
      return result;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return new CacheDictionaryEnumerator(_data.GetEnumerator(), _removalStrategy);
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return new CacheDictionaryEnumerator(_data.GetEnumerator(), _removalStrategy);
    }
    #endregion

    public class CacheDictionaryEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
      private IEnumerator<KeyValuePair<TKey, TValue>> _innerEnumerator;
      private ICacheDictionaryRemovalStrategy<TKey> _removalStrategy;

      internal CacheDictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> innerEnumerator, ICacheDictionaryRemovalStrategy<TKey> removalStrategy)
      {
        if (innerEnumerator == null)
          throw new ArgumentNullException("innerEnumerator");
        if (removalStrategy == null)
          throw new ArgumentNullException("removalStrategy");

        _innerEnumerator = innerEnumerator;
        _removalStrategy = removalStrategy;
      }

      public KeyValuePair<TKey, TValue> Current
      {
        get
        {
          KeyValuePair<TKey, TValue> result = _innerEnumerator.Current;
          _removalStrategy.KeyAccessed(result.Key);
          return result;
        }
      }

      public void Dispose()
      {
        _innerEnumerator.Dispose();
        _innerEnumerator = null;
      }

      object System.Collections.IEnumerator.Current
      {
        get {return this.Current;}
      }

      public bool MoveNext()
      {
        return _innerEnumerator.MoveNext();
      }

      public void Reset()
      {
        _innerEnumerator.Reset();
      }
    }
  }



  /// <summary>
  /// A removal strategy that removes some item.
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  public class EmptyRemovalStrategy<TKey> : ICacheDictionaryRemovalStrategy<TKey>
  {
    private HashSet<TKey> _currentKeys;

    public void Initialize(int maxSize)
    {
      _currentKeys = new HashSet<TKey>();
    }

    public void KeyAdded(TKey key)
    {
      if (!_currentKeys.Contains(key))
        _currentKeys.Add(key);
    }

    public void KeyRemoved(TKey key)
    {
      if (_currentKeys.Contains(key))
        _currentKeys.Remove(key);
    }

    public void KeyAccessed(TKey key)
    {

    }

    public TKey GetKeyToRemove()
    {
      if (_currentKeys.Count == 0)
        throw new IndexOutOfRangeException("No key to remove because the internal collection is empty");
      TKey key = _currentKeys.First();
      _currentKeys.Remove(key);
      return key;
    }

    public void Clear()
    {
      _currentKeys.Clear();
    }
  }

  /// <summary>
  /// Remove the most recently used (MRU) item from the cache
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  public class MruRemovalStrategy<TKey> : ICacheDictionaryRemovalStrategy<TKey>
  {
    private List<TKey> _items;

    public void Initialize(int maxSize)
    {
      _items = new List<TKey>(maxSize);
    }

    public void KeyAdded(TKey key)
    {
      _items.Add(key);
    }

    public void KeyRemoved(TKey key)
    {
      _items.Remove(key);
    }

    public void KeyAccessed(TKey key)
    {
      _items.Remove(key);
      _items.Add(key);
    }

    public void Clear()
    {
      _items.Clear();
    }

    public TKey GetKeyToRemove()
    {
      if (_items.Count == 0)
        throw new IndexOutOfRangeException("No key to remove because the internal collection is empty");
      TKey key = _items.Last();
      _items.Remove(key);
      return key;
    }
  }

  /// <summary>
  /// Removes the least recently used (LRU) item in the cache
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  public class LruRemovalStrategy<TKey> : ICacheDictionaryRemovalStrategy<TKey>
  {
    private List<TKey> _items;

    public void Initialize(int maxSize)
    {
      _items = new List<TKey>(maxSize);
    }

    public void KeyAdded(TKey key)
    {
      _items.Add(key);
    }

    public void KeyRemoved(TKey key)
    {
      _items.Remove(key);
    }

    public void KeyAccessed(TKey key)
    {
      _items.Remove(key);
      _items.Add(key);
    }

    public void Clear()
    {
      _items.Clear();
    }

    public TKey GetKeyToRemove()
    {
      if (_items.Count == 0)
        throw new IndexOutOfRangeException("No key to remove because the internal collection is empty");
      TKey key = _items.First();
      _items.Remove(key);
      return key;
    }
  }


}
