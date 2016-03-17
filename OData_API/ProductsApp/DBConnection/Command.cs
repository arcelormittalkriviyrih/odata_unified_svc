using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ODataApi.DBConnection
{
	public class DbCommand
	{
		#region Fields

		public SqlCommand _Command;
		private TransactionManager _TransManager = null;
		private string _ConnectionString;

		#endregion

		#region Constructors

		public DbCommand()
		{
			_Command = new SqlCommand();
		}

		public DbCommand(string pCommandText, CommandType pCommandType)
		{
			_Command = new SqlCommand(pCommandText);
			_Command.CommandType = pCommandType;
		}

		public DbCommand(TransactionManager pTransManager)
		{
			_TransManager = pTransManager;
		}

		public DbCommand(string pCommandText, CommandType pCommandType, TransactionManager pTransManager)
		{
			_TransManager = pTransManager;
			_Command = new SqlCommand(pCommandText);
			_Command.CommandType = pCommandType;
		}

		#endregion

		#region Properties

		public string ConnectionString
		{
			get
			{
				return _ConnectionString;
			}
			set
			{
				_ConnectionString = value;
			}
		}

		public string CommandText
		{
			get { return _Command.CommandText; }
			set { _Command.CommandText = value; }
		}

		public CommandType CommandType
		{
			get { return _Command.CommandType; }
			set { _Command.CommandType = value; }
		}

		public TransactionManager TransactionManager
		{
			get { return _TransManager; }
			set { _TransManager = value; }
		}

		#endregion

		#region Parameter Methods

		public void AddInParameter(string pName, DbType pDbType, object pValue)
		{
			SqlParameter sqlParam = new SqlParameter(pName, pDbType);
			sqlParam.Value = pValue;

			if ((pValue != null))
			{
				sqlParam.Value = pValue;
			}
			else
			{
				sqlParam.Value = null;
			}

			sqlParam.Direction = ParameterDirection.Input;
			_Command.Parameters.Add(sqlParam);
		}

		public void AddOutParameter(string pName, DbType pDbType)
		{
			SqlParameter sqlParam = new SqlParameter(pName, pDbType);
			sqlParam.Direction = ParameterDirection.Output;
			_Command.Parameters.Add(sqlParam);
		}

		#endregion

		#region Execute Methods

		public DataSet ExecuteDataset()
		{
			DataSet resultDs = new DataSet();
			SqlDataAdapter sqlAdapter = new SqlDataAdapter();
			try
			{
				if ((_TransManager != null))
				{
					_Command.Connection = _TransManager.Connection;
					_Command.Transaction = _TransManager.Transaction;
				}
				else
				{
					_Command.Connection = new SqlConnection(ConnectionString);
					_Command.Connection.Open();
				}
				sqlAdapter.SelectCommand = _Command;
				sqlAdapter.Fill(resultDs);
			}
			finally
			{
				if ((_TransManager == null))
				{
					if ((_Command.Connection != null))
					{
						_Command.Connection.Close();
					}
				}
			}
			return resultDs;
		}

		public IDataReader ExecuteReader()
		{
			IDataReader dataRdr;
			if ((_TransManager != null))
			{
				_Command.Connection = _TransManager.Connection;
				_Command.Transaction = _TransManager.Transaction;
			}
			else
			{
				_Command.Connection = new SqlConnection(ConnectionString);
				_Command.Connection.Open();
			}
			dataRdr = _Command.ExecuteReader();
			return dataRdr;
		}

		public object ExecuteScalar()
		{
			object resultObj;
			try
			{
				if ((_TransManager != null))
				{
					_Command.Connection = _TransManager.Connection;
					_Command.Transaction = _TransManager.Transaction;
				}
				else
				{
					_Command.Connection = new SqlConnection(ConnectionString);
					_Command.Connection.Open();
				}
				resultObj = _Command.ExecuteScalar();
			}
			finally
			{
				if ((_TransManager == null))
				{
					if ((_Command.Connection != null))
					{
						_Command.Connection.Close();
					}
				}
			}
			return resultObj;
		}

		public int ExecuteNonQuery()
		{
			int affectedRow;
			try
			{
				if ((_TransManager != null))
				{
					_Command.Connection = _TransManager.Connection;
					_Command.Transaction = _TransManager.Transaction;
				}
				else
				{
					_Command.Connection = new SqlConnection(ConnectionString);
					_Command.Connection.Open();
				}
				affectedRow = _Command.ExecuteNonQuery();
			}

			finally
			{
				if ((_TransManager == null))
				{
					if ((_Command.Connection != null))
					{
						_Command.Connection.Close();
					}
				}
			}
			return affectedRow;
		}

		public void CloseConnection()
		{
			if ((_TransManager == null))
			{
				if ((_Command.Connection != null))
				{
					if ((_Command.Connection.State != ConnectionState.Closed))
					{
						_Command.Connection.Close();
					}
				}
			}
		}

		#endregion
	}
}