using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSV  {
	public enum DataType
	{
		NULL,
		UINT,
		INT,
		STRING,
		FLOAT,
	}

	static Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
	
	int m_RowCount;
	string m_TableName = "";
	private string[] m_strHeaderNames = null;
	protected Dictionary<string, CSV.DataType> m_DataTypes = new Dictionary<string, CSV.DataType>();
	protected Dictionary<string, System.Object> m_DataTables = new Dictionary<string, System.Object>();
	
	public int GetFieldCount()
	{
		return m_DataTables.Count;
	}
	
	public int GetRowCount()
	{
		return m_RowCount;
	}
	
	public CSV(string tableName)
	{
		m_TableName = tableName;
	}
	
	public CSV(string tableName, string strData)
	{
		m_TableName = tableName;
		Parse(strData);
	}
	
	public void Parse(string strData)
	{
		byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(strData);
		System.IO.MemoryStream stream = new System.IO.MemoryStream( textBytes );
		System.IO.StreamReader reader = new System.IO.StreamReader( stream );
		
		m_RowCount = 0;
		int nCnt = 0;
		while(!reader.EndOfStream)
		{
			if(0 == nCnt)
			{
				ParseHeader( reader.ReadLine() );
				nCnt++;
			}
			else if(1 == nCnt)
			{
				ParseDataType( reader.ReadLine() );
				nCnt++;
			}
			else
				ParseData( reader );				
		}
		
		;//Debug.Log(m_RowCount.ToString() );
	}
	
	string[] ParseFields(string strData)
	{
		string[] strs = CSVParser.Split(strData);
		
		for(int i = 0; i < strs.Length; i++)
		{
			strs[i] = strs[i].TrimStart('"');
			strs[i] = strs[i].TrimEnd('"');
			i++;
		}
		
		return strs;
	}
	
	void ParseHeader(string strData)
	{
		m_strHeaderNames = CSVParser.Split(strData);
	}
	
	void ParseDataType(string strData)
	{
		string[] strTypeNames = CSVParser.Split(strData);
		
		if(m_strHeaderNames.Length != strTypeNames.Length)
			Debug.LogWarning("<CSV>Miss match (Header <-> DataType) in Table File : [" + m_TableName + "] > missing DataType set 'uint'.");
		
		int nCnt = 0;
		foreach(string strHeaderName in m_strHeaderNames)
		{
			string strHeaderNameT = strHeaderName.Replace('"', ' ').Trim();
			
			string strTypeNameT = "uint";
			if(nCnt < strTypeNames.Length)
				strTypeNameT = strTypeNames[nCnt].Replace('"', ' ');
			
			CSV.DataType dataType = CSV.DataType.UINT;
			try
			{
				dataType = (CSV.DataType)System.Enum.Parse(typeof(CSV.DataType), strTypeNameT, true);
			}
			catch(System.ArgumentException ex)
			{
				Debug.LogWarning("<CSV>Invalid Data Type Keyword in Table File : [" + m_TableName + ", " + strHeaderNameT + "] > missing DataType set 'uint'.\n" + "[Detail Log]\n" + ex.ToString());
				dataType = CSV.DataType.UINT;
			}
			
			// Add Column
			switch(dataType)
			{
			case CSV.DataType.UINT:
				m_DataTables.Add(strHeaderNameT, new Dictionary<uint, uint>());
				break;
			case CSV.DataType.INT:
				m_DataTables.Add(strHeaderNameT, new Dictionary<uint, int>());
				break;
			case CSV.DataType.FLOAT:
				m_DataTables.Add(strHeaderNameT, new Dictionary<uint, float>());
				break;
			case CSV.DataType.STRING:
				m_DataTables.Add(strHeaderNameT, new Dictionary<uint, string>());
				break;
			}
			
			// Add DataType
			m_DataTypes.Add(strHeaderNameT, dataType);
			
			nCnt++;
		}
	}

    string readLine(System.IO.StreamReader reader)
    {
        string strRet = "";
        bool bInString = false;
        while (reader.Peek() >= 0)
        {
            char ch = (char)reader.Read();
            if('\"' == ch)
                bInString = !bInString;

            if ('\r' == ch)
            {
                char ch2 = (char)reader.Read();
                if (!bInString && '\n' == ch2)
                    break;
                else
                    strRet += (ch.ToString() + ch2.ToString());
            }
            else
                strRet += ch;
        }

        //Debug.Log(strRet);
        return strRet;
    }
	
	void ParseData(System.IO.StreamReader reader)
	{
        string strData = readLine(reader);
		string[] strFieldDatas = CSVParser.Split(strData);
		
		if(m_DataTables.Count != strFieldDatas.Length)
			Debug.LogWarning("<CSV>Miss match (Header <-> Data) in Table File : [" + m_TableName + "] > missing Data set to '0'.");
		
		uint index = 0;
		try {
			index = uint.Parse( strFieldDatas[0].Replace('"', ' ') );
		}
		catch(System.Exception ex)
		{
			Debug.LogWarning("<CSV>Error Parsing Index Data : [" + m_TableName + ", " + strFieldDatas[0] + "]  > Skip To Next Line.\n"  + "[Detail Log]\n" + ex.ToString());
			return;
		}
		
		uint nCnt = 0;
		foreach(string strHeaderName in m_DataTables.Keys)
		{
			string strFieldData = "0";
			if(nCnt < m_DataTables.Count)
				strFieldData = strFieldDatas[nCnt].Replace('"', ' ').Trim();
			
			try {
				switch(m_DataTypes[strHeaderName])
				{
				case CSV.DataType.UINT:
					((Dictionary<uint, uint>)m_DataTables[strHeaderName]).Add(index, uint.Parse(strFieldData) );
					break;
				case CSV.DataType.INT:
					((Dictionary<uint, int>)m_DataTables[strHeaderName]).Add(index, int.Parse(strFieldData) );
					break;
				case CSV.DataType.FLOAT:
					((Dictionary<uint, float>)m_DataTables[strHeaderName]).Add(index, float.Parse(strFieldData) );
					break;
				case CSV.DataType.STRING:
					((Dictionary<uint, string>)m_DataTables[strHeaderName]).Add(index, strFieldData );
					break;
				}
			}
			catch(System.Exception ex)
			{
				Debug.LogWarning("<CSV>Error Parsing Data : [" + m_TableName + ", " + strHeaderName + ", " + index.ToString() + "]  > Data set to '0'." + "[Detail Log]\n" + ex.ToString());
				switch(m_DataTypes[strHeaderName])
				{
				case CSV.DataType.UINT:
					((Dictionary<uint, uint>)m_DataTables[strHeaderName]).Add(index, (uint)0);
					break;
				case CSV.DataType.INT:
					((Dictionary<uint, int>)m_DataTables[strHeaderName]).Add(index, (int)0);
					break;
				case CSV.DataType.FLOAT:
					((Dictionary<uint, float>)m_DataTables[strHeaderName]).Add(index, (float)0.0f);
					break;
				case CSV.DataType.STRING:
					((Dictionary<uint, string>)m_DataTables[strHeaderName]).Add(index, "0");
					break;
				}
			}
			
			nCnt++;
		}
		
		m_RowCount++;
	}
	
	protected  bool IsExitArray(string field)
	{
		bool bExist = m_DataTables.ContainsKey(field);
		
		if(!bExist)
		{
			Debug.LogWarning("<CSV>Missing Table Data : [" + m_TableName + ", " + field + "] > Dummy Data('0') returned.");
		}
		return bExist;
	}
	
	protected bool IsExitValue(string field, uint index, bool bLog = true)
	{
		bool bExist = m_DataTables.ContainsKey(field);
		
		if(bExist)
		{
			switch(m_DataTypes[field])
			{
			case CSV.DataType.UINT:
				bExist = ((Dictionary<uint, uint>)m_DataTables[field]).ContainsKey(index);
				break;
			case CSV.DataType.INT:
				bExist = ((Dictionary<uint, int>)m_DataTables[field]).ContainsKey(index);
				break;
			case CSV.DataType.FLOAT:
				bExist = ((Dictionary<uint, float>)m_DataTables[field]).ContainsKey(index);
				break;
			case CSV.DataType.STRING:
				bExist = ((Dictionary<uint, string>)m_DataTables[field]).ContainsKey(index);
				break;
			}
		}
		
		if(!bExist && bLog)
		{
			Debug.LogWarning("<CSV>Missing Table Data : [" + m_TableName + ", " + field + ", " + index.ToString() + "] > Dummy Data('0') returned.");
		}
		return bExist;
	}

    public CSV.DataType GetDataType(string field)
    {
        if (!m_DataTypes.ContainsKey(field))
            return CSV.DataType.NULL;

        return m_DataTypes[field];
    }
	
	protected  bool IsExactType(string field, CSV.DataType reqType)
	{
		bool bExact = (m_DataTypes.ContainsKey(field) && (m_DataTypes[field] == reqType));
		
		if(!bExact)
		{
			Debug.LogWarning("<CSV>Invalid Data Type request : [" + m_TableName + ", " + field + "] > Dummy Data('0') returned.");
		}
		return bExact;
	}
	
	public uint[] GetArrayUint(string field)
	{
		uint[] ret = null;
		if(!IsExitArray(field) || !IsExactType(field, CSV.DataType.UINT))
		{
			ret = new uint[1];
			ret[0] = 0;
			return ret;
		}
		
		ret = new uint[ ((Dictionary<uint, uint>)m_DataTables[field]).Values.Count ];
		int nCnt = 0;
		foreach(uint val in ((Dictionary<uint, uint>)m_DataTables[field]).Values)
		{
			if(!IsExactType(field, CSV.DataType.UINT))
				ret[nCnt] = 0;
			else
				ret[nCnt] = val;
			
			nCnt++;
		}
		
		return ret;
	}
	
	public string GetValueString(string field, uint index)
	{
		if(!IsExitValue(field, index) || !IsExactType(field, CSV.DataType.STRING))
			return "0";
        if (0 == ((Dictionary<uint, string>)m_DataTables[field])[index].Length)
            return "0";

		return ((Dictionary<uint, string>)m_DataTables[field])[index];
	}
	
	public int GetValueInt(string field, uint index)
	{
		if(!IsExitValue(field, index) || !IsExactType(field, CSV.DataType.INT))
			return 0;
		
		return ((Dictionary<uint, int>)m_DataTables[field])[index];
	}
	
	public uint GetValueUint(string field, uint index)
	{
		if(!IsExitValue(field, index) || !IsExactType(field, CSV.DataType.UINT))
			return 0;
		
		return ((Dictionary<uint, uint>)m_DataTables[field])[index];
	}
	
	public float GetValueFloat(string field, uint index)
	{
		if(!IsExitValue(field, index) || !IsExactType(field, CSV.DataType.FLOAT))
			return 0.0f;
		
		return ((Dictionary<uint, float>)m_DataTables[field])[index];
	}
}
