using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    /// <summary>
    /// 特殊字符过滤
    /// </summary>
    class SpecialCharFilter
    {
        /// <summary>
        /// 所有待处理的特殊字符
        /// </summary>
        private static Dictionary<UInt32, UInt32> specialCharData = new Dictionary<UInt32, UInt32>();

        /// <summary>
        /// 构造函数
        /// </summary>
        static SpecialCharFilter()
        {
            AddEmojiChar();
        }

        /// <summary>
        /// 增加一个unicode范围的码位
        /// </summary>
        /// <param name="start">起始码位</param>
        /// <param name="end">结束码位</param>
        private static void AddUnicodeRange(UInt32 start, UInt32 end)
        {
            if (start > end)
            {
                return;
            }

            for (var i = start; i <= end; i++)
            {
                AddUnicodeRange(i);
            }
        }

        /// <summary>
        /// 增加一个unicode编码
        /// </summary>
        /// <param name="code">unicode编码对象</param>
        private static void AddUnicodeRange(UInt32 code)
        {
            var resultCode = Utf16FromUnicode(code);
            specialCharData[resultCode] = code;
        }

        /// <summary>
        /// unicode转utf-16编码
        /// </summary>
        /// <param name="unicode">unicode编码</param>
        /// <returns>utf-16的编码结果</returns>
        /// <remarks>
        /// unicode转utf-16计算公式:  
        ///  参考地址:https://zh.wikipedia.org/wiki/UTF-16
        /// 如果unicode值小于等于0xFFFF(基本多语言平面的码位范围),则直接取此值作为编码值(16位)
        /// 如果unicode值大于0xFFFF,则按照下列步骤计算出32位编码值
        ///  码位减去0x10000,得到的值的范围为20比特长的0..0xFFFFF
        ///  高位的10比特的值（值的范围为0..0x3FF）被加上0xD800得到第一个码元或称作高位代理（high surrogate）
        ///  低位的10比特的值（值的范围也是0..0x3FF）被加上0xDC00得到第二个码元或称作低位代理（low surrogate）
        /// </remarks>
        public static UInt32 Utf16FromUnicode(UInt32 unicode)
        {
            if (unicode > 0x10ffff)
            {
                throw new Exception(String.Format("非法的unicode编码:0x{0}", unicode));
            }

            if ((unicode & 0xffff0000) == 0)
            {
                // 如果高位不为0，则代表是基本多语言平面的
                return unicode;
            }

            // 辅助平面码位计算
            UInt32 code2 = unicode - 0x10000;
            UInt32 highCode = 0xd800 + ((code2 & 0xffc00) >> 10);
            UInt32 lowCode = 0xdc00 + (code2 & 0x3ff);

            // 转换为utf-16编码
            UInt32 utf_16_Code = highCode;
            utf_16_Code = utf_16_Code << 16;
            utf_16_Code += lowCode;

            return utf_16_Code;
        }

        /// <summary>
        /// utf-16编码转unicode码位
        /// </summary>
        /// <param name="highCode">高位编码值</param>
        /// <param name="lowCode">低位编码值</param>
        /// <returns>编码结果</returns>
        /// <remarks>
        /// unicode编码方式：
        ///  参考地址:https://zh.wikipedia.org/wiki/UTF-16
        /// 4bit位(作为码位平面 plane) + 16bit位（作为平面内的码位）
        /// 前4bit全为0时，称作基本多语言平面，大部分都在此列
        /// 基本多语言平面内，从U+D800到U+DFFF之间的码位区块是永久保留不映射到任何字符（utf-16则以此进行编码）
        /// </remarks>
        private static UInt32 UnicodeFromUtf16(UInt16 highCode, UInt16 lowCode)
        {
            if (highCode <= lowCode)
            {
                var tmpCode = lowCode;
                lowCode = highCode;
                highCode = tmpCode;
            }

            if (lowCode <= 0)
            {
                return highCode;
            }

            if ((highCode & 0xd800) != 0xd800 ||
                (lowCode & 0xdc00) == 0xdc00)
            {
                throw new Exception(String.Format("错误的utf-16编码: HighCode:{0} LowCode:{1}", highCode, lowCode));
            }

            highCode = (UInt16)(highCode - 0xd800);
            lowCode = (UInt16)(lowCode - 0xdc00);

            UInt32 unicode = ((UInt32)highCode) << 10 + lowCode;

            return unicode;
        }

        /// <summary>
        /// 增加emoji表情符号
        /// </summary>
        /// <remarks>
        /// 表情字符大全参考：
        /// https://zh.wikipedia.org/wiki/%E7%B9%AA%E6%96%87%E5%AD%97
        /// 对应unicode版本号：Unicode 10.0版本
        /// </remarks>
        private static void AddEmojiChar()
        {
            AddUnicodeRange(0x00A9);
            AddUnicodeRange(0x00AE);

            AddUnicodeRange(0x203C);
            AddUnicodeRange(0x2049);
            AddUnicodeRange(0x2122);
            AddUnicodeRange(0x2139);
            AddUnicodeRange(0x2194, 0x2199);
            AddUnicodeRange(0x21A9, 0x21AA);
            AddUnicodeRange(0x231A, 0x231B);
            AddUnicodeRange(0x2328);
            AddUnicodeRange(0x23CF);
            AddUnicodeRange(0x23E9, 0x23F3);
            AddUnicodeRange(0x23F8, 0x23FA);
            AddUnicodeRange(0x24C2);
            AddUnicodeRange(0x25AA, 0x25AB);
            AddUnicodeRange(0x25B6);
            AddUnicodeRange(0x25C0);
            AddUnicodeRange(0x25FB, 0x25FE);

            AddUnicodeRange(0x2600, 0x2604);
            AddUnicodeRange(0x260E);
            AddUnicodeRange(0x2611);
            AddUnicodeRange(0x2614, 0x2615);
            AddUnicodeRange(0x2618);
            AddUnicodeRange(0x261D);
            AddUnicodeRange(0x2620);
            AddUnicodeRange(0x2622, 0x2623);
            AddUnicodeRange(0x2626);
            AddUnicodeRange(0x262A);
            AddUnicodeRange(0x262E, 0x262F);
            AddUnicodeRange(0x2638, 0x263A);
            AddUnicodeRange(0x2640);
            AddUnicodeRange(0x2642);
            AddUnicodeRange(0x2648, 0x2653);
            AddUnicodeRange(0x2660);
            AddUnicodeRange(0x2663);
            AddUnicodeRange(0x2665, 0x2666);
            AddUnicodeRange(0x2668);
            AddUnicodeRange(0x267B);
            AddUnicodeRange(0x267F);
            AddUnicodeRange(0x2692, 0x2697);
            AddUnicodeRange(0x2699);
            AddUnicodeRange(0x269B, 0x269C);

            AddUnicodeRange(0x26A0, 0x26A1);
            AddUnicodeRange(0x26AA, 0x26AB);
            AddUnicodeRange(0x26B0, 0x26B1);
            AddUnicodeRange(0x26BD, 0x26BE);
            AddUnicodeRange(0x26C4, 0x26C5);
            AddUnicodeRange(0x26C8);
            AddUnicodeRange(0x26CE, 0x26CF);
            AddUnicodeRange(0x26D1);
            AddUnicodeRange(0x26D3, 0x26D4);
            AddUnicodeRange(0x26E9, 0x26EA);
            AddUnicodeRange(0x26F0, 0x26F5);
            AddUnicodeRange(0x26F7, 0x26FA);
            AddUnicodeRange(0x26FD);

            AddUnicodeRange(0x2702);
            AddUnicodeRange(0x2705);
            AddUnicodeRange(0x2708, 0x270D);
            AddUnicodeRange(0x270F);
            AddUnicodeRange(0x2712);
            AddUnicodeRange(0x2714);
            AddUnicodeRange(0x2716);
            AddUnicodeRange(0x271D);
            AddUnicodeRange(0x2721);
            AddUnicodeRange(0x2728);
            AddUnicodeRange(0x2733, 0x2734);
            AddUnicodeRange(0x2744);
            AddUnicodeRange(0x2747);
            AddUnicodeRange(0x274C);
            AddUnicodeRange(0x274E);
            AddUnicodeRange(0x2753, 0x2755);
            AddUnicodeRange(0x2757);
            AddUnicodeRange(0x2763, 0x2764);
            AddUnicodeRange(0x2795, 0x2797);
            AddUnicodeRange(0x27A1);
            AddUnicodeRange(0x27B0);
            AddUnicodeRange(0x27BF);
            AddUnicodeRange(0x2934, 0x2935);
            AddUnicodeRange(0x2B05, 0x2B07);
            AddUnicodeRange(0x2B1B, 0x2B1C);
            AddUnicodeRange(0x2B50);
            AddUnicodeRange(0x2B55);
            AddUnicodeRange(0x3030);
            AddUnicodeRange(0x303D);
            AddUnicodeRange(0x3297, 0x3299);
            AddUnicodeRange(0x3299);
            AddUnicodeRange(0x1F004);
            AddUnicodeRange(0x1F0CF);
            AddUnicodeRange(0x1F170, 0x1F171);
            AddUnicodeRange(0x1F17E, 0x1F17F);
            AddUnicodeRange(0x1F18E);

            AddUnicodeRange(0x1F191, 0x1F19A);
            AddUnicodeRange(0x1F201, 0x1F202);
            AddUnicodeRange(0x1F21A);
            AddUnicodeRange(0x1F22F);
            AddUnicodeRange(0x1F232, 0x1F23A);
            AddUnicodeRange(0x1F250, 0x1F251);
            AddUnicodeRange(0x1F300, 0x1F321);
            AddUnicodeRange(0x1F324, 0x1F393);
            AddUnicodeRange(0x1F396, 0x1F397);
            AddUnicodeRange(0x1F399, 0x1F39B);
            AddUnicodeRange(0x1F39E, 0x1F3F0);
            AddUnicodeRange(0x1F3F3, 0x1F3F5);
            AddUnicodeRange(0x1F3F7, 0x1F53D);

            AddUnicodeRange(0x1F549, 0x1F54E);
            AddUnicodeRange(0x1F550, 0x1F567);
            AddUnicodeRange(0x1F56F, 0x1F570);
            AddUnicodeRange(0x1F573, 0x1F57A);
            AddUnicodeRange(0x1F587);
            AddUnicodeRange(0x1F58A, 0x1F58D);
            AddUnicodeRange(0x1F590);
            AddUnicodeRange(0x1F595, 0x1F596);
            AddUnicodeRange(0x1F5A4, 0x1F5A5);
            AddUnicodeRange(0x1F5A8);
            AddUnicodeRange(0x1F5B1, 0x1F5B2);
            AddUnicodeRange(0x1F5BC);
            AddUnicodeRange(0x1F5C2, 0x1F5C4);
            AddUnicodeRange(0x1F5D1, 0x1F5D3);
            AddUnicodeRange(0x1F5DC, 0x1F5DE);
            AddUnicodeRange(0x1F5E1);
            AddUnicodeRange(0x1F5E3);
            AddUnicodeRange(0x1F5E8);
            AddUnicodeRange(0x1F5EF);
            AddUnicodeRange(0x1F5F3);
            AddUnicodeRange(0x1F5FA, 0x1F6C5);
            AddUnicodeRange(0x1F6CB, 0x1F6D2);
            AddUnicodeRange(0x1F6E0, 0x1F6E5);
            AddUnicodeRange(0x1F6E8);
            AddUnicodeRange(0x1F6EB, 0x1F6EC);
            AddUnicodeRange(0x1F6F0);
            AddUnicodeRange(0x1F6F3, 0x1F6F8);
            AddUnicodeRange(0x1F910, 0x1F93A);
            AddUnicodeRange(0x1F93B, 0x1F93E);
            AddUnicodeRange(0x1F940, 0x1F945);
            AddUnicodeRange(0x1F947, 0x1F94C);
            AddUnicodeRange(0x1F950, 0x1F96B);
            AddUnicodeRange(0x1F980, 0x1F997);
            AddUnicodeRange(0x1F9C0);
            AddUnicodeRange(0x1F9D0, 0x1F9E6);
        }

        /// <summary>
        /// 把所有需要过滤的字符写入到文件
        /// </summary>
        /// <param name="targetPath">要写入的文件路径</param>
        public static void WriteAllSpecialCharToFile(String targetPath)
        {
            List<Byte> allBytes = new List<Byte>();
            foreach (var item in specialCharData.Keys)
            {
                if ((item & 0xffff0000) == 0)
                {
                    allBytes.AddRange(BitConverter.GetBytes((UInt16)item));
                }
                else
                {
                    allBytes.AddRange(BitConverter.GetBytes(item));
                }

                allBytes.AddRange(Encoding.GetEncoding("utf-16").GetBytes("\r\n"));
            }

            File.WriteAllText(targetPath, Encoding.GetEncoding("utf-16").GetString(allBytes.ToArray()), Encoding.UTF8);
        }

        /// <summary>
        /// 检查是否含有特殊字符
        /// </summary>
        /// <param name="val">待查看的字符串</param>
        /// <returns>是否包含有emoji字符</returns>
        public static Boolean IfHaveSpecialChar(String val)
        {
            // 由于C#在内存中本来就是使用的UTF-16进行的编码，所以可以直接把utf-16代码直接使用来进行匹配
            for (Int32 i = 0; i < val.Length; i++)
            {
                UInt16 charItem = val[i];
                if (specialCharData.ContainsKey(charItem))
                {
                    return true;
                }

                // 因为unicode的基本多语言平面(bmp)的0xd800到0xdf00没编码，而utf-16针对辅助平面用了这些编码的，所以以此作为多字符的判断
                if ((charItem & 0xd800) != 0xd800)
                {
                    continue;
                }

                if (i + 1 >= val.Length)
                {
                    // 最后一个字符必然不是4字节字符
                    continue;
                }

                // 检查辅助平面的字符
                UInt32 utf_16_code = ((UInt32)charItem << 16) + val[i + 1];
                if (specialCharData.ContainsKey(utf_16_code))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否能够被mysql的utf8_general_ci存储
        /// </summary>
        /// <returns>返回是否能够存储</returns>
        public static Boolean IfCanSaveByGeneralCi(String val)
        {
            for (Int32 i = 0; i < val.Length; i++)
            {
                if (i + 1 >= val.Length)
                {
                    // 最后一个字符必然不是4字节字符
                    continue;
                }

                if (IfInSupplementaryPlane(val[i], val[i + 1]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检查某个utf-16的编码对是否是辅助平面编码
        /// </summary>
        /// <param name="hightCode">高位部分</param>
        /// <param name="lowCode">低位部分</param>
        /// <returns>true：是处于辅助平面的码位 false:不是</returns>
        private static Boolean IfInSupplementaryPlane(UInt16 hightCode, UInt16 lowCode)
        {
            if ((hightCode & 0xd800) != 0xd800)
            {
                return false;
            }

            if ((lowCode & 0xDC00) == 0xDC00)
            {
                // 确认是一个utf-16辅助平面的编码，则返回false 因为mysql只存储了U+0000到U+FFFF的utf-8编码数值,超过的则会报错
                return true;
            }

            return false;
        }
    }
}
