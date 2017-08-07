using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Arachnode.Utilities
{
    public class ConsoleOutToRichTextBox : StringWriter
    {
        private object _writeLineLock = new object();

        private RichTextBox _richTextBox;
        private bool _scrollDownOnWriteLine;
        private int _maximumNumberOfLinesToDisplay;

        private int _numberOfLines;

        private delegate void WriteLineDelegate(string line);
        private WriteLineDelegate _writeLineDelegate;

        public ConsoleOutToRichTextBox(ref RichTextBox richTextBox, bool scrollDownOnWriteLine, int maximumNumberOfLinesToDisplay)
        {
            _richTextBox = richTextBox;
            _scrollDownOnWriteLine = scrollDownOnWriteLine;
            _maximumNumberOfLinesToDisplay = maximumNumberOfLinesToDisplay;
            
            _writeLineDelegate = new WriteLineDelegate(WriteLine);
        }

        public override void WriteLine(string line)
        {
            lock (_writeLineLock)
            {
                if (_richTextBox.InvokeRequired)
                {
                    _richTextBox.BeginInvoke(_writeLineDelegate, line);
                }
                else
                {
                    _numberOfLines += (line.Length - line.Replace("\n", string.Empty).Length) + 1;

                    while (_numberOfLines >= _maximumNumberOfLinesToDisplay)
                    {
                        int indexOfNewLine = _richTextBox.Text.IndexOf("\n");

                        _richTextBox.Text = _richTextBox.Text.Substring(indexOfNewLine + 1, _richTextBox.Text.Length - indexOfNewLine - 1);

                        _numberOfLines -= 1;
                    }

                    _richTextBox.Text += line;

                    if (!line.EndsWith(Environment.NewLine))
                    {
                        _richTextBox.Text += Environment.NewLine;
                    }

                    if (_scrollDownOnWriteLine)
                    {
                        _richTextBox.SelectionStart = _richTextBox.Text.Length;
                        _richTextBox.ScrollToCaret();
                    }

                    _richTextBox.Refresh();
                }
            }
        }
    }
}
