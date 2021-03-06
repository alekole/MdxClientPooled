﻿using System;
using System.Data;
using System.Data.Common;

namespace MdxClientPooled
{
    /// <summary>
    /// Represents a transaction to be performed on a SQL Server Analysis Services database.
    /// The transaction is a no-op, but is implemented in case a transaction is needed.
    /// </summary>
    public class MdxTransaction : DbTransaction
    {
        private MdxConnection _connection;

        internal MdxTransaction(MdxConnection connection)
        {
            if (null == connection)
                throw new ArgumentNullException("connection");

            _connection = connection;
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public override void Commit()
        { }

        /// <summary>
        /// Gets the MdxConnection that the MdxTransaction uses.
        /// </summary>
        public new MdxConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        protected override DbConnection DbConnection
        {
            get 
            {
                return Connection;
            }
        }

        /// <summary>
        /// Gets the isolation level that the MdxTransaction uses.  Always returns IsolationLevel.ReadCommitted.
        /// </summary>
        public override IsolationLevel IsolationLevel
        {
            get 
            {
                return IsolationLevel.ReadCommitted;
            }
        }

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        public override void Rollback()
        { }       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {                
                if (null != _connection)
                {
                    Rollback();
                }
                _connection = null;                
            }
            base.Dispose(disposing);
        }
    }
}
