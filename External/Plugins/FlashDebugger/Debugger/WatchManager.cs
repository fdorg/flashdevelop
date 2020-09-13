using System;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Utilities;

namespace FlashDebugger.Debugger
{
    public class WatchManager
    {
        IProject m_Project;
        string m_SaveFileFullPath;
        List<string> m_WatchList = new List<string>();

        public event EventHandler<WatchExpressionArgs> ExpressionAdded;
        public event EventHandler<WatchExpressionArgs> ExpressionRemoved;
        public event EventHandler<WatchExpressionReplaceArgs> ExpressionReplaced;
        public event EventHandler ExpressionsCleared;
        public event EventHandler ExpressionsLoaded;

        public IList<string> Watches => m_WatchList.AsReadOnly();

        public IProject Project
        {
            get => m_Project;
            set
            {
                if (value is null) return;
                m_Project = value;
                ClearAll();
                m_SaveFileFullPath = GetWatchFile(m_Project.ProjectPath);
            }
        }

        static string GetWatchFile(string path)
        {
            var cacheDir = Path.Combine(PathHelper.DataDir, nameof(FlashDebugger), "Watch");
            if (!Directory.Exists(cacheDir)) Directory.CreateDirectory(cacheDir);
            var hashFileName = HashCalculator.CalculateSHA1(path);
            return Path.Combine(cacheDir, hashFileName + ".xml");
        }

        public void ClearAll()
        {
            m_WatchList.Clear();
            OnExpressionsCleared(EventArgs.Empty);
        }

        public bool Add(string expr)
        {
            if (m_WatchList.Contains(expr)) return false;
            m_WatchList.Add(expr);
            OnExpressionAdded(expr, m_WatchList.Count - 1);
            return true;
        }

        public void InsertAt(int index, string expr)
        {
            m_WatchList.Insert(index, expr);
            OnExpressionAdded(expr, index);
        }

        public bool Remove(string expr)
        {
            var index = m_WatchList.IndexOf(expr);
            if (index == -1) return false;
            m_WatchList.RemoveAt(index);
            OnExpressionRemoved(expr, index);
            return true;
        }

        public bool RemoveAt(int index)
        {
            if (index >= m_WatchList.Count || index < 0)
                return false;

            string expr = m_WatchList[index];
            m_WatchList.RemoveAt(index);
            OnExpressionRemoved(expr, index);
            return true;
        }

        public bool Replace(string oldItem, string newItem)
        {
            if (m_WatchList.Contains(newItem)) return false;
            int itemN = m_WatchList.IndexOf(oldItem);
            if (itemN == -1) Add(newItem);
            else
            {
                m_WatchList[itemN] = newItem;
                OnExpressionReplaced(oldItem, newItem, itemN);
            }
            return true;
        }

        public void Save() => Save(m_SaveFileFullPath);

        public void Save(string filePath)
        {
            if (m_Project != null)
            {
                Util.SerializeXML<List<string>>.SaveFile(filePath, m_WatchList);
            }
        }

        public void Load() => Load(m_SaveFileFullPath);

        public void Load(string filePath)
        {
            m_WatchList = File.Exists(filePath)
                ? Util.SerializeXML<List<string>>.LoadFile(filePath)
                : new List<string>();
            OnExpressionsLoaded(EventArgs.Empty);
        }

        protected void OnExpressionAdded(string expr, int index) => ExpressionAdded?.Invoke(this, new WatchExpressionArgs(expr, index));

        protected void OnExpressionRemoved(string expr, int index) => ExpressionRemoved?.Invoke(this, new WatchExpressionArgs(expr, index));

        protected void OnExpressionReplaced(string oldExpr, string newExpr, int index) => ExpressionReplaced?.Invoke(this, new WatchExpressionReplaceArgs(oldExpr, newExpr, index));

        protected void OnExpressionsCleared(EventArgs e) => ExpressionsCleared?.Invoke(this, e);

        protected void OnExpressionsLoaded(EventArgs e) => ExpressionsLoaded?.Invoke(this, e);
    }

    public class WatchExpressionArgs : EventArgs
    {
        public string Expression { get; internal set; }

        public int Position { get; internal set; }

        public WatchExpressionArgs(string expression, int position)
        {
            Expression = expression;
            Position = position;
        }
    }

    public class WatchExpressionReplaceArgs : EventArgs
    {
        public string NewExpression { get; internal set; }

        public string OldExpression { get; internal set; }

        public int Position { get; internal set; }

        public WatchExpressionReplaceArgs(string oldExpression, string newExpression, int position)
        {
            OldExpression = oldExpression;
            NewExpression = newExpression;
            Position = position;
        }
    }
}