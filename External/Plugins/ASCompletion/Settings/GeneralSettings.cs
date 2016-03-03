using System;
using System.ComponentModel;
using System.Drawing.Design;
using Ookii.Dialogs;
using PluginCore.Localization;

namespace ASCompletion.Settings
{
    [Serializable]
    public class GeneralSettings
    {
        #region Documentation

        const bool DEFAULT_SMARTTIPS = true;
        const bool DEFAULT_JAVADOCS = true;
        static public string[] DEFAULT_TAGS = new string[] {
            "author","copy","default","deprecated","eventType","example","exampleText","exception",
            "haxe","inheritDoc","internal","link","mtasc","mxmlc","param","private","return","see",
            "serial","serialData","serialField","since","throws","usage","version"
        };
        const int DEFAULT_MAXLINES = 8;

        protected bool smartTipsEnabled = DEFAULT_SMARTTIPS;
        protected bool javadocTagsEnabled = DEFAULT_JAVADOCS;
        protected string[] javadocTags = null;
        private int descriptionLinesLimit = DEFAULT_MAXLINES;

        [DisplayName("Enable Javadoc Tags")]
        [LocalizedCategory("ASCompletion.Category.Documentation"), LocalizedDescription("ASCompletion.Description.JavadocTagsEnabled"), DefaultValue(DEFAULT_JAVADOCS)]
        public bool JavadocTagsEnabled
        {
            get { return javadocTagsEnabled; }
            set { javadocTagsEnabled = value; }
        }

        [DisplayName("Javadoc Tags")]
        [LocalizedCategory("ASCompletion.Category.Documentation"), LocalizedDescription("ASCompletion.Description.JavadocTags")]
        public string[] JavadocTags
        {
            get { return javadocTags; }
            set { javadocTags = value; }
        }

        [DisplayName("Enable Smart Tips")]
        [LocalizedCategory("ASCompletion.Category.Documentation"), LocalizedDescription("ASCompletion.Description.SmartTipsEnabled"), DefaultValue(DEFAULT_SMARTTIPS)]
        public bool SmartTipsEnabled
        {
            get { return smartTipsEnabled; }
            set { smartTipsEnabled = value; }
        }

        [DisplayName("Limit Of Description Lines")]
        [LocalizedCategory("ASCompletion.Category.Documentation"), LocalizedDescription("ASCompletion.Description.DescriptionLinesLimit"), DefaultValue(DEFAULT_MAXLINES)]
        public int DescriptionLinesLimit
        {
            get { return descriptionLinesLimit; }
            set { descriptionLinesLimit = Math.Max(1, value); }
        }


        #endregion

        #region Helpers

        const bool DEFAULT_DISABLE_CLOSEBRACE = false;
        const bool DEFAULT_DISABLE_REFORMAT = false;
        const bool DEFAULT_REFORMAT_BRACES = false;
        const bool DEFAULT_CONDENSE_WS = false;
        const bool DEFAULT_SPACEBEFOREFUNCTIONCALL = false;
        const bool DEFAULT_DISABLETYPESCOLORING = false;
        const int DEFAULT_ALWAYSCOMPLETELENGTH = 2;
        const bool DEFAULT_DISABLECALLTIP = false;
        const string DEFAULT_COMPACTCHARS = ",;.():[]";
        const string DEFAULT_SPACEDCHARS = ",;*+-=/%<>|&!^";
        const string DEFAULT_ADDSPACEAFTER = "if for while do catch with";

        private bool disableAutoCloseBraces = DEFAULT_DISABLE_CLOSEBRACE;
        private bool disableCodeReformat = DEFAULT_DISABLE_REFORMAT;
        private bool reformatBraces = DEFAULT_REFORMAT_BRACES;
        private bool disableKnownTypesColoring = DEFAULT_DISABLETYPESCOLORING;
        private bool condenseWhitespace = DEFAULT_CONDENSE_WS;
        private bool spaceBeforeFunctionCall = DEFAULT_SPACEBEFOREFUNCTIONCALL;
        private int alwaysCompleteWordLength = DEFAULT_ALWAYSCOMPLETELENGTH;
        private bool disableCallTip = DEFAULT_DISABLECALLTIP;
        private string compactChars = DEFAULT_COMPACTCHARS;
        private string spacedChars = DEFAULT_SPACEDCHARS;
        private string addSpaceAfter = DEFAULT_ADDSPACEAFTER;

