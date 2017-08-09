using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserCookieManager
{
    public class CookieFileReader : IDisposable
    {
        #region constructors
        public CookieFileReader(string filePath)
            : this(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        { }

        public CookieFileReader(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("reader");
            if (stream.Position != 0) stream.Seek(0, SeekOrigin.Begin);

            _stream = stream;
            Reset();
            Next();
        }
        #endregion

        #region fields
        private const int ItemLineCount = 9;
        private const int InvalidItemIndex = -1;
        private static readonly DateTime BaseUTC = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private Stream _stream;
        private StreamReader _reader;
        private int _currentLine;
        private int _itemIndex;
        #endregion

        #region public methods
        public string GetName()
        {
            CheckItemIndex();
            string name = ReadLine(0 + _itemIndex * ItemLineCount);
            return name;
        }

        public string GetValue()
        {
            CheckItemIndex();
            string encryptedValue = ReadLine(1 + _itemIndex * ItemLineCount);
            return encryptedValue;
        }

        public CookieOriginalHost GetOriginalHost()
        {
            CheckItemIndex();
            string originalHost = ReadLine(2 + _itemIndex * ItemLineCount);
            return new CookieOriginalHost(originalHost);
        }

        public void Reset()
        {
            _itemIndex = InvalidItemIndex;
        }

        public bool Next()
        {
            if (_itemIndex == InvalidItemIndex)
            {
                _itemIndex = 0;
                if (_currentLine != 0 || _reader == null)
                {
                    _currentLine = 0;
                    ResetReader();
                }
                return true;
            }

            while (!_reader.EndOfStream)
            {
                string text = _reader.ReadLine();
                ++_currentLine;

                if (text == "*") break;
            }

            if (!_reader.EndOfStream)
            {
                if (_stream.Length > _stream.Position + ItemLineCount)
                {
                    ++_itemIndex;
                    return true;
                }
            }

            return false;
        }

        public List<Cookie> GetCookies()
        {
            List<Cookie> cookies = new List<Cookie>();

            Reset();
            while (Next())
            {
                Cookie ck = new Cookie();
                ck.Name = GetName();
                ck.Value = GetValue();

                var originalHost = GetOriginalHost();
                ck.Domain = originalHost.Domain;
                ck.Host = originalHost.Host;
                ck.Path = originalHost.Path;

                cookies.Add(ck);
            }

            return cookies;
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            _reader?.Dispose();
            _stream?.Dispose();
            _reader = null;
            _stream = null;
        }
        #endregion

        #region private methods
        private void CheckItemIndex()
        {
            if (_itemIndex == InvalidItemIndex) throw new InvalidOperationException("Should call Next() firstly.");
        }

        private string ReadLine(int lineIndex)
        {
            if (_currentLine < lineIndex)
            {
                for (int i = _currentLine; i < lineIndex; i++) _reader.ReadLine();
            }
            else if (_currentLine > lineIndex)
            {
                ResetReader();
                for (int i = 0; i < lineIndex; i++) _reader.ReadLine();
            }

            string text = _reader.ReadLine();
            _currentLine = lineIndex + 1;
            return text;
        }

        private void ResetReader()
        {
            _reader?.Dispose();

            if (_stream.Position != 0) _stream.Seek(0, SeekOrigin.Begin);

            _reader = new StreamReader(_stream, Encoding.UTF8, true, 1024, true);
        }
        #endregion
    }
}
