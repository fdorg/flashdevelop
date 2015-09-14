using System;
using System.Collections.Generic;
using System.Reflection;

namespace PluginCore.BBCode
{
    public class IndexTree
    {

        #region STATICS


        public static IndexTree getRootNode( IndexTree tree )
        {
            if( tree == null )
                return null;
                
            IndexTree r = tree;
            while( r.parentNode != null )
                r = r.parentNode;
            
            return r;
        }
        
        public static Boolean insertLeaf( IndexTree tree, IndexTree leaf )
        {
            if( tree == null || leaf == null )
                return false;
            
            if( leaf.indexA < tree.indexA && 
                leaf.indexB > tree.indexB )
            {
            //  trace( "leaf is parent" );
                
                leaf.parentNode = tree.parentNode;
                tree.parentNode = null;
                
                return insertLeaf( leaf, tree );
            }
            
            if( !(leaf.indexA >= tree.indexA && 
                  leaf.indexB <= tree.indexB) )
            {
            //  trace( ">> or << out of bounds" );
                
                if( tree.parentNode != null )
                    return insertLeaf( tree.parentNode, leaf );
                return false;
            }
            
            if( !(leaf.indexA >= tree.indexA && 
                  leaf.indexB <= tree.indexB) )
            {
            //  trace( "incorrect out of bounds" );
                
                return false;
            }
            
            if( tree.nodes == null )
                tree.nodes = new List<IndexTree>();
            
            // find where in subnodes it can be placed;
            int i = tree.nodes.Count;
            if( i == 0 )
            {
            //  trace( "push in subnodes[0]" );
                
                leaf.parentNode = tree;
                tree.nodes.Add( leaf );
                
                return true;
            }
            
            IndexTree currNode;
            int prevIdxA = tree.indexB;
            int currIdxA = -1;
            int prevGreatA = -1;
            int prevLessB = -1;
            while( i-- > -1 )
            {
                if( i > -1 )
                {
                    currNode = tree.nodes[i];
                    currIdxA = currNode.indexB;
                }
                else
                {
                    currIdxA = tree.indexA;
                }
                
                if( leaf.indexA >= currIdxA && 
                    leaf.indexB <= prevIdxA )
                {
                //  trace( "push in subnodes after [" + i + "]" );
                    
                    leaf.parentNode = tree;
                //  tree.nodes.splice( i+1, 0, leaf);
                    tree.nodes.Insert(i + 1, leaf);

                    
                    return true;
                }
                
                if( i < 0 )
                    break;
                
                currNode = tree.nodes[i];
                    
                if( leaf.indexA >= currNode.indexA && 
                    leaf.indexB <= currNode.indexB )
                {
                //  trace( "insertLeaf in subnode[" + i + "]" );
                    
                    return insertLeaf( currNode, leaf );
                }
                
                if( leaf.indexB >= currNode.indexB && i > prevLessB )
                    prevLessB = i;
                    
                if( leaf.indexA <= currNode.indexA )
                    prevGreatA = i;
                    
                prevIdxA = tree.nodes[i].indexA;
            }
            
            if( prevGreatA > -1 && prevLessB > -1 )
            {
            //  trace("Reinserting subnodes");
                
                leaf.parentNode = tree;
                
                List<IndexTree> cuttedNodes = new List<IndexTree>();
                int k = 1 + prevLessB - prevGreatA;
                while( k-- > prevGreatA )
                {
                    cuttedNodes.Add( tree.nodes[k] );
                    tree.nodes.RemoveAt( k );
                }
                cuttedNodes.Reverse();
                tree.nodes.Insert( prevGreatA, leaf );
                /*
                tree.nodes.splice( prevGreatA,
                                1 + prevLessB - prevGreatA,
                                leaf );
                */                                                     
            //  trace("cuttedNodes.length: " + cuttedNodes.length);
                
                i = cuttedNodes.Count;
                while( i-- > 0 )
                {
                    cuttedNodes[i].parentNode = null;
                    insertLeaf( leaf, cuttedNodes[i] );
                }
                
                return true;
            }
            
        //  throw new Error("Leaf cannot be inserted");
            return false;
        }

