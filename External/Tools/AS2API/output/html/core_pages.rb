require 'output/html/html_framework'
require 'output/utils'
require 'rexml/document'

class OverviewNavLinkBuilder < NavLinkBuilder
  def href_on(page); page.base_path("overview-summary.html"); end

  def is_current?(page); page.is_a?(OverviewPage); end

  def title_on(page)
    if @conf.title
      "Overview of #{@conf.title}"
    else
      "Overview of API"
    end
  end
end

class PackageNavLinkBuilder < NavLinkBuilder
  def href_on(page)
    if page.aspackage
      "package-summary.html"
    else
      nil
    end
  end

  def is_current?(page); page.is_a?(PackageIndexPage); end

  def title_on(page)
    if page.aspackage
      "Overview of package #{page.package_display_name_for(page.aspackage)}"
    else
      nil
    end
  end
end


class TypeNavLinkBuilder < NavLinkBuilder
  def href_on(page)
    if page.astype
      page.astype.unqualified_name+".html"
    else
      nil
    end
  end

  def is_current?(page); page.is_a?(TypePage); end

  def title_on(page)
    if page.astype
      "Detail of #{page.astype.qualified_name} API"
    else
      nil
    end
  end
end


class TypePage < BasicPage

  def initialize(conf, type)
    dir = type.package_name.gsub(/\./, "/")
    super(conf, type.unqualified_name, dir)
    @type = type
    @package = type.package
    if @type.source_utf8
      @encoding = "utf-8"
    end
    @title = type.qualified_name
    @prev_type = nil
    @next_type = nil
  end

  attr_accessor :prev_type, :next_type

  def generate_body_content
      html_h1(type_description_for(@type))
      type_hierachy(@type)
      if @type.implements_interfaces?
	html_div("class"=>"interfaces") do
	  html_h2("Implemented Interfaces")
	  @type.each_interface do |interface|
	    # TODO: need to resolve interface name, make links
	    html_code do
	      link_type_proxy(interface)
	    end
	    pcdata(" ")
	  end
	end
      end
      html_div("class"=>"type_description") do
	if @type.comment
	  comment_data = @type.comment
	  html_h2("Description")
	  html_p do
	    output_doccomment_blocktag(comment_data[0])
	  end
	  if comment_data.has_seealso?
	    html_h4("See Also")
	    html_ul("class"=>"extra_info") do
	      comment_data.each_seealso do |see_comment|
		html_li do
		  output_doccomment_blocktag(see_comment)
		end
	      end
	    end
	  end
	end
      end
      
      html_div("class"=>"type_indexes") do
        field_index_list(@type) if has_or_inherits_documentable_fields?(@type)
        method_index_list(@type) if has_or_inherits_documentable_methods?(@type)
      end
      html_div("class"=>"type_details") do
      constructor_detail(@type) if @type.constructor? && document_member?(@type.constructor)
      field_detail_list(@type) if has_documentable_fields?(@type)
      method_detail_list(@type) if has_documentable_methods?(@type)
      end
  end

  def has_or_inherits_documentable_fields?(astype)
    return true if has_documentable_fields?(astype)
    astype.each_ancestor do |ancestor|
      return true if has_documentable_fields?(ancestor)
    end

    false
  end

  def has_or_inherits_documentable_methods?(astype)
    return true if has_documentable_methods?(astype)
    astype.each_ancestor do |ancestor|
      return true if has_documentable_methods?(ancestor)
    end

    false
  end

  def link_top
    yield "Overview", base_path("overview-summary.html")
  end

  def link_up
    yield package_description_for(@type.package), "package-summary.html"
  end

  def link_prev
    if @prev_type
      kind = @prev_type.is_a?(ASInterface) ? "Interface" : "Class"
      yield "#{kind} #{@prev_type.qualified_name}", link_for_type(@prev_type)
    end
  end

  def link_next
    if @next_type
      kind = @next_type.is_a?(ASInterface) ? "Interface" : "Class"
      yield "#{kind} #{@next_type.qualified_name}", link_for_type(@next_type)
    end
  end

  def field_index_list(type)
    html_div("class"=>"field_index") do
      html_h2("Field Index")
      list_fields(type)
      if type.has_ancestor?
	type.each_ancestor do |type|
	  if has_documentable_fields?(type)
	    html_h4 do
	      pcdata("Inherited from ")
	      link_type(type)
	    end
	    html_div("class"=>"extra_info") do
	      list_fields(type, link_for_type(type))
	    end
	  end
	end
      end
    end
  end

  def has_documentable_fields?(astype)
    return false if astype.is_a?(ASInterface)
    astype.each_field do |asfield|
      return true if document_member?(asfield)
    end

    false
  end

  def list_fields(type, href_prefix="")
    fields = type.fields.sort
    index = 0
	html_p do
    fields.each do |field|
      next unless document_member?(field)
      pcdata(", ") if index > 0
      link_field(field)
      index += 1
    end
	end
  end

  def method_index_list(type)
    html_div("class"=>"method_index") do
      html_h2("Method Index")
      if type.constructor? && document_member?(type.constructor)
	html_div do
	  html_code do
	    pcdata("new ")
	    link_method(type.constructor)
	  end
	end
      end
      known_method_names = []
      list_methods(type, known_method_names)
      if type.has_ancestor?
	type.each_ancestor do |type|
	  if has_documentable_methods?(type, known_method_names)
	    html_h4 do
	      pcdata("Inherited from ")
	      link_type(type)
	    end
	    html_div("class"=>"extra_info") do
	      list_methods(type, known_method_names, link_for_type(type))
	    end
	  end
	end
      end
    end
  end

  def has_documentable_methods?(astype, ignore_method_names=[])
    astype.methods.each do |asmethod|
      return true if document_member?(asmethod) && !ignore_method_names.include?(asmethod.name)
    end

    false
  end

  def list_methods(type, known_method_names, href_prefix="")
    methods = type.methods.select do |method|
      !known_method_names.include?(method.name) && document_member?(method)
    end
    methods.sort!
	html_p do
    methods.each_with_index do |method, index|
      known_method_names << method.name
      pcdata(", ") if index > 0
      link_method(method)
    end
	end
  end

  def constructor_detail(type)
    html_div("class"=>"constructor_detail_list") do
      html_h2("Constructor Detail")
      document_method(type.constructor, false)
    end
  end

  def field_detail_list(type)
    html_div("class"=>"field_detail_list") do
      html_h2("Field Detail")
	  count = 0
      type.each_field do |field|
	document_field(field, count%2==0) if document_member?(field)
	count += 1
      end
    end
  end

  def document_field(field, alt_row=false)
    css_class = "field_details"
    css_class << " alt_row" if alt_row
	html_div("class"=>css_class) do
      html_a("", "name"=>"#{field.name}")
	  html_h3(field.name)
      field_synopsis(field)
      if field.comment
	html_div("class"=>"field_info") do
	  comment_data = field.comment
	  html_p do
	  output_doccomment_blocktag(comment_data[0])
	  end
	  if comment_data.has_field_additional_info?
	    if comment_data.has_seealso?
	      document_seealso(comment_data)
	    end
	  end
	end
      end
    end
  end

  def method_detail_list(type)
    html_div("class"=>"method_detail_list") do
      html_h2("Method Detail")
      count = 0
      type.each_method do |method|
	next unless document_member?(method)
	document_method(method, count%2==0)
	count += 1
      end
    end
  end

  def document_method(method, alt_row=false)
    css_class = "method_details"
    css_class << " alt_row" if alt_row
    html_div("class"=>css_class) do
      html_a("", "name"=>"#{method.name}")
      html_h3(method.name)
      method_synopsis(method)
      html_div("class"=>"method_info") do
	if method.comment
	  comment_data = method.comment
	  html_p do
	    output_doccomment_blocktag(comment_data[0])
	  end
	  if method_additional_info?(method, comment_data)
	    # TODO: assumes that params named in docs match formal arguments
	    #       should really filter out those that don't match before this
	    #       test
	    if comment_data.has_params?
	      document_parameters(method.arguments, comment_data)
	    end
	    if comment_data.has_return?
	      document_return(comment_data)
	    end
	    if comment_data.has_exceptions?
	      document_exceptions(comment_data)
	    end
	    method_info_from_supertype(method)
	    if comment_data.has_seealso?
	      document_seealso(comment_data)
	    end
	  end
	else
	  documented_method = method.inherited_comment
	  unless documented_method.nil?
	    comment_data = documented_method.comment
	    html_p("class"=>"inherited_docs") do
	      pcdata("Description copied from ")
	      link_type(documented_method.containing_type)
	    end
	    html_p do
	      output_doccomment_blocktag(comment_data[0])
	    end
	  end
	  method_info_from_supertype(method)
	end
      end
    end
  end

  def method_info_from_supertype(method)
    if method.containing_type.is_a?(ASClass)
      spec_method = method.specified_by
      unless spec_method.nil?
	document_specified_by(spec_method)
      end
    end
    overridden_method = method.overrides
    unless overridden_method.nil?
      document_overridden(overridden_method)
    end
  end

  def type_hierachy(type)
    html_div("class"=>"type_hierachy") do
      ancestors = [type]
      type.each_ancestor {|a| ancestors << a }
      unless ancestors.empty?
	count = type_hierachy_recursive(ancestors)
      end
    end
  end

  def type_hierachy_recursive(ancestors)
    thetype = ancestors.pop
    html_ul do
      html_li do
        if ancestors.empty?
          html_strong(thetype.qualified_name)
        else
          link_type(thetype, true)
          type_hierachy_recursive(ancestors)
        end
      end
    end
  end

  def document_parameters(arguments, comment_data)
    html_h4("Parameters")
    html_table("class"=>"arguments extra_info", "summary"=>"") do
      arguments.each do |arg|
	desc = comment_data.find_param(arg.name)
	if desc
	  html_tr do
	    html_td do
	      html_code(arg.name)
	    end
	    html_td do
	      output_doccomment_blocktag(desc)
	    end
	  end
	end
      end
      # arg with magic name '..' or '...'?
      vararg = comment_data.find_param(/\.{2,3}/)
      if vararg
	html_tr do
	  html_td do
	    html_code("...", {"title", "Variable length argument list"})
	  end
	  html_td do
	    output_doccomment_blocktag(vararg)
	  end
	end
      end
    end
  end

  def document_return(comment_data)
    html_h4("Return")
    return_comment = comment_data.find_return
    html_p("class"=>"extra_info") do
      output_doccomment_blocktag(return_comment)
    end
  end

  def document_exceptions(comment_data)
    html_h4("Throws")
    html_table("class"=>"exceptions extra_info", "summary"=>"") do
      comment_data.each_exception do |exception_comment|
	html_tr do
	  html_td do
	    link_type_proxy(exception_comment.exception_type)
	  end
	  html_td do
	    output_doccomment_blocktag(exception_comment)
	  end
	end
      end
    end
  end

  def document_seealso(comment_data)
    html_h4("See Also")
    html_ul("class"=>"extra_info") do
      comment_data.each_seealso do |see_comment|
	html_li do
	  output_doccomment_blocktag(see_comment)
	end
      end
    end
  end

  def document_specified_by(method)
    html_h4("Specified By")
    html_p("class"=>"extra_info") do
      link_method(method)
      pcdata(" in ")
      link_type(method.containing_type, true)
    end
  end

  def document_overridden(method)
    html_h4("Overrides")
    html_p("class"=>"extra_info") do
      link_method(method)
      pcdata(" in ")
      link_type(method.containing_type, true)
    end
  end

  def method_additional_info?(method, comment_data)
    if method.containing_type.is_a?(ASClass)
      spec_method = method.specified_by
    else
      spec_method = nil
    end
    return comment_data.has_method_additional_info? || !spec_method.nil?
  end

  def method_synopsis(method)
    html_code("class"=>"method_synopsis") do
      if method.access.is_static
	pcdata("static ")
      end
      unless method.access.visibility.nil?
	pcdata("#{method.access.visibility.body} ")
      end
      pcdata("function ")
      html_strong("class"=>"method_name") do
	pcdata(method.name)
      end
      pcdata("(")
      method.arguments.each_with_index do |arg, index|
	pcdata(", ") if index > 0
	pcdata(arg.name)
	if arg.arg_type
	  pcdata(":")
	  link_type_proxy(arg.arg_type)
	end
      end
      pcdata(")")
      if method.return_type
	pcdata(":")
	link_type_proxy(method.return_type)
      end
    end
  end

  def field_synopsis(field)
    html_code("class"=>"field_synopsis") do
      if field.instance_of?(ASImplicitField)
	implicit_field_synopsis(field)
      else
	explicit_field_synopsis(field)
      end
    end
  end

  def explicit_field_synopsis(field)
    if field.access.is_static
      pcdata("static ")
    end
    unless field.access.visibility.nil?
      pcdata("#{field.access.visibility.body} ")
    end
    html_strong("class"=>"field_name") do
      pcdata(field.name)
    end
    if field.field_type
      pcdata(":")
      link_type_proxy(field.field_type)
    end
  end

  def implicit_field_synopsis(field)
    if field.access.is_static
      pcdata("static ")
    end
    unless field.access.visibility.nil?
      pcdata("#{field.access.visibility.body} ")
    end
    html_strong("class"=>"field_name") do
      pcdata(field.name)
    end
    field_type = field.field_type
    unless field_type.nil?
      pcdata(":")
      link_type_proxy(field_type)
    end
    unless field.readwrite?
      pcdata(" ")
      html_em("class"=>"read_write_only") do
	if field.read?
	  pcdata("[Read Only]")
	else
	  pcdata("[Write Only]")
	end
      end
    end
  end

