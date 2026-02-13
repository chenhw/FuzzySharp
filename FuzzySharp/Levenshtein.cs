using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FuzzySharp.Edits;

namespace FuzzySharp
{
    public static class Levenshtein
    {
        private static EditOp[] GetEditOps<T>(T[] arr1, T[] arr2) where T : IEquatable<T>
        {
            ArgumentNullException.ThrowIfNull(arr1);
            ArgumentNullException.ThrowIfNull(arr2);
            return GetEditOps(arr1.Length, arr1, arr2.Length, arr2);
        }

        // Special Case
        private static EditOp[] GetEditOps(string s1, string s2)
        {
            ArgumentNullException.ThrowIfNull(s1);
            ArgumentNullException.ThrowIfNull(s2);
            return GetEditOps(s1.Length, s1.ToCharArray(), s2.Length, s2.ToCharArray());
        }

        private static EditOp[] GetEditOps<T>(int len1, T[] c1, int len2, T[] c2) where T : IEquatable<T>
        {
            int i;

            int[] matrix;

            int p1 = 0;
            int p2 = 0;

            var len1o = 0;

            while (len1 > 0 && len2 > 0 && c1[p1].Equals(c2[p2]))
            {
                len1--;
                len2--;

                p1++;
                p2++;

                len1o++;
            }

            var len2o = len1o;

            /* strip common suffix */
            while (len1 > 0 && len2 > 0 && c1[p1 + len1 - 1].Equals(c2[p2 + len2 - 1]))
            {
                len1--;
                len2--;
            }

            len1++;
            len2++;

            if (len2 > int.MaxValue / len1)
            {
                throw new OverflowException("Edit operation matrix is too large.");
            }

            matrix = new int[len2 * len1];

            for (i = 0; i < len2; i++)
                matrix[i] = i;
            for (i = 1; i < len1; i++)
                matrix[len2 * i] = i;

            for (i = 1; i < len1; i++)
            {

                int ptrPrev = (i - 1) * len2;
                int ptrC    = i       * len2;
                int ptrEnd  = ptrC + len2 - 1;

                T   char1    = c1[p1 + i - 1];
                int ptrChar2 = p2;

                int x = i;

                ptrC++;

                while (ptrC <= ptrEnd)
                {

                    int c3 = matrix[ptrPrev++] + (!char1.Equals(c2[ptrChar2++]) ? 1 : 0);
                    x++;

                    if (x > c3)
                    {
                        x = c3;
                    }

                    c3 = matrix[ptrPrev] + 1;

                    if (x > c3)
                    {
                        x = c3;
                    }

                    matrix[ptrC++] = x;

                }

            }


            return EditOpsFromCostMatrix(len1, c1, p1, len1o, len2, c2, p2, len2o, matrix);
        }


