using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;

namespace ODataApi.DBConnection
{
	public class TransactionManager
	{
		#region Fields

		private SqlTransaction _Transaction;
		private SqlConnection _Connection;
		private bool _IsTransactionOpen = false; 

		#endregion

		#region Properties

		private string ConnectionString
		{
			get { return ConfigurationManager.ConnectionStrings["ConnString"].ToString(); }
		}

		public SqlTransaction Transaction
		{
			get { return _Transaction; }
		}

		public SqlConnection Connection
		{
			get { return _Connection; }
		}

		public bool IsOpen
		{
			get { return _IsTransactionOpen; }
		}

		#endregion

		#region Methods

		public void BeginTransaction()
		{
			BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public void BeginTransaction(IsolationLevel pIsolationLevel)
		{
			if ((_IsTransactionOpen == true))
			{
				throw new Exception("Transaction already open.");
			}
			else
			{
				try
				{
					_Connection = new SqlConnection(ConnectionString);
					_Connection.Open();
					_Transaction = _Connection.BeginTransaction(pIsolationLevel);
					_IsTransactionOpen = true;
				}
				catch (Exception ex)
				{
					if ((_Connection != null))
					{
						_Connection.Close();
					}
					if ((_Transaction != null))
					{
						_Transaction.Dispose();
					}
					_IsTransactionOpen = false;
					throw ex;
				}
			}
		}

		public void Commit()
		{
			if ((_IsTransactionOpen == true))
			{
				try
				{
					_Transaction.Commit();
				}
				finally
				{
					if ((_Connection != null))
					{
						_Connection.Close();
					}
					if ((_Transaction != null))
					{
						_Transaction.Dispose();
					}
					_IsTransactionOpen = false;
				}
			}
			else
			{
				throw new Exception("Transaction not started.");
			}
		}

		public void RollBack()
		{
			if ((_IsTransactionOpen == true))
			{
				try
				{
					_Transaction.Rollback();
				}
				finally
				{
					if ((_Connection != null))
					{
						_Connection.Close();
					}
					if ((_Transaction != null))
					{
						_Transaction.Dispose();
					}
					_IsTransactionOpen = false;
				}
			}
			else
			{
				throw new Exception("Transaction not started");
			}
		}

		#endregion
	}
}