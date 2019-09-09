////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> Interface for receiving DMessages from the DProtocol object </summary>
	public interface DProtocolNotifierIF
	{
		/// <summary> Issused when a message has been received from the socket</summary>
		void  messageArrived(DMessage message, DProtocol which);
		
		/// <summary> Issued when the socket connection to the player is cut </summary>
		void  disconnected();
	}
}