#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
//  
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using Arachnode.Renderer.Value.Enums;
using mshtml;

#endregion

namespace Arachnode.Renderer.Value.EventArgs
{
    public class DocumentCompleteEventArgs
    {
        public DocumentCompleteEventArgs(IHTMLDocument2 htmlDocument, bool timedOut)
        {
            ReadyState = ReadyState.Complete;
            HtmlDocument = htmlDocument;
            TimedOut = timedOut;
        }

        public ReadyState ReadyState { get; private set; }
        public IHTMLDocument2 HtmlDocument { get; private set; }
        public bool TimedOut { get; set; }
    }

    public delegate void ReadyStateChangeHandler(object sender, ReadyStateChangeEventArgs e);

    public delegate void DocumentCompleteHandler(object sender, DocumentCompleteEventArgs e);
}