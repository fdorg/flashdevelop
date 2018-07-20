//*****************************************************************//
//                                                                 //
// This file is generated automatically by Aurigma COM to .NET 1.0 //
//                                                                 //
//*****************************************************************//

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

// Change the following attribute values to modify
// the information associated with an assembly.
/*[assembly: AssemblyTitle("Interop.ShockwaveFlashObjects")]
[assembly: AssemblyDescription("Shockwave Flash (translated by Aurigma COM to .NET)")]
[assembly: AssemblyVersion("1.0")]
[assembly: AssemblyConfiguration("Retail")]
[assembly: AssemblyCompany("Aurigma Inc.")]
[assembly: AssemblyProduct("ShockwaveFlashObjects translated by Aurigma COM to .NET")]
[assembly: AssemblyCopyright("Copyright (c) 2003 by Aurigma Inc.")]
[assembly: AssemblyTrademark("Aurigma COM to .NET")]*/

// Type library name: ShockwaveFlashObjects
// Type library description: Shockwave Flash
// Type library version: 1.0
// Type library language: Neutral
// Type library guid: {D27CDB6B-AE6D-11CF-96B8-444553540000}
// Type library source file name: C:\WINDOWS\system32\Macromed\Flash\FlDbg9d.ocx

namespace Interop.ShockwaveFlashObjects
{
  /// <summary><para><c>_IShockwaveFlashEvents</c> interface.  </para><para>Event interface for Shockwave Flash</para></summary>
  // Event interface for Shockwave Flash
  [Guid("D27CDB6D-AE6D-11CF-96B8-444553540000")]
  [ComImport]
  [TypeLibType((short)4112)]
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  public interface _IShockwaveFlashEvents
  {
    /// <summary><para><c>OnReadyStateChange</c> method of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>OnReadyStateChange</c> method was the following:  <c>HRESULT OnReadyStateChange (long newState)</c>;</para></remarks>
    // IDL: HRESULT OnReadyStateChange (long newState);
    // VB6: Sub OnReadyStateChange (ByVal newState As Long)
    [DispId(-609)]
    void OnReadyStateChange (int newState);

    /// <summary><para><c>OnProgress</c> method of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>OnProgress</c> method was the following:  <c>HRESULT OnProgress (long percentDone)</c>;</para></remarks>
    // IDL: HRESULT OnProgress (long percentDone);
    // VB6: Sub OnProgress (ByVal percentDone As Long)
    [DispId(1958)]
    void OnProgress (int percentDone);

    /// <summary><para><c>FSCommand</c> method of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>FSCommand</c> method was the following:  <c>HRESULT FSCommand (BSTR command, BSTR args)</c>;</para></remarks>
    // IDL: HRESULT FSCommand (BSTR command, BSTR args);
    // VB6: Sub FSCommand (ByVal command As String, ByVal args As String)
    [DispId(150)]
    void FSCommand ([MarshalAs(UnmanagedType.BStr)] string command, [MarshalAs(UnmanagedType.BStr)] string args);

    /// <summary><para><c>FlashCall</c> method of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>FlashCall</c> method was the following:  <c>HRESULT FlashCall (BSTR request)</c>;</para></remarks>
    // IDL: HRESULT FlashCall (BSTR request);
    // VB6: Sub FlashCall (ByVal request As String)
    [DispId(197)]
    void FlashCall ([MarshalAs(UnmanagedType.BStr)] string request);
  }

  /// <summary><para>Delegate for handling <c>OnReadyStateChange</c> event of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
  /// <remarks><para>An original IDL definition of <c>OnReadyStateChange</c> event was the following:  <c>HRESULT _IShockwaveFlashEvents_OnReadyStateChangeEventHandler (long newState)</c>;</para></remarks>
  // IDL: HRESULT _IShockwaveFlashEvents_OnReadyStateChangeEventHandler (long newState);
  // VB6: Sub _IShockwaveFlashEvents_OnReadyStateChangeEventHandler (ByVal newState As Long)
  public delegate void _IShockwaveFlashEvents_OnReadyStateChangeEventHandler (int newState);

  /// <summary><para>Delegate for handling <c>OnProgress</c> event of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
  /// <remarks><para>An original IDL definition of <c>OnProgress</c> event was the following:  <c>HRESULT _IShockwaveFlashEvents_OnProgressEventHandler (long percentDone)</c>;</para></remarks>
  // IDL: HRESULT _IShockwaveFlashEvents_OnProgressEventHandler (long percentDone);
  // VB6: Sub _IShockwaveFlashEvents_OnProgressEventHandler (ByVal percentDone As Long)
  public delegate void _IShockwaveFlashEvents_OnProgressEventHandler (int percentDone);

  /// <summary><para>Delegate for handling <c>FSCommand</c> event of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
  /// <remarks><para>An original IDL definition of <c>FSCommand</c> event was the following:  <c>HRESULT _IShockwaveFlashEvents_FSCommandEventHandler (BSTR command, BSTR args)</c>;</para></remarks>
  // IDL: HRESULT _IShockwaveFlashEvents_FSCommandEventHandler (BSTR command, BSTR args);
  // VB6: Sub _IShockwaveFlashEvents_FSCommandEventHandler (ByVal command As String, ByVal args As String)
  public delegate void _IShockwaveFlashEvents_FSCommandEventHandler ([MarshalAs(UnmanagedType.BStr)] string command, [MarshalAs(UnmanagedType.BStr)] string args);

  /// <summary><para>Delegate for handling <c>FlashCall</c> event of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
  /// <remarks><para>An original IDL definition of <c>FlashCall</c> event was the following:  <c>HRESULT _IShockwaveFlashEvents_FlashCallEventHandler (BSTR request)</c>;</para></remarks>
  // IDL: HRESULT _IShockwaveFlashEvents_FlashCallEventHandler (BSTR request);
  // VB6: Sub _IShockwaveFlashEvents_FlashCallEventHandler (ByVal request As String)
  public delegate void _IShockwaveFlashEvents_FlashCallEventHandler ([MarshalAs(UnmanagedType.BStr)] string request);

  /// <summary><para>Declaration of events of <c>_IShockwaveFlashEvents</c> source interface.  </para><para>Event interface for Shockwave Flash</para></summary>
  // Event interface for Shockwave Flash
  [ComEventInterface(typeof(_IShockwaveFlashEvents),typeof(_IShockwaveFlashEvents_EventProvider))]
  [ComVisible(false)]
  public interface _IShockwaveFlashEvents_Event
  {
    /// <summary><para><c>OnReadyStateChange</c> event of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
    event _IShockwaveFlashEvents_OnReadyStateChangeEventHandler OnReadyStateChange;

    /// <summary><para><c>OnProgress</c> event of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
    event _IShockwaveFlashEvents_OnProgressEventHandler OnProgress;

    /// <summary><para><c>FSCommand</c> event of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
    event _IShockwaveFlashEvents_FSCommandEventHandler FSCommand;

    /// <summary><para><c>FlashCall</c> event of <c>_IShockwaveFlashEvents</c> interface.</para></summary>
    event _IShockwaveFlashEvents_FlashCallEventHandler FlashCall;
  }

  [ClassInterface(ClassInterfaceType.None)]
  internal class _IShockwaveFlashEvents_SinkHelper: _IShockwaveFlashEvents
  {
    public int Cookie = 0;

    public event _IShockwaveFlashEvents_OnReadyStateChangeEventHandler OnReadyStateChangeDelegate = null;
    public void Set_OnReadyStateChangeDelegate(_IShockwaveFlashEvents_OnReadyStateChangeEventHandler deleg)
    {
      OnReadyStateChangeDelegate = deleg;
    }
    public bool Is_OnReadyStateChangeDelegate(_IShockwaveFlashEvents_OnReadyStateChangeEventHandler deleg)
    {
      return (OnReadyStateChangeDelegate == deleg);
    }
    public void Clear_OnReadyStateChangeDelegate()
    {
      OnReadyStateChangeDelegate = null;
    }
    void _IShockwaveFlashEvents.OnReadyStateChange (int newState)
    {
      if (OnReadyStateChangeDelegate!=null)
        OnReadyStateChangeDelegate(newState);
    }

