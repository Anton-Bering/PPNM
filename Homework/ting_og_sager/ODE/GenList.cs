using System;
using System.Collections;
using System.Collections.Generic;
public class genlist<T> : IEnumerable<T> {
    private T[] data;
    private int _size;
    public int size => _size;
    public int Count => _size;
    public genlist(int capacity = 4) {
        data = new T[capacity];
        _size = 0;
    }
    public void add(T item) {
        if(_size == data.Length) {
            // expand capacity by factor of 2
            T[] newData = new T[data.Length * 2];
            Array.Copy(data, newData, data.Length);
            data = newData;
        }
        data[_size] = item;
        _size++;
    }
    public T this[int i] {
        get {
            if(i < 0 || i >= _size) throw new IndexOutOfRangeException("genlist index out of range");
            return data[i];
        }
        set {
            if(i < 0 || i >= _size) throw new IndexOutOfRangeException("genlist index out of range");
            data[i] = value;
        }
    }
    // Enable iteration (foreach) over genlist
    public IEnumerator<T> GetEnumerator() {
        for(int i = 0; i < _size; i++) yield return data[i];
    }
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
