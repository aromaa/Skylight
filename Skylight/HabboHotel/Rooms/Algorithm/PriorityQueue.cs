using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Algorithm
{
    internal class PriorityQueue<T, X> where T : IWeightAddable<X>
    {
        public List<T> InnerList;
        protected IComparer<T> mComparer;


        public PriorityQueue(IComparer<T> comparer, int size)
        {
            this.mComparer = comparer;
            this.InnerList = new List<T>(size);
        }

        protected virtual int OnCompare(int i, int j)
        {
            return this.mComparer.Compare(this.InnerList[i], this.InnerList[j]);
        }


        private int BinarySearch(T value)
        {
            int low = 0, high = this.InnerList.Count - 1, midpoint = 0;

            while (low <= high)
            {
                midpoint = (low + high) / 2;

                // check to see if value is equal to item in array
                if (this.mComparer.Compare(value, this.InnerList[midpoint]) == 0)
                {
                    return midpoint;
                }
                else if (this.mComparer.Compare(value, this.InnerList[midpoint]) == -1)
                    high = midpoint - 1;
                else
                    low = midpoint + 1;
            }

            // item was not found
            return low;
        }



        /// <summary>
        /// Push an object onto the PQ
        /// </summary>
        /// <param name="O">The new object</param>
        /// <returns>The index in the list where the object is _now_. This will change when objects are taken from or put onto the PQ.</returns>
        public void Push(T item)
        {
            int location = BinarySearch(item);
            this.InnerList.Insert(location, item);
        }

        /// <summary>
        /// Get the smallest object and remove it.
        /// </summary>
        /// <returns>The smallest object</returns>
        public T Pop()
        {
            if (this.InnerList.Count == 0)
                return default(T);
            T item = this.InnerList[0];
            this.InnerList.RemoveAt(0);
            return item;

        }

        public void Update(T element, X newValue)
        {
            this.InnerList.RemoveAt(BinarySearch(element));
            element.WeightChange = newValue;
            Push(element);
        }



    }
}
