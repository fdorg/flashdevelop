﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Aga.Controls.Tree;
using FlashDebugger.Controls;
using FlashDebugger.Controls.DataTree;

namespace FlashDebugger
{
	public class CopyTreeHelper
	{

		public static int _CopyTreeMaxRecursion = 10;
		public static int _CopyTreeMaxChars = 1000000;


		private static Dictionary<string, int> CopiedObjects;


		public static string GetTreeAsText(TreeNodeAdv treeNode, ValueNode dataNode, string levelSep, DataTreeControl control, int levelLimit)
		{
			StringBuilder sb = new StringBuilder();

			// ensure expanded
			control.ListChildItems(dataNode);

			// add children nodes
			CopiedObjects = new Dictionary<string, int>();
			GetTreeItemsAsText(new List<ValueNode> { dataNode }, levelSep, 0, sb, control, levelLimit);

			// free CopiedObjects
			CopiedObjects = null;

			return sb.ToString() ?? "";
		}

		private static void GetTreeItemsAsText(IList dataNodes, string levelSep, int level, StringBuilder sb, DataTreeControl control, int levelLimit)
		{

			// per node
			int len = dataNodes.Count;
			for (int c = 0; c < len; c++)
			{
				ValueNode child = (ValueNode)dataNodes[c];


				// skip if unwanted item
				if (!IsWantedChild(child))
				{
					continue;
				}


				// ensure expanded
				control.ListChildItems(child);

				// add node
				AppendTimes(sb, levelSep, level);
				if (child.IsPrimitive)
				{
					sb.Append(child.Text + " : " + child.Value + " ");
				}
				else
				{
					sb.Append(child.Text + " : " + child.ClassPath + " ");
				}

				// recurse for children .. but skip if unwanted items
				if (child.Nodes.Count > 0 && IsWantedParent(child))
				{

					// opening brace
					sb.AppendLine("{");

					string childID = child.ID;

					// add error if too many levels of recursion
					if (level <= levelLimit || levelLimit == 0)
					{

						// check if encountered before
						bool isNew = !CopiedObjects.ContainsKey(childID);
						if (!isNew)
						{

							// error
							AppendTimes(sb, levelSep, level + 1);
							sb.AppendLine("[Already listed before]");

						}
						else if (level > _CopyTreeMaxRecursion)
						{

							// error
							AppendTimes(sb, levelSep, level + 1);
							sb.AppendLine("[Recursion too deep, increase CopyTreeMaxRecursion in FlashDebugger settings]");

						}
						else
						{

							// add to list
							CopiedObjects.Add(childID, 1);

							// children
							try
							{
								GetTreeItemsAsText(child.Nodes, levelSep, level + 1, sb, control, levelLimit);
							}
							catch (System.Exception ex) { }

						}
					}


					// closing brace
					AppendTimes(sb, levelSep, level);
					sb.AppendLine("}");

				}
				else
				{
					sb.AppendLine();
				}


				// stop recursion if too long
				if (sb.Length > _CopyTreeMaxChars)
				{
					sb.Append("......");
					return;
				}


			}
		}

		private static bool IsWantedParent(DataNode parent)
		{
			try
			{

				// if is an empty array []
				// then skip it from opening and closing { } the output
				if (parent.Nodes.Count == 1 || parent.Nodes.Count == 2)
				{
					ValueNode pNode = parent as ValueNode;
					if (pNode != null && pNode.ClassPath == "Array")
					{
						DataNode child1 = (DataNode)parent.Nodes[0];
						if (child1.Text == "[static]" || child1.Text == "length")
						{
							return false;
						}
					}
				}

				// catch "cannot cast Aga.TreeNode to DataNode" 
				// TODO : fix this instead of just catching it
			}
			catch (Exception ex) { }
			return true;
		}

		private static string[] as3DisabledProps = new string[] { "stage", "parent", "root", "loaderInfo", "_nativeWindow", "nativeWindow" };

		private static bool IsWantedChild(DataNode child)
		{
			try
			{

				// skip if static
				if (child.Text == "[static]")
				{
					return false;
				}

				if (child.Parent != null)
				{
					ValueNode parent = (child.Parent as ValueNode);
					if (parent != null)
					{

						// if is an array []
						// skip [static] and "length" properties
						if (parent.ClassPath == "Array")
						{
							if (child.Text == "[static]" || child.Text == "length")
							{
								return false;
							}
						}

						// if is an AS3 display object,
						// don't go upward (stage, parent)
						if (parent.ClassPath.StartsWith("flash.display.") || parent.ClassPath == "Main")
						{
							if (Array.IndexOf(as3DisabledProps, child.Text) > -1)
							{
								return false;
							}
						}

						// if is an AS2 display object,
						// don't go upward (_parent)
						if (parent.ClassPath == "MovieClip" || parent.ClassPath == "Video")
						{
							if (child.Text == "_parent")
							{
								return false;
							}
						}

					}

				}

				// catch "cannot cast Aga.TreeNode to DataNode" 
				// TODO : fix this instead of just catching it
			}
			catch (Exception ex) { }
			return true;
		}

		private static void AppendTimes(StringBuilder sb, string append, int times)
		{
			for (int t = 0; t < times; t++)
			{
				sb.Append(append);
			}
		}

	}
}