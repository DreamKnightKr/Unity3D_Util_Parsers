using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSVAddible : CSV {	
	public CSVAddible(string tableName) : base(tableName)
	{
		{
			AddField("index", CSV.DataType.UINT);
		}
	}
	
	public void AddField(string field, CSV.DataType dataType)
	{
		if(m_DataTypes.ContainsKey(field))
			return;	
		
		m_DataTypes.Add(field, dataType);
		switch(dataType)
		{
		case CSV.DataType.UINT:
			m_DataTables.Add(field, new Dictionary<uint, uint>());
			break;
		case CSV.DataType.INT:
			m_DataTables.Add(field, new Dictionary<uint, int>());
			break;
		case CSV.DataType.FLOAT:
			m_DataTables.Add(field, new Dictionary<uint, float>());
			break;
		case CSV.DataType.STRING:
			m_DataTables.Add(field, new Dictionary<uint, string>());
			break;
		}
	}
	
	public uint GetNewIndex()
	{
		((Dictionary<uint, uint>)m_DataTables["index"]).Add(
			(uint)((Dictionary<uint, uint>)m_DataTables["index"]).Count + 1,
			(uint)((Dictionary<uint, uint>)m_DataTables["index"]).Count + 1);
		return (uint)((Dictionary<uint, uint>)m_DataTables["index"]).Count;
	}
	
	public void AddValueString(string field, uint index, string val)
	{
		if(!IsExactType(field, CSV.DataType.STRING))
			return;
		
		if(IsExitValue(field, index, false))
			((Dictionary<uint, string>)m_DataTables[field])[index] = val;
		else
			((Dictionary<uint, string>)m_DataTables[field]).Add(index, val);
	}	
	
	public void AddValueUint(string field, uint index, uint val)
	{
		if(!IsExactType(field, CSV.DataType.UINT))
			return;
		
		if(IsExitValue(field, index, false))
			((Dictionary<uint, uint>)m_DataTables[field])[index] = val;
		else
			((Dictionary<uint, uint>)m_DataTables[field]).Add(index, val);
	}
	
	public override string ToString()
	{
		// Headers
		string str = "";
		foreach(string strHeadName in m_DataTables.Keys)
		{
			str += (strHeadName + ",");
		}
		str = str.Remove( str.LastIndexOf(',') );
		str += "\r\n";
		
		
		// DataTypes
		CSV.DataType[] dataTypes = new CSV.DataType[m_DataTypes.Count];
		int nCnt = 0;
		foreach(CSV.DataType dataType in m_DataTypes.Values)
		{
			dataTypes[nCnt] = dataType;
			str += (dataType.ToString().ToLower() + ",");
			
			nCnt++;
		}
		str = str.Remove( str.LastIndexOf(',') );
		str += "\r\n";
		
		
		// Datas
		for(uint nRow = 1; nRow < (uint)((Dictionary<uint, uint>)m_DataTables["index"]).Count + 1; nRow++)
		{
			nCnt = 0;
			foreach(string strHeadName in m_DataTables.Keys)
			{
				switch(dataTypes[nCnt])
				{
				case CSV.DataType.UINT:
					str += (GetValueUint(strHeadName, nRow).ToString() + ",");
					break;
				case CSV.DataType.INT:
					str += (GetValueInt(strHeadName, nRow).ToString() + ",");
					break;
				case CSV.DataType.FLOAT:
					str += (GetValueFloat(strHeadName, nRow).ToString() + ",");
					break;
				case CSV.DataType.STRING:
					str += (GetValueString(strHeadName, nRow) + ",");
					break;
				}
				nCnt++;
			}
			
			str = str.Remove( str.LastIndexOf(',') );
			str += "\r\n";
				
		}

        str = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.ASCII.GetBytes(str));
		return str;
	}
}
