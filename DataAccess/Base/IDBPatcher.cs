using System;
using System.Data;

namespace DFCommonLib.DataAccess
{
    public interface IDBPatcher
    {
        void Init();
        bool Patch(int patchId, string sql);
        bool Successful();
    }
}