    public event _IShockwaveFlashEvents_OnProgressEventHandler OnProgressDelegate = null;
    public void Set_OnProgressDelegate(_IShockwaveFlashEvents_OnProgressEventHandler deleg)
    {
      OnProgressDelegate = deleg;
    }
    public bool Is_OnProgressDelegate(_IShockwaveFlashEvents_OnProgressEventHandler deleg)
    {
      return (OnProgressDelegate == deleg);
    }
    public void Clear_OnProgressDelegate()
    {
      OnProgressDelegate = null;
    }
    void _IShockwaveFlashEvents.OnProgress (int percentDone)
    {
      if (OnProgressDelegate!=null)
        OnProgressDelegate(percentDone);
    }

    public event _IShockwaveFlashEvents_FSCommandEventHandler FSCommandDelegate = null;
    public void Set_FSCommandDelegate(_IShockwaveFlashEvents_FSCommandEventHandler deleg)
    {
      FSCommandDelegate = deleg;
    }
    public bool Is_FSCommandDelegate(_IShockwaveFlashEvents_FSCommandEventHandler deleg)
    {
      return (FSCommandDelegate == deleg);
    }
    public void Clear_FSCommandDelegate()
    {
      FSCommandDelegate = null;
    }
    void _IShockwaveFlashEvents.FSCommand (string command, string args)
    {
      if (FSCommandDelegate!=null)
        FSCommandDelegate(command, args);
    }

    public event _IShockwaveFlashEvents_FlashCallEventHandler FlashCallDelegate = null;
    public void Set_FlashCallDelegate(_IShockwaveFlashEvents_FlashCallEventHandler deleg)
    {
      FlashCallDelegate = deleg;
    }
    public bool Is_FlashCallDelegate(_IShockwaveFlashEvents_FlashCallEventHandler deleg)
    {
      return (FlashCallDelegate == deleg);
    }
    public void Clear_FlashCallDelegate()
    {
      FlashCallDelegate = null;
    }
    void _IShockwaveFlashEvents.FlashCall (string request)
    {
      if (FlashCallDelegate!=null)
        FlashCallDelegate(request);
    }
  }

  internal class _IShockwaveFlashEvents_EventProvider: IDisposable, _IShockwaveFlashEvents_Event
  {
    IConnectionPointContainer ConnectionPointContainer;
    IConnectionPoint ConnectionPoint;
    _IShockwaveFlashEvents_SinkHelper EventSinkHelper;
    int ConnectionCount;

    // Constructor: remember ConnectionPointContainer
    _IShockwaveFlashEvents_EventProvider(object CPContainer) : base()
    {
      ConnectionPointContainer = (IConnectionPointContainer)CPContainer;
    }

    // Force disconnection from ActiveX event source
    ~_IShockwaveFlashEvents_EventProvider()
    {
      Disconnect();
      ConnectionPointContainer = null;
    }

    // Aletnative to destructor
    void IDisposable.Dispose()
    {
      Disconnect();
      ConnectionPointContainer = null;
      GC.SuppressFinalize(this);
    }

    // Connect to ActiveX event source
    void Connect()
    {
      if (ConnectionPoint == null)
      {
        ConnectionCount = 0;
        Guid g = new Guid("D27CDB6D-AE6D-11CF-96B8-444553540000");
        ConnectionPointContainer.FindConnectionPoint(ref g, out ConnectionPoint);
        EventSinkHelper = new _IShockwaveFlashEvents_SinkHelper();
        ConnectionPoint.Advise(EventSinkHelper, out EventSinkHelper.Cookie);
      }
    }

    // Disconnect from ActiveX event source
    void Disconnect()
    {
      Monitor.Enter(this);
      try {
        if (EventSinkHelper != null)
          ConnectionPoint.Unadvise(EventSinkHelper.Cookie);
        ConnectionPoint = null;
        EventSinkHelper = null;
      } catch { }
      Monitor.Exit(this);
    }

    // If no event handler present then disconnect from ActiveX event source
    void CheckDisconnect()
    {
      ConnectionCount--;
      if (ConnectionCount <= 0)
        Disconnect();
    }

    event _IShockwaveFlashEvents_OnReadyStateChangeEventHandler _IShockwaveFlashEvents_Event.OnReadyStateChange
    {
      add
      {
        Monitor.Enter(this);
        try {
          Connect();
          EventSinkHelper.OnReadyStateChangeDelegate += value;
          ConnectionCount++;
        } catch { }
        Monitor.Exit(this);
      }
      remove
      {
        if (EventSinkHelper != null)
        {
          Monitor.Enter(this);
          try {
            EventSinkHelper.OnReadyStateChangeDelegate -= value;
            CheckDisconnect();
          } catch { }
          Monitor.Exit(this);
        }
      }
    }

    event _IShockwaveFlashEvents_OnProgressEventHandler _IShockwaveFlashEvents_Event.OnProgress
    {
      add
      {
        Monitor.Enter(this);
        try {
          Connect();
          EventSinkHelper.OnProgressDelegate += value;
          ConnectionCount++;
        } catch { }
        Monitor.Exit(this);
      }
      remove
      {
        if (EventSinkHelper != null)
        {
          Monitor.Enter(this);
          try {
            EventSinkHelper.OnProgressDelegate -= value;
            CheckDisconnect();
          } catch { }
          Monitor.Exit(this);
        }
      }
    }

    event _IShockwaveFlashEvents_FSCommandEventHandler _IShockwaveFlashEvents_Event.FSCommand
    {
      add
      {
        Monitor.Enter(this);
        try {
          Connect();
          EventSinkHelper.FSCommandDelegate += value;
          ConnectionCount++;
        } catch { }
        Monitor.Exit(this);
      }
      remove
      {
        if (EventSinkHelper != null)
        {
          Monitor.Enter(this);
          try {
            EventSinkHelper.FSCommandDelegate -= value;
            CheckDisconnect();
          } catch { }
          Monitor.Exit(this);
        }
      }
    }

    event _IShockwaveFlashEvents_FlashCallEventHandler _IShockwaveFlashEvents_Event.FlashCall
    {
      add
      {
        Monitor.Enter(this);
        try {
          Connect();
          EventSinkHelper.FlashCallDelegate += value;
          ConnectionCount++;
        } catch { }
        Monitor.Exit(this);
      }
      remove
      {
        if (EventSinkHelper != null)
        {
          Monitor.Enter(this);
          try {
            EventSinkHelper.FlashCallDelegate -= value;
            CheckDisconnect();
          } catch { }
          Monitor.Exit(this);
        }
      }
    }
  }

