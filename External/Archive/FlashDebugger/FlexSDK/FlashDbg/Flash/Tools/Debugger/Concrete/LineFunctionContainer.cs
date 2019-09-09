////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using Flash.Swf;
using Flash.Swf.Types;
using Flash.Swf.Actions;
using Flash.Swf.Debug;
using Flash.Util;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> This class extends the SwfActionContainer.
	/// It performs a number of passes on the master
	/// action list in order to extract line/function
	/// mapping information.
	/// </summary>
	public class LineFunctionContainer:SwfActionContainer
	{
		public LineFunctionContainer(byte[] swf, byte[] swd):base(swf, swd)
		{
			
			// now that we've got all the action lists
			// nicely extracted and lined up we perform a 
			// bit of magic which modifies the DefineFunction 
			// records augmenting them with function names 
			// if they have have none.
			buildFunctionNames(MasterList, Header.version);
		}
		
		/// <summary> Use the action list located in the given location
		/// and return a new action location that corresponds
		/// to the next line record that is encountered
		/// after this location.  This routine does not 
		/// span into another action list.
		/// </summary>
		public virtual ActionLocation endOfSourceLine(ActionLocation l)
		{
			ActionLocation current = new ActionLocation(l);
			int size = l.actions.size();
			for (int i = l.at + 1; i < size; i++)
			{
				// hit a line record => we done
				Action a = l.actions.getAction(i);
				if (a.code == ActionList.sactionLineRecord)
					break;
				
				// hit a function => we are done
				if ((a.code == ActionConstants.sactionDefineFunction) || (a.code == ActionConstants.sactionDefineFunction2))
					break;
				
				current.at = i;
			}
			return current;
		}
		
		/// <summary> This routine is called from the DSwfInfo object
		/// and is used to obtain LineRecord information 
		/// from the ActionLists
		/// </summary>
		public virtual void  combForLineRecords(DSwfInfo info)
		{
			probeForLineRecords(MasterList, new ActionLocation(), info);
		}
		
		/// <summary> This routine is called from the DSwfInfo object
		/// and is used to obtain LineRecord information 
		/// from the ActionLists
		/// 
		/// The ActionLocation record is used as a holding
		/// container for state as we traverse the lists
		/// </summary>
		internal virtual void  probeForLineRecords(ActionList list, ActionLocation where, DSwfInfo info)
		{
			int size = list.size();
			for (int i = 0; i < size; i++)
			{
				try
				{
					// set our context
					where.at = i;
					where.actions = list;
					
					// pull the action
					Action a = list.getAction(i);
					
					// then see if we need to traverse
					if ((a.code == ActionConstants.sactionDefineFunction) || (a.code == ActionConstants.sactionDefineFunction2))
					{
						where.function = (DefineFunction) a;
						probeForLineRecords(((DefineFunction) a).actionList, where, info);
						where.function = null;
					}
					else if (a.code == ActionList.sactionLineRecord)
					{
						// hit a line record, so let's do our callback
						info.processLineRecord(where, (LineRecord) a);
					}
					else if (a is DummyAction)
					{
						// our dummy container, then we drop in
						where.className = ((DummyAction) a).ClassName;
						probeForLineRecords(((DummyAction) a).ActionList, where, info);
						where.className = null;
					}
				}
				catch (Exception e)
				{
					// this is fairly bad and probably means that we have corrupt line
					// records in the swd, the exception being an ArrayIndexOutOfBoundsException. 
					// I've seen this in cases where a bad swc is built by authoring wherein a
					// script id collision occurs and thus the offset table will contain references
					// to line numbers that are non existent in one of the scripts.
					// If its another type of exception...well, hopefully the trace message will
					// help you track it down :)
					if (Trace.error)
					{
						Trace.trace("Error processing ActionList at " + where.at + " at offset " + where.actions.getOffset(where.at) + " in swf " + info.Url); //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
						Console.Error.Write(e.StackTrace);
                        Console.Error.Flush();
                    }
				}
			}
		}
		
		/// <summary> Go off and fill our DefineFunction records with function names.</summary>
		/// <seealso cref="Flash.Swf.MovieMetaData.walkActions(ActionList, int, String[], String, System.Collections.IList)"> for a discussion on how this is done.
		/// </seealso>
		internal virtual void buildFunctionNames(ActionList list, int version)
		{
			int size = list.size();
			for (int i = 0; i < size; i++)
			{
				DummyAction a = (DummyAction) list.getAction(i);
				MovieMetaData.walkActions(a.ActionList, version, null, a.ClassName, null);
			}
		}
	}
}