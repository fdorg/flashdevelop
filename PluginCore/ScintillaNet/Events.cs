using System;

namespace ScintillaNet
{
    public delegate void FocusHandler(ScintillaControl sender);
    public delegate void ZoomHandler(ScintillaControl sender);
    public delegate void PaintedHandler(ScintillaControl sender);
    public delegate void UpdateUIHandler(ScintillaControl sender);
    public delegate void SavePointReachedHandler(ScintillaControl sender);
    public delegate void SavePointLeftHandler(ScintillaControl sender);
    public delegate void ModifyAttemptROHandler(ScintillaControl sender);
    public delegate void DoubleClickHandler(ScintillaControl sender);
    public delegate void StyleNeededHandler(ScintillaControl sender, int position);
    public delegate void CharAddedHandler(ScintillaControl sender, int ch);
    public delegate void KeyHandler(ScintillaControl sender, int ch, int modifiers);
    public delegate void ModifiedHandler(ScintillaControl sender, int position, int modificationType, string text, int length, int linesAdded, int line, int foldLevelNow, int foldLevelPrev);
    public delegate void MacroRecordHandler(ScintillaControl sender, int message, IntPtr wParam, IntPtr lParam);
    public delegate void MarginClickHandler(ScintillaControl sender, int modifiers, int position, int margin);
    public delegate void NeedShownHandler(ScintillaControl sender, int position, int length);
    public delegate void UserListSelectionHandler(ScintillaControl sender, int listType, string text);
    public delegate void URIDroppedHandler(ScintillaControl sender, string text);
    public delegate void DwellStartHandler(ScintillaControl sender, int position);
    public delegate void DwellEndHandler(ScintillaControl sender ,int position);
    public delegate void HotSpotClickHandler(ScintillaControl sender, int modifiers, int position);
    public delegate void HotSpotDoubleClickHandler(ScintillaControl sender, int modifiers, int position);
    public delegate void CallTipClickHandler(ScintillaControl sender, int position);
    public delegate void AutoCSelectionHandler(ScintillaControl sender, string text);
    public delegate void SmartIndentHandler(ScintillaControl sender);
    public delegate void UserPerformedHandler(ScintillaControl sender);
    public delegate void UndoPerformedHandler(ScintillaControl sender);
    public delegate void RedoPerformedHandler(ScintillaControl sender);
    public delegate void LastStepInUndoRedoHandler(ScintillaControl sender);
    public delegate void StyleChangedHandler(ScintillaControl sender, int position, int length);
    public delegate void TextInsertedHandler(ScintillaControl sender, int position, int length, int linesAdded);
    public delegate void TextDeletedHandler(ScintillaControl sender, int position, int length, int linesAdded);
    public delegate void FoldChangedHandler(ScintillaControl sender, int line, int foldLevelNow, int foldLevelPrev);
    public delegate void BeforeInsertHandler(ScintillaControl sender, int position, int length);
    public delegate void BeforeDeleteHandler(ScintillaControl sender, int position, int length);
    public delegate void MarkerChangedHandler(ScintillaControl sender, int line);
    public delegate void IndicatorClickHandler(ScintillaControl sender, int position);
    public delegate void IndicatorReleaseHandler(ScintillaControl sender, int position);
    public delegate void AutoCCancelledHandler(ScintillaControl sender);
    public delegate void AutoCCharDeletedHandler(ScintillaControl sender);
    public delegate void UpdateSyncHandler(ScintillaControl sender);
    public delegate void SelectionChangedHandler(ScintillaControl sender);
}
