namespace Mono.GetOptions
{
    public class Options
    {
        // Methods
        public Options()
        {
            this.ParsingMode = OptionsParsingMode.Both;
            this.BreakSingleDashManyLettersIntoManyOptions = false;
            this.EndOptionProcessingWithDoubleDash = true;
        }

        [Option("Display version and licensing information", 'V', "version")]
        public virtual WhatToDoNext DoAbout()
        {
            return this.optionParser.DoAbout();
        }

        [Option("Show this help list", '?', "help")]
        public virtual WhatToDoNext DoHelp()
        {
            return this.optionParser.DoHelp();
        }

        [Option("Show an additional help list", "help2")]
        public virtual WhatToDoNext DoHelp2()
        {
            return this.optionParser.DoHelp2();
        }

        [Option("Show usage syntax and exit", "usage")]
        public virtual WhatToDoNext DoUsage()
        {
            return this.optionParser.DoUsage();
        }

        public void ProcessArgs(string[] args)
        {
            this.optionParser = new OptionList(this);
            this.RemainingArguments = this.optionParser.ProcessArgs(args);
        }

        public void ShowBanner()
        {
            this.optionParser.ShowBanner();
        }


        // Properties
        [Option("Show verbose parsing of options", "verbosegetoptions", SecondLevelHelp=true)]
        public bool VerboseParsingOfOptions
        {
            set
            {
                OptionDetails.Verbose = value;
            }
        }


        // Fields
        public bool BreakSingleDashManyLettersIntoManyOptions;
        public bool EndOptionProcessingWithDoubleDash;
        private OptionList optionParser;
        public OptionsParsingMode ParsingMode;
        public string[] RemainingArguments;
    }
}

