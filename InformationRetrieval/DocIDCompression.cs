using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationRetrieval
{
    public class DocIDCompression
    {
        public List<string> ToBinaryNumberString(List<int[]> bytestream)
        {
            var result = new List<string>();

            foreach (var bytes in bytestream)
            {
                var str = "";

                foreach (var number in bytes)
                {
                    str += Convert.ToString(number, 2).PadLeft(8, '0') + " ";
                }

                result.Add(str);
            }

            return result;
        }

        // 128 oct = 1000 0000 bin
        // group by 7 bits => need to less than 128
        public List<int[]> EncodeVB(List<int> numbers)
        {
            List<int[]> result = new List<int[]>();

            for (int i = 0; i < numbers.Count; i++)
            {
                var bytes = VBEncodeNumber(numbers[i]);
                result.Add(bytes);
            }

            return result;
        }

        public List<int> DecodeVB(List<int[]> bytestream)
        {
            List<int> result = new List<int>();
            var number = 0;

            foreach (var bytes in bytestream)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] < 128)
                    {
                        number = 128 * number + bytes[i];
                    }
                    else
                    {
                        number = 128 * number + (bytes[i] - 128);
                    }
                }

                result.Add(number);
                number = 0;
            }

            return result;
        }

        private int[] VBEncodeNumber(int number)
        {
            List<int> result = new List<int>();

            while (true)
            {
                result.Insert(0, number % 128);

                if (number < 128)
                {
                    break;
                }

                number /= 128;
            }

            // Add 1000 0000 to the last element
            result[result.Count - 1] += 128;

            return result.ToArray();
        }
    }
}