        [DisplayName("Disable Auto-Close Blocks")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.DisableAutoCloseBraces"), 
        DefaultValue(DEFAULT_DISABLE_CLOSEBRACE)]
        public bool DisableAutoCloseBraces
        {
            get { return disableAutoCloseBraces; }
            set { disableAutoCloseBraces = value; }
        }

        [DisplayName("Condense Whitespace")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.CondenseWhitespace"), 
        DefaultValue(DEFAULT_CONDENSE_WS)]
        public bool CondenseWhitespace
        {
            get { return condenseWhitespace; }
            set { condenseWhitespace = value; }
        }

        [DisplayName("Disable Code Reformat")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.DisableCodeReformat"), 
        DefaultValue(DEFAULT_DISABLE_REFORMAT)]
        public bool DisableCodeReformat
        {
            get { return disableCodeReformat; }
            set { disableCodeReformat = value; }
        }

        [DisplayName("Reformat Braces")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.ReformatBraces"),
        DefaultValue(DEFAULT_REFORMAT_BRACES)]
        public bool ReformatBraces
        {
            get { return reformatBraces; }
            set { reformatBraces = value; }
        }

        [DisplayName("Add Space Before Function Call")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.SpaceBeforeFunctionCall"),
        DefaultValue(DEFAULT_SPACEBEFOREFUNCTIONCALL)]
        public bool SpaceBeforeFunctionCall
        {
            get { return spaceBeforeFunctionCall; }
            set { spaceBeforeFunctionCall = value; }
        }

        [DisplayName("Always Complete Word Length")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.AlwaysCompleteWordLength"),
        DefaultValue(DEFAULT_ALWAYSCOMPLETELENGTH)]
        public int AlwaysCompleteWordLength
        {
            get { return alwaysCompleteWordLength; }
            set { alwaysCompleteWordLength = value; }
        }

        [DisplayName("Disable Automatic Call-Tip")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.DisableCallTip"),
        DefaultValue(DEFAULT_DISABLECALLTIP)]
        public bool DisableCallTip
        {
            get { return disableCallTip; }
            set { disableCallTip = value; }
        }

        [DisplayName("Disable Known Types Coloring")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.DisableKnownTypesColoring"),
        DefaultValue(DEFAULT_DISABLETYPESCOLORING)]
        public bool DisableKnownTypesColoring
        {
            get { return disableKnownTypesColoring; }
            set { disableKnownTypesColoring = value; }
        }

        [DisplayName("Characters Not Surrounded By Spaces")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.CompactChars"),
        DefaultValue(DEFAULT_COMPACTCHARS)]
        public string CompactChars
        {
            get { return compactChars ?? DEFAULT_COMPACTCHARS; }
            set { compactChars = value; }
        }

        [DisplayName("Characters Requiring Whitespace")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.SpacedChars"),
        DefaultValue(DEFAULT_SPACEDCHARS)]
        public string SpacedChars
        {
            get { return spacedChars ?? DEFAULT_SPACEDCHARS; }
            set { spacedChars = value; }
        }

        [DisplayName("Always Add Space After")]
        [LocalizedCategory("ASCompletion.Category.Helpers"), LocalizedDescription("ASCompletion.Description.AddSpaceAfter"),
        DefaultValue(DEFAULT_ADDSPACEAFTER)]
        public string AddSpaceAfter
        {
            get { return addSpaceAfter ?? DEFAULT_ADDSPACEAFTER; }
            set { addSpaceAfter = value; }
        }

        #endregion

        #region Flash IDE

        private string pathToFlashIDE;