end


class PackageIndexPage < BasicPage

  def initialize(conf, package)
    dir = package_dir_for(package)
    super(conf, "package-summary", dir)
    @package = package
    @title = "#{package_description_for(@package)} API Documentation"
    @prev_package = nil
    @next_package = nil
  end

  attr_accessor :prev_package, :next_package

  def generate_body_content
      html_h1(package_description_for(@package))
      interfaces = @package.interfaces
      unless interfaces.empty?
	interfaces.sort!
	html_table("class"=>"summary_list", "summary"=>"") do
	  html_caption("Interface Summary")
	  interfaces.each do |type|
	    html_tr do
	
	      html_td do
		html_a(type.unqualified_name, {"href"=>type.unqualified_name+".html"})
	      end
	      html_td do
		if type.comment
		  output_doccomment_initial_sentence(type.comment[0])
		end
	      end
	    end
	  end
	end
      end
      classes = @package.classes
      unless classes.empty?
	classes.sort!
	html_table("class"=>"summary_list", "summary"=>"") do
	  html_caption("Class Summary")
	  classes.each do |type|
	    html_tr do
	
	      html_td do
		html_a(type.unqualified_name, {"href"=>type.unqualified_name+".html"})
	      end
	      html_td do
		if type.comment
		  output_doccomment_initial_sentence(type.comment[0])
		end
	      end
	    end
	  end
	end
      end

    if @conf.draw_diagrams
      draw_package_diagrams
      class_diagram
      interface_diagram
    end
  end

  def link_top
    yield "Overview", base_path("overview-summary.html")
  end
  def link_prev
    if @prev_package
      yield package_description_for(@prev_package), base_path(package_link_for(@prev_package, "package-summary.html"))
    end
  end
  def link_next
    if @next_package
      yield package_description_for(@next_package), base_path(package_link_for(@next_package, "package-summary.html"))
    end
  end

  def class_diagram
    dir = File.join(@conf.output_dir, path_name)
    if FileTest.exists?(File.join(dir, "package-classes.png"))
      html_h1("Class Inheritance Diagram")
      html_div("class"=>"diagram") do
	if FileTest.exists?(File.join(dir, "package-classes.cmapx"))
	  map = true
	  File.open(File.join(dir, "package-classes.cmapx")) do |io|
	    copy_xml(io, @io)
	  end
	else
	  map = false
	end
	attr = {"src"=>"package-classes.png"}
	attr["usemap"] = "class_diagram" if map
        html_img(attr)
      end
    end
  end

  def interface_diagram
    dir = File.join(@conf.output_dir, path_name)
    if FileTest.exists?(File.join(dir, "package-interfaces.png"))
      html_h1("Interface Inheritance Diagram")
      html_div("class"=>"diagram") do
	if FileTest.exists?(File.join(dir, "package-interfaces.cmapx"))
	  map = true
	  File.open(File.join(dir, "package-interfaces.cmapx")) do |io|
	    copy_xml(io, @io)
	  end
	else
	  map = false
	end
	attr = {"src"=>"package-interfaces.png"}
	attr["usemap"] = "interface_diagram" if map
        html_img(attr)
      end
    end
  end

  def draw_package_diagrams
    asclasses = @package.classes
    dir = File.join(@conf.output_dir, path_name)
    unless asclasses.empty?
      write_file(dir, "classes.dot") do |io|
	io.puts("strict digraph class_diagram {")
	  io.puts("  rankdir=LR;")
	   asclasses.each do |astype|
	    io.puts("  Type#{astype.unqualified_name}[")
	    io.puts("    label=\"#{astype.unqualified_name}\",")
	    io.puts("    URL=\"#{astype.unqualified_name}.html\",")
	    io.puts("    tooltip=\"#{astype.qualified_name}\",")
	    io.puts("    fontname=\"Times-Italic\",") if astype.is_a?(ASInterface)
	    io.puts("    shape=\"record\"")
	    io.puts("  ];")
	  end
	  asclasses.each do |astype|
	    parent = astype.extends
	    if !parent.nil? && parent.resolved?
	      if parent.resolved_type.package_name == @package.name
		io.puts("  Type#{parent.resolved_type.unqualified_name} -> Type#{astype.unqualified_name};")
	      end
	    end
	  end
	io.puts("}")
      end
      system(@conf.dot_exe, "-Tpng", "-o", File.join(dir, "package-classes.png"), File.join(dir, "classes.dot"))
      system(@conf.dot_exe, "-Tcmapx", "-o", File.join(dir, "package-classes.cmapx"), File.join(dir, "classes.dot"))
      #File.delete(File.join(dir, "types.dot"))
    end

    asinterfaces = @package.interfaces
    unless asinterfaces.empty?
      write_file(dir, "interfaces.dot") do |io|
	io.puts("strict digraph interface_diagram {")
	  io.puts("  rankdir=LR;")
	  asinterfaces.each do |astype|
	    io.puts("  #{astype.unqualified_name}[")
	    io.puts("    label=\"#{astype.unqualified_name}\",")
	    io.puts("    URL=\"#{astype.unqualified_name}.html\",")
	    io.puts("    tooltip=\"#{astype.qualified_name}\",")
	    io.puts("    fontname=\"Times-Italic\",") if astype.is_a?(ASInterface)
	    io.puts("    shape=\"record\"")
	    io.puts("  ];")
	  end
	  asinterfaces.each do |astype|
	    parent = astype.extends
	    if !parent.nil? && parent.resolved?
	      if parent.resolved_type.package_name == @package.name
		io.puts("  #{parent.resolved_type.unqualified_name} -> #{astype.unqualified_name};")
	      end
	    end
	  end
	io.puts("}")
      end
      system(@conf.dot_exe, "-Tpng", "-o", File.join(dir, "package-interfaces.png"), File.join(dir, "interfaces.dot"))
      system(@conf.dot_exe, "-Tcmapx", "-o", File.join(dir, "package-interfaces.cmapx"), File.join(dir, "interfaces.dot"))
      #File.delete(File.join(dir, "types.dot"))
    end
  end

  class XMLAdapter
    def initialize(out)
      @out = out
    end

    def tag_start(name, attrs)
      attr_map = {}
      attrs.each do |el|
	attr_map[el[0]] = el[1]
      end
      @out.start_tag(name, attr_map)
    end

    def tag_end(name)
      @out.end_tag(name)
    end

    def text(text)
      @out.pcdata(text)
    end
  end

  def copy_xml(io, out)
    listener = XMLAdapter.new(out)
    REXML::Document.parse_stream(io, listener)
  end

end

class OverviewPage < BasicPage
  def initialize(conf, type_agregator)
    super(conf, "overview-summary")
    @type_agregator = type_agregator
    @title = "API Overview"
  end

  def generate_body_content
      html_h1("API Overview")
      html_table("class"=>"summary_list", "summary"=>"") do
	html_caption("Packages")
	packages = @type_agregator.packages.sort
	packages.each do |package|
	  html_tr do
      
	    html_td do
	      name = package_display_name_for(package)
	      html_a(name, {"href"=>package_link_for(package, "package-summary.html")})
	    end
	    #html_td do
	      # TODO: package description
	    #end
	  end
	end
      end
  end
end
