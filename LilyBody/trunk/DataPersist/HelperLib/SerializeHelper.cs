using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Collections;

namespace DataPersist.HelperLib
{
	public class SerializeHelper
	{
		public static object Deserialize(Hashtable data)
		{
            if (data == null)
                return null;
			Type type=Type.GetType(data[(byte)255] as string);
			data.Remove((byte)255);
			object obj;
			if(type.IsArray)
			{
				obj=Array.CreateInstance(type.GetElementType(),data.Count);
				for(int i=0;i<data.Count;i++)
				{
					object item=data[(byte)i];
					if(NeedDeserialize(item))
					{
						item=Deserialize(item as Hashtable);
					}
					(obj as Array).SetValue(item,i);
				}
			}
			else
			{
				obj=type.Assembly.CreateInstance(type.FullName);
				
				if(obj is ICollection)
				{
					if(obj is IList)
					{
			            for (int i = 0; i < data.Count;i++ )
			            {
							object item=data[(byte)i];
							if(NeedDeserialize(item))
							{
								item=Deserialize(item as Hashtable);
							}
			                (obj as IList).Add(item);
			         	}
					}
				}
				else
				{
					PropertyInfo[] propertyInfos=type.GetProperties();
					for(int i=0; i<propertyInfos.Length;i++)
					{
						if(data.ContainsKey((byte)i))
						{
							object item=data[(byte)i];
							if(NeedDeserialize(item))
							{
								item=Deserialize(item as Hashtable);
							}
							propertyInfos[i].SetValue(obj,item,null);
						}
					}
				}
			}
			
		  return obj;
		}
		
		public static Hashtable Serialize(object obj)
  		{
            if (obj == null)
                return null;
			Hashtable data=new Hashtable();
			data[(byte)255]=obj.GetType().FullName;
			if(obj is ICollection)
			{
				int index=0;
				foreach(object item in ((IEnumerable)obj))
				{
					if(NeedSerialize(item))
					{
						data[(byte)index++]=Serialize(item);
					}
					else
					{
						data[(byte)index++]=item;
					}
				}
			}
			else
			{
				PropertyInfo[] propertyInfos=obj.GetType().GetProperties();
				for(int i=0;i<propertyInfos.Length;i++)
				{
					if(propertyInfos[i].GetAccessors().Length==2)
					{
						object item=propertyInfos[i].GetValue(obj,null);
						if(item!=null)
						{
							if(NeedSerialize(item))
							{
								data[(byte)i]=Serialize(item);
							}
							else
							{
								data[(byte)i]=item;
							}
						}
					}
				}
			}
			return data;
  		}
		
		public static bool NeedSerialize(object obj)
		{
            if (obj == null) return false;
			return !(obj.GetType().IsPrimitive || obj is string || 
				obj is byte[]|| obj is int[]||obj is Hashtable || obj.GetType().IsEnum);
		}
		
		public static bool NeedDeserialize(object obj)
		{
			return obj is Hashtable&& (obj as Hashtable).ContainsKey((byte)255);
		}
	}
}