        [DisplayName("Path To Flash IDE")]
        [LocalizedCategory("ASCompletion.Category.FlashIDE"), LocalizedDescription("ASCompletion.Description.PathToFlashIDE")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string PathToFlashIDE
        {
            get { return pathToFlashIDE; }
            set { pathToFlashIDE = value; }
        }
        #endregion

        #region Outline view

        const bool DEFAULT_EXTENDS = true;
        const bool DEFAULT_IMPORTS = true;
        const bool DEFAULT_IMPLEMENTS = true;
        const bool DEFAULT_REGIONS = true;
        const OutlineSorting DEFAULT_MODE = OutlineSorting.None;

        protected bool showExtends = DEFAULT_EXTENDS;
        protected bool showImplements = DEFAULT_IMPLEMENTS;
        protected bool showImports = DEFAULT_IMPORTS;
        protected bool showRegions = DEFAULT_REGIONS;
        protected OutlineSorting sortingMode = DEFAULT_MODE;

        [DisplayName("Show Extends")]
        [LocalizedCategory("ASCompletion.Category.OutlineView"), LocalizedDescription("ASCompletion.Description.OutlineViewShowExtends"), DefaultValue(DEFAULT_EXTENDS)]
        public bool ShowExtends
        {
            get { return showExtends; }
            set { showExtends = value; }
        }

        [DisplayName("Show Implements")]
        [LocalizedCategory("ASCompletion.Category.OutlineView"), LocalizedDescription("ASCompletion.Description.OutlineViewShowImplements"), DefaultValue(DEFAULT_IMPLEMENTS)]
        public bool ShowImplements
        {
            get { return showImplements; }
            set { showImplements = value; }
        }

        [DisplayName("Show Imports")]
        [LocalizedCategory("ASCompletion.Category.OutlineView"), LocalizedDescription("ASCompletion.Description.OutlineViewShowImports"), DefaultValue(DEFAULT_IMPORTS)]
        public bool ShowImports
        {
            get { return showImports; }
            set { showImports = value; }
        }

        [DisplayName("Show Regions")]
        [LocalizedDescription("ASCompletion.Description.ShowRegions")]
        [LocalizedCategory("ASCompletion.Category.OutlineView"), DefaultValue(DEFAULT_REGIONS)]
        public bool ShowRegions 
        {
            get { return showRegions; }
            set { showRegions = value; }
        }

        [DisplayName("Sorting Mode")]
        [LocalizedCategory("ASCompletion.Category.OutlineView"), LocalizedDescription("ASCompletion.Description.SortingMode"), DefaultValue(DEFAULT_MODE)]
        public OutlineSorting SortingMode
        {
            get { return sortingMode; }
            set { sortingMode = value; }
        }

        #endregion

        #region Advanced options

        const bool DEFAULT_CACHE = false;

        protected bool disableCache = false;
        protected string lastASVersion;

        [DisplayName("Disable Cache")]
        [LocalizedCategory("ASCompletion.Category.Advanced"), LocalizedDescription("ASCompletion.Description.DisableCache"), DefaultValue(DEFAULT_CACHE)]
        public bool DisableCache
        {
            get { return disableCache; }
            set { disableCache = value; }
        }
        
        [DisplayName("Last ActionScript Version")]
        [LocalizedCategory("ASCompletion.Category.Advanced"), LocalizedDescription("ASCompletion.Description.LastASVersion")]
        public string LastASVersion
        {
            get { return lastASVersion; }
            set { lastASVersion = value; }
        }
        #endregion

        #region Generator

        const bool DEFAULT_GENERATE_PROTECTED = false;
        const bool DEFAULT_GENERATE_STARTWITHMODIFIERS = false;
        const bool DEFAULT_GENERATE_ADDCLOSINGBRACES = false;
        const PropertiesGenerationLocations DEFAULT_GENERATE_PROPERTIES = PropertiesGenerationLocations.AfterLastPropertyDeclaration;
        const MethodsGenerationLocations DEFAULT_GENERATE_METHODS = MethodsGenerationLocations.AfterCurrentMethod;
        const string DEFAULT_GENERATE_PREFIXFIELDS = "";
        const bool DEFAULT_GENERATE_SCOPE = false;
        const HandlerNamingConventions DEFAULT_HANDLER_CONVENTION = HandlerNamingConventions.target_eventName;

        static public string[] DEFAULT_EVENTAUTOREMOVE = new string[] {
              "Event.ADDED_TO_STAGE", "Event.REMOVED_FROM_STAGE",
              "//e.target:Event.COMPLETE", "//e.target:Event.INIT"
        };

        private bool generateProtectedDeclarations = DEFAULT_GENERATE_PROTECTED;
        private string[] eventListenersAutoRemove;
        private bool startWithModifiers;
        private PropertiesGenerationLocations propertiesGenerationLocation;
        private MethodsGenerationLocations methodsGenerationLocation;
        private string prefixFields = DEFAULT_GENERATE_PREFIXFIELDS;
        private bool addClosingBraces = DEFAULT_GENERATE_ADDCLOSINGBRACES;
        private bool generateScope = DEFAULT_GENERATE_SCOPE;
        private HandlerNamingConventions handlerNamingConvention = DEFAULT_HANDLER_CONVENTION;

        [DisplayName("Event Listeners Auto Remove")]
        [LocalizedCategory("ASCompletion.Category.Generation"), LocalizedDescription("ASCompletion.Description.EventListenersAutoRemove")]
        public string[] EventListenersAutoRemove
        {
            get { return eventListenersAutoRemove ?? DEFAULT_EVENTAUTOREMOVE; }
            set { eventListenersAutoRemove = value; }
        }

        [DisplayName("Generate Protected Declarations")]
        [LocalizedCategory("ASCompletion.Category.Generation"), LocalizedDescription("ASCompletion.Description.GenerateProtectedDeclarations"),
        DefaultValue(DEFAULT_GENERATE_PROTECTED)]
        public bool GenerateProtectedDeclarations
        {
            get { return generateProtectedDeclarations; }
            set { generateProtectedDeclarations = value; }
        }
        
        [DisplayName("Start Declarations With Access Modifiers")]
        [LocalizedCategory("ASCompletion.Category.Generation"), LocalizedDescription("ASCompletion.Description.StartWithModifiers"),
        DefaultValue(DEFAULT_GENERATE_STARTWITHMODIFIERS)]
        public bool StartWithModifiers
        {
            get { return startWithModifiers; }
            set { startWithModifiers = value; }
        }

        [DisplayName("Generate Explicit Scope")]
        [LocalizedCategory("ASCompletion.Category.Generation"), LocalizedDescription("ASCompletion.Description.GenerateScope"),
        DefaultValue(DEFAULT_GENERATE_SCOPE)]
        public bool GenerateScope
        {
            get { return generateScope; }
            set { generateScope = value; }
        }

        [DisplayName("Properties Generation Location")]
        [LocalizedCategory("ASCompletion.Category.Generation"), LocalizedDescription("ASCompletion.Description.PropertiesGenerationLocation"),
        DefaultValue(DEFAULT_GENERATE_PROPERTIES)]
        public PropertiesGenerationLocations PropertiesGenerationLocation
        {
            get { return propertiesGenerationLocation; }
            set { propertiesGenerationLocation = value; }
        }

        [DisplayName("Methods Generation Location")]
        [LocalizedCategory("ASCompletion.Category.Generation"), LocalizedDescription("ASCompletion.Description.MethodsGenerationLocation"),
        DefaultValue(DEFAULT_GENERATE_METHODS)]
        public MethodsGenerationLocations MethodsGenerationLocations
        {
            get { return methodsGenerationLocation; }
            set { methodsGenerationLocation = value; }
        }

        [DisplayName("Add Closing Braces")]
        [LocalizedCategory("ASCompletion.Category.Generation"), LocalizedDescription("ASCompletion.Description.AddClosingBraces"),
        DefaultValue(DEFAULT_GENERATE_ADDCLOSINGBRACES)]
        public bool AddClosingBraces
        {
            get { return addClosingBraces; }
            set { addClosingBraces = value; }
        }

        [DisplayName("Prefix Fields When Generating From Params")]
        [LocalizedCategory("ASCompletion.Category.Generation"), LocalizedDescription("ASCompletion.Description.PrefixFields"),
        DefaultValue(DEFAULT_GENERATE_PREFIXFIELDS)]
        public string PrefixFields
        {
            get { return prefixFields; }
            set { prefixFields = value; }
        }

        [DisplayName("Handler Generation Naming Convention")]
        [LocalizedCategory("ASCompletion.Category.Generation"), LocalizedDescription("ASCompletion.Description.HandlerNamingConvention"),
        DefaultValue(DEFAULT_HANDLER_CONVENTION)]
        public HandlerNamingConventions HandlerNamingConvention
        {
            get { return handlerNamingConvention; }
            set { handlerNamingConvention = value; }
        }

        #endregion
    }

    public enum OutlineSorting
    {
        None,
        Sorted,
        SortedByKind,
        SortedSmart,
        SortedGroup
    }

    public enum PropertiesGenerationLocations
    {
        AfterLastPropertyDeclaration = 0,
        AfterVariableDeclaration = 1,
        BeforeVariableDeclaration = 2
    }

    public enum MethodsGenerationLocations
    {
        AfterCurrentMethod = 0,
        AfterSimilarAccessorMethod = 1
    }

    public enum HandlerNamingConventions
    {
        target_eventName = 0,
        target_eventNameHandler = 1,
        onTargetEventName = 2,
        handleTargetEventName = 3
    }
}
