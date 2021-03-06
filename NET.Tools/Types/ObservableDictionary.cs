﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace NET.Tools
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly IDictionary<TKey, TValue> dictionary;

        private Dispatcher dispatcher;

        #region Constructor

        public ObservableDictionary(Dispatcher dispatcher = null)
        {
            dictionary = new Dictionary<TKey, TValue>();
            this.dispatcher = dispatcher ??
                              (Application.Current != null
                                   ? Application.Current.Dispatcher
                                   : Dispatcher.CurrentDispatcher);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public ObservableDictionary(int capacity)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        #endregion

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (dispatcher.CheckAccess())
            {
                dictionary.Add(item);

                OnCollectionChanged(NotifyCollectionChangedAction.Add, item.Value);
                AddNotifyEvents(item.Value);
            } else
            {
                dispatcher.Invoke(new Action(() => Add(item)));
            }
        }

        public void Clear()
        {
            if (dispatcher.CheckAccess())
            {
                foreach (var value in Values)
                {
                    RemoveNotifyEvents(value);
                }

                dictionary.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            } else
            {
                dispatcher.Invoke(new Action(Clear));
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (dispatcher.CheckAccess())
            {
                bool result = dictionary.Remove(item);
                if (result)
                {
                    RemoveNotifyEvents(item.Value);
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, item.Value);
                }

                return result;
            } else
            {
                return (bool) dispatcher.Invoke(new Action(() => Remove(item)));
            }
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return dictionary.IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            if (dispatcher.CheckAccess())
            {
                dictionary.Add(key, value);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, value);
                AddNotifyEvents(value);
            } else
            {
                dispatcher.Invoke(new Action(() => Add(key, value)));
            }
        }

        public bool Remove(TKey key)
        {
            if (dispatcher.CheckAccess())
            {
                if (dictionary.ContainsKey(key))
                {
                    TValue value = dictionary[key];
                    dictionary.Remove(key);
                    RemoveNotifyEvents(value);
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, value);

                    return true;
                }

                return false;
            } else
            {
                return (bool) dispatcher.Invoke(new Action(() => Remove(key)));
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
            set
            {
                if (dispatcher.CheckAccess())
                {
                    if (dictionary.ContainsKey(key))
                        throw new ArgumentException("Unknown key: " + key);

                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, dictionary[key], value);
                    RemoveNotifyEvents(dictionary[key]);
                    AddNotifyEvents(value);

                    dictionary[key] = value;
                } else
                {
                    dispatcher.Invoke(new Action(() => this[key] = value));
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return dictionary.Values; }
        }

        #region Enumerator

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        #endregion

        #region Notify

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
            }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, TValue item)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item));
            }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, TValue oldItem, TValue newItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
            }
        }

        #endregion

        #region Private

        private void AddNotifyEvents(TValue item)
        {
            if (item is INotifyCollectionChanged)
            {
                (item as INotifyCollectionChanged).CollectionChanged += OnCollectionChangedEventHandler;
            }
            if (item is INotifyPropertyChanged)
            {
                (item as INotifyPropertyChanged).PropertyChanged += OnPropertyChangedEventHandler;
            }
        }

        private void RemoveNotifyEvents(TValue item)
        {
            if (item is INotifyCollectionChanged)
            {
                (item as INotifyCollectionChanged).CollectionChanged -= OnCollectionChangedEventHandler;
            }
            if (item is INotifyPropertyChanged)
            {
                (item as INotifyPropertyChanged).PropertyChanged -= OnPropertyChangedEventHandler;
            }
        }

        private void OnPropertyChangedEventHandler(object s, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        private void OnCollectionChangedEventHandler(object s, NotifyCollectionChangedEventArgs e)
        {
           OnPropertyChanged("value");
        }

        #endregion
    }
}