        private static EditOp[] EditOpsFromCostMatrix<T>(int len1, T[] c1, int p1, int o1,
                                                      int len2, T[] c2, int p2, int o2,
                                                      int[] matrix) 
            where T: IEquatable<T>
        {

            int i, j, pos;

            int ptr;

            EditOp[] ops;

            int dir = 0;

            pos = matrix[len1 * len2 - 1];

            ops = new EditOp[pos];

            i = len1 - 1;
            j = len2 - 1;

            ptr = len1 * len2 - 1;

            while (i > 0 || j > 0)
            {
                if (i != 0 && j != 0 && matrix[ptr] == matrix[ptr - len2 - 1]
                    && c1[p1 + i - 1].Equals(c2[p2 + j - 1]))
                {

                    i--;
                    j--;
                    ptr -= len2 + 1;
                    dir = 0;

                    continue;

                }

                if (dir < 0 && j != 0 && matrix[ptr] == matrix[ptr - 1] + 1)
                {

                    EditOp eop = new EditOp();

                    pos--;
                    ops[pos] = eop;
                    eop.EditType = EditType.INSERT;
                    eop.SourcePos = i + o1;
                    eop.DestPos = --j + o2;
                    ptr--;

                    continue;
                }

                if (dir > 0 && i != 0 && matrix[ptr] == matrix[ptr - len2] + 1)
                {

                    EditOp eop = new EditOp();

                    pos--;
                    ops[pos] = eop;
                    eop.EditType = EditType.DELETE;
                    eop.SourcePos = --i + o1;
                    eop.DestPos = j + o2;
                    ptr -= len2;

                    continue;

                }

                if (i != 0 && j != 0 && matrix[ptr] == matrix[ptr - len2 - 1] + 1)
                {

                    pos--;

                    EditOp eop = new EditOp();
                    ops[pos] = eop;

                    eop.EditType = EditType.REPLACE;
                    eop.SourcePos = --i + o1;
                    eop.DestPos = --j + o2;

                    ptr -= len2 + 1;
                    dir = 0;
                    continue;

                }

                if (dir == 0 && j != 0 && matrix[ptr] == matrix[ptr - 1] + 1)
                {

                    pos--;
                    EditOp eop = new EditOp();
                    ops[pos] = eop;
                    eop.EditType = EditType.INSERT;
                    eop.SourcePos = i + o1;
                    eop.DestPos = --j + o2;
                    ptr--;
                    dir = -1;

                    continue;
                }

                if (dir == 0 && i != 0 && matrix[ptr] == matrix[ptr - len2] + 1)
                {
                    pos--;
                    EditOp eop = new EditOp();
                    ops[pos] = eop;

                    eop.EditType = EditType.DELETE;
                    eop.SourcePos = --i + o1;
                    eop.DestPos = j + o2;
                    ptr -= len2;
                    dir = 1;
                    continue;
                }

                throw new InvalidOperationException("Cant calculate edit op");
            }

            return ops;

        }

        public static MatchingBlock[] GetMatchingBlocks<T>(T[] s1, T[] s2) where T : IEquatable<T>
        {
            ArgumentNullException.ThrowIfNull(s1);
            ArgumentNullException.ThrowIfNull(s2);
            return GetMatchingBlocks(s1.Length, s2.Length, GetEditOps(s1, s2));
        }

        // Special Case
        public static MatchingBlock[] GetMatchingBlocks(string s1, string s2)
        {
            ArgumentNullException.ThrowIfNull(s1);
            ArgumentNullException.ThrowIfNull(s2);

            return GetMatchingBlocks(s1.Length, s2.Length, GetEditOps(s1, s2));

        }


        public static MatchingBlock[] GetMatchingBlocks(int len1, int len2, OpCode[] ops)
        {
            ArgumentNullException.ThrowIfNull(ops);

            int n = ops.Length;

            int noOfMB, i;
            int o = 0;

            noOfMB = 0;

            for (i = n; i-- != 0; o++)
            {
                if (ops[o].EditType == EditType.KEEP)
                {
                    noOfMB++;

                    while (i != 0 && ops[o].EditType == EditType.KEEP)
                    {
                        i--;
                        o++;
                    }

                    if (i == 0)
                        break;
                }
            }

            MatchingBlock[] matchingBlocks = new MatchingBlock[noOfMB + 1];
            int mb = 0;
            o = 0;
            matchingBlocks[mb] = new MatchingBlock();

            for (i = n; i != 0; i--, o++)
            {
                if (ops[o].EditType == EditType.KEEP)
                {
                    matchingBlocks[mb].SourcePos = ops[o].SourceBegin;
                    matchingBlocks[mb].DestPos = ops[o].DestBegin;

                    while (i != 0 && ops[o].EditType == EditType.KEEP)
                    {
                        i--;
                        o++;
                    }

                    if (i == 0)
                    {
                        matchingBlocks[mb].Length = len1 - matchingBlocks[mb].SourcePos;
                        mb++;
                        break;
                    }

                    matchingBlocks[mb].Length = ops[o].SourceBegin - matchingBlocks[mb].SourcePos;
                    mb++;
                    matchingBlocks[mb] = new MatchingBlock();
                }
            }

            if (mb == noOfMB)
            {
                throw new InvalidOperationException("Internal matching-block computation failed.");
            }

            MatchingBlock finalBlock = new MatchingBlock
            {
                SourcePos = len1,
                DestPos   = len2,
                Length    = 0
            };

            matchingBlocks[mb] = finalBlock;

            return matchingBlocks;
        }


