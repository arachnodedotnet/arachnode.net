#region License : arachnode.net

// // Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
// //  
// // Permission is hereby granted, upon purchase, to any person
// // obtaining a copy of this software and associated documentation
// // files (the "Software"), to deal in the Software without
// // restriction, including without limitation the rights to use,
// // copy, merge and modify copies of the Software, and to permit persons
// // to whom the Software is furnished to do so, subject to the following
// // conditions:
// // 
// // LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// // 
// // The above copyright notice and this permission notice shall be
// // included in all copies or substantial portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// // OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// // HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// // FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// // OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System.Collections;
using System.Collections.Generic;
using Arachnode.Structures.Value;

#endregion

namespace Arachnode.Structures
{
    public class PriorityQueue<TValue> : IEnumerable<PriorityQueueItem<TValue>>
    {
        private readonly object _lock = new object();

        private readonly PriorityQueueItem<TValue> _lowerBound = new PriorityQueueItem<TValue>(default(TValue), double.MinValue);
        private readonly Dictionary<double, PriorityQueueItem<TValue>> _priorityQueue = new Dictionary<double, PriorityQueueItem<TValue>>();
        private readonly PriorityQueueItem<TValue> _upperBound = new PriorityQueueItem<TValue>(default(TValue), double.MaxValue);

        private int _count;
        private PriorityQueueItem<TValue> _currentIndexMarker;
        private PriorityQueueItem<TValue> _lastIndexMarker;

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "PriorityQueue&lt;TValue&gt;" /> class.
        /// </summary>
        public PriorityQueue()
        {
            _lowerBound.HigherPriorityIndexMarker = _upperBound;
            _upperBound.LowerPriorityIndexMarker = _lowerBound;
            _priorityQueue.Add(double.MinValue, _lowerBound);
            _priorityQueue.Add(double.MaxValue, _upperBound);
            _lastIndexMarker = _upperBound;
            _currentIndexMarker = _lowerBound;
        }

        /// <summary>
        /// 	Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _count;
                }
            }
        }

        #region IEnumerable<PriorityQueueItem<TValue>> Members

        /// <summary>
        /// 	Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// 	An <see cref = "T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator<PriorityQueueItem<TValue>> IEnumerable<PriorityQueueItem<TValue>>.GetEnumerator()
        {
            List<PriorityQueueItem<TValue>> values = new List<PriorityQueueItem<TValue>>();

            PriorityQueueItem<TValue> _marker = _upperBound;
            PriorityQueueItem<TValue> _item = null;

            while (_marker._lowerPriorityIndexMarker != null)
            {
                _item = _marker.Previous;

                while (_item != null)
                {
                    values.Add(_item);

                    _item = _item.Previous;
                }

                _marker = _marker._lowerPriorityIndexMarker;
            }

            return new List<PriorityQueueItem<TValue>>.Enumerator();
        }

        /// <summary>
        /// 	Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// 	An <see cref = "T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            List<PriorityQueueItem<TValue>> values = new List<PriorityQueueItem<TValue>>();

            PriorityQueueItem<TValue> _marker = _upperBound;
            PriorityQueueItem<TValue> _item = null;

            while (_marker._lowerPriorityIndexMarker != null)
            {
                _item = _marker.Previous;

                while (_item != null)
                {
                    values.Add(_item);

                    _item = _item.Previous;
                }

                _marker = _marker._lowerPriorityIndexMarker;
            }

            return values.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// 	Enqueues the specified value.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <param name = "priority">The priority.</param>
        public void Enqueue(TValue value, double priority)
        {
            lock (_lock)
            {
                //add index marker
                if (!_priorityQueue.ContainsKey(priority))
                {
                    PriorityQueueItem<TValue> indexMarker = new PriorityQueueItem<TValue>(default(TValue), priority);
                    indexMarker.IsIndexMarker = true;

                    _priorityQueue.Add(priority, indexMarker);

                    if (priority > _lastIndexMarker.Priority)
                    {
                        do
                        {
                            _lastIndexMarker = _lastIndexMarker.HigherPriorityIndexMarker;
                        } while (_lastIndexMarker.HigherPriorityIndexMarker != null && _lastIndexMarker.Priority < priority);

                        indexMarker.HigherPriorityIndexMarker = _lastIndexMarker;
                        indexMarker.LowerPriorityIndexMarker = _lastIndexMarker.LowerPriorityIndexMarker;

                        _lastIndexMarker.LowerPriorityIndexMarker.HigherPriorityIndexMarker = indexMarker;
                        _lastIndexMarker.LowerPriorityIndexMarker = indexMarker;
                    }
                    else if (priority < _lastIndexMarker.Priority)
                    {
                        do
                        {
                            _lastIndexMarker = _lastIndexMarker.LowerPriorityIndexMarker;
                        } while (_lastIndexMarker.LowerPriorityIndexMarker != null && _lastIndexMarker.Priority > priority);

                        indexMarker.LowerPriorityIndexMarker = _lastIndexMarker;
                        indexMarker.HigherPriorityIndexMarker = _lastIndexMarker.HigherPriorityIndexMarker;

                        _lastIndexMarker.HigherPriorityIndexMarker.LowerPriorityIndexMarker = indexMarker;
                        _lastIndexMarker.HigherPriorityIndexMarker = indexMarker;
                    }

                    _lastIndexMarker = indexMarker;
                }

                //update the current index marker
                if (priority > _currentIndexMarker.Priority)
                {
                    _currentIndexMarker = _priorityQueue[priority];
                }

                //manage the list
                PriorityQueueItem<TValue> priorityQueueItem = new PriorityQueueItem<TValue>(value, priority);

                if (_priorityQueue[priority].Previous == null)
                {
                    _priorityQueue[priority].Previous = priorityQueueItem;
                    _priorityQueue[priority].Last = priorityQueueItem;
                }
                else
                {
                    _priorityQueue[priority].Last.Previous = priorityQueueItem;
                    _priorityQueue[priority].Last = priorityQueueItem;
                }

                //increment the count
                _count++;
            }
        }

        /// <summary>
        /// 	Dequeues this instance.
        /// </summary>
        /// <returns></returns>
        public TValue Dequeue()
        {
            bool isValueAssigned = false;
            TValue value = default(TValue);

            lock (_lock)
            {
                if (_count > 0)
                {
                    do
                    {
                        if (_currentIndexMarker.Previous == null)
                        {
                            _currentIndexMarker = _currentIndexMarker.LowerPriorityIndexMarker;
                        }
                        else
                        {
                            value = _currentIndexMarker.Previous.Value;
                            isValueAssigned = true;
                            _currentIndexMarker.Previous = _currentIndexMarker.Previous.Previous;
                            if (_currentIndexMarker.Previous == null)
                            {
                                _currentIndexMarker.Last = null;
                            }
                        }
                    } while (_currentIndexMarker != null && !isValueAssigned);

                    //decrement the count
                    _count--;
                }
            }

            return value;
        }
    }
}