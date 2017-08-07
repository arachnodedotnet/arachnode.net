using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Arachnode.Utilities
{
    public class ConsoleOutToQueue : StringWriter
    {
        private object _writeLineLock = new object();
        private Queue<string> _queue = new Queue<string>();

        private int _numberOfLines;
        private int _maximumNumberOfLinesToDisplay;
        private bool _scrollDownOnWriteLine;

        public ConsoleOutToQueue(bool scrollDownOnWriteLine, int maximumNumberOfLinesToDisplay)
        {
            _scrollDownOnWriteLine = scrollDownOnWriteLine;
            _maximumNumberOfLinesToDisplay = maximumNumberOfLinesToDisplay;
        }

        public object WriteLineLock
        {
            get { return _writeLineLock; }
        }

        public Queue<string> Queue
        {
            get { return _queue; }
        }

        public override void WriteLine(string line)
        {
            lock (WriteLineLock)
            {
                Queue.Enqueue(line);
            }
        }

        public void UpdateRichTextBox(RichTextBox richTextBox)
        {
            string line = null;

            while (Queue.Count != 0)
            {
                line = Queue.Dequeue();

                _numberOfLines += (line.Length - line.Replace("\n", string.Empty).Length) + 1;

                while (_numberOfLines >= _maximumNumberOfLinesToDisplay)
                {
                    int indexOfNewLine = richTextBox.Text.IndexOf("\n");

                    richTextBox.Text = richTextBox.Text.Substring(indexOfNewLine + 1, richTextBox.Text.Length - indexOfNewLine - 1);

                    _numberOfLines -= 1;
                }

                richTextBox.Text += line;

                if (!line.EndsWith(Environment.NewLine))
                {
                    richTextBox.Text += Environment.NewLine;
                }

                if (_scrollDownOnWriteLine)
                {
                    richTextBox.SelectionStart = richTextBox.Text.Length;
                    richTextBox.ScrollToCaret();

                    richTextBox.Refresh();
                }
            }

            if (!_scrollDownOnWriteLine)
            {
                richTextBox.SelectionStart = richTextBox.Text.Length;
                richTextBox.ScrollToCaret();

                richTextBox.Refresh();
            }
        }
    }
}
