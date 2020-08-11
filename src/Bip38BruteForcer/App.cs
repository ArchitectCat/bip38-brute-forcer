using Casascius.Bitcoin;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bip38BruteForcer
{
    public class App
    {
        private const string HistoryFile = "brute_history.log";
        private const string ResultFile = "brute_result.log";

        private int _currentCounter = 0;
        private int _previousCounter = 0;
        private long _lastCheckTime = 0;

        public void Run()
        {
            var configuration = AppConfiguration.Load();

            // Output configuration
            Console.WriteLine("BIP38 BruteForcer is starting...");

            var maskedAddress = configuration.PublicAddress.Substring(0, 4)
                + string.Join("", Enumerable.Repeat<string>("*", configuration.PublicAddress.Length - 8))
                + configuration.PublicAddress.Substring(configuration.PublicAddress.Length - 4, 4);
            Console.WriteLine("Public Address: {0}", maskedAddress);

            var maskedPrivateKey = configuration.EncryptedPrivateKey.Substring(0, 4)
                + string.Join("", Enumerable.Repeat<string>("*", configuration.EncryptedPrivateKey.Length - 8))
                + configuration.EncryptedPrivateKey.Substring(configuration.EncryptedPrivateKey.Length - 4, 4);
            Console.WriteLine("Encrypted Private Key: {0}", maskedPrivateKey);

            Console.WriteLine("Using dictionary files:");

            foreach (var file in configuration.DictionaryFiles)
            {
                Console.WriteLine("\t{0}", file);
            }

            Console.WriteLine("Number of Workers: {0}",
                configuration.NumberOfWorkers > 0
                    ? configuration.NumberOfWorkers.ToString()
                    : "No Limit"
            );

            Console.WriteLine("Report Interval: {0} min(s)", configuration.ReportInterval);

            _lastCheckTime = DateTimeOffset.Now.Ticks;
            var reportInterval = TimeSpan.FromMinutes(configuration.ReportInterval);

            _currentCounter = GetLastEntryId();
            _previousCounter = _currentCounter;

            Console.WriteLine("Skipping {0} last entries...", _currentCounter);

            // Start processing
            var passwordList = new DictionaryGenerator(configuration.DictionaryFiles)
                .GetEntries()
                .Skip(_currentCounter);

            // Prepare key pair
            var keyPair = new Bip38KeyPair(configuration.EncryptedPrivateKey);

            Console.WriteLine("Starting brute force...");

            var options = new ParallelOptions() { MaxDegreeOfParallelism = configuration.NumberOfWorkers };
            Parallel.ForEach(passwordList, options, (entry, state, index) =>
            {
                // Report
                if (DateTimeOffset.Now.Ticks - _lastCheckTime > reportInterval.Ticks && _currentCounter > 0)
                {
                    _lastCheckTime = DateTimeOffset.Now.Ticks;

                    var speed = (_currentCounter - _previousCounter) / configuration.ReportInterval;
                    Interlocked.Exchange(ref _previousCounter, _currentCounter);

                    Console.WriteLine("Processed entries: {0} Speed: {1} entries/min", _currentCounter, speed);
                }

                // Decrypt
                if (keyPair.DecryptWithPassphrase(entry))
                {
                    var key = keyPair.GetUnencryptedPrivateKey();

                    if (string.Equals(configuration.PublicAddress, key.AddressBase58, StringComparison.Ordinal))
                    {
                        Console.WriteLine("### Valid password found!!!: '{0}' ###", entry);

                        SaveCurrentResult(entry, _currentCounter + 1);
                        return;
                    }
                }

                Interlocked.Increment(ref _currentCounter);
            });

            Console.WriteLine("Finished brute force. Total entries: {0}", _currentCounter + 1);
            Console.ReadLine();
        }

        public void SaveCurrentProgress()
        {
            if (!File.Exists(HistoryFile))
            {
                File.Create(HistoryFile);
            }
            File.WriteAllText(HistoryFile, $"{_currentCounter - 1}");
        }

        private int GetLastEntryId()
        {
            if (File.Exists(HistoryFile))
            {
                var history = File.ReadAllText(HistoryFile);

                if (!string.IsNullOrWhiteSpace(history))
                    return int.Parse(history);
            }
            return 0;
        }

        private void SaveCurrentResult(string password, int entryId)
        {
            if (!File.Exists(ResultFile))
            {
                File.Create(ResultFile);
            }
            File.WriteAllText(ResultFile, $"Password found: '{password}' - at Entry: {entryId}");
        }
    }
}