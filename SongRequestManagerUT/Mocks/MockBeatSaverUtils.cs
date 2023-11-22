using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongRequestManagerUT.Mocks
{
    public static class MockBeatSaverUtils
    {
        public static Task<string> MockFetchSongMetadataAsync(string id)
        {
            var filePath = $@"Mocks\MockData\Songs\{id.ToLower()}.json";
            Debug.WriteLine($"Evaluating file path {filePath}");

            if (File.Exists(filePath))
            {
                Debug.WriteLine("Found mock song data");
                return Task.FromResult(File.ReadAllText(filePath));
            }

            Debug.WriteLine("No mock data found");
            return Task.FromResult<string>(null);
        }
    }
}
