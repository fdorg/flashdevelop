require 'output/utils'
require 'xmlwriter'
require 'xhtmlwriter'
require 'output/xml/xml_formatter'

PROJECT_PAGE = "http://www.badgers-in-foil.co.uk/projects/as2api/"

NavLink = Struct.new("NavLink", :href, :content, :title, :is_current)

# superclass for a kind of object able to build a navigation link for
# BasicPage instances.
class NavLinkBuilder
  def initialize(conf, content)
    @conf, @content = conf, content
  end

  def build_for_page(page)
    NavLink.new(href_on(page), @content, title_on(page), is_current?(page))
  end
end


class Page
  include XHTMLWriter

  def initialize(base_name, path_name=nil)
    @path_name = path_name
    @base_name = base_name
    @encoding = nil
    @doctype_id = :strict
    @title = nil
    @title_extra = nil
    @type = nil
    @io = nil  # to be set during the lifetime of generate() call
  end

  attr_accessor :path_name, :encoding, :doctype_id, :title_extra

  attr_writer :title, :base_name

  def base_name
    "#{@base_name}.html"
  end

  def title
    if @title_extra
      if @title
	"#{@title} - #{@title_extra}"
      else
	@title_extra
      end
    else
      @title
    end
  end

  def generate(xml_writer)
    @io = xml_writer
    if encoding.nil?
      pi("xml version=\"1.0\"")
    else
      pi("xml version=\"1.0\" encoding=\"#{encoding}\"")
    end
    case doctype_id
    # FIXME: push this code down into XHTMLWriter, and have it switch the
    # allowed elements depending on the value passed at construction
    when :strict
      doctype("html", "PUBLIC",
              "-//W3C//DTD XHTML 1.0 Strict//EN",
	      "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd")
    when :transitional
      doctype("html", "PUBLIC",
              "-//W3C//DTD XHTML 1.0 Transitionalt//EN",
	      "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd")
    when :frameset
      doctype("html", "PUBLIC",
              "-//W3C//DTD XHTML 1.0 Frameset//EN",
	      "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd")
    else
      raise "unhandled doctype #{doctype_id.inspect}"
    end
    html_html do
      generate_head
      generate_content
    end
  end

  def generate_head
    html_head do
      html_title(title) unless title.nil?
      generate_links
      html_meta("name"=>"generator", "content"=>PROJECT_PAGE)
      unless encoding.nil?
        html_meta("http-equiv"=>"Content-Type",
	          "content"=>"text/html; charset=#{encoding}")
      end
      extra_metadata.each do |key, val|
	html_meta("name"=>key, "content"=>val)
      end
    end
  end

  def extra_metadata
    {}
  end

  def generate_links
    html_link("rel"=>"stylesheet",
             "type"=>"text/css",
	     "href"=>base_path("style.css"),
	     "title"=>"JavaDoc")
    link_top do |title, href|
      html_link("rel"=>"top", "title"=>title, "href"=>href)
    end
    link_up do |title, href|
      html_link("rel"=>"up", "title"=>title, "href"=>href)
    end
    link_prev do |title, href|
      html_link("rel"=>"prev", "title"=>title, "href"=>href)
    end
    link_next do |title, href|
      html_link("rel"=>"next", "title"=>title, "href"=>href)
    end
  end

  def link_top; end
  def link_up; end
  def link_prev; end
  def link_next; end

  def link_for_type(type)
    base_path(type.qualified_name.gsub(/\./, "/")+".html")
  end

  def link_type(type, qualified=false, attrs={})
    desc = type_description_for(type)
    attrs["title"] = desc unless desc.nil?
    if type.instance_of?(ASInterface)
      attrs["class"] = "interface_name"
    elsif type.instance_of?(ASClass)
      attrs["class"] = "class_name"
    elsif type == AS_VOID
      attrs["class"] = "void_name"
    end
    if qualified
      content = type.qualified_name
    else
      content = type.unqualified_name
    end
    if type.document?
      attrs["href"] = link_for_type(type)
      html_a(content, attrs)
    else
      html_span(content, attrs)
    end
  end

  def link_type_proxy(type_proxy, qualified=false)
    if type_proxy.resolved?
      link_type(type_proxy.resolved_type, qualified)
    else
      html_span(type_proxy.local_name, {"class"=>"unresolved_type_name"})
    end
  end

  def signature_for_method(method)
    sig = ""
    if method.access.is_static
      sig << "static "
    end
    unless method.access.visibility.nil?
      sig << "#{method.access.visibility.body} "
    end
    sig << "function "
    sig << method.name
    sig << "("
    method.arguments.each_with_index do |arg, index|
      sig << ", " if index > 0
      sig << arg.name
      if arg.arg_type
	sig << ":"
	sig << arg.arg_type.name
      end
    end
    sig << ")"
    if method.return_type
      sig << ":"
      sig << method.return_type.name
    end
    sig
  end

  def type_description_for(as_type)
    if as_type.instance_of?(ASClass)
      "Class #{as_type.qualified_name}"
    elsif as_type.instance_of?(ASInterface)
      "Interface #{as_type.qualified_name}"
    end
  end

  def link_for_method(method)
    if @type == method.containing_type
      "##{method.name}"
    else
      "#{link_for_type(method.containing_type)}##{method.name}"
    end
  end

  def link_method(method)
    sig = signature_for_method(method)
    if method.containing_type.document?
      html_a("href"=>link_for_method(method), "title"=>sig) do
	pcdata(method.name)
	pcdata("()")
      end
    else
      html_span("title"=>sig) do
	pcdata(method.name)
	pcdata("()")
      end
    end
  end

  def signature_for_field(field)
    sig = ""
    if field.access.is_static
      sig << "static "
    end
    unless field.access.visibility.nil?
      sig << "#{field.access.visibility.body} "
    end
    sig << field.name
    if field.field_type
      sig << ":"
      sig << field.field_type.name
    end
    sig
  end

  def link_for_field(field)
    if @type == field.containing_type
      "##{field.name}"
    else
      "#{link_for_type(field.containing_type)}##{field.name}"
    end
  end

  def link_field(field)
    sig = signature_for_field(field)
    if field.containing_type.document?
      html_a("href"=>link_for_field(field), "title"=>sig) do
	pcdata(field.name)
      end
    else
      html_span("title"=>sig) do
	pcdata(field.name)
      end
    end
  end

  def base_path(file)
    return file if @path_name.nil?
    ((".."+File::SEPARATOR) * @path_name.split(File::SEPARATOR).length) + file
  end

  def document_member?(member)
    !member.access.private?
  end

  def package_dir_for(package)
    package.name.gsub(/\./, "/")
  end

  def package_link_for(package, page)
    return page if package.name == ""
    package_dir_for(package) + "/" + page
  end

  def package_display_name_for(package)
    return "(Default)" if package.name == ""
    package.name
  end

  def package_description_for(package)
    "Package #{package_display_name_for(package)}"
  end
