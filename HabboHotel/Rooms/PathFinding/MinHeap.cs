﻿using System;

namespace Plus.HabboHotel.Rooms.PathFinding
{
    sealed class MinHeap<T> where T : IComparable<T>
    {
        private int _count;
        private int _capacity;
        private T _temp;
        private T _mheap;
        private T[] _array;
        private T[] _tempArray;

        public int Count
        {
            get { return _count; }
        }

        public MinHeap() : this(16) { }

        public MinHeap(int capacity)
        {
            _count = 0;
            this._capacity = capacity;
            _array = new T[capacity];
        }

        public void BuildHead()
        {
            int position;
            for (position = (_count - 1) >> 1; position >= 0; position--)
            {
                MinHeapify(position);
            }
        }

        public void Add(T item)
        {
            _count++;
            if (_count > _capacity)
            {
                DoubleArray();
            }
            _array[_count - 1] = item;
            var position = _count - 1;

            var parentPosition = ((position - 1) >> 1);

            while (position > 0 && _array[parentPosition].CompareTo(_array[position]) > 0)
            {
                _temp = _array[position];
                _array[position] = _array[parentPosition];
                _array[parentPosition] = _temp;
                position = parentPosition;
                parentPosition = ((position - 1) >> 1);
            }
        }

        private void DoubleArray()
        {
            _capacity <<= 1;
            _tempArray = new T[_capacity];
            CopyArray(_array, _tempArray);
            _array = _tempArray;
        }

        private static void CopyArray(T[] source, T[] destination)
        {
            int index;
            for (index = 0; index < source.Length; index++)
            {
                destination[index] = source[index];
            }
        }

        public T ExtractFirst()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }
            _temp = _array[0];
            _array[0] = _array[_count - 1];
            _count--;
            MinHeapify(0);
            return _temp;
        }

        private void MinHeapify(int position)
        {
            do
            {
                var left = ((position << 1) + 1);
                var right = left + 1;
                int minPosition;

                if (left < _count && _array[left].CompareTo(_array[position]) < 0)
                {
                    minPosition = left;
                }
                else
                {
                    minPosition = position;
                }

                if (right < _count && _array[right].CompareTo(_array[minPosition]) < 0)
                {
                    minPosition = right;
                }

                if (minPosition != position)
                {
                    _mheap = _array[position];
                    _array[position] = _array[minPosition];
                    _array[minPosition] = _mheap;
                    position = minPosition;
                }
                else
                {
                    return;
                }

            } while (true);
        }
    }
}
