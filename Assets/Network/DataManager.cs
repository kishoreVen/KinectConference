using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class DataManager 
{
	#region Const Variables
    public const int FloatByteLength = 4;
    public const int Vector3ByteLength = FloatByteLength * 3;

    public const int VertexCount = 13568;
    public const int NetByteCount = VertexCount * FloatByteLength;
	#endregion

    #region Static Public variables
    public static byte[] dataToSend;
    public static float[] receivedData;
    #endregion

    #region Utilities
    public static void Initialize()
    {
        dataToSend = new byte[NetByteCount]; // Initialize Depth cloud's byte array

        receivedData = new float[VertexCount]; // Initialize float array

        //ReadTestData();
    }

    public static void ReadTestData()
    {
        string[] data1lines = File.ReadAllLines("C:\\Users\\kishore_venkateshan\\Desktop\\TestData\\test1.txt");

        data1 = new float[data1lines.Length];

        for (int index = 0; index < data1lines.Length; index++)
        {
            data1[index] = float.Parse(data1lines[index]);
        }

        string[] data2lines = File.ReadAllLines("C:\\Users\\kishore_venkateshan\\Desktop\\TestData\\test2.txt");

        data2 = new float[data2lines.Length];

        for (int index = 0; index < data2lines.Length; index++)
        {
            data2[index] = float.Parse(data2lines[index]);
        }
    }

    public static byte[] MakeVector3Byte(Vector3 inVector)
	{
		byte[] xVectorBytes = BitConverter.GetBytes (inVector.x);
		byte[] yVectorBytes = BitConverter.GetBytes (inVector.y);
		byte[] zVectorBytes = BitConverter.GetBytes (inVector.z);
		
		byte[] convertedBytes = new byte[xVectorBytes.Length + yVectorBytes.Length + zVectorBytes.Length];
		
		int index = 0;
		int offset = 0;
		for(index = 0; index < xVectorBytes.Length; index++)
		{
			convertedBytes[offset + index] = xVectorBytes[index];
		}
		
		offset += index;
		
		for(index = 0; index < yVectorBytes.Length; index++)
		{
			convertedBytes[offset + index] = yVectorBytes[index];
		}
		
		offset += index;
		
		for(index = 0; index < zVectorBytes.Length; index++)
		{
			convertedBytes[offset + index] = zVectorBytes[index];
		}
		
		return convertedBytes;
	}

	public static Vector3 BreakVector3Byte(byte[] inBytes)
	{
		byte[] segmentedBytes = new byte[FloatByteLength];

		float x, y, z;

		int offset = 0;
		int index = 0;

		for(index = 0; index < FloatByteLength; index ++)
		{
			segmentedBytes[index] = inBytes[offset + index];
		}

		offset += FloatByteLength;

		x = BitConverter.ToSingle (segmentedBytes, 0);

		for(index = 0; index < FloatByteLength; index ++)
		{
			segmentedBytes[index] = inBytes[offset + index];
		}
		
		offset += FloatByteLength;

		y = BitConverter.ToSingle (segmentedBytes, 0);

		for(index = 0; index < FloatByteLength; index ++)
		{
			segmentedBytes[index] = inBytes[offset + index];
		}
		
		z = BitConverter.ToSingle (segmentedBytes, 0);

		return new Vector3(x, y, z);
	}

    public static void MakeVertexDepthBytes(ref Vector3[] vertices)
    {
        if (VertexCount == vertices.Length)
        {
            int offset = 0;

            for (int index = 0; index < VertexCount; index++, offset += FloatByteLength)
            {
                byte[] currentDepthBytes = BitConverter.GetBytes(vertices[index].z);
				//byte[] currentDepthBytes = BitConverter.GetBytes(data1[index]);

                dataToSend[offset + 0] = currentDepthBytes[0];
                dataToSend[offset + 1] = currentDepthBytes[1];
                dataToSend[offset + 2] = currentDepthBytes[2];
                dataToSend[offset + 3] = currentDepthBytes[3];
            }

			//Debug.Log ("Write Offset: " + offset);
        }
    }

    public static void BreakVertexDepthBytes(byte[] inBytes)
    {
        if (NetByteCount == inBytes.Length)
        {
			int floatIndex = 0;
            for (int index = 0; index < NetByteCount; index += FloatByteLength, floatIndex++)
            {
                receivedData[floatIndex] = BitConverter.ToSingle(inBytes, index);
            }

			//Debug.Log ("Read Offset: " + floatIndex);
        }
    }
	#endregion

    #region Test Data
    public static float[] data1;
    public static float[] data2;
    #endregion
}
