using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;//�ϻ층�� ���� ���� ������
using System.Reflection;
using System.Text;
using System;



public class MarshalTableConstant
{
    public const int charBufferSize = 256;
}
public class TableRecordParser<TMarshalStruct>
{
    public TMarshalStruct ParseRecordLine(string line)
    {
        Type type = typeof(TMarshalStruct);
        int structSize = Marshal.SizeOf(type);
        byte[] structBytes = new byte[structSize];
        int structBytesIndex = 0;

        const string spliter = ",";
        string[] fieldDataList = line.Split(spliter.ToCharArray());
        Type dataType;
        string splited;
        byte[] fieldByte;


        FieldInfo[] fieldInfos = type.GetFields();
        for(int i = 0; i < fieldInfos.Length; i++)
        {
            dataType = fieldInfos[i].FieldType;
            splited = fieldDataList[i];

            fieldByte = new byte[4];
            MakeBytesByFieldType(out fieldByte, dataType, splited);
            Buffer.BlockCopy(fieldByte, 0 ,structBytes, structBytesIndex, fieldByte.Length);// �ҽ�����, �ҽ������� ����Ʈ ������, ��� ���� , �������� ����Ʈ ������, ������ ����Ʈ
            structBytesIndex +=fieldByte.Length;//����ü�� ��ü ����Ʈ�� ����

 

        }
        TMarshalStruct tStruct = MakeStructFromBytes<TMarshalStruct>(structBytes);
        
        return tStruct;

    }
    protected void MakeBytesByFieldType(out byte[] fieldByte, Type dataType, string splite)
    {
        fieldByte = new byte[1];

        if (typeof(int) == dataType)
        {
            fieldByte = BitConverter.GetBytes(int.Parse(splite));
        }
        else if (typeof(float) == dataType)
        {
            fieldByte = BitConverter.GetBytes(float.Parse(splite));

        }

        else if (typeof(bool) == dataType)
        {
            bool value = bool.Parse(splite);
            int temp = value ? 1 : 0;
            fieldByte = BitConverter.GetBytes((int)temp);

        }
        else if(typeof(string) == dataType)
        {
            
            fieldByte = new byte[MarshalTableConstant.charBufferSize];
            byte[] byteArr = Encoding.UTF8.GetBytes(splite);
            Buffer.BlockCopy(byteArr,0, fieldByte, 0, byteArr.Length);
        }
    }
    public static T MakeStructFromBytes<T>(byte[] bytes)
    {
        int size = Marshal.SizeOf(typeof(T));
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.Copy(bytes, 0, ptr, size);

        T tStruct = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return tStruct;

    }
     
}
