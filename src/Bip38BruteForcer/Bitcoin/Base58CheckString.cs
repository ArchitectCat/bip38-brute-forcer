// Copyright 2012 Mike Caldwell (Casascius)
// This file is part of Bitcoin Address Utility.

// Bitcoin Address Utility is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Bitcoin Address Utility is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with Bitcoin Address Utility.  If not, see http://www.gnu.org/licenses/.

using System;

namespace Casascius.Bitcoin
{
    public class Base58CheckString
    {
        protected string _asString;
        protected byte[] _asBytes;

        public Base58CheckString(string fromstring) : base()
        {
            if (fromstring == null || fromstring == "")
            {
                throw new ArgumentException("Not a valid Base58Check string.  String is null or empty.");
            }

            this._asString = fromstring;
            _asBytes = Base58.ToByteArray(fromstring);
            if (_asBytes == null)
            {
                throw new ArgumentException("Not a valid Base58Check string.  Non-Base58 characters are present.");
            }

            // Now we just confirm the checksum.
            if (_asBytes.Length < 4)
            {
                // Too short for a checksum to exist
                throw new ArgumentException("Not a valid Base58Check string, checksum is not present");
            }

            int lengthWithoutChecksum = _asBytes.Length - 4;
            byte[] bytesWithoutChecksum = new byte[lengthWithoutChecksum];
            Array.Copy(_asBytes, bytesWithoutChecksum, lengthWithoutChecksum);

            // calculate the checksum
            byte[] sha256sum = Util.ComputeDoubleSha256(bytesWithoutChecksum);
            if (sha256sum[0] != _asBytes[lengthWithoutChecksum] ||
                sha256sum[1] != _asBytes[lengthWithoutChecksum + 1] ||
                sha256sum[2] != _asBytes[lengthWithoutChecksum + 2] ||
                sha256sum[3] != _asBytes[lengthWithoutChecksum + 3])
            {
                throw new ArgumentException("Not a valid Base58Check string, checksum is invalid");
            }

            // maintain only the value without the checksum
            _asBytes = bytesWithoutChecksum;
        }

        public Base58CheckString(byte[] frombytes) : base()
        {
            _asBytes = new byte[frombytes.Length];
            Array.Copy(frombytes, _asBytes, frombytes.Length);

            int lengthWithoutChecksum = frombytes.Length;
            byte[] withCheck = new byte[lengthWithoutChecksum + 4];
            Array.Copy(frombytes, withCheck, lengthWithoutChecksum);
            byte[] sha256sum = Util.ComputeDoubleSha256(frombytes);
            withCheck[lengthWithoutChecksum] = sha256sum[0];
            withCheck[lengthWithoutChecksum + 1] = sha256sum[1];
            withCheck[lengthWithoutChecksum + 2] = sha256sum[2];
            withCheck[lengthWithoutChecksum + 3] = sha256sum[3];
            _asString = Base58.FromByteArray(withCheck);
        }

        /// <summary>
        /// Returns the represented bytes as a byte array.
        /// A new copy is provided with each get.  Modifying the returned array does
        /// not modify the instance.
        /// </summary>
        public virtual byte[] AsBytes
        {
            get
            {
                byte[] rv = new byte[_asBytes.Length];
                Array.Copy(_asBytes, rv, _asBytes.Length);
                return rv;
            }
        }

        /// <summary>
        /// Returns the Base58-encoded string.
        /// </summary>
        public virtual string AsString
        {
            get
            {
                return _asString;
            }
        }
    }
}