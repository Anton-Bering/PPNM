using System;

public class genlist<T> {
    public T[] data;
    public int size = 0;
    private int capacity;

    public genlist(int capacity = 8) {
        this.capacity = capacity;
        data = new T[capacity];
    }

    public void add(T item) {
        if (size == capacity) {
            capacity *= 2;
            T[] newdata = new T[capacity];
            Array.Copy(data, newdata, size);
            data = newdata;
        }
        data[size] = item;
        size++;
    }

    public void remove(int i) {
        for (int j = i; j < size - 1; j++) data[j] = data[j + 1];
        size--;
    }

    public T this[int i] => data[i];
}
