using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;

namespace DFCommonLib.DataAccess
{
    public class OracleDbDataReader : BaseDbDataReader
    {
        private readonly IDataReader _reader;
        private readonly string _commandText;
        private readonly Stopwatch _stopwatch;

        public OracleDbDataReader(IDataReader reader, string commandText) : base(reader)
        {
            _reader = reader;
            _commandText = commandText;

            //ActivityTracing.AddActivityTrace("DbDataReader", ActivityTracing.FilterMessage(commandText));
            _stopwatch = Stopwatch.StartNew();
        }

        public long FetchSize
        {
            get
            {
                var oracleReader = _reader as OracleDataReader;
                if (oracleReader != null)
                {
                    return oracleReader.FetchSize;
                }
                return 0;

            }
            set
            {
                var oracleReader = _reader as OracleDataReader;
                if (oracleReader != null)
                {
                    oracleReader.FetchSize = value;
                }
            }
        }


        public override void Dispose()
        {
            _stopwatch.Stop();
            //ActivityTracing.AddActivityTrace("DbDataReader", "Done reading: " + ActivityTracing.FilterMessage(_commandText), _stopwatch.ElapsedMilliseconds);
            base.Dispose();
        }
    }
}