using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;

using Pilipala.Data.Resources;

namespace Pilipala.Data
{
    public class DBaseDataReader : DbDataReader
    {
        private readonly Stream _data;

        private readonly uint _recordCount;

        private bool _isClosed;

        private DBaseDataReader(Stream data)
        {
            _data = data;
            Version = GetVersion(_data);
            LastUpdated = GetLastUpdateDateTime(data);
            _recordCount = GetRecordCount(data);
        }

        public override int Depth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int FieldCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool HasRows
        {
            get
            {
                return _recordCount > 0;
            }
        }

        public override bool IsClosed
        {
            get
            {
                return _isClosed;
            }
        }

        public override int RecordsAffected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime LastUpdated { get; private set; }

        public int Version { get; private set; }

        public override object this[int ordinal]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override object this[string name]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static DBaseDataReader Create(Stream data)
        {
            return new DBaseDataReader(data);
        }

        public override void Close()
        {
            _data.Close();
            _isClosed = true;
        }

        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            throw new NotImplementedException();
        }

        private static DateTime GetLastUpdateDateTime(Stream data)
        {
            var year = 1900 + data.ReadByte();
            var month = data.ReadByte();
            var day = data.ReadByte();

            if (year < 1900 || month < 1 || month > 12 || day < 1 || day > 31)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            return new DateTime(year, month, day);
        }

        private static uint GetRecordCount(Stream data)
        {
            var bytes = new byte[4];
            if (data.Read(bytes, 0, 4) < 4)
            {
                throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }

            return BitConverter.ToUInt32(
                BitConverter.IsLittleEndian
                    ? bytes
                    : bytes.Reverse().ToArray(), 
                0);
        }

        private static int GetVersion(Stream data)
        {
            var type = data.ReadByte();
            var version = type & 7;
            if (type < 0)
            {
                version = 0;
            }

            switch (version)
            {
                case 3:
                case 4:
                    return version;
                default:
                    throw new InvalidOperationException(ErrorMessages.DBaseDataReader_InvalidFormat);
            }
        }
    }
}
