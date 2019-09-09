// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;

namespace PluginCore.BBCode
{
    public class BottomUpParser
    {
        public BottomUpParser()
        {
        }


        public IPairTagMatcher pairTagMatcher;
        public IPairTagMatchHandler pairTagHandler;

        public string input;
        public IndexTree lastTree;


        public IndexTree parse()
        {
            lastTree = null;

            if (pairTagMatcher is null || string.IsNullOrEmpty(this.input))
                return null;

            lastTree = _parse();
            return lastTree;
        }

        private IndexTree _parse()
        {
            pairTagMatcher.input = input;

            List<IPairTagMatch> openers = _findAllOpeners();
            IndexTree tree = _buildTree(openers);

            return tree;
        }

        private List<IPairTagMatch> _findAllOpeners()
        {
            List<IPairTagMatch> openers = new List<IPairTagMatch>();
            IPairTagMatch m;
            int prevI = 0;
            int prevL = 0;
            while (true)
            {
                m = pairTagMatcher.searchOpener((uint)(prevI + prevL));

                if (m is null)
                    break;

                prevI = m.tagIndex;
                prevL = (int)m.tagLength;

                if (pairTagHandler != null
                    && (!pairTagHandler.isHandleable(m)
                        || !pairTagHandler.handleTag(m)))
                    continue;

                openers.Add(m);
            }
            return openers;
        }

        private IndexTree _buildTree(List<IPairTagMatch> openers)
        {
            uint inputL = (uint)input.Length;
            bool closerOutOfBounds;
            int closerStartAt;
            Dictionary<int, IPairTagMatch> closerIndices = new Dictionary<int, IPairTagMatch>();
            IPairTagMatch mOp;
            IPairTagMatch mCl;
            IndexTree rootTree = new IndexTree(0, (int)inputL, 0, 0, null, null);
            int i = openers.Count;
            while (i-- > 0)
            {
                mOp = openers[i];
                closerStartAt = (int)(mOp.tagIndex + mOp.tagLength);
                closerOutOfBounds = false;

                while (true)
                {
                    mCl = pairTagMatcher.searchCloserFor(mOp, (uint)closerStartAt);
                    if (mCl is null)
                    {
                        mCl = new VoidCloserTagMatch((int)inputL);
                        closerOutOfBounds = true;
                    }

                    if (!closerIndices.ContainsKey(mCl.tagIndex))
                    {
                        if (mCl.tagIndex < inputL)
                            closerIndices[mCl.tagIndex] = mCl;

                        IndexTree.insertLeaf(rootTree,
                                              new IndexTree(mOp.tagIndex,
                                                             (int)(mCl.tagIndex + mCl.tagLength),
                                                             (int)(mOp.tagLength),
                                                             (int)(-mCl.tagLength),
                                                             new PairTag(mOp, mCl),
                                                             null));
                        break;
                    }

                    if (closerOutOfBounds)
                        break;
                    if (mCl != null)
                        closerStartAt = mCl.tagIndex + 1;
                }
            }
            return rootTree;
        }

    }
}