  /// <summary><para><c>IShockwaveFlash</c> interface.  </para><para>Shockwave Flash</para></summary>
  // Shockwave Flash
  [Guid("D27CDB6C-AE6D-11CF-96B8-444553540000")]
  [ComImport]
  [TypeLibType((short)4160)]
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  public interface IShockwaveFlash
  {
    /// <summary><para><c>SetZoomRect</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method SetZoomRect</para></summary>
    /// <remarks><para>An original IDL definition of <c>SetZoomRect</c> method was the following:  <c>HRESULT SetZoomRect (long left, long top, long right, long bottom)</c>;</para></remarks>
    // method SetZoomRect
    // IDL: HRESULT SetZoomRect (long left, long top, long right, long bottom);
    // VB6: Sub SetZoomRect (ByVal left As Long, ByVal top As Long, ByVal right As Long, ByVal bottom As Long)
    [DispId(109)]
    void SetZoomRect (int left, int top, int right, int bottom);

    /// <summary><para><c>Zoom</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method Zoom</para></summary>
    /// <remarks><para>An original IDL definition of <c>Zoom</c> method was the following:  <c>HRESULT Zoom (int factor)</c>;</para></remarks>
    // method Zoom
    // IDL: HRESULT Zoom (int factor);
    // VB6: Sub Zoom (ByVal factor As Long)
    [DispId(118)]
    void Zoom (int factor);

    /// <summary><para><c>Pan</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method Pan</para></summary>
    /// <remarks><para>An original IDL definition of <c>Pan</c> method was the following:  <c>HRESULT Pan (long x, long y, int mode)</c>;</para></remarks>
    // method Pan
    // IDL: HRESULT Pan (long x, long y, int mode);
    // VB6: Sub Pan (ByVal x As Long, ByVal y As Long, ByVal mode As Long)
    [DispId(119)]
    void Pan (int x, int y, int mode);

    /// <summary><para><c>Play</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method Play</para></summary>
    /// <remarks><para>An original IDL definition of <c>Play</c> method was the following:  <c>HRESULT Play (void)</c>;</para></remarks>
    // method Play
    // IDL: HRESULT Play (void);
    // VB6: Sub Play
    [DispId(112)]
    void Play ();

    /// <summary><para><c>Stop</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method Stop</para></summary>
    /// <remarks><para>An original IDL definition of <c>Stop</c> method was the following:  <c>HRESULT Stop (void)</c>;</para></remarks>
    // method Stop
    // IDL: HRESULT Stop (void);
    // VB6: Sub Stop
    [DispId(113)]
    void Stop ();

    /// <summary><para><c>Back</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method Back</para></summary>
    /// <remarks><para>An original IDL definition of <c>Back</c> method was the following:  <c>HRESULT Back (void)</c>;</para></remarks>
    // method Back
    // IDL: HRESULT Back (void);
    // VB6: Sub Back
    [DispId(114)]
    void Back ();

    /// <summary><para><c>Forward</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method Forward</para></summary>
    /// <remarks><para>An original IDL definition of <c>Forward</c> method was the following:  <c>HRESULT Forward (void)</c>;</para></remarks>
    // method Forward
    // IDL: HRESULT Forward (void);
    // VB6: Sub Forward
    [DispId(115)]
    void Forward ();

    /// <summary><para><c>Rewind</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method Rewind</para></summary>
    /// <remarks><para>An original IDL definition of <c>Rewind</c> method was the following:  <c>HRESULT Rewind (void)</c>;</para></remarks>
    // method Rewind
    // IDL: HRESULT Rewind (void);
    // VB6: Sub Rewind
    [DispId(116)]
    void Rewind ();

    /// <summary><para><c>StopPlay</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method StopPlay</para></summary>
    /// <remarks><para>An original IDL definition of <c>StopPlay</c> method was the following:  <c>HRESULT StopPlay (void)</c>;</para></remarks>
    // method StopPlay
    // IDL: HRESULT StopPlay (void);
    // VB6: Sub StopPlay
    [DispId(126)]
    void StopPlay ();

    /// <summary><para><c>GotoFrame</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method GotoFrame</para></summary>
    /// <remarks><para>An original IDL definition of <c>GotoFrame</c> method was the following:  <c>HRESULT GotoFrame (long FrameNum)</c>;</para></remarks>
    // method GotoFrame
    // IDL: HRESULT GotoFrame (long FrameNum);
    // VB6: Sub GotoFrame (ByVal FrameNum As Long)
    [DispId(127)]
    void GotoFrame (int FrameNum);

    /// <summary><para><c>CurrentFrame</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method CurrentFrame</para></summary>
    /// <remarks><para>An original IDL definition of <c>CurrentFrame</c> method was the following:  <c>HRESULT CurrentFrame ([out, retval] long* ReturnValue)</c>;</para></remarks>
    // method CurrentFrame
    // IDL: HRESULT CurrentFrame ([out, retval] long* ReturnValue);
    // VB6: Function CurrentFrame As Long
    [DispId(128)]
    int CurrentFrame ();

    /// <summary><para><c>IsPlaying</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method IsPlaying</para></summary>
    /// <remarks><para>An original IDL definition of <c>IsPlaying</c> method was the following:  <c>HRESULT IsPlaying ([out, retval] VARIANT_BOOL* ReturnValue)</c>;</para></remarks>
    // method IsPlaying
    // IDL: HRESULT IsPlaying ([out, retval] VARIANT_BOOL* ReturnValue);
    // VB6: Function IsPlaying As Boolean
    [DispId(129)]
    [return: MarshalAs(UnmanagedType.VariantBool)]
    bool IsPlaying ();

    /// <summary><para><c>PercentLoaded</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method PercentLoaded</para></summary>
    /// <remarks><para>An original IDL definition of <c>PercentLoaded</c> method was the following:  <c>HRESULT PercentLoaded ([out, retval] long* ReturnValue)</c>;</para></remarks>
    // method PercentLoaded
    // IDL: HRESULT PercentLoaded ([out, retval] long* ReturnValue);
    // VB6: Function PercentLoaded As Long
    [DispId(130)]
    int PercentLoaded ();

    /// <summary><para><c>FrameLoaded</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method FrameLoaded</para></summary>
    /// <remarks><para>An original IDL definition of <c>FrameLoaded</c> method was the following:  <c>HRESULT FrameLoaded (long FrameNum, [out, retval] VARIANT_BOOL* ReturnValue)</c>;</para></remarks>
    // method FrameLoaded
    // IDL: HRESULT FrameLoaded (long FrameNum, [out, retval] VARIANT_BOOL* ReturnValue);
    // VB6: Function FrameLoaded (ByVal FrameNum As Long) As Boolean
    [DispId(131)]
    [return: MarshalAs(UnmanagedType.VariantBool)]
    bool FrameLoaded (int FrameNum);

    /// <summary><para><c>FlashVersion</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method FlashVersion</para></summary>
    /// <remarks><para>An original IDL definition of <c>FlashVersion</c> method was the following:  <c>HRESULT FlashVersion ([out, retval] long* ReturnValue)</c>;</para></remarks>
    // method FlashVersion
    // IDL: HRESULT FlashVersion ([out, retval] long* ReturnValue);
    // VB6: Function FlashVersion As Long
    [DispId(132)]
    int FlashVersion ();

    /// <summary><para><c>LoadMovie</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method LoadMovie</para></summary>
    /// <remarks><para>An original IDL definition of <c>LoadMovie</c> method was the following:  <c>HRESULT LoadMovie (int layer, BSTR url)</c>;</para></remarks>
    // method LoadMovie
    // IDL: HRESULT LoadMovie (int layer, BSTR url);
    // VB6: Sub LoadMovie (ByVal layer As Long, ByVal url As String)
    [DispId(142)]
    void LoadMovie (int layer, [MarshalAs(UnmanagedType.BStr)] string url);

    /// <summary><para><c>TGotoFrame</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TGotoFrame</para></summary>
    /// <remarks><para>An original IDL definition of <c>TGotoFrame</c> method was the following:  <c>HRESULT TGotoFrame (BSTR target, long FrameNum)</c>;</para></remarks>
    // method TGotoFrame
    // IDL: HRESULT TGotoFrame (BSTR target, long FrameNum);
    // VB6: Sub TGotoFrame (ByVal target As String, ByVal FrameNum As Long)
    [DispId(143)]
    void TGotoFrame ([MarshalAs(UnmanagedType.BStr)] string target, int FrameNum);

    /// <summary><para><c>TGotoLabel</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TGotoLabel</para></summary>
    /// <remarks><para>An original IDL definition of <c>TGotoLabel</c> method was the following:  <c>HRESULT TGotoLabel (BSTR target, BSTR label)</c>;</para></remarks>
    // method TGotoLabel
    // IDL: HRESULT TGotoLabel (BSTR target, BSTR label);
    // VB6: Sub TGotoLabel (ByVal target As String, ByVal label As String)
    [DispId(144)]
    void TGotoLabel ([MarshalAs(UnmanagedType.BStr)] string target, [MarshalAs(UnmanagedType.BStr)] string label);

    /// <summary><para><c>TCurrentFrame</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TCurrentFrame</para></summary>
    /// <remarks><para>An original IDL definition of <c>TCurrentFrame</c> method was the following:  <c>HRESULT TCurrentFrame (BSTR target, [out, retval] long* ReturnValue)</c>;</para></remarks>
    // method TCurrentFrame
    // IDL: HRESULT TCurrentFrame (BSTR target, [out, retval] long* ReturnValue);
    // VB6: Function TCurrentFrame (ByVal target As String) As Long
    [DispId(145)]
    int TCurrentFrame ([MarshalAs(UnmanagedType.BStr)] string target);

    /// <summary><para><c>TCurrentLabel</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TCurrentLabel</para></summary>
    /// <remarks><para>An original IDL definition of <c>TCurrentLabel</c> method was the following:  <c>HRESULT TCurrentLabel (BSTR target, [out, retval] BSTR* ReturnValue)</c>;</para></remarks>
    // method TCurrentLabel
    // IDL: HRESULT TCurrentLabel (BSTR target, [out, retval] BSTR* ReturnValue);
    // VB6: Function TCurrentLabel (ByVal target As String) As String
    [DispId(146)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string TCurrentLabel ([MarshalAs(UnmanagedType.BStr)] string target);

    /// <summary><para><c>TPlay</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TPlay</para></summary>
    /// <remarks><para>An original IDL definition of <c>TPlay</c> method was the following:  <c>HRESULT TPlay (BSTR target)</c>;</para></remarks>
    // method TPlay
    // IDL: HRESULT TPlay (BSTR target);
    // VB6: Sub TPlay (ByVal target As String)
    [DispId(147)]
    void TPlay ([MarshalAs(UnmanagedType.BStr)] string target);

    /// <summary><para><c>TStopPlay</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TStopPlay</para></summary>
    /// <remarks><para>An original IDL definition of <c>TStopPlay</c> method was the following:  <c>HRESULT TStopPlay (BSTR target)</c>;</para></remarks>
    // method TStopPlay
    // IDL: HRESULT TStopPlay (BSTR target);
    // VB6: Sub TStopPlay (ByVal target As String)
    [DispId(148)]
    void TStopPlay ([MarshalAs(UnmanagedType.BStr)] string target);

    /// <summary><para><c>SetVariable</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method SetVariable</para></summary>
    /// <remarks><para>An original IDL definition of <c>SetVariable</c> method was the following:  <c>HRESULT SetVariable (BSTR name, BSTR value)</c>;</para></remarks>
    // method SetVariable
    // IDL: HRESULT SetVariable (BSTR name, BSTR value);
    // VB6: Sub SetVariable (ByVal name As String, ByVal value As String)
    [DispId(151)]
    void SetVariable ([MarshalAs(UnmanagedType.BStr)] string name, [MarshalAs(UnmanagedType.BStr)] string value);

    /// <summary><para><c>GetVariable</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method GetVariable</para></summary>
    /// <remarks><para>An original IDL definition of <c>GetVariable</c> method was the following:  <c>HRESULT GetVariable (BSTR name, [out, retval] BSTR* ReturnValue)</c>;</para></remarks>
    // method GetVariable
    // IDL: HRESULT GetVariable (BSTR name, [out, retval] BSTR* ReturnValue);
    // VB6: Function GetVariable (ByVal name As String) As String
    [DispId(152)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string GetVariable ([MarshalAs(UnmanagedType.BStr)] string name);

    /// <summary><para><c>TSetProperty</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TSetProperty</para></summary>
    /// <remarks><para>An original IDL definition of <c>TSetProperty</c> method was the following:  <c>HRESULT TSetProperty (BSTR target, int property, BSTR value)</c>;</para></remarks>
    // method TSetProperty
    // IDL: HRESULT TSetProperty (BSTR target, int property, BSTR value);
    // VB6: Sub TSetProperty (ByVal target As String, ByVal property As Long, ByVal value As String)
    [DispId(153)]
    void TSetProperty ([MarshalAs(UnmanagedType.BStr)] string target, int property, [MarshalAs(UnmanagedType.BStr)] string value);

    /// <summary><para><c>TGetProperty</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TGetProperty</para></summary>
    /// <remarks><para>An original IDL definition of <c>TGetProperty</c> method was the following:  <c>HRESULT TGetProperty (BSTR target, int property, [out, retval] BSTR* ReturnValue)</c>;</para></remarks>
    // method TGetProperty
    // IDL: HRESULT TGetProperty (BSTR target, int property, [out, retval] BSTR* ReturnValue);
    // VB6: Function TGetProperty (ByVal target As String, ByVal property As Long) As String
    [DispId(154)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string TGetProperty ([MarshalAs(UnmanagedType.BStr)] string target, int property);

    /// <summary><para><c>TCallFrame</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TCallFrame</para></summary>
    /// <remarks><para>An original IDL definition of <c>TCallFrame</c> method was the following:  <c>HRESULT TCallFrame (BSTR target, int FrameNum)</c>;</para></remarks>
    // method TCallFrame
    // IDL: HRESULT TCallFrame (BSTR target, int FrameNum);
    // VB6: Sub TCallFrame (ByVal target As String, ByVal FrameNum As Long)
    [DispId(155)]
    void TCallFrame ([MarshalAs(UnmanagedType.BStr)] string target, int FrameNum);

    /// <summary><para><c>TCallLabel</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TCallLabel</para></summary>
    /// <remarks><para>An original IDL definition of <c>TCallLabel</c> method was the following:  <c>HRESULT TCallLabel (BSTR target, BSTR label)</c>;</para></remarks>
    // method TCallLabel
    // IDL: HRESULT TCallLabel (BSTR target, BSTR label);
    // VB6: Sub TCallLabel (ByVal target As String, ByVal label As String)
    [DispId(156)]
    void TCallLabel ([MarshalAs(UnmanagedType.BStr)] string target, [MarshalAs(UnmanagedType.BStr)] string label);

    /// <summary><para><c>TSetPropertyNum</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TSetPropertyNum</para></summary>
    /// <remarks><para>An original IDL definition of <c>TSetPropertyNum</c> method was the following:  <c>HRESULT TSetPropertyNum (BSTR target, int property, double value)</c>;</para></remarks>
    // method TSetPropertyNum
    // IDL: HRESULT TSetPropertyNum (BSTR target, int property, double value);
    // VB6: Sub TSetPropertyNum (ByVal target As String, ByVal property As Long, ByVal value As Double)
    [DispId(157)]
    void TSetPropertyNum ([MarshalAs(UnmanagedType.BStr)] string target, int property, double value);

    /// <summary><para><c>TGetPropertyNum</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TGetPropertyNum</para></summary>
    /// <remarks><para>An original IDL definition of <c>TGetPropertyNum</c> method was the following:  <c>HRESULT TGetPropertyNum (BSTR target, int property, [out, retval] double* ReturnValue)</c>;</para></remarks>
    // method TGetPropertyNum
    // IDL: HRESULT TGetPropertyNum (BSTR target, int property, [out, retval] double* ReturnValue);
    // VB6: Function TGetPropertyNum (ByVal target As String, ByVal property As Long) As Double
    [DispId(158)]
    double TGetPropertyNum ([MarshalAs(UnmanagedType.BStr)] string target, int property);

    /// <summary><para><c>TGetPropertyAsNumber</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method TGetPropertyAsNumber</para></summary>
    /// <remarks><para>An original IDL definition of <c>TGetPropertyAsNumber</c> method was the following:  <c>HRESULT TGetPropertyAsNumber (BSTR target, int property, [out, retval] double* ReturnValue)</c>;</para></remarks>
    // method TGetPropertyAsNumber
    // IDL: HRESULT TGetPropertyAsNumber (BSTR target, int property, [out, retval] double* ReturnValue);
    // VB6: Function TGetPropertyAsNumber (ByVal target As String, ByVal property As Long) As Double
    [DispId(172)]
    double TGetPropertyAsNumber ([MarshalAs(UnmanagedType.BStr)] string target, int property);

    /// <summary><para><c>EnforceLocalSecurity</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method EnforceLocalSecurity</para></summary>
    /// <remarks><para>An original IDL definition of <c>EnforceLocalSecurity</c> method was the following:  <c>HRESULT EnforceLocalSecurity (void)</c>;</para></remarks>
    // method EnforceLocalSecurity
    // IDL: HRESULT EnforceLocalSecurity (void);
    // VB6: Sub EnforceLocalSecurity
    [DispId(193)]
    void EnforceLocalSecurity ();

    /// <summary><para><c>CallFunction</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method Call</para></summary>
    /// <remarks><para>An original IDL definition of <c>CallFunction</c> method was the following:  <c>HRESULT CallFunction (BSTR request, [out, retval] BSTR* ReturnValue)</c>;</para></remarks>
    // method Call
    // IDL: HRESULT CallFunction (BSTR request, [out, retval] BSTR* ReturnValue);
    // VB6: Function CallFunction (ByVal request As String) As String
    [DispId(198)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string CallFunction ([MarshalAs(UnmanagedType.BStr)] string request);

    /// <summary><para><c>SetReturnValue</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method SetReturnValue</para></summary>
    /// <remarks><para>An original IDL definition of <c>SetReturnValue</c> method was the following:  <c>HRESULT SetReturnValue (BSTR returnValue)</c>;</para></remarks>
    // method SetReturnValue
    // IDL: HRESULT SetReturnValue (BSTR returnValue);
    // VB6: Sub SetReturnValue (ByVal returnValue As String)
    [DispId(199)]
    void SetReturnValue ([MarshalAs(UnmanagedType.BStr)] string returnValue);

    /// <summary><para><c>DisableLocalSecurity</c> method of <c>IShockwaveFlash</c> interface.  </para><para>method DisableLocalSecurity</para></summary>
    /// <remarks><para>An original IDL definition of <c>DisableLocalSecurity</c> method was the following:  <c>HRESULT DisableLocalSecurity (void)</c>;</para></remarks>
    // method DisableLocalSecurity
    // IDL: HRESULT DisableLocalSecurity (void);
    // VB6: Sub DisableLocalSecurity
    [DispId(200)]
    void DisableLocalSecurity ();

    /// <summary><para><c>AlignMode</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property AlignMode</para></summary>
    /// <remarks><para>An original IDL definition of <c>AlignMode</c> property was the following:  <c>int AlignMode</c>;</para></remarks>
    // property AlignMode
    // IDL: int AlignMode;
    // VB6: AlignMode As Long
    int AlignMode
    {
      // IDL: HRESULT AlignMode ([out, retval] int* ReturnValue);
      // VB6: Function AlignMode As Long
      [DispId(121)]
      get;
      // IDL: HRESULT AlignMode (int value);
      // VB6: Sub AlignMode (ByVal value As Long)
      [DispId(121)]
      set;
    }

    /// <summary><para><c>AllowFullScreen</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property AllowFullScreen</para></summary>
    /// <remarks><para>An original IDL definition of <c>AllowFullScreen</c> property was the following:  <c>BSTR AllowFullScreen</c>;</para></remarks>
    // property AllowFullScreen
    // IDL: BSTR AllowFullScreen;
    // VB6: AllowFullScreen As String
    string AllowFullScreen
    {
      // IDL: HRESULT AllowFullScreen ([out, retval] BSTR* ReturnValue);
      // VB6: Function AllowFullScreen As String
      [DispId(202)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT AllowFullScreen (BSTR value);
      // VB6: Sub AllowFullScreen (ByVal value As String)
      [DispId(202)]
      set;
    }

    /// <summary><para><c>AllowNetworking</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property AllowNetworking</para></summary>
    /// <remarks><para>An original IDL definition of <c>AllowNetworking</c> property was the following:  <c>BSTR AllowNetworking</c>;</para></remarks>
    // property AllowNetworking
    // IDL: BSTR AllowNetworking;
    // VB6: AllowNetworking As String
    string AllowNetworking
    {
      // IDL: HRESULT AllowNetworking ([out, retval] BSTR* ReturnValue);
      // VB6: Function AllowNetworking As String
      [DispId(201)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT AllowNetworking (BSTR value);
      // VB6: Sub AllowNetworking (ByVal value As String)
      [DispId(201)]
      set;
    }

    /// <summary><para><c>AllowScriptAccess</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property AllowScriptAccess</para></summary>
    /// <remarks><para>An original IDL definition of <c>AllowScriptAccess</c> property was the following:  <c>BSTR AllowScriptAccess</c>;</para></remarks>
    // property AllowScriptAccess
    // IDL: BSTR AllowScriptAccess;
    // VB6: AllowScriptAccess As String
    string AllowScriptAccess
    {
      // IDL: HRESULT AllowScriptAccess ([out, retval] BSTR* ReturnValue);
      // VB6: Function AllowScriptAccess As String
      [DispId(171)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT AllowScriptAccess (BSTR value);
      // VB6: Sub AllowScriptAccess (ByVal value As String)
      [DispId(171)]
      set;
    }

    /// <summary><para><c>BackgroundColor</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property BackgroundColor</para></summary>
    /// <remarks><para>An original IDL definition of <c>BackgroundColor</c> property was the following:  <c>long BackgroundColor</c>;</para></remarks>
    // property BackgroundColor
    // IDL: long BackgroundColor;
    // VB6: BackgroundColor As Long
    int BackgroundColor
    {
      // IDL: HRESULT BackgroundColor ([out, retval] long* ReturnValue);
      // VB6: Function BackgroundColor As Long
      [DispId(123)]
      get;
      // IDL: HRESULT BackgroundColor (long value);
      // VB6: Sub BackgroundColor (ByVal value As Long)
      [DispId(123)]
      set;
    }

    /// <summary><para><c>Base</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property Base</para></summary>
    /// <remarks><para>An original IDL definition of <c>Base</c> property was the following:  <c>BSTR Base</c>;</para></remarks>
    // property Base
    // IDL: BSTR Base;
    // VB6: Base As String
    string Base
    {
      // IDL: HRESULT Base ([out, retval] BSTR* ReturnValue);
      // VB6: Function Base As String
      [DispId(136)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT Base (BSTR value);
      // VB6: Sub Base (ByVal value As String)
      [DispId(136)]
      set;
    }

    /// <summary><para><c>BGColor</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property BGColor</para></summary>
    /// <remarks><para>An original IDL definition of <c>BGColor</c> property was the following:  <c>BSTR BGColor</c>;</para></remarks>
    // property BGColor
    // IDL: BSTR BGColor;
    // VB6: BGColor As String
    string BGColor
    {
      // IDL: HRESULT BGColor ([out, retval] BSTR* ReturnValue);
      // VB6: Function BGColor As String
      [DispId(140)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT BGColor (BSTR value);
      // VB6: Sub BGColor (ByVal value As String)
      [DispId(140)]
      set;
    }

    /// <summary><para><c>DeviceFont</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property DeviceFont</para></summary>
    /// <remarks><para>An original IDL definition of <c>DeviceFont</c> property was the following:  <c>VARIANT_BOOL DeviceFont</c>;</para></remarks>
    // property DeviceFont
    // IDL: VARIANT_BOOL DeviceFont;
    // VB6: DeviceFont As Boolean
    bool DeviceFont
    {
      // IDL: HRESULT DeviceFont ([out, retval] VARIANT_BOOL* ReturnValue);
      // VB6: Function DeviceFont As Boolean
      [DispId(138)]
      [return: MarshalAs(UnmanagedType.VariantBool)]
      get;
      // IDL: HRESULT DeviceFont (VARIANT_BOOL value);
      // VB6: Sub DeviceFont (ByVal value As Boolean)
      [DispId(138)]
      set;
    }

    /// <summary><para><c>EmbedMovie</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property EmbedMovie</para></summary>
    /// <remarks><para>An original IDL definition of <c>EmbedMovie</c> property was the following:  <c>VARIANT_BOOL EmbedMovie</c>;</para></remarks>
    // property EmbedMovie
    // IDL: VARIANT_BOOL EmbedMovie;
    // VB6: EmbedMovie As Boolean
    bool EmbedMovie
    {
      // IDL: HRESULT EmbedMovie ([out, retval] VARIANT_BOOL* ReturnValue);
      // VB6: Function EmbedMovie As Boolean
      [DispId(139)]
      [return: MarshalAs(UnmanagedType.VariantBool)]
      get;
      // IDL: HRESULT EmbedMovie (VARIANT_BOOL value);
      // VB6: Sub EmbedMovie (ByVal value As Boolean)
      [DispId(139)]
      set;
    }

    /// <summary><para><c>FlashVars</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property FlashVars</para></summary>
    /// <remarks><para>An original IDL definition of <c>FlashVars</c> property was the following:  <c>BSTR FlashVars</c>;</para></remarks>
    // property FlashVars
    // IDL: BSTR FlashVars;
    // VB6: FlashVars As String
    string FlashVars
    {
      // IDL: HRESULT FlashVars ([out, retval] BSTR* ReturnValue);
      // VB6: Function FlashVars As String
      [DispId(170)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT FlashVars (BSTR value);
      // VB6: Sub FlashVars (ByVal value As String)
      [DispId(170)]
      set;
    }

    /// <summary><para><c>FrameNum</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property FrameNum</para></summary>
    /// <remarks><para>An original IDL definition of <c>FrameNum</c> property was the following:  <c>long FrameNum</c>;</para></remarks>
    // property FrameNum
    // IDL: long FrameNum;
    // VB6: FrameNum As Long
    int FrameNum
    {
      // IDL: HRESULT FrameNum ([out, retval] long* ReturnValue);
      // VB6: Function FrameNum As Long
      [DispId(107)]
      get;
      // IDL: HRESULT FrameNum (long value);
      // VB6: Sub FrameNum (ByVal value As Long)
      [DispId(107)]
      set;
    }

    /// <summary><para><c>InlineData</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property inline-data</para></summary>
    /// <remarks><para>An original IDL definition of <c>InlineData</c> property was the following:  <c>IUnknown* InlineData</c>;</para></remarks>
    // property inline-data
    // IDL: IUnknown* InlineData;
    // VB6: InlineData As IUnknown
    object InlineData
    {
      // IDL: HRESULT InlineData ([out, retval] IUnknown** ReturnValue);
      // VB6: Function InlineData As IUnknown
      [DispId(191)]
      [return: MarshalAs(UnmanagedType.IUnknown)]
      get;
      // IDL: HRESULT InlineData (IUnknown* value);
      // VB6: Sub InlineData (ByVal value As IUnknown)
      [DispId(191)]
      set;
    }

    /// <summary><para><c>Loop</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property Loop</para></summary>
    /// <remarks><para>An original IDL definition of <c>Loop</c> property was the following:  <c>VARIANT_BOOL Loop</c>;</para></remarks>
    // property Loop
    // IDL: VARIANT_BOOL Loop;
    // VB6: Loop As Boolean
    bool Loop
    {
      // IDL: HRESULT Loop ([out, retval] VARIANT_BOOL* ReturnValue);
      // VB6: Function Loop As Boolean
      [DispId(106)]
      [return: MarshalAs(UnmanagedType.VariantBool)]
      get;
      // IDL: HRESULT Loop (VARIANT_BOOL value);
      // VB6: Sub Loop (ByVal value As Boolean)
      [DispId(106)]
      set;
    }

    /// <summary><para><c>Menu</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property Menu</para></summary>
    /// <remarks><para>An original IDL definition of <c>Menu</c> property was the following:  <c>VARIANT_BOOL Menu</c>;</para></remarks>
    // property Menu
    // IDL: VARIANT_BOOL Menu;
    // VB6: Menu As Boolean
    bool Menu
    {
      // IDL: HRESULT Menu ([out, retval] VARIANT_BOOL* ReturnValue);
      // VB6: Function Menu As Boolean
      [DispId(135)]
      [return: MarshalAs(UnmanagedType.VariantBool)]
      get;
      // IDL: HRESULT Menu (VARIANT_BOOL value);
      // VB6: Sub Menu (ByVal value As Boolean)
      [DispId(135)]
      set;
    }

    /// <summary><para><c>Movie</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property Movie</para></summary>
    /// <remarks><para>An original IDL definition of <c>Movie</c> property was the following:  <c>BSTR Movie</c>;</para></remarks>
    // property Movie
    // IDL: BSTR Movie;
    // VB6: Movie As String
    string Movie
    {
      // IDL: HRESULT Movie ([out, retval] BSTR* ReturnValue);
      // VB6: Function Movie As String
      [DispId(102)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT Movie (BSTR value);
      // VB6: Sub Movie (ByVal value As String)
      [DispId(102)]
      set;
    }

    /// <summary><para><c>MovieData</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property MovieData</para></summary>
    /// <remarks><para>An original IDL definition of <c>MovieData</c> property was the following:  <c>BSTR MovieData</c>;</para></remarks>
    // property MovieData
    // IDL: BSTR MovieData;
    // VB6: MovieData As String
    string MovieData
    {
      // IDL: HRESULT MovieData ([out, retval] BSTR* ReturnValue);
      // VB6: Function MovieData As String
      [DispId(190)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT MovieData (BSTR value);
      // VB6: Sub MovieData (ByVal value As String)
      [DispId(190)]
      set;
    }

    /// <summary><para><c>Playing</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property Playing</para></summary>
    /// <remarks><para>An original IDL definition of <c>Playing</c> property was the following:  <c>VARIANT_BOOL Playing</c>;</para></remarks>
    // property Playing
    // IDL: VARIANT_BOOL Playing;
    // VB6: Playing As Boolean
    bool Playing
    {
      // IDL: HRESULT Playing ([out, retval] VARIANT_BOOL* ReturnValue);
      // VB6: Function Playing As Boolean
      [DispId(125)]
      [return: MarshalAs(UnmanagedType.VariantBool)]
      get;
      // IDL: HRESULT Playing (VARIANT_BOOL value);
      // VB6: Sub Playing (ByVal value As Boolean)
      [DispId(125)]
      set;
    }

    /// <summary><para><c>Profile</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property Profile</para></summary>
    /// <remarks><para>An original IDL definition of <c>Profile</c> property was the following:  <c>VARIANT_BOOL Profile</c>;</para></remarks>
    // property Profile
    // IDL: VARIANT_BOOL Profile;
    // VB6: Profile As Boolean
    bool Profile
    {
      // IDL: HRESULT Profile ([out, retval] VARIANT_BOOL* ReturnValue);
      // VB6: Function Profile As Boolean
      [DispId(194)]
      [return: MarshalAs(UnmanagedType.VariantBool)]
      get;
      // IDL: HRESULT Profile (VARIANT_BOOL value);
      // VB6: Sub Profile (ByVal value As Boolean)
      [DispId(194)]
      set;
    }

    /// <summary><para><c>ProfileAddress</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property ProfileAddress</para></summary>
    /// <remarks><para>An original IDL definition of <c>ProfileAddress</c> property was the following:  <c>BSTR ProfileAddress</c>;</para></remarks>
    // property ProfileAddress
    // IDL: BSTR ProfileAddress;
    // VB6: ProfileAddress As String
    string ProfileAddress
    {
      // IDL: HRESULT ProfileAddress ([out, retval] BSTR* ReturnValue);
      // VB6: Function ProfileAddress As String
      [DispId(195)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT ProfileAddress (BSTR value);
      // VB6: Sub ProfileAddress (ByVal value As String)
      [DispId(195)]
      set;
    }

    /// <summary><para><c>ProfilePort</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property ProfilePort</para></summary>
    /// <remarks><para>An original IDL definition of <c>ProfilePort</c> property was the following:  <c>long ProfilePort</c>;</para></remarks>
    // property ProfilePort
    // IDL: long ProfilePort;
    // VB6: ProfilePort As Long
    int ProfilePort
    {
      // IDL: HRESULT ProfilePort ([out, retval] long* ReturnValue);
      // VB6: Function ProfilePort As Long
      [DispId(196)]
      get;
      // IDL: HRESULT ProfilePort (long value);
      // VB6: Sub ProfilePort (ByVal value As Long)
      [DispId(196)]
      set;
    }

    /// <summary><para><c>Quality</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property Quality</para></summary>
    /// <remarks><para>An original IDL definition of <c>Quality</c> property was the following:  <c>int Quality</c>;</para></remarks>
    // property Quality
    // IDL: int Quality;
    // VB6: Quality As Long
    int Quality
    {
      // IDL: HRESULT Quality ([out, retval] int* ReturnValue);
      // VB6: Function Quality As Long
      [DispId(105)]
      get;
      // IDL: HRESULT Quality (int value);
      // VB6: Sub Quality (ByVal value As Long)
      [DispId(105)]
      set;
    }

    /// <summary><para><c>Quality2</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property Quality2</para></summary>
    /// <remarks><para>An original IDL definition of <c>Quality2</c> property was the following:  <c>BSTR Quality2</c>;</para></remarks>
    // property Quality2
    // IDL: BSTR Quality2;
    // VB6: Quality2 As String
    string Quality2
    {
      // IDL: HRESULT Quality2 ([out, retval] BSTR* ReturnValue);
      // VB6: Function Quality2 As String
      [DispId(141)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT Quality2 (BSTR value);
      // VB6: Sub Quality2 (ByVal value As String)
      [DispId(141)]
      set;
    }

    /// <summary><para><c>ReadyState</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property ReadyState</para></summary>
    /// <remarks><para>An original IDL definition of <c>ReadyState</c> property was the following:  <c>long ReadyState</c>;</para></remarks>
    // property ReadyState
    // IDL: long ReadyState;
    // VB6: ReadyState As Long
    int ReadyState
    {
      // IDL: HRESULT ReadyState ([out, retval] long* ReturnValue);
      // VB6: Function ReadyState As Long
      [DispId(-525)]
      get;
    }

    /// <summary><para><c>SAlign</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property SAlign</para></summary>
    /// <remarks><para>An original IDL definition of <c>SAlign</c> property was the following:  <c>BSTR SAlign</c>;</para></remarks>
    // property SAlign
    // IDL: BSTR SAlign;
    // VB6: SAlign As String
    string SAlign
    {
      // IDL: HRESULT SAlign ([out, retval] BSTR* ReturnValue);
      // VB6: Function SAlign As String
      [DispId(134)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT SAlign (BSTR value);
      // VB6: Sub SAlign (ByVal value As String)
      [DispId(134)]
      set;
    }

    /// <summary><para><c>Scale</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property Scale</para></summary>
    /// <remarks><para>An original IDL definition of <c>Scale</c> property was the following:  <c>BSTR Scale</c>;</para></remarks>
    // property Scale
    // IDL: BSTR Scale;
    // VB6: Scale As String
    string Scale
    {
      // IDL: HRESULT Scale ([out, retval] BSTR* ReturnValue);
      // VB6: Function Scale As String
      [DispId(137)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT Scale (BSTR value);
      // VB6: Sub Scale (ByVal value As String)
      [DispId(137)]
      set;
    }

    /// <summary><para><c>ScaleMode</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property ScaleMode</para></summary>
    /// <remarks><para>An original IDL definition of <c>ScaleMode</c> property was the following:  <c>int ScaleMode</c>;</para></remarks>
    // property ScaleMode
    // IDL: int ScaleMode;
    // VB6: ScaleMode As Long
    int ScaleMode
    {
      // IDL: HRESULT ScaleMode ([out, retval] int* ReturnValue);
      // VB6: Function ScaleMode As Long
      [DispId(120)]
      get;
      // IDL: HRESULT ScaleMode (int value);
      // VB6: Sub ScaleMode (ByVal value As Long)
      [DispId(120)]
      set;
    }

    /// <summary><para><c>SeamlessTabbing</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property SeamlessTabbing</para></summary>
    /// <remarks><para>An original IDL definition of <c>SeamlessTabbing</c> property was the following:  <c>VARIANT_BOOL SeamlessTabbing</c>;</para></remarks>
    // property SeamlessTabbing
    // IDL: VARIANT_BOOL SeamlessTabbing;
    // VB6: SeamlessTabbing As Boolean
    bool SeamlessTabbing
    {
      // IDL: HRESULT SeamlessTabbing ([out, retval] VARIANT_BOOL* ReturnValue);
      // VB6: Function SeamlessTabbing As Boolean
      [DispId(192)]
      [return: MarshalAs(UnmanagedType.VariantBool)]
      get;
      // IDL: HRESULT SeamlessTabbing (VARIANT_BOOL value);
      // VB6: Sub SeamlessTabbing (ByVal value As Boolean)
      [DispId(192)]
      set;
    }

    /// <summary><para><c>SWRemote</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property SWRemote</para></summary>
    /// <remarks><para>An original IDL definition of <c>SWRemote</c> property was the following:  <c>BSTR SWRemote</c>;</para></remarks>
    // property SWRemote
    // IDL: BSTR SWRemote;
    // VB6: SWRemote As String
    string SWRemote
    {
      // IDL: HRESULT SWRemote ([out, retval] BSTR* ReturnValue);
      // VB6: Function SWRemote As String
      [DispId(159)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT SWRemote (BSTR value);
      // VB6: Sub SWRemote (ByVal value As String)
      [DispId(159)]
      set;
    }

    /// <summary><para><c>TotalFrames</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property TotalFrames</para></summary>
    /// <remarks><para>An original IDL definition of <c>TotalFrames</c> property was the following:  <c>long TotalFrames</c>;</para></remarks>
    // property TotalFrames
    // IDL: long TotalFrames;
    // VB6: TotalFrames As Long
    int TotalFrames
    {
      // IDL: HRESULT TotalFrames ([out, retval] long* ReturnValue);
      // VB6: Function TotalFrames As Long
      [DispId(124)]
      get;
    }

    /// <summary><para><c>WMode</c> property of <c>IShockwaveFlash</c> interface.  </para><para>property WMode</para></summary>
    /// <remarks><para>An original IDL definition of <c>WMode</c> property was the following:  <c>BSTR WMode</c>;</para></remarks>
    // property WMode
    // IDL: BSTR WMode;
    // VB6: WMode As String
    string WMode
    {
      // IDL: HRESULT WMode ([out, retval] BSTR* ReturnValue);
      // VB6: Function WMode As String
      [DispId(133)]
      [return: MarshalAs(UnmanagedType.BStr)]
      get;
      // IDL: HRESULT WMode (BSTR value);
      // VB6: Sub WMode (ByVal value As String)
      [DispId(133)]
      set;
    }
  }

  /// <summary><para><c>IDispatchEx</c> interface.</para></summary>
  [Guid("A6EF9860-C720-11D0-9337-00A0C90DCAA9")]
  [ComImport]
  [TypeLibType((short)4096)]
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  public interface IDispatchEx
  {
    /// <summary><para><c>GetDispID</c> method of <c>IDispatchEx</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>GetDispID</c> method was the following:  <c>HRESULT GetDispID (BSTR bstrName, unsigned long grfdex, [out] long* pid)</c>;</para></remarks>
    // IDL: HRESULT GetDispID (BSTR bstrName, unsigned long grfdex, [out] long* pid);
    // VB6: Sub GetDispID (ByVal bstrName As String, ByVal grfdex As Long, pid As Long)
    [DispId(1610743808)]
    void GetDispID ([MarshalAs(UnmanagedType.BStr)] string bstrName, uint grfdex, [Out] out int pid);

    /// <summary><para><c>RemoteInvokeEx</c> method of <c>IDispatchEx</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>RemoteInvokeEx</c> method was the following:  <c>HRESULT RemoteInvokeEx (long id, unsigned long lcid, unsigned long dwFlags, [in] Interop.stdole.DISPPARAMS* pdp, [out] VARIANT* pvarRes, [out] Interop.stdole.EXCEPINFO* pei, IServiceProvider* pspCaller, unsigned int cvarRefArg, [in] unsigned int* rgiRefArg, [in, out] VARIANT* rgvarRefArg)</c>;</para></remarks>
    // IDL: HRESULT RemoteInvokeEx (long id, unsigned long lcid, unsigned long dwFlags, [in] Interop.stdole.DISPPARAMS* pdp, [out] VARIANT* pvarRes, [out] Interop.stdole.EXCEPINFO* pei, IServiceProvider* pspCaller, unsigned int cvarRefArg, [in] unsigned int* rgiRefArg, [in, out] VARIANT* rgvarRefArg);
    // VB6: Sub RemoteInvokeEx (ByVal id As Long, ByVal lcid As Long, ByVal dwFlags As Long, pdp As Interop.stdole.DISPPARAMS, pvarRes As Any, pei As Interop.stdole.EXCEPINFO, ByVal pspCaller As IServiceProvider, ByVal cvarRefArg As Long, rgiRefArg As Long, rgvarRefArg As Any)
    //[DispId(1610743809)]
    //void RemoteInvokeEx (int id, uint lcid, uint dwFlags, [In] ref Interop.stdole.DISPPARAMS pdp, [Out] out object pvarRes, [Out] out Interop.stdole.EXCEPINFO pei, IServiceProvider pspCaller, uint cvarRefArg, [In] ref uint rgiRefArg, [In, Out] ref object rgvarRefArg);

    /// <summary><para><c>DeleteMemberByName</c> method of <c>IDispatchEx</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>DeleteMemberByName</c> method was the following:  <c>HRESULT DeleteMemberByName (BSTR bstrName, unsigned long grfdex)</c>;</para></remarks>
    // IDL: HRESULT DeleteMemberByName (BSTR bstrName, unsigned long grfdex);
    // VB6: Sub DeleteMemberByName (ByVal bstrName As String, ByVal grfdex As Long)
    [DispId(1610743810)]
    void DeleteMemberByName ([MarshalAs(UnmanagedType.BStr)] string bstrName, uint grfdex);

    /// <summary><para><c>DeleteMemberByDispID</c> method of <c>IDispatchEx</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>DeleteMemberByDispID</c> method was the following:  <c>HRESULT DeleteMemberByDispID (long id)</c>;</para></remarks>
    // IDL: HRESULT DeleteMemberByDispID (long id);
    // VB6: Sub DeleteMemberByDispID (ByVal id As Long)
    [DispId(1610743811)]
    void DeleteMemberByDispID (int id);

    /// <summary><para><c>GetMemberProperties</c> method of <c>IDispatchEx</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>GetMemberProperties</c> method was the following:  <c>HRESULT GetMemberProperties (long id, unsigned long grfdexFetch, [out] unsigned long* pgrfdex)</c>;</para></remarks>
    // IDL: HRESULT GetMemberProperties (long id, unsigned long grfdexFetch, [out] unsigned long* pgrfdex);
    // VB6: Sub GetMemberProperties (ByVal id As Long, ByVal grfdexFetch As Long, pgrfdex As Long)
    [DispId(1610743812)]
    void GetMemberProperties (int id, uint grfdexFetch, [Out] out uint pgrfdex);

    /// <summary><para><c>GetMemberName</c> method of <c>IDispatchEx</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>GetMemberName</c> method was the following:  <c>HRESULT GetMemberName (long id, [out] BSTR* pbstrName)</c>;</para></remarks>
    // IDL: HRESULT GetMemberName (long id, [out] BSTR* pbstrName);
    // VB6: Sub GetMemberName (ByVal id As Long, pbstrName As String)
    [DispId(1610743813)]
    void GetMemberName (int id, [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrName);

    /// <summary><para><c>GetNextDispID</c> method of <c>IDispatchEx</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>GetNextDispID</c> method was the following:  <c>HRESULT GetNextDispID (unsigned long grfdex, long id, [out] long* pid)</c>;</para></remarks>
    // IDL: HRESULT GetNextDispID (unsigned long grfdex, long id, [out] long* pid);
    // VB6: Sub GetNextDispID (ByVal grfdex As Long, ByVal id As Long, pid As Long)
    [DispId(1610743814)]
    void GetNextDispID (uint grfdex, int id, [Out] out int pid);

    /// <summary><para><c>GetNameSpaceParent</c> method of <c>IDispatchEx</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>GetNameSpaceParent</c> method was the following:  <c>HRESULT GetNameSpaceParent ([out] IUnknown** ppunk)</c>;</para></remarks>
    // IDL: HRESULT GetNameSpaceParent ([out] IUnknown** ppunk);
    // VB6: Sub GetNameSpaceParent (ppunk As IUnknown)
    [DispId(1610743815)]
    void GetNameSpaceParent ([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppunk);
  }

  /// <summary><para><c>IFlashFactory</c> interface.  </para><para>IFlashFactory Interface</para></summary>
  // IFlashFactory Interface
  [Guid("D27CDB70-AE6D-11CF-96B8-444553540000")]
  [ComImport]
  [TypeLibType((short)0)]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IFlashFactory
  {
  }

  /// <summary><para><c>IFlashObjectInterface</c> interface.  </para><para>IFlashObjectInterface Interface</para></summary>
  // IFlashObjectInterface Interface
  [Guid("D27CDB72-AE6D-11CF-96B8-444553540000")]
  [ComImport]
  [TypeLibType((short)4096)]
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  public interface IFlashObjectInterface
  {
  }

  /// <summary><para><c>IServiceProvider</c> interface.</para></summary>
  [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
  [ComImport]
  [TypeLibType((short)0)]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IServiceProvider
  {
    /// <summary><para><c>RemoteQueryService</c> method of <c>IServiceProvider</c> interface.</para></summary>
    /// <remarks><para>An original IDL definition of <c>RemoteQueryService</c> method was the following:  <c>HRESULT RemoteQueryService ([in] Interop.stdole.GUID* guidService, [in] Interop.stdole.GUID* riid, [out] IUnknown** ppvObject)</c>;</para></remarks>
    // IDL: HRESULT RemoteQueryService ([in] Interop.stdole.GUID* guidService, [in] Interop.stdole.GUID* riid, [out] IUnknown** ppvObject);
    // VB6: Sub RemoteQueryService (guidService As Interop.stdole.GUID, riid As Interop.stdole.GUID, ppvObject As IUnknown)
    //void RemoteQueryService ([In] ref Interop.stdole.GUID guidService, [In] ref Interop.stdole.GUID riid, [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);
  }

  /// <summary><para><c>FlashObjectInterface</c> interface.IFlashObjectInterface Interface</para></summary>
  // IFlashObjectInterface Interface
  [Guid("D27CDB72-AE6D-11CF-96B8-444553540000")]
  [ComImport]
  [CoClass(typeof(FlashObjectInterfaceClass))]
  public interface FlashObjectInterface: IFlashObjectInterface
  {
  }

  /// <summary><para><c>FlashObjectInterfaceClass</c> class.  </para><para>IFlashObjectInterface Interface</para></summary>
  /// <remarks>The following sample shows how to use FlashObjectInterfaceClass class.  You should simply create new class instance and cast it to FlashObjectInterface interface.  After this you can call interface methods and access its properties: <code>
  /// FlashObjectInterface A = (FlashObjectInterface) new FlashObjectInterfaceClass();
  /// A.[method name]();  A.[property name] = [value]; [variable] = A.[property name];
  /// </code></remarks>
  // IFlashObjectInterface Interface
  [Guid("D27CDB71-AE6D-11CF-96B8-444553540000")]
  [ComImport]
  [TypeLibType((short)2)]
  [ClassInterface(ClassInterfaceType.None)]
  public class FlashObjectInterfaceClass // : IFlashObjectInterface, FlashObjectInterface
  {
  }

  /// <summary><para><c>FlashProp</c> interface.Macromedia Flash Player Properties</para></summary>
  // Macromedia Flash Player Properties
  // [ComImport, Guid( <Undefined> )] 
  /*[CoClass(typeof(FlashPropClass))]
  public interface FlashProp
  {
  }*/

  /// <summary><para><c>FlashPropClass</c> class.  </para><para>Macromedia Flash Player Properties</para></summary>
  /// <remarks>The following sample shows how to use FlashPropClass class.  You should simply create new class instance and cast it to FlashProp interface.  After this you can call interface methods and access its properties: <code>
  /// FlashProp A = (FlashProp) new FlashPropClass();
  /// A.[method name]();  A.[property name] = [value]; [variable] = A.[property name];
  /// </code></remarks>
  // Macromedia Flash Player Properties
  [Guid("1171A62F-05D2-11D1-83FC-00A0C9089C5A")]
  [ComImport]
  [TypeLibType((short)2)]
  [ClassInterface(ClassInterfaceType.None)]
  public class FlashPropClass // : FlashProp
  {
  }

  /// <summary><para><c>ShockwaveFlash</c> interface.Shockwave Flash</para></summary>
  // Shockwave Flash
  [Guid("D27CDB6C-AE6D-11CF-96B8-444553540000")]
  [ComImport]
  [CoClass(typeof(ShockwaveFlashClass))]
  public interface ShockwaveFlash: IShockwaveFlash
  {
  }

  /// <summary><para><c>ShockwaveFlashClass</c> class.  </para><para>Shockwave Flash</para></summary>
  /// <remarks>The following sample shows how to use ShockwaveFlashClass class.  You should simply create new class instance and cast it to ShockwaveFlash interface.  After this you can call interface methods and access its properties: <code>
  /// ShockwaveFlash A = (ShockwaveFlash) new ShockwaveFlashClass();
  /// A.[method name]();  A.[property name] = [value]; [variable] = A.[property name];
  /// </code></remarks>
  // Shockwave Flash
  [Guid("D27CDB6E-AE6D-11CF-96B8-444553540000")]
  [ComImport]
  [TypeLibType((short)2)]
  [ClassInterface(ClassInterfaceType.None)]
  [ComSourceInterfaces("_IShockwaveFlashEvents")]
  public class ShockwaveFlashClass // : IShockwaveFlash, ShockwaveFlash, _IShockwaveFlashEvents_Event
  {
  }
}
