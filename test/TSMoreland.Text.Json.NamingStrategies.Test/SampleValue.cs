// 
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Runtime.Serialization;

namespace TSMoreland.Text.Json.NamingStrategies.Test;

public enum SampleValue
{
    Alpha = 0,
    Bravo = 2,
    Charlie = 4,
    DeltaFoxtrot = 6,

    // ReSharper disable once InconsistentNaming
    UTCZulu = 8,

    [EnumMember(Value = "Golf")]
    Golf = 10,
}

public enum SampleUInt32Value : uint
{
    None = 0,
    Alpha = 1u,
};

public enum SampleInt64Value : long
{
    None = 0,
    Alpha = 1_000_000L,
};
public enum SampleUInt64Value : ulong
{
    None = 0,
    Alpha = 1_000_000uL,
};

public enum SampleInt16Value : short
{
    None = 0,
   Alpha = (short)1,
};
public enum SampleUInt16Value : ushort
{
    None = 0,
    Alpha = (ushort)1u,
};

public enum SampleSByteValue : sbyte
{
    None = 0,
    Alpha = (sbyte)1,
}

public enum SampleByteValue : byte
{
    None = 0,
    Alpha = (byte)1,
}

