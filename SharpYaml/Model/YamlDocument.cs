﻿// Copyright (c) SharpYaml - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Collections.Generic;
using SharpYaml.Events;
using SharpYaml.Tokens;
using DocumentEnd = SharpYaml.Events.DocumentEnd;
using DocumentStart = SharpYaml.Events.DocumentStart;

namespace SharpYaml.Model
{
    public class YamlDocument : YamlNode {
        private DocumentStart _documentStart;
        private DocumentEnd _documentEnd;
        private YamlElement _contents;

        public YamlDocument() {
            _documentStart = new DocumentStart(null, new TagDirectiveCollection(), true);
            _documentEnd = new DocumentEnd(true);
        }

        YamlDocument(DocumentStart documentStart, DocumentEnd documentEnd, YamlElement contents) {
            this._documentStart = documentStart;
            this._documentEnd = documentEnd;
            this._contents = contents;
        }

        public static YamlDocument Load(EventReader eventReader) {
            var documentStart = eventReader.Allow<DocumentStart>();

            var contents = ReadElement(eventReader);

            var documentEnd = eventReader.Allow<DocumentEnd>();

            return new YamlDocument(documentStart, documentEnd, contents);
        }

        public override IEnumerable<ParsingEvent> EnumerateEvents() {
            yield return _documentStart;

            foreach (var evnt in _contents.EnumerateEvents()) {
                yield return evnt;
            }

            yield return _documentEnd;
        }

        public DocumentStart DocumentStart {
            get => _documentStart;
            set => _documentStart = value;
        }

        public DocumentEnd DocumentEnd {
            get => _documentEnd;
            set => _documentEnd = value;
        }

        public YamlElement Contents {
            get { return _contents; }
            set { _contents = value; }
        }

        public override YamlNode DeepClone() {
            var documentVersionCopy = _documentStart.Version == null
                ? null
                : new VersionDirective(_documentStart.Version.Version, _documentStart.Version.Start, _documentStart.Version.End);

            var documentTagsCopy = _documentStart.Tags == null ? null : new TagDirectiveCollection(_documentStart.Tags);

            var documentStartCopy = new DocumentStart(documentVersionCopy, documentTagsCopy, _documentStart.IsImplicit,
                _documentStart.Start, _documentStart.End);

            var documentEndCopy = new DocumentEnd(_documentEnd.IsImplicit, _documentEnd.Start, _documentEnd.End);

            return new YamlDocument(documentStartCopy, documentEndCopy, (YamlElement) Contents?.DeepClone());
        }
    }
}