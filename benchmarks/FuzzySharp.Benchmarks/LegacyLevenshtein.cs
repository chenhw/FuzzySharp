using System;

namespace FuzzySharp.Benchmarks;

internal static class LegacyLevenshtein
{
    public static int EditDistance(string s1, string s2, int xcost = 0)
    {
        ArgumentNullException.ThrowIfNull(s1);
        ArgumentNullException.ThrowIfNull(s2);
        return EditDistance(s1.ToCharArray(), s2.ToCharArray(), xcost);
    }

    public static double GetRatio(string s1, string s2)
    {
        ArgumentNullException.ThrowIfNull(s1);
        ArgumentNullException.ThrowIfNull(s2);
        return GetRatio(s1.ToCharArray(), s2.ToCharArray());
    }

    private static int EditDistance<T>(T[] c1, T[] c2, int xcost = 0) where T : IEquatable<T>
    {
        int i;
        int half;

        int str1 = 0;
        int str2 = 0;

        int len1 = c1.Length;
        int len2 = c2.Length;

        while (len1 > 0 && len2 > 0 && c1[str1].Equals(c2[str2]))
        {
            len1--;
            len2--;
            str1++;
            str2++;
        }

        while (len1 > 0 && len2 > 0 && c1[str1 + len1 - 1].Equals(c2[str2 + len2 - 1]))
        {
            len1--;
            len2--;
        }

        if (len1 == 0)
        {
            return len2;
        }

        if (len2 == 0)
        {
            return len1;
        }

        if (len1 > len2)
        {
            int nx = len1;
            int temp = str1;

            len1 = len2;
            len2 = nx;

            str1 = str2;
            str2 = temp;

            (c1, c2) = (c2, c1);
        }

        if (len1 == 1)
        {
            if (xcost != 0)
            {
                return len2 + 1 - 2 * Memchr(c2, str2, c1[str1], len2);
            }

            return len2 - Memchr(c2, str2, c1[str1], len2);
        }

        len1++;
        len2++;
        half = len1 >> 1;

        var row = new int[len2];
        int end = len2 - 1;

        for (i = 0; i < len2 - (xcost != 0 ? 0 : half); i++)
        {
            row[i] = i;
        }

        if (xcost != 0)
        {
            for (i = 1; i < len1; i++)
            {
                int p = 1;
                T ch1 = c1[str1 + i - 1];
                int c2p = str2;

                int d = i;
                int x = i;

                while (p <= end)
                {
                    if (ch1.Equals(c2[c2p++]))
                    {
                        x = --d;
                    }
                    else
                    {
                        x++;
                    }

                    d = row[p] + 1;

                    if (x > d)
                    {
                        x = d;
                    }

                    row[p++] = x;
                }
            }
        }
        else
        {
            row[0] = len1 - half - 1;
            for (i = 1; i < len1; i++)
            {
                int p;
                T ch1 = c1[str1 + i - 1];
                int c2p;

                int d;
                int x;

                if (i >= len1 - half)
                {
                    int offset = i - (len1 - half);
                    c2p = str2 + offset;
                    p = offset;
                    int c3 = row[p++] + (!ch1.Equals(c2[c2p++]) ? 1 : 0);
                    x = row[p] + 1;
                    d = x;
                    if (x > c3)
                    {
                        x = c3;
                    }

                    row[p++] = x;
                }
                else
                {
                    p = 1;
                    c2p = str2;
                    d = x = i;
                }

                if (i <= half + 1)
                {
                    end = len2 + i - half - 2;
                }

                while (p <= end)
                {
                    int c3 = --d + (!ch1.Equals(c2[c2p++]) ? 1 : 0);
                    x++;
                    if (x > c3)
                    {
                        x = c3;
                    }

                    d = row[p] + 1;
                    if (x > d)
                    {
                        x = d;
                    }

                    row[p++] = x;
                }

                if (i <= half)
                {
                    int c3 = --d + (!ch1.Equals(c2[c2p]) ? 1 : 0);
                    x++;
                    if (x > c3)
                    {
                        x = c3;
                    }

                    row[p] = x;
                }
            }
        }

        return row[end];
    }

    private static int Memchr<T>(T[] haystack, int offset, T needle, int num) where T : IEquatable<T>
    {
        if (num == 0)
        {
            return 0;
        }

        int p = 0;
        do
        {
            if (haystack[offset + p].Equals(needle))
            {
                return 1;
            }

            p++;
        } while (--num != 0);

        return 0;
    }

    private static double GetRatio<T>(T[] input1, T[] input2) where T : IEquatable<T>
    {
        int len1 = input1.Length;
        int len2 = input2.Length;
        int lensum = len1 + len2;
        int editDistance = EditDistance(input1, input2, 1);
        return editDistance == 0 ? 1 : (lensum - editDistance) / (double)lensum;
    }
}