end

class BasicPage < Page
  def initialize(conf, base_name, path_name=nil)
    super(base_name, path_name)
    @conf = conf
    @package = nil
    @navigation = nil
  end

  attr_accessor :navigation

  def astype; @type; end

  def aspackage; @package; end

  def generate_content
    html_body do
	  generate_navigation
      generate_body_content
      generate_footer
    end
  end

  def generate_footer
    html_div("class"=>"footer") do
	  html_span("Generated by ")
      html_a("as2api", {"href"=>PROJECT_PAGE, "title"=>"ActionScript 2 API Documentation Generator", "target"=>"_blank"})
	  html_span(" FlashDevelop edition.")
    end
  end

  def output_doccomment_blocktag(block)
    block.each_inline do |inline|
      output_doccomment_inlinetag(inline)
    end
  end

  def output_doccomment_inlinetag(inline)
    if inline.is_a?(String)
      passthrough(inline)  # allow HTML through unabused (though I wish it were
                           # easy to require it be valid XHTML)
    elsif inline.is_a?(LinkTag)
      if inline.target && inline.member
	if inline.target.resolved?
	  href = link_for_type(inline.target.resolved_type)
	  if inline.member =~ /\(/
	    target = "##{$`}"
	  else
	    target = "##{inline.member}"
	  end
	  href << target
	  html_a("href"=>href) do
	    pcdata("#{inline.target.name}.#{inline.member}")
	  end
	else
	  pcdata("#{inline.target.name}##{inline.member}")
	end
      elsif inline.target
	link_type_proxy(inline.target)
      else
	if inline.member =~ /\(/
	  target = "##{$`}"
	else
	  target = "##{inline.member}"
	end
	html_a("href"=>target) do
	  pcdata(inline.member)
	end
      end
    elsif inline.is_a?(CodeTag)
      input = StringIO.new(inline.text)
      input.lineno = inline.lineno
      highlight = CodeHighlighter.new
      highlight.number_lines = false
      if inline.text =~ /[\n\r]/
	html_pre("class"=>"code") do
	  highlight.highlight(input, self)
	end
      else
	html_code do
	  highlight.highlight(input, self)
	end
      end
    else
      html_em(inline.inspect)
    end
  end

  def output_doccomment_initial_sentence(block)
    block.each_inline do |inline|
      if inline.is_a?(String)
	if inline =~ /(?:[\.:]\s+[A-Z])|(?:[\.:]\s+\Z)|(?:<\/?[Pp]\b)/
	  output_doccomment_inlinetag($`)
	  return
	else
	  output_doccomment_inlinetag(inline)
	end
      else
	output_doccomment_inlinetag(inline)
      end
    end
  end

  def generate_navigation
    html_ul("class"=>"main_nav", "id"=>"main_nav") do
      @navigation.each do |nav|
	link = nav.build_for_page(self)
	html_li do
	  if link.is_current
	    html_span(link.content, {"class"=>"button nav_current"})
	  else
	    if link.href
	      attrs = {"href"=>link.href, "class"=>"button"}
	      attrs["title"] = link.title if link.title
	      html_a(link.content, attrs)
	    else
	      if link.title
		html_span(link.content, {"title"=>link.title, "class"=>"button"})
	      else
		html_span(link.content, {"class"=>"button"})
	      end
	    end
	  end
	end
      end
    end
  end
end


def create_page(output_dir, page, format)
  if page.path_name
    dir = File.join(output_dir, page.path_name)
  else
    dir = output_dir
  end
  write_file(dir, page.base_name) do |io|
    if format
      out = XMLFormatter.new(XMLWriter.new(io))
      out.inlines ["span", "abbr", "acronym", "cite", "code", "dfn", "em", "kbd", "q", "samp", "strong", "var", "p", "address", "h1", "h2", "h3", "h4", "h5", "h6", "a", "dt", "dd", "li", "ins", "del", "bdo", "b", "big", "i", "small", "sub", "sup", "tt", "img", "th", "td",]
    else
      out = XMLWriter.new(io)
    end
    page.generate(out)
  end
end


# creates the pages in the given list by calling each object's #generate_page()
# method
def create_all_pages(conf, list)
  conf.progress_listener.generating_pages(list.length) do
    list.each_with_index do |page, index|
      page.title_extra = conf.title
      page.encoding = conf.input_encoding
      conf.progress_listener.generate_page(index, page)
      create_page(conf.output_dir, page, conf.format_html)
    end
  end
end


# vim:softtabstop=2:shiftwidth=2
