using System;
using System.Data;

namespace DFCommonLib.DataAccess
{
    public interface IDBPatcher
    {
        void Init(string system);
        bool Patch(string system, int patchId, string sql);
        bool Successful();
    }
}