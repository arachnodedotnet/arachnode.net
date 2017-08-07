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

namespace Arachnode.Structures.Value
{
    /// <summary>
    /// </summary>
    /// <typeparam name = "TValue">The type of the value.</typeparam>
    public class PriorityQueueItem<TValue>
    {
        private readonly double _priority;
        private readonly TValue _value;

        internal PriorityQueueItem<TValue> _higherPriorityIndexMarker;
        internal bool _isIndexMarker;
        internal PriorityQueueItem<TValue> _last;
        internal PriorityQueueItem<TValue> _lowerPriorityIndexMarker;
        internal PriorityQueueItem<TValue> _previous;

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "PriorityQueueItem&lt;TValue&gt;" /> class.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <param name = "priority">The priority.</param>
        internal PriorityQueueItem(TValue value, double priority)
        {
            _value = value;
            _priority = priority;
        }

        /// <summary>
        /// 	Gets or sets the higher priority index marker.
        /// </summary>
        /// <value>The higher priority index marker.</value>
        internal PriorityQueueItem<TValue> HigherPriorityIndexMarker
        {
            get { return _higherPriorityIndexMarker; }
            set { _higherPriorityIndexMarker = value; }
        }

        /// <summary>
        /// 	Gets or sets the lower priority index marker.
        /// </summary>
        /// <value>The lower priority index marker.</value>
        internal PriorityQueueItem<TValue> LowerPriorityIndexMarker
        {
            get { return _lowerPriorityIndexMarker; }
            set { _lowerPriorityIndexMarker = value; }
        }

        /// <summary>
        /// 	Gets or sets the previous.
        /// </summary>
        /// <value>The previous.</value>
        internal PriorityQueueItem<TValue> Previous
        {
            get { return _previous; }
            set { _previous = value; }
        }

        /// <summary>
        /// 	Gets or sets the last.
        /// </summary>
        /// <value>The last.</value>
        internal PriorityQueueItem<TValue> Last
        {
            get { return _last; }
            set { _last = value; }
        }

        /// <summary>
        /// 	Gets or sets a value indicating whether this instance is index marker.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is index marker; otherwise, <c>false</c>.
        /// </value>
        internal bool IsIndexMarker
        {
            get { return _isIndexMarker; }
            set { _isIndexMarker = value; }
        }

        /// <summary>
        /// 	Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public TValue Value
        {
            get { return _value; }
        }

        /// <summary>
        /// 	Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public double Priority
        {
            get { return _priority; }
        }
    }
}