        public static List<IndexTree> flattenTree( IndexTree tree )
        {
            if( tree == null )
                return null;
            
            IndexTree t;
            List<IndexTree> flats = new List<IndexTree>();
            int offsetA = tree.offsetA;
            int prevIdxB = tree.indexA;
            int i = -1;
            int l = tree.nodes.Count;
            while( ++i < l )
            {
                t = tree.nodes[i];
                
                if( t == null )
                    continue;

                IndexTree fTree = new IndexTree(prevIdxB, t.indexA, offsetA, 0, tree.data, tree.parentNode);

                flats.Add( fTree );
                flats.AddRange( flattenTree( t ) );
                
                prevIdxB = t.indexB;
                offsetA = 0;
            }
            
            flats.Add( new IndexTree( prevIdxB, tree.indexB, offsetA, tree.offsetB, tree.data, tree.parentNode ) );
            
            return flats;
        }
        
        public static void normalizeTree( IndexTree tree )
        {
            _normalizeTree( tree, 0 );
        }
        
        private static int _normalizeTree( IndexTree tree, int offset )
        {
            if( tree == null )
                return offset;
            
            tree.indexA -= offset;
            offset += tree.offsetA;
            tree.offsetA = 0;
            
            if( tree.nodes != null )
            {
                int i = -1;
                int l = tree.nodes.Count;
                while( ++i < l )
                {
                    offset = _normalizeTree( tree.nodes[i], offset );
                }
            }
            
            offset -= tree.offsetB;
            tree.indexB -= offset;
            tree.offsetB = 0;
            
            return offset;
        }

        public static IndexTree cloneTree( IndexTree tree )
        {
            if( tree == null )
                return null;
            
            IndexTree clone = new IndexTree( tree.indexA,
                                             tree.indexB,
                                             tree.offsetA,
                                             tree.offsetB,
                                             tree.data,
                                             null );

            if (tree.data != null && tree.data.GetType().GetMethod("clone") != null)
                clone.data = tree.data.GetType().InvokeMember("clone", BindingFlags.InvokeMethod, null, tree.data, new object[] { });
                    
            IndexTree cloneNode;
            int i = -1;
            int l = tree.nodes.Count;
            while( ++i < l )
            {
                cloneNode = cloneTree( tree.nodes[i] );
                cloneNode.parentNode = clone;

                clone.nodes.Add(cloneNode);
            }
            
            return clone;
        }
        
        public static String treeToString( IndexTree tree )
        {
            return treeToString( tree, "", false );
        }
        public static String treeToString( IndexTree tree, String space, Boolean noEolAndEnd )
        {
            if( tree == null )
                return null;
            
            String s = "";
            
            if( !noEolAndEnd )
                s += "\n";
            
            s += space + "<tree"
                       + " indexA=" + tree.indexA.ToString()
                       + " indexB=" + tree.indexB.ToString()
                       + " offsetA=" + tree.offsetA.ToString()
                       + " offsetB=" + tree.offsetB.ToString()
                       + " nodes=" + tree.nodes.Count.ToString()
                       + " data='" + (tree.data == null ? "null" : tree.data.ToString()) + "'"
                       + " root='" + (getRootNode(tree).data == null ? "null" : getRootNode(tree).data.ToString()) + "'"
                       + ">";
                       
            int i = -1;
            int l = tree.nodes.Count;
            while( ++i < l )
            {
                s += treeToString( tree.nodes[i], space + "    ", noEolAndEnd );
            }
            
            if( !noEolAndEnd )
                s += "\n" + space + "</>";
            
            return s;
        }


        #endregion

        #region INSTANCE


        public IndexTree()
        {
            _init(-1, -1, 0, 0, null, null);
        }
        public IndexTree( int indexA,
                          int indexB,
                          int offsetA,
                          int offsetB,
                          object data,
                          IndexTree parentNode )
        {
            _init(indexA, indexB, offsetA, offsetB, data, parentNode);
        }
        private void _init( int indexA,
                            int indexB,
                            int offsetA,
                            int offsetB,
                            object data,
                            IndexTree parentNode )
        {
            this.indexA = indexA;
            this.indexB = indexB;
            this.offsetA = offsetA;
            this.offsetB = offsetB;
            this.data = data;
            this.parentNode = parentNode;

            nodes = new List<IndexTree>();
        }


        public IndexTree parentNode;
        public List<IndexTree> nodes;
        
        public int indexA;
        public int indexB;
        
        public int offsetA;
        public int offsetB;
        
        public object data/*<T>*/;


        #endregion

    }
}
