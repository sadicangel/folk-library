namespace FolkLibrary.App.Models;
public sealed class NavigationParameters : IDictionary<string, object>
{
    private readonly Dictionary<string, object> _parameters = new();

    public object this[string key] { get => ((IDictionary<string, object>)_parameters)[key]; set => ((IDictionary<string, object>)_parameters)[key] = value; }

    public ICollection<string> Keys { get => ((IDictionary<string, object>)_parameters).Keys; }
    public ICollection<object> Values { get => ((IDictionary<string, object>)_parameters).Values; }
    public int Count { get => ((ICollection<KeyValuePair<string, object>>)_parameters).Count; }
    public bool IsReadOnly { get => ((ICollection<KeyValuePair<string, object>>)_parameters).IsReadOnly; }

    public void Add(string key, object value) => ((IDictionary<string, object>)_parameters).Add(key, value);
    public void Add(KeyValuePair<string, object> item) => ((ICollection<KeyValuePair<string, object>>)_parameters).Add(item);
    public void Clear() => ((ICollection<KeyValuePair<string, object>>)_parameters).Clear();
    public bool Contains(KeyValuePair<string, object> item) => ((ICollection<KeyValuePair<string, object>>)_parameters).Contains(item);
    public bool ContainsKey(string key) => ((IDictionary<string, object>)_parameters).ContainsKey(key);
    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, object>>)_parameters).CopyTo(array, arrayIndex);
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, object>>)_parameters).GetEnumerator();
    public bool Remove(string key) => ((IDictionary<string, object>)_parameters).Remove(key);
    public bool Remove(KeyValuePair<string, object> item) => ((ICollection<KeyValuePair<string, object>>)_parameters).Remove(item);
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => ((IDictionary<string, object>)_parameters).TryGetValue(key, out value);
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_parameters).GetEnumerator();
}