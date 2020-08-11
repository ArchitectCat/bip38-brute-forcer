using System.Collections.Generic;
using System.IO;

namespace Bip38BruteForcer
{
    public class DictionaryGenerator
    {
        private readonly string[] _dictionaryFiles;
        private readonly int _bufferSize;

        public DictionaryGenerator(string[] dictionaryFiles, int bufferSize = 32768)
        {
            _dictionaryFiles = dictionaryFiles;
            _bufferSize = bufferSize;
        }

        public IEnumerable<string> GetEntries()
        {
            foreach (var filePath in _dictionaryFiles)
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read,
                    FileShare.Read, _bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
                using var reader = new StreamReader(stream);

                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;

                    yield return line;
                }
            }
        }
    }
}