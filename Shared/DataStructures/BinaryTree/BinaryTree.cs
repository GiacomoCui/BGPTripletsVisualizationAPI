using Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Shared.DataStructures.BinaryTree
{
    public class BinaryTree<T>
    {
        private BinaryTreeNode<T> Root = new();

        /// <summary>
        /// Try to add a node to a Binary Tree. If the node exists yet it does nothing returning false
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="CIDR">Number of bits to search the tree</param>
        /// <param name="value">Value to insert</param>
        /// <returns>True if the value is added, false if the node is already present</returns>
        public bool TryAdd(byte[] prefix, int CIDR, T value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (prefix is null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            if (CIDR < 0 || CIDR > prefix.Length * 8)
            {
                throw new ArgumentException("CIDR is negative or greater than passed prefix bits", nameof(CIDR));
            }
            return GetNode(prefix, CIDR).ConcurrentAddInfo(value);
        }

        public T GetInfo(byte[] prefix, int CIDR)
        {
            if (prefix is null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            if (CIDR < 0 || CIDR > prefix.Length * 8)
            {
                throw new ArgumentException("CIDR is negative or greater than passed prefix bits", nameof(CIDR));
            }

            return GetNode(prefix, CIDR).Info;
        }

        public T GetOrAdd(byte[] prefix, int CIDR, Func<T> insertCallback)
        {
            if (insertCallback is null)
            {
                throw new ArgumentNullException(nameof(insertCallback));
            }

            if (prefix is null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            if (CIDR < 0 || CIDR > prefix.Length * 8)
            {
                throw new ArgumentException("CIDR is negative or greater than passed prefix bits", nameof(CIDR));
            }

            return GetNode(prefix, CIDR).GetOrAddInfo(insertCallback);
        }

        public BinaryTreeNode<T> GetNode(byte[] key, int validBits)
        {
            BinaryTreeNode<T> actualNode = Root;

            if (validBits == 0)
            {
                return Root;
            }

            for (int i = 0; i < key.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bool bit = key[i].GetBitLittleEndian(j);
                    if (bit)
                    {
                        actualNode = actualNode.TrueNode;
                    }
                    else
                    {
                        actualNode = actualNode.FalseNode;
                    }

                    validBits--;

                    if (validBits == 0)
                    {
                        return actualNode;
                    }
                }
            }
            throw new Exception();
        }

        public void GetAllLeafs(out List<T> leaves)
        {
            leaves = new List<T>();
            Root.GetAllLeafs(ref leaves);
        }

        public void Empty()
        {
            Root = new BinaryTreeNode<T>();
        }
    }
}