        private static MatchingBlock[] GetMatchingBlocks(int len1, int len2, EditOp[] ops)
        {

            int n = ops.Length;

            int numberOfMatchingBlocks, i, SourcePos, DestPos;

            numberOfMatchingBlocks = 0;

            int o = 0;

            SourcePos = DestPos = 0;

            EditType type;

            for (i = n; i != 0;)
            {


                while (ops[o].EditType == EditType.KEEP && --i != 0)
                {
                    o++;
                }

                if (i == 0)
                    break;

                if (SourcePos < ops[o].SourcePos || DestPos < ops[o].DestPos)
                {

                    numberOfMatchingBlocks++;
                    SourcePos = ops[o].SourcePos;
                    DestPos = ops[o].DestPos;

                }

                type = ops[o].EditType;

                switch (type)
                {
                    case EditType.REPLACE:
                        do
                        {
                            SourcePos++;
                            DestPos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    case EditType.DELETE:
                        do
                        {
                            SourcePos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    case EditType.INSERT:
                        do
                        {
                            DestPos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    default:
                        break;
                }
            }

            if (SourcePos < len1 || DestPos < len2)
            {
                numberOfMatchingBlocks++;
            }

            MatchingBlock[] matchingBlocks = new MatchingBlock[numberOfMatchingBlocks + 1];

            o = 0;
            SourcePos = DestPos = 0;
            int mbIndex = 0;


            for (i = n; i != 0;)
            {

                while (ops[o].EditType == EditType.KEEP && --i != 0)
                    o++;

                if (i == 0)
                    break;

                if (SourcePos < ops[o].SourcePos || DestPos < ops[o].DestPos)
                {
                    MatchingBlock mb = new MatchingBlock();

                    mb.SourcePos = SourcePos;
                    mb.DestPos = DestPos;
                    mb.Length = ops[o].SourcePos - SourcePos;
                    SourcePos = ops[o].SourcePos;
                    DestPos = ops[o].DestPos;

                    matchingBlocks[mbIndex++] = mb;

                }

                type = ops[o].EditType;

                switch (type)
                {
                    case EditType.REPLACE:
                        do
                        {
                            SourcePos++;
                            DestPos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    case EditType.DELETE:
                        do
                        {
                            SourcePos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    case EditType.INSERT:
                        do
                        {
                            DestPos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    default:
                        break;
                }
            }

            if (SourcePos < len1 || DestPos < len2)
            {
                if (len1 - SourcePos != len2 - DestPos)
                {
                    throw new InvalidOperationException("Edit-op block lengths are inconsistent.");
                }

                MatchingBlock mb = new MatchingBlock();
                mb.SourcePos = SourcePos;
                mb.DestPos = DestPos;
                mb.Length = len1 - SourcePos;

                matchingBlocks[mbIndex++] = mb;
            }

            if (numberOfMatchingBlocks != mbIndex)
            {
                throw new InvalidOperationException("Matching block count mismatch.");
            }

            MatchingBlock finalBlock = new MatchingBlock();
            finalBlock.SourcePos = len1;
            finalBlock.DestPos = len2;
            finalBlock.Length = 0;

            matchingBlocks[mbIndex] = finalBlock;

            return matchingBlocks;
        }


        private static OpCode[] EditOpsToOpCodes(EditOp[] ops, int len1, int len2)
        {
            int n = ops.Length;
            int noOfBlocks, i, SourcePos, DestPos;
            int o = 0;
            EditType type;

            noOfBlocks = 0;
            SourcePos = DestPos = 0;

            for (i = n; i != 0;)
            {

                while (ops[o].EditType == EditType.KEEP && --i != 0)
                {
                    o++;
                }

                if (i == 0)
                    break;

                if (SourcePos < ops[o].SourcePos || DestPos < ops[o].DestPos)
                {

                    noOfBlocks++;
                    SourcePos = ops[o].SourcePos;
                    DestPos = ops[o].DestPos;

                }

                // Each edit operation starts a new block
                noOfBlocks++;
                type = ops[o].EditType;

                switch (type)
                {
                    case EditType.REPLACE:
                        do
                        {
                            SourcePos++;
                            DestPos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    case EditType.DELETE:
                        do
                        {
                            SourcePos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    case EditType.INSERT:
                        do
                        {
                            DestPos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    default:
                        break;
                }
            }

            if (SourcePos < len1 || DestPos < len2)
                noOfBlocks++;

            OpCode[] opCodes = new OpCode[noOfBlocks];

            o = 0;
            SourcePos = DestPos = 0;
            int oIndex = 0;

            for (i = n; i != 0;)
            {

                while (ops[o].EditType == EditType.KEEP && --i != 0)
                    o++;

                if (i == 0)
                    break;

                OpCode oc = new OpCode();
                opCodes[oIndex] = oc;
                oc.SourceBegin = SourcePos;
                oc.DestBegin = DestPos;

                if (SourcePos < ops[o].SourcePos || DestPos < ops[o].DestPos)
                {

                    oc.EditType = EditType.KEEP;
                    SourcePos = oc.SourceEnd = ops[o].SourcePos;
                    DestPos = oc.DestEnd = ops[o].DestPos;

                    oIndex++;
                    OpCode oc2 = new OpCode();
                    opCodes[oIndex] = oc2;
                    oc2.SourceBegin = SourcePos;
                    oc2.DestBegin = DestPos;

                }

                type = ops[o].EditType;

                switch (type)
                {
                    case EditType.REPLACE:
                        do
                        {
                            SourcePos++;
                            DestPos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    case EditType.DELETE:
                        do
                        {
                            SourcePos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    case EditType.INSERT:
                        do
                        {
                            DestPos++;
                            i--;
                            o++;
                        } while (i != 0 && ops[o].EditType == type &&
                                SourcePos == ops[o].SourcePos && DestPos == ops[o].DestPos);
                        break;

                    default:
                        break;
                }

                opCodes[oIndex].EditType = type;
                opCodes[oIndex].SourceEnd = SourcePos;
                opCodes[oIndex].DestEnd = DestPos;
                oIndex++;
            }

            if (SourcePos < len1 || DestPos < len2)
            {

                if (len1 - SourcePos != len2 - DestPos)
                {
                    throw new InvalidOperationException("Opcode range lengths are inconsistent.");
                }
                if (opCodes[oIndex] == null)
                    opCodes[oIndex] = new OpCode();
                opCodes[oIndex].EditType = EditType.KEEP;
                opCodes[oIndex].SourceBegin = SourcePos;
                opCodes[oIndex].DestBegin = DestPos;
                opCodes[oIndex].SourceEnd = len1;
                opCodes[oIndex].DestEnd = len2;

                oIndex++;

            }

            if (oIndex != noOfBlocks)
            {
                throw new InvalidOperationException("Opcode block count mismatch.");
            }

            return opCodes;
        }

        private const int ShortStringThreshold = 64;
        private const int StackAllocIntThreshold = 256;

        // Special Case
        public static int EditDistance(string s1, string s2, int xcost = 0)
        {
            ArgumentNullException.ThrowIfNull(s1);
            ArgumentNullException.ThrowIfNull(s2);

            return EditDistance(s1.AsSpan(), s2.AsSpan(), xcost);
        }

        public static int EditDistance<T>(T[] c1, T[] c2, int xcost = 0) where T : IEquatable<T>
        {
            ArgumentNullException.ThrowIfNull(c1);
            ArgumentNullException.ThrowIfNull(c2);

            return EditDistance(c1.AsSpan(), c2.AsSpan(), xcost);
        }

        private static int EditDistance(ReadOnlySpan<char> left, ReadOnlySpan<char> right, int xcost)
        {
            TrimCommonAffixes(ref left, ref right);

            int len1 = left.Length;
            int len2 = right.Length;

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
                ReadOnlySpan<char> temp = left;
                left = right;
                right = temp;

                len1 = left.Length;
                len2 = right.Length;
            }

            if (len1 == 1)
            {
                int found = right.IndexOf(left[0]) >= 0 ? 1 : 0;
                return xcost != 0 ? len2 + 1 - (2 * found) : len2 - found;
            }

            int substitutionCost = xcost != 0 ? 2 : 1;
            if (len2 <= ShortStringThreshold)
            {
                return EditDistanceCoreShort(left, right, substitutionCost);
            }

            return EditDistanceCore(left, right, substitutionCost);
        }

        private static int EditDistance<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, int xcost) where T : IEquatable<T>
        {
            TrimCommonAffixes(ref left, ref right);

            int len1 = left.Length;
            int len2 = right.Length;

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
                ReadOnlySpan<T> temp = left;
                left = right;
                right = temp;

                len1 = left.Length;
                len2 = right.Length;
            }

            if (len1 == 1)
            {
                int found = Contains(right, left[0]);
                return xcost != 0 ? len2 + 1 - (2 * found) : len2 - found;
            }

            int substitutionCost = xcost != 0 ? 2 : 1;
            if (len2 <= ShortStringThreshold)
            {
                return EditDistanceCoreShort(left, right, substitutionCost);
            }

            return EditDistanceCore(left, right, substitutionCost);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int EditDistanceCoreShort(ReadOnlySpan<char> left, ReadOnlySpan<char> right, int substitutionCost)
        {
            Span<int> row = stackalloc int[ShortStringThreshold + 1];
            return ComputeEditDistance(left, right, substitutionCost, row[..(right.Length + 1)]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int EditDistanceCoreShort<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, int substitutionCost) where T : IEquatable<T>
        {
            Span<int> row = stackalloc int[ShortStringThreshold + 1];
            return ComputeEditDistance(left, right, substitutionCost, row[..(right.Length + 1)]);
        }

        private static int EditDistanceCore(ReadOnlySpan<char> left, ReadOnlySpan<char> right, int substitutionCost)
        {
            int rowLength = right.Length + 1;
            if (rowLength <= StackAllocIntThreshold)
            {
                Span<int> row = stackalloc int[rowLength];
                return ComputeEditDistance(left, right, substitutionCost, row);
            }

            int[] rented = ArrayPool<int>.Shared.Rent(rowLength);
            try
            {
                return ComputeEditDistance(left, right, substitutionCost, rented.AsSpan(0, rowLength));
            }
            finally
            {
                ArrayPool<int>.Shared.Return(rented);
            }
        }

        private static int EditDistanceCore<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, int substitutionCost) where T : IEquatable<T>
        {
            int rowLength = right.Length + 1;
            if (rowLength <= StackAllocIntThreshold)
            {
                Span<int> row = stackalloc int[rowLength];
                return ComputeEditDistance(left, right, substitutionCost, row);
            }

            int[] rented = ArrayPool<int>.Shared.Rent(rowLength);
            try
            {
                return ComputeEditDistance(left, right, substitutionCost, rented.AsSpan(0, rowLength));
            }
            finally
            {
                ArrayPool<int>.Shared.Return(rented);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ComputeEditDistance(ReadOnlySpan<char> left, ReadOnlySpan<char> right, int substitutionCost, Span<int> row)
        {
            int rowLength = right.Length + 1;
            for (int j = 0; j < rowLength; j++)
            {
                row[j] = j;
            }

            if (substitutionCost == 2)
            {
                int end = rowLength - 1;

                for (int i = 1; i <= left.Length; i++)
                {
                    int p = 1;
                    int d = i;
                    int x = i;
                    char leftValue = left[i - 1];

                    while (p <= end)
                    {
                        if (leftValue == right[p - 1])
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

                return row[end];
            }

            for (int i = 1; i <= left.Length; i++)
            {
                int previousDiagonal = row[0];
                row[0] = i;
                char leftValue = left[i - 1];

                for (int j = 1; j < rowLength; j++)
                {
                    int previousRow = row[j];
                    int insertion = row[j - 1] + 1;
                    int deletion = previousRow + 1;
                    int substitution = previousDiagonal + (leftValue == right[j - 1] ? 0 : substitutionCost);

                    int best = insertion < deletion ? insertion : deletion;
                    if (substitution < best)
                    {
                        best = substitution;
                    }

                    row[j] = best;
                    previousDiagonal = previousRow;
                }
            }

            return row[rowLength - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ComputeEditDistance<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, int substitutionCost, Span<int> row) where T : IEquatable<T>
        {
            int rowLength = right.Length + 1;
            for (int j = 0; j < rowLength; j++)
            {
                row[j] = j;
            }

            if (substitutionCost == 2)
            {
                int end = rowLength - 1;

                for (int i = 1; i <= left.Length; i++)
                {
                    int p = 1;
                    int d = i;
                    int x = i;
                    T leftValue = left[i - 1];

                    while (p <= end)
                    {
                        if (leftValue.Equals(right[p - 1]))
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

                return row[end];
            }

            for (int i = 1; i <= left.Length; i++)
            {
                int previousDiagonal = row[0];
                row[0] = i;
                T leftValue = left[i - 1];

                for (int j = 1; j < rowLength; j++)
                {
                    int previousRow = row[j];
                    int insertion = row[j - 1] + 1;
                    int deletion = previousRow + 1;
                    int substitution = previousDiagonal + (leftValue.Equals(right[j - 1]) ? 0 : substitutionCost);

                    int best = insertion < deletion ? insertion : deletion;
                    if (substitution < best)
                    {
                        best = substitution;
                    }

                    row[j] = best;
                    previousDiagonal = previousRow;
                }
            }

            return row[rowLength - 1];
        }

        private static void TrimCommonAffixes(ref ReadOnlySpan<char> left, ref ReadOnlySpan<char> right)
        {
            int prefix = 0;
            int max = Math.Min(left.Length, right.Length);

            while (prefix < max && left[prefix] == right[prefix])
            {
                prefix++;
            }

            if (prefix != 0)
            {
                left = left[prefix..];
                right = right[prefix..];
            }

            int suffix = 0;
            max = Math.Min(left.Length, right.Length);

            while (suffix < max && left[left.Length - suffix - 1] == right[right.Length - suffix - 1])
            {
                suffix++;
            }

            if (suffix != 0)
            {
                left = left[..^suffix];
                right = right[..^suffix];
            }
        }

        private static void TrimCommonAffixes<T>(ref ReadOnlySpan<T> left, ref ReadOnlySpan<T> right) where T : IEquatable<T>
        {
            int prefix = 0;
            int max = Math.Min(left.Length, right.Length);

            while (prefix < max && left[prefix].Equals(right[prefix]))
            {
                prefix++;
            }

            if (prefix != 0)
            {
                left = left[prefix..];
                right = right[prefix..];
            }

            int suffix = 0;
            max = Math.Min(left.Length, right.Length);

            while (suffix < max && left[left.Length - suffix - 1].Equals(right[right.Length - suffix - 1]))
            {
                suffix++;
            }

            if (suffix != 0)
            {
                left = left[..^suffix];
                right = right[..^suffix];
            }
        }

        private static int Contains<T>(ReadOnlySpan<T> haystack, T needle) where T : IEquatable<T>
        {
            for (int i = 0; i < haystack.Length; i++)
            {
                if (haystack[i].Equals(needle))
                {
                    return 1;
                }
            }

            return 0;
        }

        public static double GetRatio<T>(T[] input1, T[] input2) where T : IEquatable<T>
        {
            ArgumentNullException.ThrowIfNull(input1);
            ArgumentNullException.ThrowIfNull(input2);

            int len1   = input1.Length;
            int len2   = input2.Length;
            int lensum = len1 + len2;

            int editDistance = EditDistance(input1.AsSpan(), input2.AsSpan(), 1);

            return editDistance == 0 ? 1 : (lensum - editDistance) / (double)lensum;
        }

        public static double GetRatio<T>(IEnumerable<T> input1, IEnumerable<T> input2) where T : IEquatable<T>
        {
            ArgumentNullException.ThrowIfNull(input1);
            ArgumentNullException.ThrowIfNull(input2);

            var s1 = input1.ToArray();
            var s2 = input2.ToArray();
            int len1 = s1.Length;
            int len2 = s2.Length;
            int lensum = len1 + len2;

            int editDistance = EditDistance(s1.AsSpan(), s2.AsSpan(), 1);

            return editDistance == 0 ? 1 : (lensum - editDistance) / (double)lensum;
        }

        // Special Case
        public static double GetRatio(string s1, string s2)
        {
            ArgumentNullException.ThrowIfNull(s1);
            ArgumentNullException.ThrowIfNull(s2);

            int lensum = s1.Length + s2.Length;
            int editDistance = EditDistance(s1.AsSpan(), s2.AsSpan(), 1);
            return editDistance == 0 ? 1 : (lensum - editDistance) / (double)lensum;
        }
    }
}
