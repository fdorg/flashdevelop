module XHTMLWriter

  private

  TAGS = [
    "br",
    "span",
    "abbr",
    "acronym",
    "cite",
    "code",
    "dfn",
    "em",
    "kbd",
    "q",
    "samp",
    "strong",
    "var",
    "div",
    "p",
    "address",
    "blockquote",
    "pre",
    "h1",
    "h2",
    "h3",
    "h4",
    "h5",
    "h6",
    "a",
    "dl",
    "dt",
    "dd",
    "ol",
    "ul",
    "li",
    "ins",
    "del",
    "bdo",
    "ruby",
    "rbc",
    "rtc",
    "rb",
    "rt",
    "rp",
    "b",
    "big",
    "i",
    "small",
    "sub",
    "sup",
    "tt",
    "hr",
    "link",
    "meta",
    "base",
    "script",
    "noscript",
    "style",
    "img",
    "area",
    "map",
    "param",
    "object",
    "table",
    "caption",
    "thead",
    "tfoot",
    "tbody",
    "colgroup",
    "col",
    "tr",
    "th",
    "td",
    "form",
    "label",
    "input",
    "select",
    "optgroup",
    "option",
    "textarea",
    "fieldset",
    "legend",
    "button",
    "title",
    "head",
    "body",
    "html"
  ]

  TAGS << "frameset" << "noframes" << "frame"

  TAGS.each do |name|
    class_eval <<-HERE
      def html_#{name}(*args)
	if block_given?
	  @io.element("#{name}", *args) { yield }
	else
	  if args.length == 0
	    @io.empty_tag("#{name}")
	  else
	    if args[0].instance_of?(String)
	      @io.simple_element("#{name}", *args)
	    else
	      @io.empty_tag("#{name}", *args)
	    end
	  end
	end
      end
    HERE
  end

  public

  def pcdata(text)
    @io.pcdata(text)
  end

  def pi(text)
    @io.pi(text)
  end

  def comment(text)
    @io.comment(text)
  end

  def doctype(name, syspub, public_id, system_id)
    @io.doctype(name, syspub, public_id, system_id)
  end

  def passthrough(text)
    @io.passthrough(text)
  end

  def xml; @io end
end

# vim:sw=2
