using System;
using System.Collections.Generic;

namespace Shared.DataStructures.BinaryTree
{
    public class BinaryTreeNode<T>
    {
        private BinaryTreeNode<T> _trueNode;
        private BinaryTreeNode<T> _falseNode;
        public BinaryTreeNode<T> TrueNode
        {
            get
            {
                if (_trueNode is null)
                {
                    lock (LockTrue)
                    {
                        _trueNode ??= new BinaryTreeNode<T>();
                    }
                }
                return _trueNode;
            }
            set { _trueNode = value; }
        }
        public BinaryTreeNode<T> FalseNode
        {
            get
            {
                if (_falseNode is null)
                {
                    lock (LockFalse)
                    {
                        _falseNode ??= new BinaryTreeNode<T>();
                    }
                }
                return _falseNode;
            }
            set { _falseNode = value; }
        }
        public T Info { get; set; }
        private readonly object LockAddInfo = new();
        private readonly object LockTrue = new();
        private readonly object LockFalse = new();

        public bool AddInfo(T info)
        {
            if (Info is null)
            {
                Info = info;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ConcurrentAddInfo(T info)
        {
            lock (LockAddInfo)
            {
                if (Info is null)
                {
                    Info = info;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public T GetOrAddInfo(Func<T> insertCallback)
        {
            if (insertCallback is null)
            {
                throw new ArgumentNullException(nameof(insertCallback));
            }

            lock (LockAddInfo)
            {
                Info ??= insertCallback.Invoke();
                return Info;
            }
        }

        internal void GetAllLeafs(ref List<T> appoggio)
        {
            if (Info is not null)
            {
                appoggio.Add(Info);
            }
            _falseNode?.GetAllLeafs(ref appoggio);
            _trueNode?.GetAllLeafs(ref appoggio);
        }

        internal IEnumerable<T> GetAllLeafsYield()
        {
            if (Info is not null)
            {
                yield return Info;
            }
            if (_falseNode is not null)
            {
                foreach (var elem in _falseNode.GetAllLeafsYield())
                {
                    yield return elem;
                }
            }
            if (_trueNode is not null)
            {
                foreach (var elem in _trueNode.GetAllLeafsYield())
                {
                    yield return elem;
                }
            }
        }
    }
}
