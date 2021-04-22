using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.HashFunction.SpookyHash;

namespace HashFunction
{
    internal class DataResampler
    {
        private ISpookyHashV2 _hasher;

        private int _hashBucketCount;
        private int _hashSeed;

        private int _resamplingPercentage { get; set; }

        internal DataResampler(ISpookyHashV2 hasher, int hashBucketCount, int hashSeed, int resamplingPercentage)
        {
            this._hasher = hasher;
            this._hashBucketCount = hashBucketCount;
            this._hashSeed = hashSeed;
            this._resamplingPercentage = resamplingPercentage;
        }

        public bool ShouldKeep(string tenantId, string assignmentId)
        {
            string hashKey = tenantId + this._hashSeed + assignmentId;
            byte[] hashKeyBytes = Encoding.UTF8.GetBytes(hashKey);

            byte[] hashBytes = this._hasher.ComputeHash(hashKeyBytes).Hash;

            UInt32 hashBytesShouldRotate = 0;
            for (int i = 0; i < sizeof(int); i++)
            {
                hashBytesShouldRotate <<= 8;
                hashBytesShouldRotate |= hashBytes[i];
            }

            var rangeSize = _resamplingPercentage * 10;
            var hashValue = (int?)(hashBytesShouldRotate % _hashBucketCount);
            return hashValue <= (rangeSize - 1);
        }
    }
}