using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Setting {
    public Vector2Int div;
    public List<int> pattern;
    public Setting() {
      div = new Vector2Int();
      pattern = new List<int>();
    }
    public Setting(Vector2Int div, List<int> pattern) {
      this.div = div;
      this.pattern = pattern;
    }
}

[Serializable]
public class JsonDictionary<TKey, TValue> : ISerializationCallbackReceiver {
  [Serializable]
  private struct KeyValuePair {
      [SerializeField][UsedImplicitly] private TKey key;
      [SerializeField][UsedImplicitly] private TValue value;
      public TKey Key => key;
      public TValue Value => value;
      public KeyValuePair(TKey key, TValue value) {
        this.key   = key;
        this.value = value;
      }
  }
  [SerializeField][UsedImplicitly] private KeyValuePair[] keyValuePairs = default;
  private SortedDictionary<TKey, TValue> dictionary;
  public SortedDictionary<TKey, TValue> Dictionary => dictionary;
  public JsonDictionary(SortedDictionary<TKey, TValue> d) {
    dictionary = d;
  }
  void ISerializationCallbackReceiver.OnBeforeSerialize() {
    keyValuePairs = dictionary.Select(p => new KeyValuePair(p.Key, p.Value)).ToArray();
  }
  void ISerializationCallbackReceiver.OnAfterDeserialize() {
    dictionary = new SortedDictionary<TKey, TValue>(keyValuePairs.ToDictionary(p => p.Key, p => p.Value));
    keyValuePairs = null;
  }
}
