using System;
using System.Collections;
using System.IO;
using System.Resources;

namespace MissingResources
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentNullException("Source");
            }

            if (!File.Exists(args[0]))
            {
                throw new FileNotFoundException($"Source file not found: {args[0]}");
            }

            string sourceFilename = args[0];

            if (args.Length == 1)
            {
                throw new ArgumentNullException("Target");
            }

            if (!File.Exists(args[1]))
            {
                throw new FileNotFoundException($"Target file not found: {args[1]}");
            }

            string targetFilename = args[1];

            var missingFilename = Path.GetFileNameWithoutExtension(targetFilename)
                + "-missing"
                + Path.GetExtension(targetFilename);

            var missingCount = 0;
            var foundCount = 0;

            using (var sourceReader = new ResXResourceReader(sourceFilename)
            {
                UseResXDataNodes = true
            })
            {
                using (var targetSet = new ResXResourceSet(targetFilename))
                {
                    using (var missingWriter = new ResXResourceWriter(missingFilename))
                    {
                        foreach (DictionaryEntry sourceEntry in sourceReader)
                        {
                            var targetEntry = targetSet.GetObject(sourceEntry.Key.ToString());
                            if (targetEntry == null)
                            {
                                missingCount++;
                                Console.WriteLine($"Missing key: '{sourceEntry.Key}'");
                                missingWriter.AddResource((ResXDataNode)sourceEntry.Value);
                            }
                            else
                            {
                                foundCount++;
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"Found {foundCount}, missing {missingCount} out of {foundCount + missingCount} total keys.");
            if (missingCount == 0)
            {
                File.Delete(missingFilename);
            }
            else
            {
                Console.WriteLine($"File '{missingFilename}' generated with the missing keys.");
            }
#if DEBUG
            Console.WriteLine("Hit any key to continue...");
            Console.ReadKey();
#endif
        }
    }
}
