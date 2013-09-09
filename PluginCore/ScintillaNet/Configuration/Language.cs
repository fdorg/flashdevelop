using System;
using System.Runtime;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [SerializableAttribute()]
    public class Language : ConfigItem
    {
        public Lexer lexer;
        
    	[XmlAttributeAttribute()]
        public string name;
        
        [XmlElementAttribute(ElementName="line-comment")]
        public string linecomment;
        
        [XmlElementAttribute(ElementName="comment-start")]
        public string commentstart;
        
        [XmlElementAttribute(ElementName="comment-end")]
        public string commentend;
        
        [XmlElementAttribute(ElementName="file-extensions")]
        public string fileextensions;
		
		[XmlElementAttribute(ElementName="character-class")]
		public CharacterClass characterclass;

		[XmlElementAttribute(ElementName="editor-style")]
		public EditorStyle editorstyle;

        [XmlArrayAttribute("use-keywords")]
        [XmlArrayItemAttribute("keyword")]
        public UseKeyword[] usekeywords;

        [XmlArrayAttribute("use-styles")]
        [XmlArrayItemAttribute("style")]
        public UseStyle[] usestyles;

        public UseStyle GetUseStyle(int style)
        {
			foreach (UseStyle us in this.usestyles)
			{
				if (us.key == style)
				{
					return us;
				}
			}
			return null;
        }
        
        public UseKeyword GetUseKeyword(int key)
        {
			foreach (UseKeyword uk in this.usekeywords)
			{
				if (uk.key == key)
				{
					return uk;
				}
			}
			return null;
        }
        
        public override void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            base.init(utility, theParent);
            if (usekeywords == null) usekeywords = new UseKeyword[0];
            if (usestyles == null) usestyles = new UseStyle[0];
            for (int j = 0; j<usekeywords.Length; j++)
            {
                usekeywords[j].init(utility, _parent);
            }
            for (int i = 0; i<usestyles.Length; i++)
            {
                usestyles[i].language = this;
                usestyles[i].init(utility, _parent);
            }
            if (lexer != null) lexer.init(utility, _parent);
            if (characterclass == null) characterclass = new CharacterClass();
            characterclass.init(utility, _parent);
			if (editorstyle != null) editorstyle.init(utility, _parent);
        }
        
    }
    
}
