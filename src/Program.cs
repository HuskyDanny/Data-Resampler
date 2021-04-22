using System;
using System.Text;
using System.Collections.Generic;
using System.Data.HashFunction.SpookyHash;

namespace HashFunction
{
    internal class Program
    {
        private const int HASHCOUNT = 1000;
        private const int PERCENTAGE = 10;

        private const int HASHSEED = 121;
        private const int SAMPLE_DATA_SIZE = 1000000;

        private static void Main(string[] args)
        {
            List<string> tenantIds = new List<string> { "walmart", "pepsi", "costco" };
            List<int> tenantIdsRatio = new List<int> { 89, 10, 1 };

            Dictionary<string, int> tenantIdSelectedCounts = new Dictionary<string, int>();
            foreach (var row in PrepareSampleData(tenantIds, tenantIdsRatio))
            {
                string tenantId = row.Item1;
                string assignmentId = row.Item2;

                var hasher = SpookyHashV2Factory.Instance.Create(new SpookyHashConfig());
                var dataResampler = new DataResampler(hasher, HASHCOUNT, HASHSEED, PERCENTAGE);

                if (dataResampler.ShouldKeep(tenantId, assignmentId))
                {
                    if (!tenantIdSelectedCounts.ContainsKey(tenantId))
                    {
                        tenantIdSelectedCounts[tenantId] = 0;
                    }
                    else
                    {
                        tenantIdSelectedCounts[tenantId] += 1;
                    }
                }
            }

            int totalCount = 0;
            foreach (var key in tenantIdSelectedCounts.Keys)
            {
                var count = tenantIdSelectedCounts[key];
                Console.WriteLine("{0}TrueCount : ${1}; ", key, count);
                totalCount += count;
            }

            Console.WriteLine("totalTrueCount : {0}", totalCount);
        }

        private static List<Tuple<string, string>> PrepareSampleData(List<string> tenantIds, List<int> tenantIdsRatio)
        {
            List<Tuple<string, string>> sampleData = new List<Tuple<string, string>>();

            int i = 0;

            while (i < SAMPLE_DATA_SIZE)
            {
                for (int j = 0; j < tenantIds.Count; j++)
                {
                    for (int k = 0; k < tenantIdsRatio[j]; k++)
                    {
                        sampleData.Add(Tuple.Create(tenantIds[j], Guid.NewGuid().ToString()));
                        i += 1;
                    }
                }
            }

            return sampleData;
        }
    }
}